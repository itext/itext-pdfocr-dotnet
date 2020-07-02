/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: iText Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Common.Logging;
using iText.IO.Util;
using iText.Pdfocr;
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.StyledXmlParser.Jsoup.Select;

namespace iText.Pdfocr.Tesseract4 {
    /// <summary>Helper class.</summary>
    public class TesseractHelper {
        /// <summary>The logger.</summary>
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(iText.Pdfocr.Tesseract4.TesseractHelper)
            );

        /// <summary>Patterns for matching hOCR element bboxes.</summary>
        private static readonly Regex BBOX_PATTERN = iText.IO.Util.StringUtil.RegexCompile(".*bbox(\\s+\\d+){4}.*"
            );

        private static readonly Regex BBOX_COORDINATE_PATTERN = iText.IO.Util.StringUtil.RegexCompile(".*\\s+(\\d+)\\s+(\\d+)\\s+(\\d+)\\s+(\\d+).*"
            );

        /// <summary>Indices in array representing bbox.</summary>
        private const int LEFT_IDX = 0;

        private const int BOTTOM_IDX = 1;

        private const int RIGHT_IDX = 2;

        private const int TOP_IDX = 3;

        /// <summary>Size of the array containing bbox.</summary>
        private const int BBOX_ARRAY_SIZE = 4;

        /// <summary>
        /// Creates a new
        /// <see cref="TesseractHelper"/>
        /// instance.
        /// </summary>
        private TesseractHelper() {
        }

        /// <summary>
        /// Parses each hocr file from the provided list, retrieves text, and
        /// returns data in the format described below.
        /// </summary>
        /// <param name="inputFiles">list of input files</param>
        /// <param name="textPositioning">
        /// 
        /// <see cref="TextPositioning"/>
        /// </param>
        /// <returns>
        /// 
        /// <see cref="System.Collections.IDictionary{K, V}"/>
        /// where key is
        /// <see cref="int?"/>
        /// representing the number of the page and value is
        /// <see cref="System.Collections.IList{E}"/>
        /// of
        /// <see cref="iText.Pdfocr.TextInfo"/>
        /// elements where each
        /// <see cref="iText.Pdfocr.TextInfo"/>
        /// element contains a word or a line and its 4
        /// coordinates(bbox)
        /// </returns>
        public static IDictionary<int, IList<TextInfo>> ParseHocrFile(IList<FileInfo> inputFiles, TextPositioning 
            textPositioning) {
            IDictionary<int, IList<TextInfo>> imageData = new LinkedDictionary<int, IList<TextInfo>>();
            IDictionary<String, iText.StyledXmlParser.Jsoup.Nodes.Node> unparsedBBoxes = new LinkedDictionary<String, 
                iText.StyledXmlParser.Jsoup.Nodes.Node>();
            foreach (FileInfo inputFile in inputFiles) {
                if (inputFile != null && File.Exists(System.IO.Path.Combine(inputFile.FullName))) {
                    FileStream fileInputStream = new FileStream(inputFile.FullName, FileMode.Open, FileAccess.Read);
                    Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(fileInputStream, System.Text.Encoding.UTF8.Name(), 
                        inputFile.FullName);
                    Elements pages = doc.GetElementsByClass("ocr_page");
                    IList<String> searchedClasses = TextPositioning.BY_LINES.Equals(textPositioning) ? JavaUtil.ArraysAsList("ocr_line"
                        , "ocr_caption") : JavaCollectionsUtil.SingletonList<String>("ocrx_word");
                    foreach (iText.StyledXmlParser.Jsoup.Nodes.Element page in pages) {
                        String[] pageNum = iText.IO.Util.StringUtil.Split(page.Id(), "page_");
                        int pageNumber = Convert.ToInt32(pageNum[pageNum.Length - 1], System.Globalization.CultureInfo.InvariantCulture
                            );
                        IList<TextInfo> textData = new List<TextInfo>();
                        if (searchedClasses.Count > 0) {
                            Elements objects = page.GetElementsByClass(searchedClasses[0]);
                            for (int i = 1; i < searchedClasses.Count; i++) {
                                Elements foundElements = page.GetElementsByClass(searchedClasses[i]);
                                for (int j = 0; j < foundElements.Count; j++) {
                                    objects.Add(foundElements[j]);
                                }
                            }
                            foreach (iText.StyledXmlParser.Jsoup.Nodes.Element obj in objects) {
                                IList<float> coordinates = GetAlignedBBox(obj, textPositioning, unparsedBBoxes);
                                textData.Add(new TextInfo(obj.Text(), coordinates));
                            }
                        }
                        if (textData.Count > 0) {
                            if (imageData.ContainsKey(pageNumber)) {
                                pageNumber = Enumerable.Max(imageData.Keys) + 1;
                            }
                            imageData.Put(pageNumber, textData);
                        }
                    }
                    fileInputStream.Dispose();
                }
            }
            foreach (iText.StyledXmlParser.Jsoup.Nodes.Node node in unparsedBBoxes.Values) {
                LOGGER.Warn(MessageFormatUtil.Format(Tesseract4LogMessageConstant.CANNOT_PARSE_NODE_BBOX, node.ToString())
                    );
            }
            return imageData;
        }

        /// <summary>Get and align (if needed) bbox of the element.</summary>
        internal static IList<float> GetAlignedBBox(iText.StyledXmlParser.Jsoup.Nodes.Element @object, TextPositioning
             textPositioning, IDictionary<String, iText.StyledXmlParser.Jsoup.Nodes.Node> unparsedBBoxes) {
            IList<float> coordinates = ParseBBox(@object, unparsedBBoxes);
            if (TextPositioning.BY_WORDS_AND_LINES == textPositioning || TextPositioning.BY_WORDS == textPositioning) {
                iText.StyledXmlParser.Jsoup.Nodes.Node line = @object.Parent();
                IList<float> lineCoordinates = ParseBBox(line, unparsedBBoxes);
                if (TextPositioning.BY_WORDS_AND_LINES == textPositioning) {
                    coordinates[BOTTOM_IDX] = lineCoordinates[BOTTOM_IDX];
                    coordinates[TOP_IDX] = lineCoordinates[TOP_IDX];
                }
                DetectAndFixBrokenBBoxes(@object, coordinates, lineCoordinates, unparsedBBoxes);
            }
            return coordinates;
        }

        /// <summary>Parses element bbox.</summary>
        /// <param name="node">element containing bbox</param>
        /// <param name="unparsedBBoxes">list of element ids with bboxes which could not be parsed</param>
        /// <returns>parsed bbox</returns>
        internal static IList<float> ParseBBox(iText.StyledXmlParser.Jsoup.Nodes.Node node, IDictionary<String, iText.StyledXmlParser.Jsoup.Nodes.Node
            > unparsedBBoxes) {
            IList<float> bbox = new List<float>();
            Match bboxMatcher = iText.IO.Util.StringUtil.Match(BBOX_PATTERN, node.Attr("title"));
            if (bboxMatcher.Success) {
                Match bboxCoordinateMatcher = iText.IO.Util.StringUtil.Match(BBOX_COORDINATE_PATTERN, iText.IO.Util.StringUtil.Group
                    (bboxMatcher));
                if (bboxCoordinateMatcher.Success) {
                    for (int i = 0; i < BBOX_ARRAY_SIZE; i++) {
                        String coord = iText.IO.Util.StringUtil.Group(bboxCoordinateMatcher, i + 1);
                        bbox.Add(float.Parse(coord, System.Globalization.CultureInfo.InvariantCulture));
                    }
                }
            }
            if (bbox.Count == 0) {
                bbox = JavaUtil.ArraysAsList(0f, 0f, 0f, 0f);
                String id = node.Attr("id");
                if (id != null && !unparsedBBoxes.ContainsKey(id)) {
                    unparsedBBoxes.Put(id, node);
                }
            }
            return bbox;
        }

        /// <summary>Sometimes hOCR file contains broke character bboxes which are equal to page bbox.</summary>
        /// <remarks>
        /// Sometimes hOCR file contains broke character bboxes which are equal to page bbox.
        /// This method attempts to detect and fix them.
        /// </remarks>
        internal static void DetectAndFixBrokenBBoxes(iText.StyledXmlParser.Jsoup.Nodes.Element @object, IList<float
            > coordinates, IList<float> lineCoordinates, IDictionary<String, iText.StyledXmlParser.Jsoup.Nodes.Node
            > unparsedBBoxes) {
            if (coordinates[LEFT_IDX] < lineCoordinates[LEFT_IDX] || coordinates[LEFT_IDX] > lineCoordinates[RIGHT_IDX
                ]) {
                if (@object.PreviousElementSibling() == null) {
                    coordinates[LEFT_IDX] = lineCoordinates[LEFT_IDX];
                }
                else {
                    iText.StyledXmlParser.Jsoup.Nodes.Element sibling = @object.PreviousElementSibling();
                    IList<float> siblingBBox = ParseBBox(sibling, unparsedBBoxes);
                    coordinates[LEFT_IDX] = siblingBBox[RIGHT_IDX];
                }
            }
            if (coordinates[RIGHT_IDX] > lineCoordinates[RIGHT_IDX] || coordinates[RIGHT_IDX] < lineCoordinates[LEFT_IDX
                ]) {
                if (@object.NextElementSibling() == null) {
                    coordinates[RIGHT_IDX] = lineCoordinates[RIGHT_IDX];
                }
                else {
                    iText.StyledXmlParser.Jsoup.Nodes.Element sibling = @object.NextElementSibling();
                    IList<float> siblingBBox = ParseBBox(sibling, unparsedBBoxes);
                    coordinates[RIGHT_IDX] = siblingBBox[LEFT_IDX];
                }
            }
        }

        /// <summary>Deletes file using provided path.</summary>
        /// <param name="pathToFile">path to the file to be deleted</param>
        internal static void DeleteFile(String pathToFile) {
            try {
                if (pathToFile != null && !String.IsNullOrEmpty(pathToFile) && File.Exists(System.IO.Path.Combine(pathToFile
                    ))) {
                    File.Delete(System.IO.Path.Combine(pathToFile));
                }
            }
            catch (Exception e) {
                LOGGER.Info(MessageFormatUtil.Format(Tesseract4LogMessageConstant.CANNOT_DELETE_FILE, pathToFile, e.Message
                    ));
            }
        }

        /// <summary>Reads from text file to string.</summary>
        /// <param name="txtFile">
        /// input
        /// <see cref="System.IO.FileInfo"/>
        /// to be read
        /// </param>
        /// <returns>
        /// result
        /// <see cref="System.String"/>
        /// from provided text file
        /// </returns>
        internal static String ReadTxtFile(FileInfo txtFile) {
            String content = null;
            try {
                content = iText.IO.Util.JavaUtil.GetStringForBytes(File.ReadAllBytes(txtFile.FullName), System.Text.Encoding
                    .UTF8);
            }
            catch (System.IO.IOException e) {
                LOGGER.Error(MessageFormatUtil.Format(Tesseract4LogMessageConstant.CANNOT_READ_FILE, txtFile.FullName, e.Message
                    ));
            }
            return content;
        }

        /// <summary>
        /// Writes provided
        /// <see cref="System.String"/>
        /// to text file using
        /// provided path.
        /// </summary>
        /// <param name="path">
        /// path as
        /// <see cref="System.String"/>
        /// to file to be created
        /// </param>
        /// <param name="data">
        /// text data in required format as
        /// <see cref="System.String"/>
        /// </param>
        internal static void WriteToTextFile(String path, String data) {
            try {
                using (TextWriter writer = new StreamWriter(new FileStream(path, FileMode.Create), System.Text.Encoding.UTF8
                    )) {
                    writer.Write(data);
                }
            }
            catch (System.IO.IOException e) {
                LOGGER.Error(MessageFormatUtil.Format(Tesseract4LogMessageConstant.CANNOT_WRITE_TO_FILE, path, e.Message));
            }
        }

        /// <summary>Runs given command.</summary>
        /// <param name="execPath">path to the executable</param>
        /// <param name="paramsList">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of command line arguments
        /// </param>
        internal static void RunCommand(String execPath, IList<String> paramsList) {
            try {
                String @params = String.Join(" ", paramsList);
                bool cmdSucceeded = SystemUtil.RunProcessAndWait(execPath, @params);
                if (!cmdSucceeded) {
                    LOGGER.Error(MessageFormatUtil.Format(Tesseract4LogMessageConstant.COMMAND_FAILED, execPath + " " + @params
                        ));
                    throw new Tesseract4OcrException(Tesseract4OcrException.TESSERACT_FAILED);
                }
            }
            catch (Exception e) {
                // NOSONAR
                LOGGER.Error(MessageFormatUtil.Format(Tesseract4LogMessageConstant.COMMAND_FAILED, e.Message));
                throw new Tesseract4OcrException(Tesseract4OcrException.TESSERACT_FAILED);
            }
        }
    }
}

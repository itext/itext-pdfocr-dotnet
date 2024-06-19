/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
Authors: Apryse Software.

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
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Pdfocr;
using iText.Pdfocr.Tesseract4.Exceptions;
using iText.Pdfocr.Tesseract4.Logs;
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.StyledXmlParser.Jsoup.Select;

namespace iText.Pdfocr.Tesseract4 {
    /// <summary>Helper class.</summary>
    public class TesseractHelper {
        /// <summary>The logger.</summary>
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Pdfocr.Tesseract4.TesseractHelper
            ));

        /// <summary>Patterns for matching hOCR element bboxes.</summary>
        private static readonly Regex BBOX_PATTERN = iText.Commons.Utils.StringUtil.RegexCompile(".*bbox(\\s+\\d+){4}.*"
            );

        private static readonly Regex BBOX_COORDINATE_PATTERN = iText.Commons.Utils.StringUtil.RegexCompile(".*\\s+(\\d+)\\s+(\\d+)\\s+(\\d+)\\s+(\\d+).*"
            );

        private static readonly Regex WCONF_PATTERN = iText.Commons.Utils.StringUtil.RegexCompile("^.*(x_wconf *\\d+).*$"
            );

        /// <summary>Size of the array containing bbox.</summary>
        private const int BBOX_ARRAY_SIZE = 4;

        /// <summary>Indices in array representing bbox.</summary>
        private const int LEFT_IDX = 0;

        private const int TOP_IDX = 1;

        private const int RIGHT_IDX = 2;

        private const int BOTTOM_IDX = 3;

        /// <summary>The Constant to convert pixels to points.</summary>
        private const float PX_TO_PT = 3F / 4F;

        private const String NEW_LINE_PATTERN = "\n+";

        private const String SPACE_PATTERN = " +";

        private const String NEW_LINE_OR_SPACE_PATTERN = "[\n ]+";

        private const String PAGE_PREFIX_PATTERN = "page_";

        private const String OCR_PAGE = "ocr_page";

        private const String OCR_LINE = "ocr_line";

        private const String OCR_CAPTION = "ocr_caption";

        private const String OCRX_WORD = "ocrx_word";

        private const String TITLE = "title";

        private const String X_WCONF = "x_wconf";

        /// <summary>
        /// Creates a new
        /// <see cref="TesseractHelper"/>
        /// instance.
        /// </summary>
        private TesseractHelper() {
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Parses each hocr file from the provided list, retrieves text, and
        /// returns data in the format described below.
        /// </summary>
        /// <param name="inputFiles">list of input files</param>
        /// <param name="txtInputFiles">
        /// list of input files in txt format used to make hocr recognition result more precise.
        /// This is needed for cases of Thai language or some Chinese dialects
        /// where every character is interpreted as a single word.
        /// For more information see https://github.com/tesseract-ocr/tesseract/issues/2702
        /// </param>
        /// <param name="tesseract4OcrEngineProperties">
        /// 
        /// <see cref="Tesseract4OcrEngineProperties"/>
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
        internal static IDictionary<int, IList<TextInfo>> ParseHocrFile(IList<FileInfo> inputFiles, IList<FileInfo
            > txtInputFiles, Tesseract4OcrEngineProperties tesseract4OcrEngineProperties) {
            IDictionary<int, IList<TextInfo>> imageData = new LinkedDictionary<int, IList<TextInfo>>();
            IDictionary<String, iText.StyledXmlParser.Jsoup.Nodes.Node> unparsedBBoxes = new LinkedDictionary<String, 
                iText.StyledXmlParser.Jsoup.Nodes.Node>();
            for (int inputFileIdx = 0; inputFileIdx < inputFiles.Count; inputFileIdx++) {
                FileInfo inputFile = inputFiles[inputFileIdx];
                IList<String> txt = null;
                if (txtInputFiles != null) {
                    FileInfo txtInputFile = txtInputFiles[inputFileIdx];
                    txt = File.ReadAllLines(txtInputFile.FullName, System.Text.Encoding.UTF8);
                }
                if (inputFile != null && File.Exists(System.IO.Path.Combine(inputFile.FullName))) {
                    FileStream fileInputStream = new FileStream(inputFile.FullName, FileMode.Open, FileAccess.Read);
                    Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(fileInputStream, System.Text.Encoding.UTF8.Name(), 
                        inputFile.FullName);
                    Elements pages = doc.GetElementsByClass(OCR_PAGE);
                    foreach (iText.StyledXmlParser.Jsoup.Nodes.Element page in pages) {
                        String[] pageNum = iText.Commons.Utils.StringUtil.Split(page.Id(), PAGE_PREFIX_PATTERN);
                        int pageNumber = Convert.ToInt32(pageNum[pageNum.Length - 1], System.Globalization.CultureInfo.InvariantCulture
                            );
                        IList<TextInfo> textData = GetTextData(page, tesseract4OcrEngineProperties, txt, unparsedBBoxes);
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
                LOGGER.LogWarning(MessageFormatUtil.Format(Tesseract4LogMessageConstant.CANNOT_PARSE_NODE_BBOX, node.ToString
                    ()));
            }
            return imageData;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Get and align (if needed) bbox of the element.</summary>
        internal static Rectangle GetAlignedBBox(iText.StyledXmlParser.Jsoup.Nodes.Element @object, TextPositioning
             textPositioning, Rectangle pageBbox, IDictionary<String, iText.StyledXmlParser.Jsoup.Nodes.Node> unparsedBBoxes
            ) {
            Rectangle box = ParseBBox(@object, pageBbox, unparsedBBoxes);
            if (TextPositioning.BY_WORDS_AND_LINES == textPositioning || TextPositioning.BY_WORDS == textPositioning) {
                iText.StyledXmlParser.Jsoup.Nodes.Node line = @object.Parent();
                Rectangle lineBbox = ParseBBox(line, pageBbox, unparsedBBoxes);
                if (TextPositioning.BY_WORDS_AND_LINES == textPositioning) {
                    box.SetBbox(box.GetLeft(), lineBbox.GetBottom(), box.GetRight(), lineBbox.GetTop());
                }
                DetectAndFixBrokenBBoxes(@object, box, lineBbox, pageBbox, unparsedBBoxes);
            }
            return box;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Parses element bbox.</summary>
        /// <param name="node">element containing bbox</param>
        /// <param name="pageBBox">element containing parent page bbox</param>
        /// <param name="unparsedBBoxes">list of element ids with bboxes which could not be parsed</param>
        /// <returns>parsed bbox</returns>
        internal static Rectangle ParseBBox(iText.StyledXmlParser.Jsoup.Nodes.Node node, Rectangle pageBBox, IDictionary
            <String, iText.StyledXmlParser.Jsoup.Nodes.Node> unparsedBBoxes) {
            IList<float> bbox = new List<float>();
            Matcher bboxMatcher = iText.Commons.Utils.Matcher.Match(BBOX_PATTERN, node.Attr(TITLE));
            if (bboxMatcher.Matches()) {
                Matcher bboxCoordinateMatcher = iText.Commons.Utils.Matcher.Match(BBOX_COORDINATE_PATTERN, bboxMatcher.Group
                    ());
                if (bboxCoordinateMatcher.Matches()) {
                    for (int i = 0; i < BBOX_ARRAY_SIZE; i++) {
                        String coord = bboxCoordinateMatcher.Group(i + 1);
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
            if (pageBBox == null) {
                return new Rectangle(ToPoints(bbox[LEFT_IDX]), ToPoints(bbox[TOP_IDX]), ToPoints(bbox[RIGHT_IDX]), ToPoints
                    (bbox[BOTTOM_IDX] - bbox[TOP_IDX]));
            }
            else {
                return new Rectangle(0, 0).SetBbox(ToPoints(bbox[LEFT_IDX]), pageBBox.GetTop() - ToPoints(bbox[TOP_IDX]), 
                    ToPoints(bbox[RIGHT_IDX]), pageBBox.GetTop() - ToPoints(bbox[BOTTOM_IDX]));
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Sometimes hOCR file contains broke character bboxes which are equal to page bbox.</summary>
        /// <remarks>
        /// Sometimes hOCR file contains broke character bboxes which are equal to page bbox.
        /// This method attempts to detect and fix them.
        /// </remarks>
        internal static void DetectAndFixBrokenBBoxes(iText.StyledXmlParser.Jsoup.Nodes.Element @object, Rectangle
             bbox, Rectangle lineBbox, Rectangle pageBbox, IDictionary<String, iText.StyledXmlParser.Jsoup.Nodes.Node
            > unparsedBBoxes) {
            if (bbox.GetLeft() < lineBbox.GetLeft() || bbox.GetLeft() > lineBbox.GetRight()) {
                if (@object.PreviousElementSibling() == null) {
                    bbox.SetX(lineBbox.GetLeft());
                }
                else {
                    iText.StyledXmlParser.Jsoup.Nodes.Element sibling = @object.PreviousElementSibling();
                    Rectangle siblingBBox = ParseBBox(sibling, pageBbox, unparsedBBoxes);
                    bbox.SetX(siblingBBox.GetRight());
                }
            }
            if (bbox.GetRight() > lineBbox.GetRight() || bbox.GetRight() < lineBbox.GetLeft()) {
                if (@object.NextElementSibling() == null) {
                    bbox.SetBbox(bbox.GetLeft(), bbox.GetBottom(), lineBbox.GetRight(), bbox.GetTop());
                }
                else {
                    iText.StyledXmlParser.Jsoup.Nodes.Element sibling = @object.NextElementSibling();
                    Rectangle siblingBBox = ParseBBox(sibling, pageBbox, unparsedBBoxes);
                    bbox.SetBbox(bbox.GetLeft(), bbox.GetBottom(), siblingBBox.GetLeft(), bbox.GetTop());
                }
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Converts points to pixels.</summary>
        internal static float ToPixels(float pt) {
            return pt / PX_TO_PT;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Converts pixels to points.</summary>
        internal static float ToPoints(float px) {
            return px * PX_TO_PT;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Deletes file using provided path.</summary>
        /// <param name="pathToFile">path to the file to be deleted</param>
        internal static void DeleteFile(String pathToFile) {
            try {
                if (pathToFile != null && !String.IsNullOrEmpty(pathToFile) && File.Exists(System.IO.Path.Combine(pathToFile
                    ))) {
                    File.Delete(System.IO.Path.Combine(pathToFile));
                }
            }
            catch (System.IO.IOException e) {
                LOGGER.LogInformation(MessageFormatUtil.Format(Tesseract4LogMessageConstant.CANNOT_DELETE_FILE, pathToFile
                    , e.Message));
            }
            catch (SecurityException e) {
                LOGGER.LogInformation(MessageFormatUtil.Format(Tesseract4LogMessageConstant.CANNOT_DELETE_FILE, pathToFile
                    , e.Message));
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
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
                content = iText.Commons.Utils.JavaUtil.GetStringForBytes(File.ReadAllBytes(txtFile.FullName), System.Text.Encoding
                    .UTF8);
            }
            catch (System.IO.IOException e) {
                LOGGER.LogError(MessageFormatUtil.Format(Tesseract4LogMessageConstant.CANNOT_READ_FILE, txtFile.FullName, 
                    e.Message));
            }
            return content;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
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
                throw new PdfOcrInputTesseract4Exception(PdfOcrTesseract4ExceptionMessageConstant.CANNOT_WRITE_TO_FILE, e);
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Runs given command.</summary>
        /// <param name="execPath">path to the executable</param>
        /// <param name="paramsList">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of command line arguments
        /// </param>
        internal static void RunCommand(String execPath, IList<String> paramsList) {
            RunCommand(execPath, paramsList, null);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Runs given command from the specific working directory.</summary>
        /// <param name="execPath">path to the executable</param>
        /// <param name="paramsList">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of command line arguments
        /// </param>
        /// <param name="workingDirPath">path to the working directory</param>
        internal static void RunCommand(String execPath, IList<String> paramsList, String workingDirPath) {
            try {
                String @params = String.Join(" ", paramsList);
                bool cmdSucceeded = SystemUtil.RunProcessAndWait(execPath, @params, workingDirPath);
                if (!cmdSucceeded) {
                    LOGGER.LogError(MessageFormatUtil.Format(Tesseract4LogMessageConstant.COMMAND_FAILED, execPath + " " + @params
                        ));
                    throw new PdfOcrTesseract4Exception(PdfOcrTesseract4ExceptionMessageConstant.TESSERACT_FAILED);
                }
            }
            catch (Exception e) {
                LOGGER.LogError(MessageFormatUtil.Format(Tesseract4LogMessageConstant.COMMAND_FAILED, e.Message));
                throw new PdfOcrTesseract4Exception(PdfOcrTesseract4ExceptionMessageConstant.TESSERACT_FAILED);
            }
        }
//\endcond

        /// <summary>Gets list of text infos from hocr page.</summary>
        private static IList<TextInfo> GetTextData(iText.StyledXmlParser.Jsoup.Nodes.Element page, Tesseract4OcrEngineProperties
             tesseract4OcrEngineProperties, IList<String> txt, IDictionary<String, iText.StyledXmlParser.Jsoup.Nodes.Node
            > unparsedBBoxes) {
            Rectangle pageBbox = ParseBBox(page, null, unparsedBBoxes);
            IList<String> searchedClasses = JavaUtil.ArraysAsList(OCR_LINE, OCR_CAPTION);
            Elements objects = new Elements();
            for (int i = 0; i < searchedClasses.Count; i++) {
                Elements foundElements = page.GetElementsByClass(searchedClasses[i]);
                for (int j = 0; j < foundElements.Count; j++) {
                    objects.Add(foundElements[j]);
                }
            }
            return GetTextData(objects, tesseract4OcrEngineProperties, txt, pageBbox, unparsedBBoxes);
        }

        /// <summary>Gets list of text infos from elements within hocr page.</summary>
        private static IList<TextInfo> GetTextData(IList<iText.StyledXmlParser.Jsoup.Nodes.Element> pageObjects, Tesseract4OcrEngineProperties
             tesseract4OcrEngineProperties, IList<String> txt, Rectangle pageBbox, IDictionary<String, iText.StyledXmlParser.Jsoup.Nodes.Node
            > unparsedBBoxes) {
            IList<TextInfo> textData = new List<TextInfo>();
            foreach (iText.StyledXmlParser.Jsoup.Nodes.Element lineOrCaption in pageObjects) {
                if (!String.IsNullOrEmpty(lineOrCaption.Text()) && IsElementConfident(lineOrCaption, tesseract4OcrEngineProperties
                    .GetMinimalConfidenceLevel())) {
                    String hocrLineInTxt = FindHocrLineInTxt(lineOrCaption, txt);
                    if (tesseract4OcrEngineProperties.GetTextPositioning() == TextPositioning.BY_WORDS || tesseract4OcrEngineProperties
                        .GetTextPositioning() == TextPositioning.BY_WORDS_AND_LINES) {
                        foreach (TextInfo ti in GetTextDataForWords(lineOrCaption, hocrLineInTxt, tesseract4OcrEngineProperties.GetTextPositioning
                            (), pageBbox, unparsedBBoxes)) {
                            textData.Add(ti);
                        }
                    }
                    else {
                        foreach (TextInfo ti in GetTextDataForLines(lineOrCaption, hocrLineInTxt, pageBbox, unparsedBBoxes)) {
                            textData.Add(ti);
                        }
                    }
                }
            }
            return textData;
        }

        /// <summary>
        /// Decides if <c>lineOrCaption</c> is confident or not given into account
        /// minimalConfidenceLevel property of
        /// <see cref="Tesseract4OcrEngineProperties"/>.
        /// </summary>
        private static bool IsElementConfident(iText.StyledXmlParser.Jsoup.Nodes.Element lineOrCaption, int minimalConfidenceLevel
            ) {
            if (minimalConfidenceLevel == 0) {
                return true;
            }
            else {
                int wconfTotal = 0;
                int wconfCount = 0;
                foreach (iText.StyledXmlParser.Jsoup.Nodes.Node node in lineOrCaption.ChildNodes()) {
                    if (node is iText.StyledXmlParser.Jsoup.Nodes.Element) {
                        String title = ((iText.StyledXmlParser.Jsoup.Nodes.Element)node).Attr(TITLE);
                        Matcher matcher = iText.Commons.Utils.Matcher.Match(WCONF_PATTERN, title);
                        if (matcher.Matches()) {
                            String wconf = null;
                            try {
                                wconf = matcher.Group(1);
                            }
                            catch (Exception) {
                            }
                            //No need to do anything here
                            if (wconf != null) {
                                wconf = iText.Commons.Utils.StringUtil.ReplaceAll(wconf, X_WCONF, "").Trim();
                                wconfTotal += Convert.ToInt32(wconf, System.Globalization.CultureInfo.InvariantCulture);
                                wconfCount++;
                            }
                        }
                    }
                }
                if (wconfCount > 0) {
                    return wconfTotal / wconfCount >= minimalConfidenceLevel;
                }
                else {
                    return true;
                }
            }
        }

        /// <summary>Gets list of words represented by text infos from hocr line.</summary>
        private static IList<TextInfo> GetTextDataForWords(iText.StyledXmlParser.Jsoup.Nodes.Element lineOrCaption
            , String txtLine, TextPositioning textPositioning, Rectangle pageBbox, IDictionary<String, iText.StyledXmlParser.Jsoup.Nodes.Node
            > unparsedBBoxes) {
            IList<TextInfo> textData = new List<TextInfo>();
            if (txtLine == null) {
                foreach (iText.StyledXmlParser.Jsoup.Nodes.Element word in lineOrCaption.GetElementsByClass(OCRX_WORD)) {
                    Rectangle bboxRect = GetAlignedBBox(word, textPositioning, pageBbox, unparsedBBoxes);
                    AddToTextData(textData, word.Text(), bboxRect);
                }
            }
            else {
                IList<TextInfo> textInfos = new List<TextInfo>();
                String txtLine1 = iText.Commons.Utils.StringUtil.ReplaceAll(txtLine, NEW_LINE_PATTERN, "");
                String txtLine2 = iText.Commons.Utils.StringUtil.ReplaceAll(txtLine1, SPACE_PATTERN, " ");
                String[] lineItems = iText.Commons.Utils.StringUtil.Split(txtLine2, " ");
                foreach (iText.StyledXmlParser.Jsoup.Nodes.Element word in lineOrCaption.GetElementsByClass(OCRX_WORD)) {
                    Rectangle bboxRect = GetAlignedBBox(word, textPositioning, pageBbox, unparsedBBoxes);
                    textInfos.Add(new TextInfo(word.Text(), bboxRect));
                    if (iText.Commons.Utils.StringUtil.ReplaceAll(lineItems[0], NEW_LINE_OR_SPACE_PATTERN, "").Equals(iText.Commons.Utils.StringUtil.ReplaceAll
                        (GetTextInfosText(textInfos), SPACE_PATTERN, ""))) {
                        lineItems = JavaUtil.ArraysCopyOfRange(lineItems, 1, lineItems.Length);
                        AddToTextData(textData, MergeTextInfos(textInfos));
                        textInfos.Clear();
                    }
                }
            }
            return textData;
        }

        /// <summary>Gets list of lines represented by text infos from hocr line.</summary>
        private static IList<TextInfo> GetTextDataForLines(iText.StyledXmlParser.Jsoup.Nodes.Element lineOrCaption
            , String txtLine, Rectangle pageBbox, IDictionary<String, iText.StyledXmlParser.Jsoup.Nodes.Node> unparsedBBoxes
            ) {
            IList<TextInfo> textData = new List<TextInfo>();
            Rectangle bboxRect = GetAlignedBBox(lineOrCaption, TextPositioning.BY_LINES, pageBbox, unparsedBBoxes);
            if (txtLine == null) {
                AddToTextData(textData, lineOrCaption.Text(), bboxRect);
            }
            else {
                AddToTextData(textData, txtLine, bboxRect);
            }
            return textData;
        }

        /// <summary>Add text chunk represented by text and bbox to list of text infos.</summary>
        private static void AddToTextData(IList<TextInfo> textData, String text, Rectangle bboxRect) {
            TextInfo textInfo = new TextInfo(text, bboxRect);
            textData.Add(textInfo);
        }

        /// <summary>Add text chunk represented by text info to list of text infos.</summary>
        private static void AddToTextData(IList<TextInfo> textData, TextInfo textInfo) {
            String text = textInfo.GetText();
            Rectangle bboxRect = textInfo.GetBboxRect();
            AddToTextData(textData, text, bboxRect);
        }

        /// <summary>Gets common text for list of text infos.</summary>
        private static String GetTextInfosText(IList<TextInfo> textInfos) {
            StringBuilder text = new StringBuilder();
            foreach (TextInfo textInfo in textInfos) {
                text.Append(textInfo.GetText());
            }
            return text.ToString();
        }

        /// <summary>Merges text infos.</summary>
        /// <param name="textInfos">source to merge</param>
        /// <returns>merged text info</returns>
        private static TextInfo MergeTextInfos(IList<TextInfo> textInfos) {
            TextInfo textInfo = new TextInfo(textInfos[0]);
            for (int i = 1; i < textInfos.Count; i++) {
                textInfo.SetText(textInfo.GetText() + textInfos[i].GetText());
                Rectangle leftBBox = textInfo.GetBboxRect();
                Rectangle rightBBox = textInfos[i].GetBboxRect();
                textInfo.SetBboxRect(new Rectangle(0, 0).SetBbox(leftBBox.GetLeft(), Math.Min(leftBBox.GetBottom(), rightBBox
                    .GetBottom()), rightBBox.GetRight(), Math.Max(leftBBox.GetTop(), rightBBox.GetTop())));
            }
            return textInfo;
        }

        /// <summary>Attempts to find HOCR line text in provided TXT.</summary>
        /// <returns>text line if found, otherwise null</returns>
        private static String FindHocrLineInTxt(iText.StyledXmlParser.Jsoup.Nodes.Element line, IList<String> txt) {
            if (txt == null) {
                return null;
            }
            String hocrLineText = iText.Commons.Utils.StringUtil.ReplaceAll(line.Text(), SPACE_PATTERN, "");
            if (String.IsNullOrEmpty(hocrLineText)) {
                return null;
            }
            foreach (String txtLine in txt) {
                if (iText.Commons.Utils.StringUtil.ReplaceAll(txtLine, SPACE_PATTERN, "").Equals(hocrLineText)) {
                    return txtLine;
                }
            }
            return null;
        }
    }
}

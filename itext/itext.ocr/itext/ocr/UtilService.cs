using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Common.Logging;
using iText.IO.Image;
using iText.IO.Util;
using iText.Kernel.Geom;
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.StyledXmlParser.Jsoup.Select;

namespace iText.Ocr {
    /// <summary>Helper class.</summary>
    public sealed class UtilService {
        /// <summary>Constantsfor points per inch (for tests).</summary>
        private const float POINTS_PER_INCH = 72.0f;

        /// <summary>UtilService logger.</summary>
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(iText.Ocr.UtilService));

        /// <summary>Encoding UTF-8 string.</summary>
        private static String encodingUTF8 = "UTF-8";

        /// <summary>Constant to convert pixels to points (for tests).</summary>
        internal const float PX_TO_PT = 3f / 4f;

        /// <summary>Private constructor for util class.</summary>
        private UtilService() {
        }

        /// <summary>Read text file to string.</summary>
        /// <param name="txtFile">File</param>
        /// <returns>String</returns>
        public static String ReadTxtFile(FileInfo txtFile) {
            String content = null;
            try {
                content = iText.IO.Util.JavaUtil.GetStringForBytes(System.IO.File.ReadAllBytes(txtFile.FullName), System.Text.Encoding
                    .UTF8);
            }
            catch (System.IO.IOException e) {
                LOGGER.Error("Cannot read file " + txtFile.FullName + " with error " + e.Message);
            }
            return content;
        }

        /// <summary>Convert from pixels to points.</summary>
        /// <param name="pixels">float</param>
        /// <returns>float</returns>
        public static float GetPoints(float pixels) {
            return pixels * PX_TO_PT;
        }

        /// <summary>Delete file using provided path.</summary>
        /// <param name="pathToFile">String</param>
        public static void DeleteFile(String pathToFile) {
            if (pathToFile != null && !String.IsNullOrEmpty(pathToFile) && File.Exists(System.IO.Path.Combine(pathToFile
                ))) {
                try {
                    File.Delete(System.IO.Path.Combine(pathToFile));
                }
                catch (System.IO.IOException e) {
                    LOGGER.Info("File " + pathToFile + " cannot be deleted: " + e.Message);
                }
            }
        }

        /// <summary>
        /// Parse `hocr` file, retrieve text, and return in the format
        /// described below.
        /// </summary>
        /// <remarks>
        /// Parse `hocr` file, retrieve text, and return in the format
        /// described below.
        /// each list element : Map.Entry<String, List&lt;integer>&gt; contains
        /// word or line as a key and its 4 coordinates(bbox) as a values
        /// </remarks>
        /// <param name="inputFiles">list ofo input files</param>
        /// <param name="textPositioning">TextPositioning</param>
        /// <returns>Map<Integer, List&lt;textinfo>&gt;</returns>
        internal static IDictionary<int, IList<TextInfo>> ParseHocrFile(IList<FileInfo> inputFiles, IOcrReader.TextPositioning
             textPositioning) {
            IDictionary<int, IList<TextInfo>> imageData = new LinkedDictionary<int, IList<TextInfo>>();
            foreach (FileInfo inputFile in inputFiles) {
                if (inputFile != null && File.Exists(System.IO.Path.Combine(inputFile.FullName))) {
                    Document doc = iText.StyledXmlParser.Jsoup.Jsoup.Parse(new FileStream(inputFile.FullName, FileMode.Open, FileAccess.Read
                        ), encodingUTF8, inputFile.FullName);
                    Elements pages = doc.GetElementsByClass("ocr_page");
                    Regex bboxPattern = iText.IO.Util.StringUtil.RegexCompile(".*bbox(\\s+\\d+){4}.*");
                    Regex bboxCoordinatePattern = iText.IO.Util.StringUtil.RegexCompile(".*\\s+(\\d+)\\s+(\\d+)\\s+(\\d+)\\s+(\\d+).*"
                        );
                    IList<String> searchedClasses = IOcrReader.TextPositioning.byLines.Equals(textPositioning) ? JavaUtil.ArraysAsList
                        ("ocr_line", "ocr_caption") : JavaCollectionsUtil.SingletonList<String>("ocrx_word");
                    foreach (iText.StyledXmlParser.Jsoup.Nodes.Element page in pages) {
                        String[] pageNum = iText.IO.Util.StringUtil.Split(page.Id(), "page_");
                        int pageNumber = Convert.ToInt32(pageNum[pageNum.Length - 1]);
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
                                String value = obj.Attr("title");
                                Match bboxMatcher = iText.IO.Util.StringUtil.Match(bboxPattern, value);
                                if (bboxMatcher.Success) {
                                    Match bboxCoordinateMatcher = iText.IO.Util.StringUtil.Match(bboxCoordinatePattern, iText.IO.Util.StringUtil.Group
                                        (bboxMatcher));
                                    if (bboxCoordinateMatcher.Success) {
                                        IList<float> coordinates = new List<float>();
                                        for (int i = 0; i < 4; i++) {
                                            String coord = iText.IO.Util.StringUtil.Group(bboxCoordinateMatcher, i + 1);
                                            coordinates.Add(float.Parse(coord, System.Globalization.CultureInfo.InvariantCulture));
                                        }
                                        textData.Add(new TextInfo(obj.Text(), coordinates));
                                    }
                                }
                            }
                        }
                        if (textData.Count > 0) {
                            if (imageData.ContainsKey(pageNumber)) {
                                pageNumber = Enumerable.Max(imageData.Keys) + 1;
                            }
                            imageData.Put(pageNumber, textData);
                        }
                    }
                }
            }
            return imageData;
        }

        /// <summary>
        /// Calculate the size of the PDF document page
        /// should transform pixels to points and according to image resolution.
        /// </summary>
        /// <param name="imageData">ImageData</param>
        /// <param name="scaleMode">IPdfRenderer.ScaleMode</param>
        /// <param name="requiredSize">Rectangle</param>
        /// <returns>Rectangle</returns>
        internal static Rectangle CalculateImageSize(ImageData imageData, IPdfRenderer.ScaleMode scaleMode, Rectangle
             requiredSize) {
            // Adjust image size and dpi
            // The resolution of a PDF file is 72pt per inch
            float dotsPerPointX = 1.0f;
            float dotsPerPointY = 1.0f;
            if (imageData != null && imageData.GetDpiX() > 0 && imageData.GetDpiY() > 0) {
                dotsPerPointX = imageData.GetDpiX() / POINTS_PER_INCH;
                dotsPerPointY = imageData.GetDpiY() / POINTS_PER_INCH;
            }
            if (imageData != null) {
                float imgWidthPt = GetPoints(imageData.GetWidth());
                float imgHeightPt = GetPoints(imageData.GetHeight());
                LOGGER.Info("Original image size in pixels: (" + imageData.GetWidth() + ", " + imageData.GetHeight() + ")"
                    );
                if (scaleMode == IPdfRenderer.ScaleMode.keepOriginalSize) {
                    Rectangle size = new Rectangle(imgWidthPt, imgHeightPt);
                    LOGGER.Info("Final size in points: (" + size.GetWidth() + ", " + size.GetHeight() + ")");
                    return size;
                }
                else {
                    Rectangle size = new Rectangle(requiredSize.GetWidth(), requiredSize.GetHeight());
                    // scale image according to the page size and scale mode
                    if (scaleMode == IPdfRenderer.ScaleMode.scaleHeight) {
                        float newHeight = imgHeightPt * requiredSize.GetWidth() / imgWidthPt;
                        size.SetHeight(newHeight);
                    }
                    else {
                        if (scaleMode == IPdfRenderer.ScaleMode.scaleWidth) {
                            float newWidth = imgWidthPt * requiredSize.GetHeight() / imgHeightPt;
                            size.SetWidth(newWidth);
                        }
                        else {
                            if (scaleMode == IPdfRenderer.ScaleMode.scaleToFit) {
                                float ratio = Math.Min(requiredSize.GetWidth() / imgWidthPt, requiredSize.GetHeight() / imgHeightPt);
                                size.SetWidth(imgWidthPt * ratio);
                                size.SetHeight(imgHeightPt * ratio);
                            }
                        }
                    }
                    LOGGER.Info("Final size in points: (" + size.GetWidth() + ", " + size.GetHeight() + ")");
                    return size;
                }
            }
            else {
                return requiredSize;
            }
        }
    }
}

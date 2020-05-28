using System;
using System.Collections.Generic;
using System.IO;
using Common.Logging;
using iText.IO.Image;
using iText.IO.Source;
using iText.IO.Util;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Renderer;

namespace iText.Pdfocr {
    internal class PdfCreatorUtil {
        /// <summary>The Constant to convert pixels to points.</summary>
        internal const float PX_TO_PT = 3f / 4f;

        /// <summary>The Constant for points per inch.</summary>
        private const float POINTS_PER_INCH = 72.0f;

        /// <summary>The logger.</summary>
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(PdfCreatorUtil));

        /// <summary>
        /// Calculates font size according to given bbox height, width and selected
        /// font.
        /// </summary>
        /// <param name="document">
        /// PDF document as a
        /// <see cref="iText.Layout.Document"/>
        /// object
        /// </param>
        /// <param name="line">text line</param>
        /// <param name="font">font for the placed text (could be custom or default)</param>
        /// <param name="bboxHeightPt">height of bbox calculated by OCR Reader</param>
        /// <param name="bboxWidthPt">width of bbox calculated by OCR Reader</param>
        /// <returns>font size</returns>
        internal static float CalculateFontSize(Document document, String line, PdfFont font, float bboxHeightPt, 
            float bboxWidthPt) {
            Rectangle bbox = new Rectangle(bboxWidthPt * 1.5f, bboxHeightPt * 1.5f);
            Paragraph paragraph = new Paragraph(line);
            paragraph.SetWidth(bboxWidthPt);
            paragraph.SetFont(font);
            // setting minimum and maximum (approx.) values for font size
            float fontSize = 1;
            float maxFontSize = bboxHeightPt * 2;
            while (Math.Abs(fontSize - maxFontSize) > 1e-1) {
                float curFontSize = (fontSize + maxFontSize) / 2;
                paragraph.SetFontSize(curFontSize);
                IRenderer renderer = paragraph.CreateRendererSubTree().SetParent(document.GetRenderer());
                LayoutContext context = new LayoutContext(new LayoutArea(1, bbox));
                if (renderer.Layout(context).GetStatus() == LayoutResult.FULL) {
                    fontSize = curFontSize;
                }
                else {
                    maxFontSize = curFontSize;
                }
            }
            return fontSize;
        }

        /// <summary>Calculates image coordinates on the page.</summary>
        /// <param name="size">size of the page</param>
        /// <param name="imageSize">size of the image</param>
        /// <returns>list of two elements (coordinates): first - x, second - y.</returns>
        internal static Point CalculateImageCoordinates(Rectangle size, Rectangle imageSize) {
            float x = 0;
            float y = 0;
            if (size != null) {
                if (imageSize.GetHeight() < size.GetHeight()) {
                    y = (size.GetHeight() - imageSize.GetHeight()) / 2;
                }
                if (imageSize.GetWidth() < size.GetWidth()) {
                    x = (size.GetWidth() - imageSize.GetWidth()) / 2;
                }
            }
            return new Point(x, y);
        }

        /// <summary>
        /// Retrieves
        /// <see cref="iText.IO.Image.ImageData"/>
        /// from the
        /// input
        /// <see cref="System.IO.FileInfo"/>.
        /// </summary>
        /// <param name="inputImage">
        /// input image as
        /// <see cref="System.IO.FileInfo"/>
        /// </param>
        /// <returns>
        /// list of
        /// <see cref="iText.IO.Image.ImageData"/>
        /// objects
        /// (more than one element in the list if it is a multipage tiff)
        /// </returns>
        internal static IList<ImageData> GetImageData(FileInfo inputImage) {
            IList<ImageData> images = new List<ImageData>();
            String ext = "";
            int index = inputImage.FullName.LastIndexOf('.');
            if (index > 0) {
                ext = new String(inputImage.FullName.ToCharArray(), index + 1, inputImage.FullName.Length - index - 1);
                if ("tiff".Equals(ext.ToLowerInvariant()) || "tif".Equals(ext.ToLowerInvariant())) {
                    int tiffPages = GetNumberOfPageTiff(inputImage);
                    for (int page = 0; page < tiffPages; page++) {
                        byte[] bytes = System.IO.File.ReadAllBytes(inputImage.FullName);
                        ImageData imageData = ImageDataFactory.CreateTiff(bytes, true, page + 1, true);
                        images.Add(imageData);
                    }
                }
                else {
                    try {
                        ImageData imageData = ImageDataFactory.Create(inputImage.FullName);
                        images.Add(imageData);
                    }
                    catch (iText.IO.IOException e) {
                        LOGGER.Error(MessageFormatUtil.Format(PdfOcrLogMessageConstant.CANNOT_READ_INPUT_IMAGE, e.Message));
                        throw new OcrException(OcrException.CANNOT_READ_INPUT_IMAGE, e);
                    }
                }
            }
            return images;
        }

        /// <summary>
        /// Calculates the size of the PDF document page according to the provided
        /// <see cref="ScaleMode"/>.
        /// </summary>
        /// <param name="imageData">
        /// input image or its one page as
        /// <see cref="iText.IO.Image.ImageData"/>
        /// </param>
        /// <param name="scaleMode">
        /// required
        /// <see cref="ScaleMode"/>
        /// that could be
        /// set using
        /// <see cref="OcrPdfCreatorProperties.SetScaleMode(ScaleMode)"/>
        /// method
        /// </param>
        /// <param name="requiredSize">
        /// size of the page that could be using
        /// <see cref="OcrPdfCreatorProperties.SetPageSize(iText.Kernel.Geom.Rectangle)"/>
        /// method
        /// </param>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// </returns>
        internal static Rectangle CalculateImageSize(ImageData imageData, ScaleMode scaleMode, Rectangle requiredSize
            ) {
            if (imageData != null) {
                float imgWidthPt = GetPoints(imageData.GetWidth());
                float imgHeightPt = GetPoints(imageData.GetHeight());
                // page size will be equal to the image size if page size or
                // scale mode are not set
                if (requiredSize == null || scaleMode == null) {
                    return new Rectangle(imgWidthPt, imgHeightPt);
                }
                else {
                    Rectangle size = new Rectangle(requiredSize.GetWidth(), requiredSize.GetHeight());
                    // scale image according to the page size and scale mode
                    if (scaleMode == ScaleMode.SCALE_HEIGHT) {
                        float newHeight = imgHeightPt * requiredSize.GetWidth() / imgWidthPt;
                        size.SetHeight(newHeight);
                    }
                    else {
                        if (scaleMode == ScaleMode.SCALE_WIDTH) {
                            float newWidth = imgWidthPt * requiredSize.GetHeight() / imgHeightPt;
                            size.SetWidth(newWidth);
                        }
                        else {
                            if (scaleMode == ScaleMode.SCALE_TO_FIT) {
                                float ratio = Math.Min(requiredSize.GetWidth() / imgWidthPt, requiredSize.GetHeight() / imgHeightPt);
                                size.SetWidth(imgWidthPt * ratio);
                                size.SetHeight(imgHeightPt * ratio);
                            }
                        }
                    }
                    return size;
                }
            }
            else {
                return requiredSize;
            }
        }

        /// <summary>Converts value from pixels to points.</summary>
        /// <param name="pixels">input value in pixels</param>
        /// <returns>result value in points</returns>
        internal static float GetPoints(float pixels) {
            return pixels * PX_TO_PT;
        }

        /// <summary>Counts number of pages in the provided tiff image.</summary>
        /// <param name="inputImage">
        /// input image
        /// <see cref="System.IO.FileInfo"/>
        /// </param>
        /// <returns>number of pages in the provided TIFF image</returns>
        private static int GetNumberOfPageTiff(FileInfo inputImage) {
            RandomAccessFileOrArray raf = new RandomAccessFileOrArray(new RandomAccessSourceFactory().CreateBestSource
                (inputImage.FullName));
            int numOfPages = TiffImageData.GetNumberOfPages(raf);
            raf.Close();
            return numOfPages;
        }
    }
}

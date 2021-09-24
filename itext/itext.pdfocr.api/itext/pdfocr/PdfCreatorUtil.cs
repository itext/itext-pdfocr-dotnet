/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.IO.Image;
using iText.IO.Source;
using iText.Kernel.Geom;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Renderer;
using iText.Pdfocr.Exceptions;
using iText.Pdfocr.Logs;

namespace iText.Pdfocr {
    internal class PdfCreatorUtil {
        /// <summary>The Constant to convert pixels to points.</summary>
        internal const float PX_TO_PT = 3f / 4f;

        /// <summary>The Constant for points per inch.</summary>
        private const float POINTS_PER_INCH = 72.0f;

        /// <summary>The logger.</summary>
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(PdfCreatorUtil));

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
        /// <param name="fontFamily">default font family</param>
        /// <param name="bboxHeightPt">height of bbox calculated by OCR Reader</param>
        /// <param name="bboxWidthPt">width of bbox calculated by OCR Reader</param>
        /// <returns>font size</returns>
        internal static float CalculateFontSize(Document document, String line, String fontFamily, float bboxHeightPt
            , float bboxWidthPt) {
            Rectangle bbox = new Rectangle(bboxWidthPt * 1.5f, bboxHeightPt * 1.5f);
            // setting minimum and maximum (approx.) values for font size
            float fontSize = 1;
            float maxFontSize = bbox.GetHeight();
            try {
                Paragraph paragraph = new Paragraph(line);
                paragraph.SetWidth(bbox.GetWidth());
                paragraph.SetFontFamily(fontFamily);
                while (Math.Abs(fontSize - maxFontSize) > 1e-1) {
                    float curFontSize = (fontSize + maxFontSize) / 2;
                    paragraph.SetFontSize(curFontSize);
                    ParagraphRenderer renderer = (ParagraphRenderer)paragraph.CreateRendererSubTree().SetParent(document.GetRenderer
                        ());
                    LayoutContext context = new LayoutContext(new LayoutArea(1, bbox));
                    if (renderer.Layout(context).GetStatus() == LayoutResult.FULL && renderer.GetLines().Count == 1) {
                        fontSize = curFontSize;
                    }
                    else {
                        maxFontSize = curFontSize;
                    }
                }
            }
            catch (InvalidOperationException e) {
                LOGGER.LogError(PdfOcrLogMessageConstant.PROVIDED_FONT_PROVIDER_IS_INVALID);
                throw new OcrException(OcrException.CANNOT_RESOLVE_PROVIDED_FONTS, e);
            }
            return fontSize;
        }

        /// <summary>
        /// Calculated real width of a paragraph with given text line, font provider
        /// and font size.
        /// </summary>
        /// <param name="document">
        /// PDF document as a
        /// <see cref="iText.Layout.Document"/>
        /// object
        /// </param>
        /// <param name="line">text line</param>
        /// <param name="fontFamily">default font family</param>
        /// <param name="fontSize">calculated font size</param>
        /// <returns>real width of text line in paragraph</returns>
        internal static float GetRealLineWidth(Document document, String line, String fontFamily, float fontSize) {
            Paragraph paragraph = new Paragraph(line);
            paragraph.SetFontFamily(fontFamily);
            paragraph.SetFontSize(fontSize);
            IRenderer renderer = paragraph.CreateRendererSubTree().SetParent(document.GetRenderer());
            return ((ParagraphRenderer)renderer).GetMinMaxWidth().GetMaxWidth();
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
        /// <param name="imageRotationHandler">
        /// image rotation handler
        /// <see cref="IImageRotationHandler"/>
        /// </param>
        /// <returns>
        /// list of
        /// <see cref="iText.IO.Image.ImageData"/>
        /// objects
        /// (more than one element in the list if it is a multipage tiff)
        /// </returns>
        internal static IList<ImageData> GetImageData(FileInfo inputImage, IImageRotationHandler imageRotationHandler
            ) {
            IList<ImageData> images = new List<ImageData>();
            try {
                using (Stream imageStream = new FileStream(inputImage.FullName, FileMode.Open, FileAccess.Read)) {
                    ImageType imageType = ImageTypeDetector.DetectImageType(imageStream);
                    if (ImageType.TIFF == imageType) {
                        int tiffPages = GetNumberOfPageTiff(inputImage);
                        for (int page = 0; page < tiffPages; page++) {
                            byte[] bytes = File.ReadAllBytes(inputImage.FullName);
                            ImageData imageData = ImageDataFactory.CreateTiff(bytes, true, page + 1, true);
                            if (imageRotationHandler != null) {
                                imageData = imageRotationHandler.ApplyRotation(imageData);
                            }
                            images.Add(imageData);
                        }
                    }
                    else {
                        ImageData imageData = ImageDataFactory.Create(inputImage.FullName);
                        if (imageRotationHandler != null) {
                            imageData = imageRotationHandler.ApplyRotation(imageData);
                        }
                        images.Add(imageData);
                    }
                }
            }
            catch (System.IO.IOException e) {
                LOGGER.LogError(MessageFormatUtil.Format(PdfOcrLogMessageConstant.CANNOT_READ_INPUT_IMAGE, e.Message));
                throw new OcrException(OcrException.CANNOT_READ_INPUT_IMAGE, e);
            }
            catch (iText.IO.Exceptions.IOException e) {
                LOGGER.LogError(MessageFormatUtil.Format(PdfOcrLogMessageConstant.CANNOT_READ_INPUT_IMAGE, e.Message));
                throw new OcrException(OcrException.CANNOT_READ_INPUT_IMAGE, e);
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

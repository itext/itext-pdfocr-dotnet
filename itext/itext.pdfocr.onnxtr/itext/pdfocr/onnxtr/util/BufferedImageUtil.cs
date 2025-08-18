/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using System.Runtime.InteropServices;
using iText.Commons.Utils;
using iText.Pdfocr.Onnxtr.Exceptions;
using OpenCvSharp;

namespace iText.Pdfocr.Onnxtr.Util {
    /// <summary>
    /// Additional algorithms for working with
    /// <see cref="SkiaSharp.SKBitmap"/>.
    /// </summary>
    public sealed class BufferedImageUtil {
        private BufferedImageUtil() {
        }

        /// <summary>Converts a collection of images to a batched ML model input in a BCHW format with 3 channels.</summary>
        /// <remarks>
        /// Converts a collection of images to a batched ML model input in a BCHW format with 3 channels.
        /// This does aspect-preserving image resizing to fit the input shape.
        /// </remarks>
        /// <param name="images">collection of images to convert to model input</param>
        /// <param name="properties">model input properties</param>
        /// <returns>batched BCHW model input MD-array</returns>
        public static FloatBufferMdArray ToBchwInput(ICollection<IronSoftware.Drawing.AnyBitmap> images, OnnxInputProperties
             properties) {
            // Currently properties guarantee RGB, this is just in case this changes later
            if (properties.GetChannelCount() != 3) {
                throw new ArgumentException(PdfOcrOnnxTrExceptionMessageConstant.ONLY_SUPPORT_RGB_IMAGES);
            }
            if (images.Count > properties.GetBatchSize()) {
                throw new ArgumentException(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.TOO_MANY_IMAGES, 
                    images.Count, properties.GetBatchSize()));
            }
            long[] inputShape = new long[] { images.Count, properties.GetChannelCount(), properties.GetHeight(), properties
                .GetWidth() };
            int bufferSize = CalculateBufferCapacity(inputShape);
            float[] inputData = new float[bufferSize / sizeof(float)];

            int currentIndex = 0;
            foreach (IronSoftware.Drawing.AnyBitmap image in images) {
                using (SkiaSharp.SKBitmap resizedImage = Resize(image, properties.GetWidth(), properties.GetHeight(),
                           properties.UseSymmetricPad())) {
                    using (SkiaSharp.SKPixmap raster = resizedImage.PeekPixels()) {
                        byte[] pixelBytes = new byte[raster.BytesSize];
                        Marshal.Copy(raster.GetPixels(), pixelBytes, 0, pixelBytes.Length);

                        int stride = raster.RowBytes;
                        int bytesPerPixel = raster.Info.BytesPerPixel;

                        // 1. R
                        for (int y = 0; y < properties.GetHeight(); y++) {
                            int rowStart = y * stride;
                            for (int x = 0; x < properties.GetWidth(); x++) {
                                int index = rowStart + x * bytesPerPixel;
                                float r = pixelBytes[index + 2] / 255f;
                                inputData[currentIndex++] = (r - properties.GetRedMean()) / properties.GetRedStd();
                            }
                        }

                        // 2. G
                        for (int y = 0; y < properties.GetHeight(); y++) {
                            int rowStart = y * stride;
                            for (int x = 0; x < properties.GetWidth(); x++) {
                                int index = rowStart + x * bytesPerPixel;
                                float g = pixelBytes[index + 1] / 255f;
                                inputData[currentIndex++] = (g - properties.GetGreenMean()) / properties.GetGreenStd();
                            }
                        }

                        // 3. B
                        for (int y = 0; y < properties.GetHeight(); y++) {
                            int rowStart = y * stride;
                            for (int x = 0; x < properties.GetWidth(); x++) {
                                int index = rowStart + x * bytesPerPixel;
                                float b = pixelBytes[index] / 255f;
                                inputData[currentIndex++] = (b - properties.GetBlueMean()) / properties.GetBlueStd();
                            }
                        }
                    }
                }
            }
            return new FloatBufferMdArray(inputData, inputShape);
        }

        /// <summary>Rotates image based on text orientation.</summary>
        /// <remarks>Rotates image based on text orientation. If no rotation necessary, same image is returned.</remarks>
        /// <param name="image">image to rotate</param>
        /// <param name="orientation">text orientation used to rotate the image</param>
        /// <returns>new rotated image, or same image, if no rotation is required</returns>
        public static IronSoftware.Drawing.AnyBitmap Rotate(IronSoftware.Drawing.AnyBitmap image, TextOrientation orientation) {
            if (orientation == TextOrientation.HORIZONTAL) {
                return image;
            }
            int oldW = BufferedImageUtil.GetWidth(image);
            int oldH = BufferedImageUtil.GetHeight(image);
            int newW;
            int newH;
            double angle;
            if (orientation == TextOrientation.HORIZONTAL_ROTATED_180) {
                newW = oldW;
                newH = oldH;
                angle = 180;
            }
            else {
                newW = oldH;
                newH = oldW;
                if (orientation == TextOrientation.HORIZONTAL_ROTATED_90) {
                    angle = 90;
                }
                else {
                    angle = 270;
                }
            }
            SkiaSharp.SKBitmap rotated = 
                new SkiaSharp.SKBitmap(newW, newH, SkiaSharp.SKColorType.Bgra8888, SkiaSharp.SKAlphaType.Premul);
            using (SkiaSharp.SKCanvas graphics = new SkiaSharp.SKCanvas(rotated)) {
                graphics.Translate((float)((newW - oldW) / 2.0), (float)((newH - oldH) / 2.0));
                float centerX = BufferedImageUtil.GetWidth(image) / 2.0f;
                float centerY = BufferedImageUtil.GetHeight(image) / 2.0f;
                graphics.Translate(centerX, centerY);
                graphics.RotateDegrees((float)angle);
                graphics.Translate(-centerX, -centerY);
                graphics.DrawImage(image, 0, 0);
            }
            return rotated;
        }

        /// <summary>Extracts sub-images from an image, based on provided rotated 4-point boxes.</summary>
        /// <remarks>
        /// Extracts sub-images from an image, based on provided rotated 4-point boxes. Sub-images are
        /// transformed to fit the whole image without (in our use cases it is just rotation).
        /// </remarks>
        /// <param name="image">original image to be used for extraction</param>
        /// <param name="boxes">list of 4-point boxes. Points should be in the following order: BL, TL, TR, BR</param>
        /// <returns>list of extracted image boxes</returns>
        public static IList<IronSoftware.Drawing.AnyBitmap> ExtractBoxes(IronSoftware.Drawing.AnyBitmap image, 
            ICollection<iText.Kernel.Geom.Point[]> boxes) {
            IList<IronSoftware.Drawing.AnyBitmap> boxesImages = new List<IronSoftware.Drawing.AnyBitmap>(boxes.Count);

            using (Mat imageMat = iText.Pdfocr.Onnxtr.Util.BufferedImageUtil.ToRgbMat(image)) {
                foreach (iText.Kernel.Geom.Point[] box in boxes) {
                    float boxWidth = (float)box[1].Distance(box[2]);
                    float boxHeight = (float)box[1].Distance(box[0]);
                    using (Mat transformationMat = CalculateBoxTransformationMat(box, boxWidth, boxHeight)) {
                        using (Mat boxImageMat = new Mat((int)boxHeight, (int)boxWidth, MatType.CV_8UC4)) {
                            OpenCvSharp.Size size = new OpenCvSharp.Size((int)boxWidth, (int)boxHeight);
                            Cv2.WarpAffine(imageMat, boxImageMat, transformationMat, size);
                            boxesImages.Add(iText.Pdfocr.Onnxtr.Util.BufferedImageUtil.FromRgbMat(boxImageMat));
                        }
                    }
                }
            }
            return boxesImages;
        }

        /// <summary>Gets width of an image.</summary>
        /// <remarks>
        /// IronSoftware.Drawing.AnyBitmap.Width uses SixLabors.ImageSharp under the hood to load the image size info,
        /// but in that case image is auto oriented and sometimes page is rotated causing swapped width and height.
        /// That's why we always use SkiaSharp implementation to get correct size we work with.
        /// </remarks>
        /// <param name="image">image to get width</param>
        /// <returns>image width</returns>
        public static int GetWidth(IronSoftware.Drawing.AnyBitmap image) {
            return ((SkiaSharp.SKBitmap)image).Width;
        }

        /// <summary>Gets height of an image.</summary>
        /// <remarks>
        /// IronSoftware.Drawing.AnyBitmap.Height uses SixLabors.ImageSharp under the hood to load the image size info,
        /// but in that case image is auto oriented and sometimes page is rotated causing swapped width and height.
        /// That's why we always use SkiaSharp implementation to get correct size we work with.
        /// </remarks>
        /// <param name="image">image to get height</param>
        /// <returns>image height</returns>
        public static int GetHeight(IronSoftware.Drawing.AnyBitmap image) {
            return ((SkiaSharp.SKBitmap)image).Height;
        }

        /// <summary>Converts an image to an RGBA Mat for use in OpenCV.</summary>
        /// <param name="image">image to convert</param>
        /// <returns>RGBA 8UC4 OpenCV Mat with the image</returns>
        private static Mat ToRgbMat(IronSoftware.Drawing.AnyBitmap image) {
            int width = BufferedImageUtil.GetWidth(image);
            int height = BufferedImageUtil.GetHeight(image);
            Mat resultMat = new Mat(height, width, MatType.CV_8UC4);

            SkiaSharp.SKBitmap bgraImage = GetBgraBitmap(image);
            using (SkiaSharp.SKPixmap bitmapData = ((SkiaSharp.SKBitmap)bgraImage).PeekPixels()) {

                int stride = bitmapData.RowBytes;
                int effectiveWidth = width * bitmapData.Info.BytesPerPixel;

                for (int y = 0; y < height; y++) {
                    IntPtr srcLine = bitmapData.GetPixels() + y * stride;
                    IntPtr targetLine = resultMat.Ptr(y);

                    byte[] lineData = new byte[effectiveWidth];
                    Marshal.Copy(srcLine, lineData, 0, effectiveWidth);
                    Marshal.Copy(lineData, 0, targetLine, effectiveWidth);
                }
            }

            return resultMat;
        }

        /// <summary>Converts an RGBA 8UC4 OpenCV Mat to a buffered image.</summary>
        /// <param name="rgb">RGBA 8UC4 OpenCV Mat to convert</param>
        /// <returns>buffered image based on Mat</returns>
        private static IronSoftware.Drawing.AnyBitmap FromRgbMat(Mat rgb) {
            if (rgb.Type() != MatType.CV_8UC4) {
                throw new ArgumentException(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_MAT_TYPE
                    , rgb.Type()));
            }
            SkiaSharp.SKBitmap image = new SkiaSharp.SKBitmap(rgb.Cols, rgb.Rows, 
                SkiaSharp.SKColorType.Bgra8888, SkiaSharp.SKAlphaType.Premul);

            var indexer = rgb.GetGenericIndexer<Vec4b>();
            for (int y = 0; y < rgb.Height; y++) {
                for (int x = 0; x < rgb.Width; x++) {
                    Vec4b color = indexer[y, x];
                    image.SetPixel(x, y, new SkiaSharp.SKColor(
                        color.Item0, // R
                        color.Item1, // G
                        color.Item2,  // B
                        color.Item3 // A
                    ));
                }
            }
            return image;
        }

        /// <summary>Creates a new image with an aspect ratio preserving resize.</summary>
        /// <remarks>Creates a new image with an aspect ratio preserving resize. New blank pixel will have black color.
        ///     </remarks>
        /// <param name="image">image to resize</param>
        /// <param name="width">target width</param>
        /// <param name="height">target height</param>
        /// <param name="symmetricPad">whether padding should be symmetric or should it be bottom-right</param>
        /// <returns>new resized image</returns>
        private static SkiaSharp.SKBitmap Resize(IronSoftware.Drawing.AnyBitmap image, int width, int height, 
            bool symmetricPad) {
            // It is pretty unlikely, that the image is already the correct size, so no need for an exception
            SkiaSharp.SKBitmap result = 
                new SkiaSharp.SKBitmap(width, height, SkiaSharp.SKColorType.Bgra8888, SkiaSharp.SKAlphaType.Premul);
            using (SkiaSharp.SKCanvas graphics = new SkiaSharp.SKCanvas(result)) {
                // Decided to use white background color for the cases when the image is transparent (e.g. PNG),
                // see WeirdWordsDoImageOcrTest for example. With transparent background text is not recognized,
                // on the black one the most frequent black text won't be visible.
                graphics.Clear(SkiaSharp.SKColors.White);

                int sourceWidth = BufferedImageUtil.GetWidth(image);
                int sourceHeight = BufferedImageUtil.GetHeight(image);
                double widthRatio = (double)width / sourceWidth;
                double heightRatio = (double)height / sourceHeight;
                using (SkiaSharp.SKPaint fillPaint = new SkiaSharp.SKPaint()) {
                    fillPaint.Color = SkiaSharp.SKColors.Black;
                    fillPaint.Style = SkiaSharp.SKPaintStyle.Fill;
                    fillPaint.IsAntialias = true;
                    using (SkiaSharp.SKPaint imagePaint = new SkiaSharp.SKPaint()) {
                        SkiaSharp.SKSamplingOptions options = new SkiaSharp.SKSamplingOptions(
                            SkiaSharp.SKFilterMode.Linear, SkiaSharp.SKMipmapMode.Linear);
                        imagePaint.IsAntialias = true;
                        if (heightRatio > widthRatio) {
                            int scaledHeight = (int)MathematicUtil.Round(sourceHeight * widthRatio);
                            int yPos;
                            if (symmetricPad) {
                                yPos = (height - scaledHeight) / 2;
                                graphics.DrawRect(0, 0, width, yPos, fillPaint);
                            } else {
                                yPos = 0;
                            }

                            graphics.DrawRect(0, yPos + scaledHeight, width, height - scaledHeight - yPos, fillPaint);
                            graphics.DrawImage(image, new SkiaSharp.SKRect(0, yPos, width, yPos + scaledHeight), 
                                options, imagePaint);
                        } else {
                            int scaledWidth = (int)MathematicUtil.Round(sourceWidth * heightRatio);
                            int xPos;
                            if (symmetricPad) {
                                xPos = (width - scaledWidth) / 2;
                                graphics.DrawRect(0, 0, xPos, height, new SkiaSharp.SKPaint());
                            } else {
                                xPos = 0;
                            }

                            graphics.DrawRect(xPos + scaledWidth, 0, width - scaledWidth - xPos, height, fillPaint);
                            graphics.DrawImage(image, 
                                new SkiaSharp.SKRect(xPos, 0, xPos + scaledWidth, height), options, imagePaint);
                        }
                    }
                }
            }
            return result;
        }

        private static Mat CalculateBoxTransformationMat(iText.Kernel.Geom.Point[] box, float boxWidth, float boxHeight) {
            using (Mat srcPoints = new Mat(3, 2, MatType.CV_32F)) {
                using (Mat dstPoints = new Mat(3, 2, MatType.CV_32F)) {
                    for (int i = 0; i < 3; ++i) {
                        srcPoints.Set<float>(i, 0, (float)box[i].GetX());
                        srcPoints.Set<float>(i, 1, (float)box[i].GetY());
                    }

                    dstPoints.Set<float>(0, 0, 0F);
                    dstPoints.Set<float>(0, 1, boxHeight - 1);
                    dstPoints.Set<float>(1, 0, 0F);
                    dstPoints.Set<float>(1, 1, 0F);
                    dstPoints.Set<float>(2, 0, boxWidth - 1);
                    dstPoints.Set<float>(2, 1, 0F);
                    return Cv2.GetAffineTransform(srcPoints, dstPoints);
                }
            }
        }

        /// <summary>Returns the byte capacity required for a float32 buffer of the specified shape.</summary>
        /// <param name="shape">shape of the MD-array</param>
        /// <returns>the byte capacity required for a float32 buffer of the specified shape</returns>
        private static int CalculateBufferCapacity(long[] shape) {
            int capacity = sizeof(float);
            foreach (long dim in shape) {
                capacity *= (int)dim;
            }
            return capacity;
        }

        /// <summary>Converts image to BGRA pixel format.</summary>
        /// <param name="image">IronSoftware.Drawing.AnyBitmap to convert</param>
        /// <returns>SkiaSharp.SKBitmap in BGRA pixel format</returns>
        private static SkiaSharp.SKBitmap GetBgraBitmap(IronSoftware.Drawing.AnyBitmap image) {
            SkiaSharp.SKBitmap skiaImage = (SkiaSharp.SKBitmap)image;
            if (skiaImage.ColorType == SkiaSharp.SKColorType.Bgra8888) {
                return skiaImage;
            }
            SkiaSharp.SKBitmap bgraImage = new SkiaSharp.SKBitmap(skiaImage.Width, skiaImage.Height,
                SkiaSharp.SKColorType.Bgra8888, SkiaSharp.SKAlphaType.Premul);
            using (SkiaSharp.SKCanvas graphics = new SkiaSharp.SKCanvas(bgraImage)) {
                graphics.DrawBitmap(image, 0, 0);
            }
            return bgraImage;
        }
    }
}

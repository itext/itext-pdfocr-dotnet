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
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
#if !NETSTANDARD2_0
using System.Drawing;
#endif // !NETSTANDARD2_0
using iText.Commons.Utils;
using OpenCvSharp;
using Point = iText.Kernel.Geom.Point;
using Rectangle = System.Drawing.Rectangle;

namespace iText.Pdfocr.Onnxtr.Util {
    /// <summary>
    /// Additional algorithms for working with
    /// <see cref="System.Drawing.Bitmap"/>.
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
        public static FloatBufferMdArray ToBchwInput(ICollection<System.Drawing.Bitmap> images, OnnxInputProperties
             properties) {
            // Currently properties guarantee RGB, this is just in case this changes later
            if (properties.GetChannelCount() != 3) {
                throw new ArgumentException("toBchwInput only support RGB images");
            }
            if (images.Count > properties.GetBatchSize()) {
                throw new ArgumentException("Too many images (" + images.Count + ") " + "for the provided batch size (" + 
                    properties.GetBatchSize() + ")");
            }
            long[] inputShape = new long[] { images.Count, properties.GetChannelCount(), properties.GetHeight(), properties
                .GetWidth() };
            /*
             * It is important to do it via ByteBuffer with allocateDirect. If the
             * buffer is non-direct, it will allocate a direct buffer within the
             * ONNX runtime and copy the buffer there instead. So we will waste
             * twice the memory for no reason.
             *
             * For some reason there doesn't seem to be a way to allocate a direct
             * buffer via FloatBuffer itself...
             */
            int bufferSize = CalculateBufferCapacity(inputShape);
            float[] inputData = new float[bufferSize / sizeof(float)];
            int imageSize = properties.GetWidth() * properties.GetHeight();

            int offset = 0;
            foreach (System.Drawing.Bitmap image in images) {
                using (System.Drawing.Bitmap resizedImage = Resize(image, properties.GetWidth(), properties.GetHeight(),
                           properties.UseSymmetricPad())) {
                    var rect = new Rectangle(0, 0, resizedImage.Width, resizedImage.Height);
                    BitmapData raster = resizedImage.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                    try {
                        int stride = raster.Stride;
                        byte[] pixelBytes = new byte[stride * resizedImage.Height];
                        Marshal.Copy(raster.Scan0, pixelBytes, 0, pixelBytes.Length);

                        int baseIndex = offset * 3 * imageSize;

                        // 1. R
                        for (int y = 0; y < properties.GetHeight(); y++) {
                            for (int x = 0; x < properties.GetWidth(); x++) {
                                int index = y * stride + x * 3 + 2; 
                                float r = pixelBytes[index] / 255f;
                                inputData[baseIndex + y * properties.GetWidth() + x] =
                                    (r - properties.GetRedMean()) / properties.GetRedStd();
                            }
                        }

                        // 2. G
                        int greenOffset = imageSize;
                        for (int y = 0; y < properties.GetHeight(); y++) {
                            for (int x = 0; x < properties.GetWidth(); x++) {
                                int index = y * stride + x * 3 + 1; 
                                float g = pixelBytes[index] / 255f;
                                inputData[baseIndex + greenOffset + y * properties.GetWidth() + x] =
                                    (g - properties.GetGreenMean()) / properties.GetGreenStd();
                            }
                        }

                        // 3. B
                        int blueOffset = 2 * imageSize;
                        for (int y = 0; y < properties.GetHeight(); y++) {
                            for (int x = 0; x < properties.GetWidth(); x++) {
                                int index = y * stride + x * 3; 
                                float b = pixelBytes[index] / 255f;
                                inputData[baseIndex + blueOffset + y * properties.GetWidth() + x] =
                                    (b - properties.GetBlueMean()) / properties.GetBlueStd();
                            }
                        }
                    }
                    finally {
                        resizedImage.UnlockBits(raster);
                    }
                    offset++;
                }
            }
            return new FloatBufferMdArray(inputData, inputShape);
        }

        /// <summary>Converts an image to an RGB Mat for use in OpenCV.</summary>
        /// <param name="image">image to convert</param>
        /// <returns>RGB 8UC3 OpenCV Mat with the image</returns>
        public static Mat ToRgbMat(System.Drawing.Bitmap image) {
            int width = image.Width;
            int height = image.Height;
            Mat resultMat = new Mat(height, width, MatType.CV_8UC3);
            
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData bitmapData = image.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            try {
                int stride = bitmapData.Stride;
                int effectiveWidth = width * 3;

                for (int y = 0; y < height; y++) {
                    IntPtr srcLine = bitmapData.Scan0 + y * stride;
                    IntPtr targetLine = resultMat.Ptr(y);

                    byte[] lineData = new byte[effectiveWidth];
                    Marshal.Copy(srcLine, lineData, 0, effectiveWidth);
                    Marshal.Copy(lineData, 0, targetLine, effectiveWidth);
                }
            }
            finally {
                image.UnlockBits(bitmapData);
            }
            return resultMat;
        }

        /// <summary>Converts an RGB 8UC3 OpenCV Mat to a buffered image.</summary>
        /// <param name="rgb">RGB 8UC3 OpenCV Mat to convert</param>
        /// <returns>buffered image based on Mat</returns>
        public static System.Drawing.Bitmap FromRgbMat(Mat rgb) {
            if (rgb.Type() != MatType.CV_8UC3) {
                throw new ArgumentException("Unexpected Mat type");
            }
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(rgb.Cols, rgb.Rows, PixelFormat.Format24bppRgb
                );

            var indexer = rgb.GetGenericIndexer<Vec3b>();
            for (int y = 0; y < rgb.Height; y++) {
                for (int x = 0; x < rgb.Width; x++) {
                    Vec3b color = indexer[y, x];
                    image.SetPixel(x, y, Color.FromArgb(
                        color.Item2, // R
                        color.Item1, // G
                        color.Item0  // B
                    ));
                }
            }
            return image;
        }

        /// <summary>Rotates image based on text orientation.</summary>
        /// <remarks>Rotates image based on text orientation. If no rotation necessary, same image is returned.</remarks>
        /// <param name="image">image to rotate</param>
        /// <param name="orientation">text orientation used to rotate the image</param>
        /// <returns>new rotated image, or same image, if no rotation is required</returns>
        public static System.Drawing.Bitmap Rotate(System.Drawing.Bitmap image, TextOrientation orientation) {
            if (orientation == TextOrientation.HORIZONTAL) {
                return image;
            }
            int oldW = image.Width;
            int oldH = image.Height;
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
            System.Drawing.Bitmap rotated = new System.Drawing.Bitmap(newW, newH, PixelFormat.Format24bppRgb);
            using (Graphics graphics = Graphics.FromImage(rotated)) {
                graphics.TranslateTransform((float)((newW - oldW) / 2.0), (float)((newH - oldH) / 2.0));
                float centerX = image.Width / 2.0f;
                float centerY = image.Height / 2.0f;
                graphics.TranslateTransform(centerX, centerY);
                graphics.RotateTransform((float)angle);
                graphics.TranslateTransform(-centerX, -centerY);
                graphics.DrawImage(image, 0, 0);
            }
            return rotated;
        }

        /// <summary>Creates a new image with an aspect ratio preserving resize.</summary>
        /// <remarks>Creates a new image with an aspect ratio preserving resize. New blank pixel will have black color.
        ///     </remarks>
        /// <param name="image">image to resize</param>
        /// <param name="width">target width</param>
        /// <param name="height">target height</param>
        /// <param name="symmetricPad">whether padding should be symmetric or should it be bottom-right</param>
        /// <returns>new resized image</returns>
        public static System.Drawing.Bitmap Resize(System.Drawing.Bitmap image, int width, int height, bool symmetricPad
            ) {
            // It is pretty unlikely, that the image is already the correct size, so no need for an exception
            System.Drawing.Bitmap result = new System.Drawing.Bitmap(width, height, PixelFormat.Format24bppRgb);
            using (Graphics graphics = Graphics.FromImage(result)) {
                graphics.Clear(Color.Black);
                graphics.InterpolationMode = InterpolationMode.Bilinear;

                int sourceWidth = image.Width;
                int sourceHeight = image.Height;
                double widthRatio = (double)width / sourceWidth;
                double heightRatio = (double)height / sourceHeight;
                if (heightRatio > widthRatio) {
                    int scaledHeight = (int)MathematicUtil.Round(sourceHeight * widthRatio);
                    int yPos;
                    if (symmetricPad) {
                        yPos = (height - scaledHeight) / 2;
                        graphics.FillRectangle(new SolidBrush(Color.Black), 0, 0, width, yPos);
                    }
                    else {
                        yPos = 0;
                    }

                    graphics.FillRectangle(new SolidBrush(Color.Black), 0, yPos + scaledHeight, width, height - scaledHeight - yPos);
                    graphics.DrawImage(image, 0, yPos, width, scaledHeight);
                }
                else {
                    int scaledWidth = (int)MathematicUtil.Round(sourceWidth * heightRatio);
                    int xPos;
                    if (symmetricPad) {
                        xPos = (width - scaledWidth) / 2;
                        graphics.FillRectangle(new SolidBrush(Color.Black), 0, 0, xPos, height);
                    }
                    else {
                        xPos = 0;
                    }

                    graphics.FillRectangle(new SolidBrush(Color.Black), xPos + scaledWidth, 0, width - scaledWidth - xPos, height);
                    graphics.DrawImage(image, xPos, 0, scaledWidth, height);
                }
            }

            return result;
        }

        /// <summary>Extracts sub-images from an image, based on provided rotated 4-point boxes.</summary>
        /// <remarks>
        /// Extracts sub-images from an image, based on provided rotated 4-point boxes. Sub-images are
        /// transformed to fit the whole image without (in our use cases it is just rotation).
        /// </remarks>
        /// <param name="image">original image to be used for extraction</param>
        /// <param name="boxes">list of 4-point boxes. Points should be in the following order: BL, TL, TR, BR</param>
        /// <returns>list of extracted image boxes</returns>
        public static IList<System.Drawing.Bitmap> ExtractBoxes(System.Drawing.Bitmap image, ICollection<Point[]> 
            boxes) {
            IList<System.Drawing.Bitmap> boxesImages = new List<System.Drawing.Bitmap>(boxes.Count);
            using (Mat imageMat = iText.Pdfocr.Onnxtr.Util.BufferedImageUtil.ToRgbMat(image)) {
                foreach (Point[] box in boxes) {
                    float boxWidth = (float)box[1].Distance(box[2]);
                    float boxHeight = (float)box[1].Distance(box[0]);
                    using (Mat transformationMat = CalculateBoxTransformationMat(box, boxWidth, boxHeight)) {
                        using (Mat boxImageMat = new Mat((int)boxHeight, (int)boxWidth, MatType.CV_8UC3)) {
                            OpenCvSharp.Size size = new OpenCvSharp.Size((int)boxWidth, (int)boxHeight);
                            Cv2.WarpAffine(imageMat, boxImageMat, transformationMat, size);
                            boxesImages.Add(iText.Pdfocr.Onnxtr.Util.BufferedImageUtil.FromRgbMat(boxImageMat));
                        }
                    }
                }
            }
            return boxesImages;
        }

        private static Mat CalculateBoxTransformationMat(Point[] box, float boxWidth, float boxHeight) {
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
    }
}

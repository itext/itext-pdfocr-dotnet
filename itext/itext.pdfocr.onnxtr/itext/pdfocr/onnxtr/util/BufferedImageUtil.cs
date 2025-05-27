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
#if !NETSTANDARD2_0
using System.Drawing;
#endif // !NETSTANDARD2_0
using Java.Awt;
using Java.Awt.Image;
using Org.Bytedeco.Javacpp.Indexer;
using Org.Bytedeco.Opencv.Global;
using Org.Bytedeco.Opencv.Opencv_core;
using Org.Opencv.Core;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Pdfocr;
using iText.Pdfocr.Onnxtr;

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
        /// <param name="images">Collection of images to convert to model input.</param>
        /// <param name="properties">Model input properties.</param>
        /// <returns>Batched BCHW model input MD-array.</returns>
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
            FloatBuffer inputData = ByteBuffer.AllocateDirect(CalculateBufferCapacity(inputShape)).Order(ByteOrder.NativeOrder
                ()).AsFloatBuffer();
            foreach (System.Drawing.Bitmap image in images) {
                System.Drawing.Bitmap resizedImage = Resize(image, properties.GetWidth(), properties.GetHeight(), properties
                    .UseSymmetricPad());
                System.Diagnostics.Debug.Assert(resizedImage.GetType() == System.Drawing.Bitmap.TYPE_3BYTE_BGR);
                // Doing normalization at the same time as we fill the buffer
                WritableRaster raster = resizedImage.GetRaster();
                for (int y = 0; y < resizedImage.GetHeight(); ++y) {
                    for (int x = 0; x < resizedImage.GetWidth(); ++x) {
                        float r = raster.GetSample(x, y, 2) / 255F;
                        inputData.Put((r - properties.GetRedMean()) / properties.GetRedStd());
                    }
                }
                for (int y = 0; y < resizedImage.GetHeight(); ++y) {
                    for (int x = 0; x < resizedImage.GetWidth(); ++x) {
                        float g = raster.GetSample(x, y, 1) / 255F;
                        inputData.Put((g - properties.GetGreenMean()) / properties.GetGreenStd());
                    }
                }
                for (int y = 0; y < resizedImage.GetHeight(); ++y) {
                    for (int x = 0; x < resizedImage.GetWidth(); ++x) {
                        float b = raster.GetSample(x, y, 0) / 255F;
                        inputData.Put((b - properties.GetBlueMean()) / properties.GetBlueStd());
                    }
                }
            }
            inputData.Rewind();
            return new FloatBufferMdArray(inputData, inputShape);
        }

        /// <summary>Converts an image to an RGB Mat for use in OpenCV.</summary>
        /// <param name="image">Image to convert.</param>
        /// <returns>RGB 8UC3 OpenCV Mat with the image.</returns>
        public static Mat ToRgbMat(System.Drawing.Bitmap image) {
            Mat resultMat = new Mat(image.GetHeight(), image.GetWidth(), CvType.CV_8UC3);
            using (UByteIndexer resultMatIndexer = resultMat.CreateIndexer()) {
                for (int y = 0; y < image.GetHeight(); ++y) {
                    for (int x = 0; x < image.GetWidth(); ++x) {
                        int rgb = image.GetRGB(x, y);
                        int r = (rgb >> 16) & 0xFF;
                        int g = (rgb >> 8) & 0xFF;
                        int b = rgb & 0xFF;
                        resultMatIndexer.Put(y, (long)x, r, g, b);
                    }
                }
            }
            return resultMat;
        }

        /// <summary>Converts an RGB 8UC3 OpenCV Mat to a buffered image.</summary>
        /// <param name="rgb">RGB 8UC3 OpenCV Mat to convert.</param>
        /// <returns>Buffered image based on Mat.</returns>
        public static System.Drawing.Bitmap FromRgbMat(Mat rgb) {
            if (rgb.Type() != CvType.CV_8UC3) {
                throw new ArgumentException("Unexpected Mat type");
            }
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(rgb.Cols(), rgb.Rows(), System.Drawing.Bitmap.TYPE_3BYTE_BGR
                );
            int[] rgbBuffer = new int[3];
            using (UByteIndexer rgbIndexer = rgb.CreateIndexer()) {
                for (int y = 0; y < image.GetHeight(); ++y) {
                    for (int x = 0; x < image.GetWidth(); ++x) {
                        rgbIndexer.Get(y, x, rgbBuffer);
                        int rgbValue = 0xFF000000 | (rgbBuffer[0] << 16) | (rgbBuffer[1] << 8) | rgbBuffer[2];
                        image.SetRGB(x, y, rgbValue);
                    }
                }
            }
            return image;
        }

        /// <summary>Rotates image based on text orientation.</summary>
        /// <remarks>Rotates image based on text orientation. If no rotation necessary, same image is returned.</remarks>
        /// <param name="image">Image to rotate.</param>
        /// <param name="orientation">Text orientation used to rotate the image.</param>
        /// <returns>New rotated image, or same image, if no rotation is required.</returns>
        public static System.Drawing.Bitmap Rotate(System.Drawing.Bitmap image, TextOrientation orientation) {
            if (orientation == TextOrientation.HORIZONTAL) {
                return image;
            }
            int oldW = image.GetWidth();
            int oldH = image.GetHeight();
            int newW;
            int newH;
            double angle;
            if (orientation == TextOrientation.HORIZONTAL_ROTATED_180) {
                newW = oldW;
                newH = oldH;
                angle = Math.PI;
            }
            else {
                newW = oldH;
                newH = oldW;
                if (orientation == TextOrientation.HORIZONTAL_ROTATED_90) {
                    angle = 0.5 * Math.PI;
                }
                else {
                    angle = 1.5 * Math.PI;
                }
            }
            System.Drawing.Bitmap rotated = new System.Drawing.Bitmap(newW, newH, image.GetType());
            Graphics2D graphics = rotated.CreateGraphics();
            graphics.Translate((newW - oldW) / 2.0, (newH - oldH) / 2.0);
            graphics.Rotate(angle, image.GetWidth() / 2.0, image.GetHeight() / 2.0);
            graphics.DrawImage(image, 0, 0, null);
            graphics.Dispose();
            return rotated;
        }

        /// <summary>Creates a new image with an aspect ratio preserving resize.</summary>
        /// <remarks>
        /// Creates a new image with an aspect ratio preserving resize. New blank pixel will have black
        /// color.
        /// </remarks>
        /// <param name="image">Image to resize.</param>
        /// <param name="width">Target width.</param>
        /// <param name="height">Target height.</param>
        /// <param name="symmetricPad">Whether padding should be symmetric or should it be bottom-right.</param>
        /// <returns>New resized image.</returns>
        public static System.Drawing.Bitmap Resize(System.Drawing.Bitmap image, int width, int height, bool symmetricPad
            ) {
            // It is pretty unlikely, that the image is already the correct size, so no need for an exception
            System.Drawing.Bitmap result = new System.Drawing.Bitmap(width, height, System.Drawing.Bitmap.TYPE_3BYTE_BGR
                );
            Graphics2D graphics = result.CreateGraphics();
            graphics.SetColor(Color.BLACK);
            graphics.SetRenderingHint(RenderingHints.KEY_INTERPOLATION, RenderingHints.VALUE_INTERPOLATION_BILINEAR);
            int sourceWidth = image.GetWidth();
            int sourceHeight = image.GetHeight();
            double widthRatio = (double)width / sourceWidth;
            double heightRatio = (double)height / sourceHeight;
            if (heightRatio > widthRatio) {
                int scaledHeight = (int)MathematicUtil.Round(sourceHeight * widthRatio);
                int yPos;
                if (symmetricPad) {
                    yPos = (height - scaledHeight) / 2;
                    graphics.FillRect(0, 0, width, yPos);
                }
                else {
                    yPos = 0;
                }
                graphics.FillRect(0, yPos + scaledHeight, width, height - scaledHeight - yPos);
                graphics.DrawImage(image, 0, yPos, width, scaledHeight, Color.WHITE, null);
            }
            else {
                int scaledWidth = (int)MathematicUtil.Round(sourceWidth * heightRatio);
                int xPos;
                if (symmetricPad) {
                    xPos = (width - scaledWidth) / 2;
                    graphics.FillRect(0, 0, xPos, height);
                }
                else {
                    xPos = 0;
                }
                graphics.FillRect(xPos + scaledWidth, 0, width - scaledWidth - xPos, height);
                graphics.DrawImage(image, xPos, 0, scaledWidth, height, Color.WHITE, null);
            }
            graphics.Dispose();
            return result;
        }

        /// <summary>Extracts sub-images from an image, based on provided rotated 4-point boxes.</summary>
        /// <remarks>
        /// Extracts sub-images from an image, based on provided rotated 4-point boxes. Sub-images are
        /// transformed to fit the whole image without (in our use cases it is just rotation).
        /// </remarks>
        /// <param name="image">Original image to be used for extraction.</param>
        /// <param name="boxes">List of 4-point boxes. Points should be in the following order: BL, TL, TR, BR.</param>
        /// <returns>List of extracted image boxes.</returns>
        public static IList<System.Drawing.Bitmap> ExtractBoxes(System.Drawing.Bitmap image, ICollection<Point[]> 
            boxes) {
            IList<System.Drawing.Bitmap> boxesImages = new List<System.Drawing.Bitmap>(boxes.Count);
            using (Mat imageMat = iText.Pdfocr.Onnxtr.Util.BufferedImageUtil.ToRgbMat(image)) {
                foreach (Point[] box in boxes) {
                    float boxWidth = (float)box[1].Distance(box[2]);
                    float boxHeight = (float)box[1].Distance(box[0]);
                    using (Mat transformationMat = CalculateBoxTransformationMat(box, boxWidth, boxHeight)) {
                        using (Mat boxImageMat = new Mat((int)boxHeight, (int)boxWidth, CvType.CV_8UC3)) {
                            using (Size size = new Size((int)boxWidth, (int)boxHeight)) {
                                Opencv_imgproc.WarpAffine(imageMat, boxImageMat, transformationMat, size);
                                boxesImages.Add(iText.Pdfocr.Onnxtr.Util.BufferedImageUtil.FromRgbMat(boxImageMat));
                            }
                        }
                    }
                }
            }
            return boxesImages;
        }

        private static Mat CalculateBoxTransformationMat(Point[] box, float boxWidth, float boxHeight) {
            using (Mat srcPoints = new Mat(3, 2, CvType.CV_32F)) {
                using (Mat dstPoints = new Mat(3, 2, CvType.CV_32F)) {
                    using (FloatIndexer srcPointsIndexer = srcPoints.CreateIndexer()) {
                        using (FloatIndexer dstPointsIndexer = dstPoints.CreateIndexer()) {
                            for (int i = 0; i < 3; ++i) {
                                srcPointsIndexer.Put(i, (float)box[i].GetX(), (float)box[i].GetY());
                            }
                            dstPointsIndexer.Put(0, 0F, boxHeight - 1);
                            dstPointsIndexer.Put(1, 0F, 0F);
                            dstPointsIndexer.Put(2, boxWidth - 1, 0F);
                            return Opencv_imgproc.GetAffineTransform(srcPoints, dstPoints);
                        }
                    }
                }
            }
        }

        /// <summary>Returns the byte capacity required for a float32 buffer of the specified shape.</summary>
        /// <param name="shape">Shape of the MD-array.</param>
        /// <returns>The byte capacity required for a float32 buffer of the specified shape.</returns>
        private static int CalculateBufferCapacity(long[] shape) {
            int capacity = float.BYTES;
            foreach (long dim in shape) {
                capacity *= (int)dim;
            }
            return capacity;
        }
    }
}

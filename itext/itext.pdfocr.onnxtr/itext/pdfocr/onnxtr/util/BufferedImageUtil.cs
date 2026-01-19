/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
        /// <summary>
        /// Band index to retrieve a gray channel sample from a Raster.
        /// </summary>
        private static readonly int BAND_GRAY = 0;

        private BufferedImageUtil() {
        }

        /// <summary>Converts a collection of images to a batched ML model input in a BCHW format with 1 or 3 channels.</summary>
        /// <remarks>
        /// Converts a collection of images to a batched ML model input in a BCHW format with 1 or 3 channels.
        /// This does aspect-preserving image resizing to fit the input shape.
        /// </remarks>
        /// <param name="images">collection of images to convert to model input</param>
        /// <param name="properties">model input properties</param>
        /// <returns>batched BCHW model input MD-array</returns>
        public static FloatBufferMdArray ToBchwInput(ICollection<IronSoftware.Drawing.AnyBitmap> images, OnnxInputProperties
             properties) {
            if (images.Count == 0) {
                throw new ArgumentException(PdfOcrOnnxTrExceptionMessageConstant.SHOULD_BE_AT_LEAST_ONE_IMAGE);
            }
            if (images.Count > properties.GetBatchSize()) {
                throw new ArgumentException(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.TOO_MANY_IMAGES, 
                    images.Count, properties.GetBatchSize()));
            }
            ImageResizeOptions resizeOptions = properties.GetImageResizeOptions();
            Dimensions2D batchDimensions = CalcOutputDimensions(images, resizeOptions);
            long[] inputShape = new long[] { 
                images.Count, 
                resizeOptions.GetChannelConfiguration().GetChannelCount(),
                batchDimensions.GetHeight(),
                batchDimensions.GetWidth()
            };
            int bufferSize = CalculateBufferCapacity(inputShape);
            float[] inputData = new float[bufferSize / sizeof(float)];

            int currentIndex = 0;
            foreach (IronSoftware.Drawing.AnyBitmap image in images) {
                using (SkiaSharp.SKBitmap resizedImage = Resize(image, 
                           batchDimensions.GetWidth(),
                           batchDimensions.GetHeight(),
                           resizeOptions)) {
                    currentIndex = PutImageWithNormalization(inputData, resizedImage, properties, currentIndex);
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
                new SkiaSharp.SKBitmap(newW, newH, SkiaSharp.SKColorType.Bgra8888, SkiaSharp.SKAlphaType.Unpremul);
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

        /// <summary>Based on the provided ImageResizeOptions, calculates the dimensions to
        /// which a batch of images should be scaled and padded.</summary>
        /// <param name="images">batch of images to scale/pad</param>
        /// <param name="resizeOptions">resize options to take into consideration for scaling/padding</param>
        /// <returns>the calculated dimensions</returns>
        public static Dimensions2D CalcOutputDimensions(ICollection<IronSoftware.Drawing.AnyBitmap> images,
            ImageResizeOptions resizeOptions) {
            /*
             * Calculating target dimensions for each image, We need to know them
             * all before creating a batch buffers, as width and height should be the same
             * for each image in the batch. So we need to know the maximum sizes
             * before we create the buffers. And we don't really want to bloat
             * peak memory usage by creating an array of resized images...
             */
            ICollection<Dimensions2D> targetDimensions = new List<Dimensions2D>(images.Count);
            int maxWidth = 0;
            int maxHeight = 0;
            foreach (IronSoftware.Drawing.AnyBitmap image in images) {
                Dimensions2D dimensions = CalcOutputDimensions(image, resizeOptions);
                targetDimensions.Add(dimensions);
                maxWidth = Math.Max(maxWidth, dimensions.GetWidth());
                maxHeight = Math.Max(maxHeight, dimensions.GetHeight());
            }
            return new Dimensions2D(maxWidth, maxHeight);
        }

        /// <summary>Based on the provided ImageResizeOptions, calculates the dimensions of
        /// the output image, to where there original image should be scaled and placed with padding.
        /// The returned dimensions will always satisfy the minimum constraints.
        /// Maximum constraints will also be satisfied, if the dimension multiple is 1, but if it is greater,
        /// it may round up to be higher than maximum.</summary>
        /// <param name="image">image, that will be scaled/padded</param>
        /// <param name="resizeOptions">resize options to take into consideration for scaling/padding</param>
        /// <returns>the calculated dimensions</returns>
        public static Dimensions2D CalcOutputDimensions(IronSoftware.Drawing.AnyBitmap image,
            ImageResizeOptions resizeOptions) {
            int targetWidth = BufferedImageUtil.GetWidth(image);
            int targetHeight = BufferedImageUtil.GetHeight(image);

            /*
             * If the image is smaller in one of the dimensions, we will try to
             * resize it in a way that both dimensions at least match "min". In the
             * case, when one of the dimensions gets too big and goes over "max",
             * then we will shrink it to fit max again in the next block
             */
            double widthToMinMul = (double)resizeOptions.GetMinWidth() / targetWidth;
            double heightToMinMul = (double)resizeOptions.GetMinHeight() / targetHeight;
            if (widthToMinMul > 1.0 || heightToMinMul > 1.0) {
                if (widthToMinMul >= heightToMinMul) {
                    targetWidth = resizeOptions.GetMinWidth();
                    targetHeight = Math.Max(resizeOptions.GetMinHeight(),
                        (int)Math.Round(widthToMinMul * targetHeight));
                } else {
                    targetWidth = Math.Max(resizeOptions.GetMinWidth(), (int)Math.Round(heightToMinMul * targetWidth));
                    targetHeight = resizeOptions.GetMinHeight();
                }
            }

            /*
             * If the image is bigger in one of the dimensions, we will shrink it
             * in a way to satisfy the "max" constraints. In case one of the
             * dimensions will fall below its "min" constraint afterward, we will
             * pad it back.
             */
            double widthToMaxMul = (double)resizeOptions.GetMaxWidth() / targetWidth;
            double heightToMaxMul = (double)resizeOptions.GetMaxHeight() / targetHeight;
            if (widthToMaxMul < 1.0 || heightToMaxMul < 1.0) {
                if (widthToMaxMul <= heightToMaxMul) {
                    targetWidth = resizeOptions.GetMaxWidth();
                    targetHeight = (int)MathUtil.Clamp(
                        widthToMaxMul * targetHeight, resizeOptions.GetMinHeight(), resizeOptions.GetMaxHeight()
                    );
                } else {
                    targetWidth = (int)MathUtil.Clamp(
                        heightToMaxMul * targetWidth, resizeOptions.GetMinWidth(), resizeOptions.GetMaxWidth()
                    );
                    targetHeight = resizeOptions.GetMaxHeight();
                }
            }

            // Rounding-up to multiple here
            int widthMultiple = resizeOptions.GetWidthMultiple();
            int heightMultiple = resizeOptions.GetHeightMultiple();
            return new Dimensions2D(
                (targetWidth + (widthMultiple - 1)) / widthMultiple * widthMultiple,
                (targetHeight + (heightMultiple - 1)) / heightMultiple * heightMultiple
            );
        }

        /// <summary>
        /// Truncates the input image, so that neither width/height, nor
        /// height/width ratios exceed the limit.
        /// </summary>
        /// <remarks>
        /// Truncates the input image, so that neither width/height, nor
        /// height/width ratios exceed the limit.
        /// <para />
        /// If width/height ratio exceeds the limit, the image will be truncated
        /// on left and right equally.
        /// <para />
        /// If height/width ratio exceeds the limit, the image will be truncated
        /// on top and bottom equally.
        /// </remarks>
        /// <param name="image">input image to truncate</param>
        /// <param name="ratioLimit">target ratio limit</param>
        /// <returns>the truncated image</returns>
        public static IronSoftware.Drawing.AnyBitmap TruncateToRatio(IronSoftware.Drawing.AnyBitmap image, 
            double ratioLimit) {
            int width = BufferedImageUtil.GetWidth(image);
            int height = BufferedImageUtil.GetHeight(image);
            // If w/h ratio is too big, truncating by width
            double imageRatio = (double)width / height;
            if (imageRatio > ratioLimit) {
                int newWidth = Math.Max(1, (int)(ratioLimit * height));
                int newX = (width - newWidth) / 2;
                return image.GetSubimage(newX, 0, newWidth, height);
            }
            // If h/w ratio is too big, truncating by height
            double imageRatioInv = 1.0 / imageRatio;
            if (imageRatioInv > ratioLimit) {
                int newHeight = Math.Max(1, (int)(ratioLimit * width));
                int newY = (height - newHeight) / 2;
                return image.GetSubimage(0, newY, width, newHeight);
            }
            // Otherwise leaving as-is
            return image;
        }

        /// <summary>Creates a new image with an aspect ratio preserving resize.</summary>
        /// <param name="image">image to resize</param>
        /// <param name="width">target width</param>
        /// <param name="height">target height</param>
        /// <param name="paddingStrategy">padding strategy to use</param>
        /// <param name="targetType">type of the created image</param>
        /// <returns>new resized image</returns>
        internal static SkiaSharp.SKBitmap Resize(IronSoftware.Drawing.AnyBitmap image, int width, int height, 
            ImageResizeOptions resizeOptions) {
            PaddingStrategy paddingStrategy = resizeOptions.GetPaddingStrategy();
            // It is pretty unlikely, that the image is already the correct size, so no need for an exception
            SkiaSharp.SKBitmap result = new SkiaSharp.SKBitmap(width, height, 
                ToImageType(resizeOptions.GetChannelConfiguration()), SkiaSharp.SKAlphaType.Unpremul);
            using (SkiaSharp.SKCanvas graphics = new SkiaSharp.SKCanvas(result)) {
                // Decided to use white background color for the cases when the image is transparent (e.g. PNG),
                // see WeirdWordsDoImageOcrTest for example. With transparent background text is not recognized,
                // on the black one the most frequent black text won't be visible.
                graphics.Clear(SkiaSharp.SKColors.White);

                int sourceWidth = BufferedImageUtil.GetWidth(image);
                int sourceHeight = BufferedImageUtil.GetHeight(image);
                double widthRatio = (double)width / sourceWidth;
                double heightRatio = (double)height / sourceHeight;

                if (heightRatio > widthRatio) {
                    int scaledHeight = Math.Min(height, (int)MathematicUtil.Round(sourceHeight * widthRatio));
                    DrawResizedImage(graphics, width, height, image, width, scaledHeight, paddingStrategy);
                } else {
                    int scaledWidth = Math.Min(width, (int)MathematicUtil.Round(sourceWidth * heightRatio));
                    DrawResizedImage(graphics, width, height, image, scaledWidth, height, paddingStrategy);
                }
            }

            return result;
        }

        internal static ImageChannelConfiguration GetImageType(SkiaSharp.SKBitmap image) {
            switch (((SkiaSharp.SKBitmap)image).ColorType) {
                case SkiaSharp.SKColorType.Bgra8888:
                    return ImageChannelConfiguration.BGR;
                case SkiaSharp.SKColorType.Rgba8888:
                case SkiaSharp.SKColorType.Rgb888x:
                    return ImageChannelConfiguration.RGB;
                case SkiaSharp.SKColorType.Gray8:
                    return ImageChannelConfiguration.GRAYSCALE;
                default:
                    throw new ArgumentException(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_CHANNEL_CONFIGURATION);
            }
        }

        private static int PutImageWithNormalization(float[] outputBuffer, SkiaSharp.SKBitmap image,
            OnnxInputProperties props, int currentIndex) {
            ImageChannelConfiguration channelConfiguration = props.GetImageResizeOptions().GetChannelConfiguration();

            if (ImageChannelConfiguration.GRAYSCALE == channelConfiguration) {
                return PutGrayscaleImageWithNormalization(outputBuffer, image, props, currentIndex);
            }

            if (ImageChannelConfiguration.RGB == channelConfiguration) {
                return PutRgbImageWithNormalization(outputBuffer, image, props, currentIndex);
            }

            if (ImageChannelConfiguration.BGR == channelConfiguration) {
                return PutBgrImageWithNormalization(outputBuffer, image, props, currentIndex);
            }

            throw new ArgumentException(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_CHANNEL_CONFIGURATION);
        }

        private static int PutGrayscaleImageWithNormalization(float[] outputBuffer, SkiaSharp.SKBitmap image, 
            OnnxInputProperties props, int index) {
            if (!ImageChannelConfiguration.GRAYSCALE.Equals(GetImageType(image))) {
                throw new ArgumentException("Invalid image type!");
            }
            index = PutImageBandWithNormalization(outputBuffer, image, props, BAND_GRAY, props.GetGrayMean(), props.GetGrayStd(), index); 
            return index;
        } 

        private static int PutRgbImageWithNormalization(float[] outputBuffer, SkiaSharp.SKBitmap image, 
            OnnxInputProperties props, int index) {
            if (!ImageChannelConfiguration.RGB.Equals(GetImageType(image))) {
                throw new ArgumentException("Invalid image type!");
            }
            index = PutImageBandWithNormalization(outputBuffer, image, props, 0, props.GetRedMean(), props.GetRedStd(), index);
            index = PutImageBandWithNormalization(outputBuffer, image, props, 1, props.GetGreenMean(), props.GetGreenStd(), index);
            index = PutImageBandWithNormalization(outputBuffer, image, props, 2, props.GetBlueMean(), props.GetBlueStd(), index);
            return index;
        }

        private static int PutBgrImageWithNormalization(float[] outputBuffer, SkiaSharp.SKBitmap image, 
            OnnxInputProperties props, int index) {
            if (!ImageChannelConfiguration.BGR.Equals(GetImageType(image))) {
                throw new ArgumentException("Invalid image type!");
            }
            index = PutImageBandWithNormalization(outputBuffer, image, props, 0, props.GetBlueMean(), props.GetBlueStd(), index);
            index = PutImageBandWithNormalization(outputBuffer, image, props, 1, props.GetGreenMean(), props.GetGreenStd(), index);
            index = PutImageBandWithNormalization(outputBuffer, image, props, 2, props.GetRedMean(), props.GetRedStd(), index);
            return index;
        }

        private static int PutImageBandWithNormalization(float[] outputBuffer, SkiaSharp.SKBitmap image, 
            OnnxInputProperties properties, int band, double mean, double std, int currentIndex) {
            using (SkiaSharp.SKPixmap raster = image.PeekPixels()) {
                byte[] pixelBytes = new byte[raster.BytesSize];
                Marshal.Copy(raster.GetPixels(), pixelBytes, 0, pixelBytes.Length);

                int stride = raster.RowBytes;
                int bytesPerPixel = raster.Info.BytesPerPixel;

                for (int y = 0; y < raster.Height; y++) {
                    int rowStart = y * stride;
                    for (int x = 0; x < raster.Width; x++) {
                        int index = rowStart + x * bytesPerPixel;
                        float v = pixelBytes[index + band] / 255f;
                        outputBuffer[currentIndex++] = (float)((v - mean) / std);
                    }
                }
            }
            return currentIndex;
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
                SkiaSharp.SKColorType.Bgra8888, SkiaSharp.SKAlphaType.Unpremul);

            Mat.Indexer<Vec4b> indexer = rgb.GetGenericIndexer<Vec4b>();
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
            
        private static void DrawResizedImage(SkiaSharp.SKCanvas output, int outputWidth, int outputHeight, 
            IronSoftware.Drawing.AnyBitmap image, int targetWidth, int targetHeight, PaddingStrategy paddingStrategy) {
            // Figuring where to put the image
            int xPos = 0;
            int yPos = 0;
            if (paddingStrategy.UsesSymmetricPadding()) {
                xPos += (outputWidth - targetWidth) / 2;
                yPos += (outputHeight - targetHeight) / 2;
            } else if (!paddingStrategy.UsesBottomRightPadding()) {
                throw new ArgumentException(MessageFormatUtil.Format(
                        PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_PADDING_STRATEGY, paddingStrategy
                ));
            }
            // Drawing all the paddings first
            if (paddingStrategy.UsesSolidColor()) {
                // Might as well just fill the whole output, images are small
                // anyway, so they might just be in cache whole
                using (SkiaSharp.SKPaint fillPaint = new SkiaSharp.SKPaint()) {
                    fillPaint.Color = (SkiaSharp.SKColor)paddingStrategy.GetSolidColor();
                    fillPaint.Style = SkiaSharp.SKPaintStyle.Fill;
                    fillPaint.FilterQuality = SkiaSharp.SKFilterQuality.Medium;
                    fillPaint.IsAntialias = true;
                    output.DrawRect(0, 0, outputWidth, outputHeight, fillPaint);
                }
            } else {
                int sourceWidth = GetWidth(image);
                int sourceHeight = GetHeight(image);
                // Top padding
                if (yPos > 0) {
                    using (SkiaSharp.SKPaint fillPaint = new SkiaSharp.SKPaint()) {
                        fillPaint.Color = (SkiaSharp.SKColor)IronSoftware.Drawing.Color.White;
                        fillPaint.Style = SkiaSharp.SKPaintStyle.Fill;
                        fillPaint.FilterQuality = SkiaSharp.SKFilterQuality.Medium;
                        fillPaint.IsAntialias = true;
                        output.DrawImage(image,
                            new SkiaSharp.SKRect(0, 0, sourceWidth, 1),
                            new SkiaSharp.SKRect(0, 0, outputWidth, yPos),
                            fillPaint);
                    }
                }
    
                // Right padding
                int rightPaddingX = xPos + targetWidth;
                if (rightPaddingX < outputWidth) {
                    using (SkiaSharp.SKPaint fillPaint = new SkiaSharp.SKPaint()) {
                        fillPaint.Color = (SkiaSharp.SKColor)IronSoftware.Drawing.Color.White;
                        fillPaint.Style = SkiaSharp.SKPaintStyle.Fill;
                        fillPaint.FilterQuality = SkiaSharp.SKFilterQuality.Medium;
                        fillPaint.IsAntialias = true;
                        output.DrawImage(image,
                            new SkiaSharp.SKRect(sourceWidth - 1, 0, sourceWidth, sourceHeight),
                            new SkiaSharp.SKRect(rightPaddingX, 0, outputWidth, outputHeight),
                            fillPaint);
                    }
                }
    
                // Bottom padding
                int bottomPaddingY = yPos + targetHeight;
                if (bottomPaddingY < outputHeight) {
                    using (SkiaSharp.SKPaint fillPaint = new SkiaSharp.SKPaint()) {
                        fillPaint.Color = (SkiaSharp.SKColor)IronSoftware.Drawing.Color.White;
                        fillPaint.Style = SkiaSharp.SKPaintStyle.Fill;
                        fillPaint.FilterQuality = SkiaSharp.SKFilterQuality.Medium;
                        fillPaint.IsAntialias = true;
                        output.DrawImage(image,
                            new SkiaSharp.SKRect(0, sourceHeight - 1, sourceWidth, sourceHeight),
                            new SkiaSharp.SKRect(0, bottomPaddingY, outputWidth, outputHeight),
                            fillPaint);
                    }
                }
    
                // Left padding
                if (xPos > 0) {
                    using (SkiaSharp.SKPaint fillPaint = new SkiaSharp.SKPaint()) {
                        fillPaint.Color = (SkiaSharp.SKColor)IronSoftware.Drawing.Color.White;
                        fillPaint.Style = SkiaSharp.SKPaintStyle.Fill;
                        fillPaint.FilterQuality = SkiaSharp.SKFilterQuality.Medium;
                        fillPaint.IsAntialias = true;
                        output.DrawImage(image,
                            new SkiaSharp.SKRect(0, 0, 1, sourceHeight),
                            new SkiaSharp.SKRect(0, 0, xPos, outputHeight),
                            fillPaint);
                    }
                }
            }
    
            // Drawing the image itself
            using (SkiaSharp.SKPaint imagePaint = new SkiaSharp.SKPaint()) { 
                imagePaint.FilterQuality = SkiaSharp.SKFilterQuality.Medium; 
                imagePaint.IsAntialias = true;
                output.DrawBitmap(image, new SkiaSharp.SKRect(xPos, yPos, xPos + targetWidth, yPos + targetHeight), imagePaint);
            }
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

        private static SkiaSharp.SKColorType ToImageType(ImageChannelConfiguration channelConfiguration) {
            if (ImageChannelConfiguration.GRAYSCALE == channelConfiguration) {
                return SkiaSharp.SKColorType.Gray8;
            } else if (ImageChannelConfiguration.RGB == channelConfiguration) {
                return SkiaSharp.SKColorType.Rgb888x;
            } else if (ImageChannelConfiguration.BGR == channelConfiguration) {
                return SkiaSharp.SKColorType.Bgra8888;
            }

            throw new ArgumentException(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_CHANNEL_CONFIGURATION);
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
                SkiaSharp.SKColorType.Bgra8888, SkiaSharp.SKAlphaType.Unpremul);
            using (SkiaSharp.SKCanvas graphics = new SkiaSharp.SKCanvas(bgraImage)) {
                graphics.DrawBitmap(image, 0, 0);
            } 
            return bgraImage;
        }
    }
}

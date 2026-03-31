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
using System.IO;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Xobject;
using iText.Pdfocr.Util;

namespace iText.Pdfocr {
//\cond DO_NOT_DOCUMENT
    /// <summary>
    /// Class to extract images on page content stream processing, see
    /// <see cref="iText.Kernel.Pdf.Canvas.Parser.PdfCanvasProcessor"/>.
    /// </summary>
    internal sealed class ImageExtraction {
        private ImageExtraction() {
        }

//\cond DO_NOT_DOCUMENT
        // Empty constructor to forbid instantiation
        /// <returns>map where the key is an image path and the value is a position on the page</returns>
        internal static IList<ImageExtraction.PageImageData> ExtractImagesFromPdfPage(PdfPage pdfPage) {
            ImageExtraction.CanvasImageExtractor listener = new ImageExtraction.CanvasImageExtractor();
            PdfCanvasProcessor processor = new PdfCanvasProcessor(listener);
            processor.ProcessPageContent(pdfPage);
            IDictionary<PdfImageXObject, Rectangle> images = listener.GetImages();
            // Now output to temp files
            IList<ImageExtraction.PageImageData> pageImageData = new List<ImageExtraction.PageImageData>(images.Count);
            foreach (KeyValuePair<PdfImageXObject, Rectangle> image in images) {
                String extension = image.Key.IdentifyImageFileExtension();
                String imageFilePath = PdfOcrFileUtil.GetTempFilePath("pdfocr_img_" + Guid.NewGuid(), "." + extension);
                using (Stream fos = FileUtil.GetFileOutputStream(imageFilePath)) {
                    byte[] imageBytes = image.Key.GetImageBytes();
                    fos.Write(imageBytes, 0, imageBytes.Length);
                    pageImageData.Add(new ImageExtraction.PageImageData(new FileInfo(imageFilePath), image.Key, image.Value));
                }
            }
            return pageImageData;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal sealed class PageImageData {
            private FileInfo file;

            private PdfImageXObject xObject;

            private Rectangle pagePosition;

//\cond DO_NOT_DOCUMENT
            internal PageImageData(FileInfo file, PdfImageXObject xObject, Rectangle pagePosition) {
                this.file = file;
                this.xObject = xObject;
                this.pagePosition = pagePosition;
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal FileInfo GetPath() {
                return file;
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal PdfImageXObject GetXObject() {
                return xObject;
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal Rectangle GetPagePosition() {
                return pagePosition;
            }
//\endcond

            public override int GetHashCode() {
                return JavaUtil.ArraysHashCode((Object)file, xObject, pagePosition);
            }

            public override bool Equals(Object o) {
                if (this == o) {
                    return true;
                }
                if (o == null || GetType() != o.GetType()) {
                    return false;
                }
                ImageExtraction.PageImageData that = (ImageExtraction.PageImageData)o;
                return file == that.file && xObject == that.xObject && pagePosition == that.pagePosition;
            }
        }
//\endcond

        // Consider moving to kernel if reused anywhere else
        private sealed class CanvasImageExtractor : IEventListener {
            // Image xobject - position on a page
            private readonly IDictionary<PdfImageXObject, Rectangle> images = new LinkedDictionary<PdfImageXObject, Rectangle
                >();

//\cond DO_NOT_DOCUMENT
            internal CanvasImageExtractor() {
            }
//\endcond

            // Empty constructor
            public void EventOccurred(IEventData data, EventType type) {
                if (type == EventType.RENDER_IMAGE) {
                    ImageRenderInfo renderInfo = (ImageRenderInfo)data;
                    Matrix imageCtm = renderInfo.GetImageCtm();
                    Rectangle bbox = CalcImageRect(imageCtm);
                    images.Put(renderInfo.GetImage(), bbox);
                }
            }

            public ICollection<EventType> GetSupportedEvents() {
                return new HashSet<EventType>(JavaCollectionsUtil.SingletonList(EventType.RENDER_IMAGE));
            }

//\cond DO_NOT_DOCUMENT
            internal IDictionary<PdfImageXObject, Rectangle> GetImages() {
                return images;
            }
//\endcond

            private Rectangle CalcImageRect(Matrix ctm) {
                Point[] points = TransformPoints(ctm, new Point(0, 0), new Point(0, 1), new Point(1, 0), new Point(1, 1));
                return Rectangle.CalculateBBox(JavaUtil.ArraysAsList(points));
            }

            private Point[] TransformPoints(Matrix transformationMatrix, params Point[] points) {
                AffineTransform t = new AffineTransform(transformationMatrix.Get(Matrix.I11), transformationMatrix.Get(Matrix
                    .I12), transformationMatrix.Get(Matrix.I21), transformationMatrix.Get(Matrix.I22), transformationMatrix
                    .Get(Matrix.I31), transformationMatrix.Get(Matrix.I32));
                Point[] transformed = new Point[points.Length];
                t.Transform(points, 0, transformed, 0, points.Length);
                return transformed;
            }
        }
    }
//\endcond
}

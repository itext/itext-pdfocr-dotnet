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
using iText.Kernel.Geom;
using iText.Pdfocr.Structuretree;

namespace iText.Pdfocr {
    /// <summary>
    /// This class describes how recognized text is positioned on the image
    /// providing bbox for each text item (could be a line or a word).
    /// </summary>
    public class TextInfo {
        /// <summary>Image pixel to PDF point ratio.</summary>
        private const float PX_TO_PT = 0.75F;

        /// <summary>Contains any text.</summary>
        private String text;

        /// <summary>
        /// Array of 4
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// s describing text bbox (lower-left based relative to text) expressed in PDF points.
        /// </summary>
        private Point[] textPoints;

        /// <summary>
        /// If LogicalStructureTreeItem is set, then
        /// <see cref="TextInfo"/>
        /// s are expected to be in logical order.
        /// </summary>
        private LogicalStructureTreeItem logicalStructureTreeItem;

        /// <summary>
        /// Creates a new
        /// <see cref="TextInfo"/>
        /// instance.
        /// </summary>
        public TextInfo() {
        }

        /// <summary>
        /// Creates a new
        /// <see cref="TextInfo"/>
        /// instance from existing one.
        /// </summary>
        /// <param name="textInfo">to create from</param>
        public TextInfo(iText.Pdfocr.TextInfo textInfo) {
            this.text = textInfo.text;
            this.textPoints = (Point[])textInfo.textPoints.Clone();
        }

        /// <summary>
        /// Creates new
        /// <see cref="TextInfo"/>
        /// instance.
        /// </summary>
        /// <param name="text">text string</param>
        /// <param name="bbox">
        /// array of 4
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// s describing text bbox (lower-left based relative to text)
        /// expressed in points (0 - lower-left, 1 - upper-left, 2 - upper-right, 3 - lower-right point)
        /// </param>
        public TextInfo(String text, Point[] bbox) {
            this.text = text;
            this.textPoints = bbox;
        }

        /// <summary>
        /// Creates new
        /// <see cref="TextInfo"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// Creates new
        /// <see cref="TextInfo"/>
        /// instance. Could be used for not rotated text chunks.
        /// </remarks>
        /// <param name="text">text string</param>
        /// <param name="bbox">
        /// 
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// describing text bounding box expressed in PDF points
        /// </param>
        public TextInfo(String text, Rectangle bbox) {
            this.text = text;
            this.textPoints = new Point[] { new Point(bbox.GetLeft(), bbox.GetBottom()), new Point(bbox.GetLeft(), bbox
                .GetTop()), new Point(bbox.GetRight(), bbox.GetTop()), new Point(bbox.GetRight(), bbox.GetBottom()) };
        }

        /// <summary>Gets text element.</summary>
        /// <returns>text string</returns>
        public virtual String GetText() {
            return text;
        }

        /// <summary>Sets text element.</summary>
        /// <param name="newText">retrieved text</param>
        /// <returns>this instance</returns>
        public virtual iText.Pdfocr.TextInfo SetText(String newText) {
            text = newText;
            return this;
        }

        /// <summary>
        /// Gets array of 4
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// s describing text bbox (lower-left based relative to text) expressed in points.
        /// </summary>
        /// <remarks>
        /// Gets array of 4
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// s describing text bbox (lower-left based relative to text) expressed in points.
        /// <para />
        /// Point array stores text polygon in the following order relative to text:
        /// 0 - lower-left, 1 - upper-left, 2 - upper-right, 3 - lower-right point.
        /// <para />
        /// The following coordinate system is used for points coordinate:
        /// the origin is located in left bottom corner of the page,
        /// vertical (y) coordinates increase from the bottom of the page to the top,
        /// horizontal (x) coordinates increase from the left side of the page to the right,
        /// axe unit is user space unit which we call PDF point (1 PDF point = 1/72 inch = 4/3 pixel).
        /// </remarks>
        /// <returns>
        /// array of 4
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// s describing text bbox (lower-left based relative to text) expressed in points
        /// </returns>
        public virtual Point[] GetTextPoints() {
            return textPoints;
        }

        /// <summary>
        /// Sets array of 4
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// s describing text bbox (lower-left based relative to text) expressed in points.
        /// </summary>
        /// <remarks>
        /// Sets array of 4
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// s describing text bbox (lower-left based relative to text) expressed in points.
        /// <para />
        /// Point array should store text polygon in the following order relative to text:
        /// 0 - lower-left, 1 - upper-left, 2 - upper-right, 3 - lower-right point.
        /// <para />
        /// The following coordinate system is used for points coordinate:
        /// the origin is located in left bottom corner of the page,
        /// vertical (y) coordinates increase from the bottom of the page to the top,
        /// horizontal (x) coordinates increase from the left side of the page to the right,
        /// axe unit is user space unit which we call PDF point (1 PDF point = 1/72 inch = 4/3 pixel).
        /// </remarks>
        /// <param name="textPoints">
        /// array of 4
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// s describing text bbox (lower-left based relative to text)
        /// expressed in points
        /// </param>
        /// <returns>this instance</returns>
        public virtual iText.Pdfocr.TextInfo SetTextPoints(Point[] textPoints) {
            this.textPoints = textPoints;
            return this;
        }

        /// <summary>
        /// Gets array of 4
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// s describing text bbox (lower-left based relative to text) expressed in pixels.
        /// </summary>
        /// <remarks>
        /// Gets array of 4
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// s describing text bbox (lower-left based relative to text) expressed in pixels.
        /// <para />
        /// Point array stores text polygon in the following order relative to text:
        /// 0 - lower-left, 1 - upper-left, 2 - upper-right, 3 - lower-right point.
        /// <para />
        /// The following coordinate system is used for text points coordinate:
        /// the origin is located in left top corner of the page (image),
        /// vertical (y) coordinates increase from the top of the page to the bottom,
        /// horizontal (x) coordinates increase from the left side of the page to the right,
        /// axe unit is pixel (1 pixel = 1/96 inch = 0.75 PDF point).
        /// </remarks>
        /// <param name="imageHeight">
        /// height of the image to convert the text PDF points to image pixels coordinates.
        /// Used to change the
        /// <c>y</c>
        /// origin
        /// </param>
        /// <returns>
        /// array of 4
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// s describing text bbox (lower-left based relative to text) expressed in pixels
        /// </returns>
        public virtual Point[] GetPixelTextPoints(int imageHeight) {
            Point[] result = new Point[this.textPoints.Length];
            for (int i = 0; i < result.Length; ++i) {
                result[i] = new Point(this.textPoints[i].GetX() / PX_TO_PT, imageHeight - this.textPoints[i].GetY() / PX_TO_PT
                    );
            }
            return result;
        }

        /// <summary>
        /// Sets an array of 4
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// s describing text bbox (lower-left based relative to text) expressed in pixels.
        /// </summary>
        /// <remarks>
        /// Sets an array of 4
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// s describing text bbox (lower-left based relative to text) expressed in pixels.
        /// <para />
        /// Point array should store text polygon in the following order relative to text:
        /// 0 - lower-left, 1 - upper-left, 2 - upper-right, 3 - lower-right point.
        /// <para />
        /// The following coordinate system is used for text points coordinate:
        /// the origin is located in left top corner of the page,
        /// vertical (y) coordinates increase from the top of the page to the bottom,
        /// horizontal (x) coordinates increase from the left side of the page to the right,
        /// axe unit is pixel (1 pixel = 1/96 inch = 0.75 PDF point).
        /// </remarks>
        /// <param name="textPoints">
        /// array of 4
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// s describing text bbox (0 - lower-left, 1 - upper-left,
        /// 2 - upper-right, 3 - lower-right relative to text) expressed in pixels
        /// </param>
        /// <param name="imageHeight">
        /// height of the image to convert the text PDF points to image pixels coordinates.
        /// Used to change the
        /// <c>y</c>
        /// origin
        /// </param>
        /// <returns>
        /// array of 4
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// s describing text bbox (lower-left based relative to text) expressed in pixels
        /// </returns>
        public virtual iText.Pdfocr.TextInfo SetPixelTextPoints(Point[] textPoints, int imageHeight) {
            Point[] result = new Point[textPoints.Length];
            for (int i = 0; i < result.Length; ++i) {
                result[i] = new Point(PX_TO_PT * textPoints[i].GetX(), PX_TO_PT * (imageHeight - textPoints[i].GetY()));
            }
            this.textPoints = result;
            return this;
        }

        /// <summary>Converts a text polygon to a bounding box.</summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// representing text bounding box
        /// </returns>
        public virtual Rectangle GetBBoxRect() {
            float minX = (float)this.textPoints[0].GetX();
            float maxX = minX;
            float minY = (float)this.textPoints[0].GetY();
            float maxY = minY;
            for (int i = 1; i < this.textPoints.Length; ++i) {
                float x = (float)this.textPoints[i].GetX();
                if (x < minX) {
                    minX = x;
                }
                else {
                    if (x > maxX) {
                        maxX = x;
                    }
                }
                float y = (float)this.textPoints[i].GetY();
                if (y < minY) {
                    minY = y;
                }
                else {
                    if (y > maxY) {
                        maxY = y;
                    }
                }
            }
            return new Rectangle(minX, minY, maxX - minX, maxY - minY);
        }

        /// <summary>
        /// Returns the text rotation angle in radian for this
        /// <see cref="TextInfo"/>
        /// in the range of -pi to pi.
        /// </summary>
        /// <returns>
        /// the text rotation angle in radian for the current
        /// <see cref="TextInfo"/>
        /// </returns>
        public virtual float GetRotationAngle() {
            double dx = textPoints[3].GetX() - textPoints[0].GetX();
            double dy = textPoints[3].GetY() - textPoints[0].GetY();
            return (float)Math.Atan2(dy, dx);
        }

        /// <summary>Retrieves structure tree item for the text item.</summary>
        /// <returns>structure tree item.</returns>
        public virtual LogicalStructureTreeItem GetLogicalStructureTreeItem() {
            return logicalStructureTreeItem;
        }

        /// <summary>Sets logical structure tree parent item for the text info.</summary>
        /// <remarks>
        /// Sets logical structure tree parent item for the text info. It allows to organize text chunks
        /// into logical hierarchy, e.g. specify document paragraphs, tables, etc.
        /// <para />
        /// If LogicalStructureTreeItem is set, then the list of
        /// <see cref="TextInfo"/>
        /// s in
        /// <see cref="IOcrEngine.DoImageOcr(System.IO.FileInfo)"/>
        /// return value is expected to be in logical order.
        /// </remarks>
        /// <param name="logicalStructureTreeItem">structure tree item</param>
        public virtual void SetLogicalStructureTreeItem(LogicalStructureTreeItem logicalStructureTreeItem) {
            this.logicalStructureTreeItem = logicalStructureTreeItem;
        }
    }
}

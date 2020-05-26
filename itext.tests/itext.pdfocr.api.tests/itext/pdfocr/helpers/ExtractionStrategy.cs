using System;
using System.Collections.Generic;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace iText.Pdfocr.Helpers {
    public class ExtractionStrategy : LocationTextExtractionStrategy {
        private Rectangle imageBBoxRectangle;

        private Color fillColor;

        private String layerName;

        private PdfFont pdfFont;

        public ExtractionStrategy(String name)
            : base() {
            layerName = name;
        }

        public virtual Color GetFillColor() {
            return fillColor;
        }

        public virtual void SetFillColor(Color color) {
            fillColor = color;
        }

        public virtual PdfFont GetPdfFont() {
            return pdfFont;
        }

        public virtual void SetPdfFont(PdfFont font) {
            pdfFont = font;
        }

        public virtual Rectangle GetImageBBoxRectangle() {
            return this.imageBBoxRectangle;
        }

        public override void EventOccurred(IEventData data, EventType type) {
            IList<CanvasTag> tagHierarchy = null;
            if (type.Equals(EventType.RENDER_TEXT)) {
                TextRenderInfo textRenderInfo = (TextRenderInfo)data;
                tagHierarchy = textRenderInfo.GetCanvasTagHierarchy();
            }
            else {
                if (type.Equals(EventType.RENDER_IMAGE)) {
                    ImageRenderInfo imageRenderInfo = (ImageRenderInfo)data;
                    tagHierarchy = imageRenderInfo.GetCanvasTagHierarchy();
                }
            }
            if (tagHierarchy != null) {
                foreach (CanvasTag tag in tagHierarchy) {
                    PdfDictionary dict = tag.GetProperties();
                    String name = dict.Get(PdfName.Name).ToString();
                    if (name.Equals(layerName)) {
                        if (type.Equals(EventType.RENDER_TEXT)) {
                            TextRenderInfo renderInfo = (TextRenderInfo)data;
                            SetFillColor(renderInfo.GetGraphicsState().GetFillColor());
                            SetPdfFont(renderInfo.GetGraphicsState().GetFont());
                            base.EventOccurred(data, type);
                            break;
                        }
                        else {
                            if (type.Equals(EventType.RENDER_IMAGE)) {
                                ImageRenderInfo renderInfo = (ImageRenderInfo)data;
                                Matrix ctm = renderInfo.GetImageCtm();
                                this.imageBBoxRectangle = new Rectangle(ctm.Get(6), ctm.Get(7), ctm.Get(0), ctm.Get(4));
                                break;
                            }
                        }
                    }
                }
            }
        }

        protected override bool IsChunkAtWordBoundary(TextChunk chunk, TextChunk previousChunk) {
            ITextChunkLocation curLoc = chunk.GetLocation();
            ITextChunkLocation prevLoc = previousChunk.GetLocation();
            if (curLoc.GetStartLocation().Equals(curLoc.GetEndLocation()) || prevLoc.GetEndLocation().Equals(prevLoc.GetStartLocation
                ())) {
                return false;
            }
            return curLoc.DistParallelEnd() - prevLoc.DistParallelStart() > (curLoc.GetCharSpaceWidth() + prevLoc.GetCharSpaceWidth
                ()) / 2.0f;
        }
    }
}

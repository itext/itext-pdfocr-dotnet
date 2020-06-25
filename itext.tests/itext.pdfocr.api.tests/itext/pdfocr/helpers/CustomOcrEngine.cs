/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using iText.IO.Util;
using iText.Kernel.Counter.Event;
using iText.Pdfocr;
using iText.Pdfocr.Events;

namespace iText.Pdfocr.Helpers {
    public class CustomOcrEngine : IOcrEngine, IThreadLocalMetaInfoAware {
        private OcrEngineProperties ocrEngineProperties;

        private IMetaInfo threadLocalMetaInfo;

        public CustomOcrEngine() {
        }

        public CustomOcrEngine(OcrEngineProperties ocrEngineProperties) {
            this.ocrEngineProperties = new OcrEngineProperties(ocrEngineProperties);
        }

        public virtual IDictionary<int, IList<TextInfo>> DoImageOcr(FileInfo input) {
            IDictionary<int, IList<TextInfo>> result = new Dictionary<int, IList<TextInfo>>();
            String text = PdfHelper.DEFAULT_TEXT;
            if (input.FullName.Contains(PdfHelper.THAI_IMAGE_NAME)) {
                text = PdfHelper.THAI_TEXT;
            }
            TextInfo textInfo = new TextInfo(text, JavaUtil.ArraysAsList(204.0f, 158.0f, 742.0f, 294.0f));
            result.Put(1, JavaCollectionsUtil.SingletonList<TextInfo>(textInfo));
            return result;
        }

        public virtual void CreateTxtFile(IList<FileInfo> inputImages, FileInfo txtFile) {
        }

        public virtual IMetaInfo GetThreadLocalMetaInfo() {
            return threadLocalMetaInfo;
        }

        public virtual IThreadLocalMetaInfoAware SetThreadLocalMetaInfo(IMetaInfo metaInfo) {
            this.threadLocalMetaInfo = metaInfo;
            return this;
        }

        public virtual OcrEngineProperties GetOcrEngineProperties() {
            return ocrEngineProperties;
        }
    }
}

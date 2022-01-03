/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
using System.Collections.Generic;
using System.IO;
using iText.Commons.Actions.Contexts;
using iText.Commons.Actions.Data;
using iText.Commons.Utils;
using iText.Pdfocr;

namespace iText.Pdfocr.Helpers {
    public class CustomProductAwareOcrEngine : IOcrEngine, IProductAware {
        private bool getMetaInfoContainerTriggered = false;

        public CustomProductAwareOcrEngine() {
        }

        public virtual IDictionary<int, IList<TextInfo>> DoImageOcr(FileInfo input) {
            return JavaCollectionsUtil.EmptyMap<int, IList<TextInfo>>();
        }

        public virtual IDictionary<int, IList<TextInfo>> DoImageOcr(FileInfo input, OcrProcessContext ocrProcessContext
            ) {
            return DoImageOcr(input);
        }

        public virtual void CreateTxtFile(IList<FileInfo> inputImages, FileInfo txtFile) {
        }

        public virtual void CreateTxtFile(IList<FileInfo> inputImages, FileInfo txtFile, OcrProcessContext ocrProcessContext
            ) {
        }

        public virtual OcrEngineProperties GetOcrEngineProperties() {
            return null;
        }

        public virtual PdfOcrMetaInfoContainer GetMetaInfoContainer() {
            getMetaInfoContainerTriggered = true;
            return new PdfOcrMetaInfoContainer(new CustomProductAwareOcrEngine.DummyMetaInfo());
        }

        public virtual ProductData GetProductData() {
            return null;
        }

        public virtual bool IsGetMetaInfoContainerTriggered() {
            return getMetaInfoContainerTriggered;
        }

        private class DummyMetaInfo : IMetaInfo {
        }
    }
}

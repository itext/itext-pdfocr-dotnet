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
using iText.Kernel.Counter.Event;

namespace iText.Pdfocr {
    /// <summary>The meta info that is used internally by pdfOcr to pass a wrapped custom meta data</summary>
    public class OcrPdfCreatorMetaInfo : IMetaInfo, IMetaInfoWrapper {
        private IMetaInfo wrappedMetaInfo;

        private Guid uuid;

        private OcrPdfCreatorMetaInfo.PdfDocumentType pdfDocumentType;

        /// <summary>Creates an inner meta info wrapper</summary>
        /// <param name="wrappedMetaInfo">the meta info to be wrapped</param>
        /// <param name="uuid">a unique String which corresponds to the ocr event for which this meta info is passed</param>
        /// <param name="pdfDocumentType">a type of the document which is created during the corresponding ocr event</param>
        public OcrPdfCreatorMetaInfo(IMetaInfo wrappedMetaInfo, Guid uuid, OcrPdfCreatorMetaInfo.PdfDocumentType pdfDocumentType
            ) {
            this.wrappedMetaInfo = wrappedMetaInfo;
            this.uuid = uuid;
            this.pdfDocumentType = pdfDocumentType;
        }

        /// <summary>Gets the unique String which corresponds to the ocr event for which this meta info is passed</summary>
        /// <returns>the unique String which corresponds to the ocr event for which this meta info is passed</returns>
        public virtual Guid GetDocumentId() {
            return uuid;
        }

        /// <summary>Gets the type of the document which is created during the corresponding ocr event</summary>
        /// <returns>the type of the document which is created during the corresponding ocr event</returns>
        public virtual OcrPdfCreatorMetaInfo.PdfDocumentType GetPdfDocumentType() {
            return pdfDocumentType;
        }

        public virtual IMetaInfo GetWrappedMetaInfo() {
            return wrappedMetaInfo;
        }

        /// <summary>The enum which represents types of documents, for which pdfOcr sends different events</summary>
        public enum PdfDocumentType {
            PDF,
            PDFA
        }
    }
}

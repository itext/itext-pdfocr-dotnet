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
using iText.Commons.Actions;
using iText.Commons.Actions.Data;
using iText.Commons.Utils;
using iText.Pdfocr.Exceptions;

namespace iText.Pdfocr.Statistics {
    /// <summary>Class which represents an event for specifying type of an ocr processing.</summary>
    /// <remarks>
    /// Class which represents an event for specifying type of an ocr processing.
    /// For internal usage only.
    /// </remarks>
    public class PdfOcrOutputTypeStatisticsEvent : AbstractStatisticsEvent {
        private const String OCR_OUTPUT_TYPE = "ocrOutput";

        private readonly PdfOcrOutputType type;

        /// <summary>Creates instance of pdfOcr statistics event.</summary>
        /// <param name="type">pdfCcr output type</param>
        /// <param name="productData">product data</param>
        public PdfOcrOutputTypeStatisticsEvent(PdfOcrOutputType type, ProductData productData)
            : base(productData) {
            if (type == null) {
                throw new PdfOcrException(PdfOcrExceptionMessageConstant.STATISTICS_EVENT_TYPE_CANT_BE_NULL);
            }
            if (null == PdfOcrOutputTypeStatisticsAggregator.GetKeyForType(type)) {
                throw new PdfOcrException(PdfOcrExceptionMessageConstant.STATISTICS_EVENT_TYPE_IS_NOT_DETECTED);
            }
            this.type = type;
        }

        /// <summary><inheritDoc/></summary>
        public override AbstractStatisticsAggregator CreateStatisticsAggregatorFromName(String statisticsName) {
            if (OCR_OUTPUT_TYPE.Equals(statisticsName)) {
                return new PdfOcrOutputTypeStatisticsAggregator();
            }
            return base.CreateStatisticsAggregatorFromName(statisticsName);
        }

        /// <summary><inheritDoc/></summary>
        public override IList<String> GetStatisticsNames() {
            return JavaCollectionsUtil.SingletonList(OCR_OUTPUT_TYPE);
        }

        /// <summary>Gets the type of statistic event.</summary>
        /// <returns>the statistics event type</returns>
        public virtual PdfOcrOutputType GetPdfOcrStatisticsEventType() {
            return type;
        }
    }
}

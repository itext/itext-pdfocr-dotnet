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
using System;
using System.Collections.Generic;
using iText.Commons.Actions;
using iText.Commons.Utils;
using iText.Pdfocr.Exceptions;

namespace iText.Pdfocr.Statistics {
    /// <summary>Statistics aggregator which aggregates types of ocr processing.</summary>
    internal class PdfOcrOutputTypeStatisticsAggregator : AbstractStatisticsAggregator {
        private const String STRING_FOR_DATA = "data";

        private const String STRING_FOR_PDF = "pdf";

        private const String STRING_FOR_PDFA = "pdfa";

        private static readonly IDictionary<PdfOcrOutputType, String> OCR_OUTPUT_TYPES;

        static PdfOcrOutputTypeStatisticsAggregator() {
            IDictionary<PdfOcrOutputType, String> temp = new Dictionary<PdfOcrOutputType, String>();
            temp.Put(PdfOcrOutputType.DATA, STRING_FOR_DATA);
            temp.Put(PdfOcrOutputType.PDF, STRING_FOR_PDF);
            temp.Put(PdfOcrOutputType.PDFA, STRING_FOR_PDFA);
            OCR_OUTPUT_TYPES = JavaCollectionsUtil.UnmodifiableMap(temp);
        }

        private readonly Object Lock = new Object();

        private readonly IDictionary<String, long?> numberOfUsagesPerType = new LinkedDictionary<String, long?>();

        /// <summary>Aggregates pdfOcr event type.</summary>
        /// <param name="event">
        /// 
        /// <see cref="PdfOcrOutputTypeStatisticsEvent"/>
        /// instance
        /// </param>
        public override void Aggregate(AbstractStatisticsEvent @event) {
            if (!(@event is PdfOcrOutputTypeStatisticsEvent)) {
                return;
            }
            // the event's properties are required to be not null
            PdfOcrOutputType type = ((PdfOcrOutputTypeStatisticsEvent)@event).GetPdfOcrStatisticsEventType();
            String fileTypeKey = GetKeyForType(type);
            if (null == fileTypeKey) {
                // this line is not expected to be reached, since an exception should have been thrown on event creation
                throw new PdfOcrException(PdfOcrExceptionMessageConstant.STATISTICS_EVENT_TYPE_IS_NOT_DETECTED);
            }
            lock (Lock) {
                long? documentsOfThisRange = numberOfUsagesPerType.Get(fileTypeKey);
                long? currentValue = documentsOfThisRange == null ? 1L : (documentsOfThisRange + 1L);
                numberOfUsagesPerType.Put(fileTypeKey, currentValue);
            }
        }

        /// <summary>Retrieves Map where keys are pdfOcr event types and values are the amounts of such events.</summary>
        /// <returns>
        /// aggregated
        /// <see cref="System.Collections.IDictionary{K, V}"/>
        /// </returns>
        public override Object RetrieveAggregation() {
            return JavaCollectionsUtil.UnmodifiableMap(numberOfUsagesPerType);
        }

        /// <summary>Merges data about amounts of pdfOcr event types from the provided aggregator into this aggregator.
        ///     </summary>
        /// <param name="aggregator">
        /// 
        /// <see cref="PdfOcrOutputTypeStatisticsAggregator"/>
        /// from which data will be taken.
        /// </param>
        public override void Merge(AbstractStatisticsAggregator aggregator) {
            if (!(aggregator is PdfOcrOutputTypeStatisticsAggregator)) {
                return;
            }
            IDictionary<String, long?> otherNumberOfFiles = ((PdfOcrOutputTypeStatisticsAggregator)aggregator).numberOfUsagesPerType;
            lock (Lock) {
                MapUtil.Merge(this.numberOfUsagesPerType, otherNumberOfFiles, (el1, el2) => {
                    if (el2 == null) {
                        return el1;
                    }
                    else {
                        return el1 + el2;
                    }
                }
                );
            }
        }

        internal static String GetKeyForType(PdfOcrOutputType type) {
            return OCR_OUTPUT_TYPES.Get(type);
        }
    }
}

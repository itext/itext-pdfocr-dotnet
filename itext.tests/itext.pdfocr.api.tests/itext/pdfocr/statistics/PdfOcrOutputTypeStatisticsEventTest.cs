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
using iText.Commons.Actions.Data;
using iText.Commons.Logs;
using iText.Commons.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Pdfocr.Statistics {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfOcrOutputTypeStatisticsEventTest : ExtendedITextTest {
        private static readonly ProductData DUMMY_PRODUCT_DATA = new ProductData("test-product", "inner_product", 
            "1.0.0", 1900, 2100);

        [NUnit.Framework.Test]
        public virtual void DefaultEventTest() {
            PdfOcrOutputTypeStatisticsEvent @event = new PdfOcrOutputTypeStatisticsEvent(PdfOcrOutputType.PDF, DUMMY_PRODUCT_DATA
                );
            NUnit.Framework.Assert.AreEqual(PdfOcrOutputType.PDF, @event.GetPdfOcrStatisticsEventType());
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.SingletonList("ocrOutput"), @event.GetStatisticsNames(
                ));
            NUnit.Framework.Assert.AreEqual(typeof(PdfOcrOutputTypeStatisticsAggregator), @event.CreateStatisticsAggregatorFromName
                ("ocrOutput").GetType());
        }

        [NUnit.Framework.Test]
        [LogMessage(CommonsLogMessageConstant.INVALID_STATISTICS_NAME)]
        public virtual void InvalidAggregatorNameTest() {
            NUnit.Framework.Assert.IsNull(new PdfOcrOutputTypeStatisticsEvent(PdfOcrOutputType.PDF, DUMMY_PRODUCT_DATA
                ).CreateStatisticsAggregatorFromName("dummy name"));
        }
    }
}

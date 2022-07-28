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
using iText.Commons.Actions.Data;
using iText.Test;

namespace iText.Pdfocr.Statistics {
    [NUnit.Framework.Category("Unit test")]
    public class PdfOcrOutputTypeStatisticsAggregatorTest : ExtendedITextTest {
        private static readonly ProductData DUMMY_PRODUCT_DATA = new ProductData("test-product", "inner_product", 
            "1.0.0", 1900, 2100);

        [NUnit.Framework.Test]
        public virtual void AggregateEventTest() {
            PdfOcrOutputTypeStatisticsAggregator aggregator = new PdfOcrOutputTypeStatisticsAggregator();
            aggregator.Aggregate(new PdfOcrOutputTypeStatisticsEvent(PdfOcrOutputType.PDF, DUMMY_PRODUCT_DATA));
            aggregator.Aggregate(new PdfOcrOutputTypeStatisticsEvent(PdfOcrOutputType.PDFA, DUMMY_PRODUCT_DATA));
            aggregator.Aggregate(new PdfOcrOutputTypeStatisticsEvent(PdfOcrOutputType.DATA, DUMMY_PRODUCT_DATA));
            aggregator.Aggregate(new PdfOcrOutputTypeStatisticsEvent(PdfOcrOutputType.PDFA, DUMMY_PRODUCT_DATA));
            aggregator.Aggregate(new PdfOcrOutputTypeStatisticsEvent(PdfOcrOutputType.PDF, DUMMY_PRODUCT_DATA));
            IDictionary<String, long?> aggregation = (IDictionary<String, long?>)aggregator.RetrieveAggregation();
            NUnit.Framework.Assert.AreEqual(3, aggregation.Count);
            long? numberOfOcrProcessesWithGivenOutput = aggregation.Get("data");
            NUnit.Framework.Assert.AreEqual(1L, numberOfOcrProcessesWithGivenOutput);
            numberOfOcrProcessesWithGivenOutput = aggregation.Get("pdf");
            NUnit.Framework.Assert.AreEqual(2L, numberOfOcrProcessesWithGivenOutput);
            numberOfOcrProcessesWithGivenOutput = aggregation.Get("pdfa");
            NUnit.Framework.Assert.AreEqual(2L, numberOfOcrProcessesWithGivenOutput);
        }

        [NUnit.Framework.Test]
        public virtual void MergeTest() {
            PdfOcrOutputTypeStatisticsAggregator firstAggregator = new PdfOcrOutputTypeStatisticsAggregator();
            PdfOcrOutputTypeStatisticsAggregator secondAggregator = new PdfOcrOutputTypeStatisticsAggregator();
            firstAggregator.Aggregate(new PdfOcrOutputTypeStatisticsEvent(PdfOcrOutputType.PDF, DUMMY_PRODUCT_DATA));
            firstAggregator.Aggregate(new PdfOcrOutputTypeStatisticsEvent(PdfOcrOutputType.PDFA, DUMMY_PRODUCT_DATA));
            secondAggregator.Aggregate(new PdfOcrOutputTypeStatisticsEvent(PdfOcrOutputType.DATA, DUMMY_PRODUCT_DATA));
            secondAggregator.Aggregate(new PdfOcrOutputTypeStatisticsEvent(PdfOcrOutputType.PDFA, DUMMY_PRODUCT_DATA));
            secondAggregator.Aggregate(new PdfOcrOutputTypeStatisticsEvent(PdfOcrOutputType.PDF, DUMMY_PRODUCT_DATA));
            firstAggregator.Merge(secondAggregator);
            IDictionary<String, long?> aggregation = (IDictionary<String, long?>)firstAggregator.RetrieveAggregation();
            NUnit.Framework.Assert.AreEqual(3, aggregation.Count);
            long? numberOfOcrProcessesWithGivenOutput = aggregation.Get("data");
            NUnit.Framework.Assert.AreEqual(1L, numberOfOcrProcessesWithGivenOutput);
            numberOfOcrProcessesWithGivenOutput = aggregation.Get("pdf");
            NUnit.Framework.Assert.AreEqual(2L, numberOfOcrProcessesWithGivenOutput);
            numberOfOcrProcessesWithGivenOutput = aggregation.Get("pdfa");
            NUnit.Framework.Assert.AreEqual(2L, numberOfOcrProcessesWithGivenOutput);
        }

        [NUnit.Framework.Test]
        public virtual void AggregateInvalidEventTest() {
            PdfOcrOutputTypeStatisticsAggregator aggregator = new PdfOcrOutputTypeStatisticsAggregator();
            aggregator.Aggregate(new PdfOcrOutputTypeStatisticsAggregatorTest.DummyAbstractStatisticsEvent(DUMMY_PRODUCT_DATA
                ));
            NUnit.Framework.Assert.IsTrue(((IDictionary<String, long?>)aggregator.RetrieveAggregation()).IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void MergeInvalidAggregatorTest() {
            PdfOcrOutputTypeStatisticsAggregator aggregator = new PdfOcrOutputTypeStatisticsAggregator();
            aggregator.Merge(new PdfOcrOutputTypeStatisticsAggregatorTest.DummyAbstractStatisticsAggregator());
            NUnit.Framework.Assert.IsTrue(((IDictionary<String, long?>)aggregator.RetrieveAggregation()).IsEmpty());
        }

        private class DummyAbstractStatisticsEvent : AbstractStatisticsEvent {
            protected internal DummyAbstractStatisticsEvent(ProductData productData)
                : base(productData) {
            }

            public override IList<String> GetStatisticsNames() {
                return null;
            }
        }

        private class DummyAbstractStatisticsAggregator : AbstractStatisticsAggregator {
            public override void Aggregate(AbstractStatisticsEvent @event) {
            }

            public override Object RetrieveAggregation() {
                return null;
            }

            public override void Merge(AbstractStatisticsAggregator aggregator) {
            }
        }
    }
}

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
using iText.Pdfocr.Onnxtr.Exceptions;
using iText.Pdfocr.Util;

namespace iText.Pdfocr.Onnxtr.Util {
    /// <summary>Static utility class to help with batching.</summary>
    public sealed class Batching {
        private Batching() {
        }

        /// <summary>Wraps an existing iterator into a new one, which output List-based batches,</summary>
        /// <param name="iterator">iterator to wrap</param>
        /// <param name="batchSize">target batch size. Last batch might have smaller size</param>
        /// <returns>batch iterator wrapper</returns>
        /// <typeparam name="E">input iterator element type</typeparam>
        public static IEnumerator<IList<E>> Wrap<E>(IEnumerator<E> iterator, int batchSize) {
            Objects.RequireNonNull(iterator);
            if (batchSize <= 0) {
                throw new ArgumentException(PdfOcrOnnxTrExceptionMessageConstant.BATCH_SIZE_SHOULD_BE_POSITIVE);
            }

            while (true) {
                var batch = new List<E>(batchSize);
                int count = 0;

                while (count < batchSize && iterator.MoveNext()) {
                    batch.Add(iterator.Current);
                    count++;
                }

                if (batch.Count == 0)
                    yield break;

                yield return batch;
            }
        }
    }
}

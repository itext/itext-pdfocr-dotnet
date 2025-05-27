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

namespace iText.Pdfocr.Onnxtr.Util {
    /// <summary>Static utility class to help with batching.</summary>
    public sealed class Batching {
        private Batching() {
        }

        /// <summary>Wraps an existing iterator into a new one, which output List-based batches,</summary>
        /// <param name="iterator">Iterator to wrap.</param>
        /// <param name="batchSize">Target batch size. Last batch might have smaller size.</param>
        /// <returns>Batch iterator wrapper.</returns>
        /// <typeparam name="E">Input iterator element type.</typeparam>
        public static IEnumerator<IList<E>> Wrap<E>(IEnumerator<E> iterator, int batchSize) {
            Objects.RequireNonNull(iterator);
            if (batchSize <= 0) {
                throw new ArgumentException("batchSize should be positive");
            }
            return new _IEnumerator_53(iterator, batchSize);
        }

        private sealed class _IEnumerator_53 : IEnumerator<IList<E>> {
            public _IEnumerator_53(IEnumerator<E> iterator, int batchSize) {
                this.iterator = iterator;
                this.batchSize = batchSize;
            }

            public bool HasNext() {
                return iterator.HasNext();
            }

            public IList<E> Next() {
                if (!this.HasNext()) {
                    throw new NullReferenceException();
                }
                IList<E> batch = new List<E>(batchSize);
                for (int i = 0; i < batchSize && iterator.HasNext(); ++i) {
                    batch.Add(iterator.Next());
                }
                return batch;
            }

            private readonly IEnumerator<E> iterator;

            private readonly int batchSize;
        }
    }
}

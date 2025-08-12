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
using System.Collections;
using System.Collections.Generic;
using iText.Pdfocr.Onnxtr.Exceptions;
using iText.Pdfocr.Util;

namespace iText.Pdfocr.Onnxtr.Util {
    /// <summary>Generator with batch processing.</summary>
    /// <remarks>
    /// Generator with batch processing. It goes over and processes input values in batches and results
    /// are cached. This is useful for use in ML-models, where you want to process stuff in batches
    /// instead of one-by-ine.
    /// </remarks>
    /// <typeparam name="T">input type</typeparam>
    /// <typeparam name="R">output type</typeparam>
    public class BatchProcessingGenerator<T, R> : IEnumerator<R> {
        private readonly IEnumerator<IList<T>> batchIterator;

        private readonly IBatchProcessor<T, R> batchProcessor;

        /// <summary>Processing result cache.</summary>
        private IList<R> batchResult = null;

        /// <summary>Current position in the processing result cache.</summary>
        private int batchResultIndex;

        /// <summary>Creates a new generator with the provided batch iterator and processor.</summary>
        /// <param name="batchIterator">input batch iterator</param>
        /// <param name="batchProcessor">batch processor</param>
        public BatchProcessingGenerator(IEnumerator<IList<T>> batchIterator, IBatchProcessor<T, R> batchProcessor) {
            this.batchIterator = Objects.RequireNonNull(batchIterator);
            this.batchProcessor = Objects.RequireNonNull(batchProcessor);
        }

        /// <summary><inheritDoc/></summary>
        public virtual bool HasNext() {
            return batchResult != null || batchIterator.MoveNext();
        }

        /// <summary><inheritDoc/></summary>
        public virtual R Next() {
            if (batchResult == null) {
                IList<T> batch = batchIterator.Current;
                batchResult = batchProcessor.ProcessBatch(batch);
                if (batchResult == null || batchResult.Count != batch.Count) {
                    throw new InvalidOperationException(PdfOcrOnnxTrExceptionMessageConstant.INVALID_NUMBER_OF_OUTPUTS);
                }
                batchResultIndex = 0;
            }
            R result = batchResult[batchResultIndex];
            ++batchResultIndex;
            if (batchResultIndex >= batchResult.Count) {
                batchResult = null;
            }
            return result;
        }

        public void Dispose() {
            
        }
        
        public bool MoveNext() {
            if (!HasNext()) {
                return false;
            }
            
            Current = Next();
            return true;
        }
        
        public void Reset() {
            throw new System.NotSupportedException();
        }
        
        public R Current { get; private set; }
        
        object IEnumerator.Current {
            get { return Current; }
        }
    }
}

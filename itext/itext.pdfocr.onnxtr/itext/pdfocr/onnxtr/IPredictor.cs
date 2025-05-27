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

namespace iText.Pdfocr.Onnxtr {
    /// <summary>Interface of a generic predictor.</summary>
    /// <remarks>Interface of a generic predictor. It takes a stream of inputs and returns a same-sizes stream of outputs.
    ///     </remarks>
    /// <typeparam name="T">input type</typeparam>
    /// <typeparam name="R">output type</typeparam>
    public interface IPredictor<T, R> : IDisposable {
        IEnumerator<R> Predict(IEnumerator<T> inputs);

        IEnumerator<R> Predict(IEnumerable<T> inputs);

        void System.IDisposable.Dispose() {
            Close();
        }
    }
}

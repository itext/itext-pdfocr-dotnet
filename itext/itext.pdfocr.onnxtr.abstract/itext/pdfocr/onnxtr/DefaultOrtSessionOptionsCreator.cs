/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using System.Linq;
using Microsoft.ML.OnnxRuntime;

namespace iText.Pdfocr.Onnxtr {
    /// <summary>
    /// Default implementation of
    /// <see cref="IOrtSessionOptionsCreator"/>.
    /// </summary>
    /// <remarks>
    /// Default implementation of
    /// <see cref="IOrtSessionOptionsCreator"/>.
    /// <para />
    /// <c>CUDA</c>
    /// execution provider is added if available, otherwise default
    /// <c>CPU</c>
    /// execution provider is used.
    /// </remarks>
    public class DefaultOrtSessionOptionsCreator : IOrtSessionOptionsCreator {
        public virtual SessionOptions Create() {
            SessionOptions ortOptions = new SessionOptions();
            try {
                if (OrtEnv.Instance().GetAvailableProviders().Contains("CUDAExecutionProvider")) {
                    ortOptions.AppendExecutionProvider_CUDA(0);    
                }
                else {
                    ortOptions.AppendExecutionProvider_CPU();    
                }
                ortOptions.ExecutionMode = ExecutionMode.ORT_SEQUENTIAL;
                ortOptions.GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_ALL;
                ortOptions.IntraOpNumThreads = -1;
                ortOptions.InterOpNumThreads = -1;
                return ortOptions;
            } catch (Exception e) {
                ortOptions.Close();
                throw;
            }
        }
    }
}
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
using iText.Pdfocr.Util;

namespace iText.Pdfocr.Onnx {
    /// <summary>Properties for configuring ONNX models.</summary>
    /// <remarks>
    /// Properties for configuring ONNX models.
    /// <para />
    /// It contains a path to the model, model input properties and an ONNX runtime session options creator.
    /// </remarks>
    public abstract class AbstractOnnxPredictorProperties {
        /// <summary>Default ONNX runtime session options creator.</summary>
        protected internal static readonly IOrtSessionOptionsCreator DEFAULT_ORT_SESSION_CREATOR = new DefaultOrtSessionOptionsCreator
            ();

        /// <summary>Path to the ONNX model to load.</summary>
        protected internal readonly String modelPath;

        /// <summary>Properties of the inputs of the ONNX model.</summary>
        /// <remarks>
        /// Properties of the inputs of the ONNX model. Used for validation (both
        /// input and output, since output mask size is the same) and pre-processing.
        /// </remarks>
        protected internal readonly OnnxInputProperties inputProperties;

        /// <summary>ONNX runtime session options creator.</summary>
        protected internal readonly IOrtSessionOptionsCreator ortSessionOptionsCreator;

        /// <summary>Creates new predictor properties.</summary>
        /// <param name="modelPath">path to the ONNX model to load</param>
        /// <param name="inputProperties">ONNX model input properties</param>
        /// <param name="ortSessionOptionsCreator">ONNX runtime session options creator</param>
        public AbstractOnnxPredictorProperties(String modelPath, OnnxInputProperties inputProperties, IOrtSessionOptionsCreator
             ortSessionOptionsCreator) {
            this.modelPath = Objects.RequireNonNull(modelPath);
            this.inputProperties = Objects.RequireNonNull(inputProperties);
            this.ortSessionOptionsCreator = Objects.RequireNonNull(ortSessionOptionsCreator);
        }

        /// <summary>Returns the ONNX runtime session options creator.</summary>
        /// <returns>the ONNX runtime session options creator</returns>
        public virtual IOrtSessionOptionsCreator GetOrtSessionOptionsCreator() {
            return ortSessionOptionsCreator;
        }

        /// <summary>Returns the ONNX model input properties.</summary>
        /// <returns>the ONNX model input properties</returns>
        public virtual OnnxInputProperties GetInputProperties() {
            return inputProperties;
        }

        /// <summary>Returns the path to the ONNX model.</summary>
        /// <returns>the path to the ONNX model</returns>
        public virtual String GetModelPath() {
            return modelPath;
        }
    }
}

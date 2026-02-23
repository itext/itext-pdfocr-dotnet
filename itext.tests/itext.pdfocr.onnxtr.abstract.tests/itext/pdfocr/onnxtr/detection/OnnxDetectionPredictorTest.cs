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
using iText.Pdfocr.Onnxtr;
using iText.Test;

namespace iText.Pdfocr.Onnxtr.Detection {
    [NUnit.Framework.Category("UnitTest")]
    public class OnnxDetectionPredictorTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void NullModelPathTest() {
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => OnnxDetectionPredictor.DbNet(null));
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => OnnxDetectionPredictor.DbNet(null, null
                ));
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => OnnxDetectionPredictor.Fast(null));
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => OnnxDetectionPredictor.Fast(null, null)
                );
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => OnnxDetectionPredictor.LinkNet(null));
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => OnnxDetectionPredictor.LinkNet(null, null
                ));
            NUnit.Framework.Assert.Catch(typeof(Exception), () => OnnxDetectionPredictor.PaddleOcr(null));
            NUnit.Framework.Assert.Catch(typeof(Exception), () => OnnxDetectionPredictor.PaddleOcr(null, new DefaultOrtSessionOptionsCreator
                ()));
            NUnit.Framework.Assert.Catch(typeof(Exception), () => OnnxDetectionPredictor.PaddleOcr(null, ""));
            NUnit.Framework.Assert.Catch(typeof(Exception), () => OnnxDetectionPredictor.PaddleOcr(null, "", null));
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => OnnxDetectionPredictor.EasyOcr(null));
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => OnnxDetectionPredictor.EasyOcr(null, null
                ));
        }
    }
}

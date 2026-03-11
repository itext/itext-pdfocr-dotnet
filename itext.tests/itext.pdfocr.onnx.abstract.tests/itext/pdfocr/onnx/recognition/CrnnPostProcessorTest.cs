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
using iText.Pdfocr.Onnx;
using iText.Test;
using iText.pdfOcr.Onnx;

namespace iText.Pdfocr.Onnx.Recognition {
    [NUnit.Framework.Category("UnitTest")]
    public class CrnnPostProcessorTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void InitWithInvalidVocabulary() {
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => new CrnnPostProcessor(null));
        }

        [NUnit.Framework.Test]
        public virtual void Process() {
            Vocabulary vocab = new Vocabulary("ABCD ");
            CrnnPostProcessor processor = new CrnnPostProcessor(vocab);
            FloatBufferWrapper probs = FloatBufferWrapper.Wrap(new float[] { 0.11F, 0.15F, 0.13F, 0.12F, 0.11F, 0.97F, 
                        // [blank]
                        0.75F, 0.22F, 0.14F, 0.56F, 0.67F, 0.01F, 
                        // "A"
                        0.24F, 0.14F, 0.10F, 0.42F, 0.13F, 0.96F, 
                        // [blank]
                        0.88F, 0.42F, 0.24F, 0.17F, 0.12F, 0.04F, 
                        // "A"
                        0.66F, 0.33F, 0.17F, 0.52F, 0.47F, 0.06F, 
                        // "A" (should get skipped)
                        0.24F, 0.17F, 0.34F, 0.18F, 0.89F, 0.03F, 
                        // " "
                        0.13F, 0.17F, 0.24F, 0.16F, 0.19F, 0.92F, 
                        // [blank]
                        0.05F, 0.27F, 0.34F, 0.77F, 0.24F, 0.05F, 
                        // "D"
                        0.14F, 0.27F, 0.12F, 0.36F, 0.56F, 0.89F });
            // [blank]
            FloatBufferMdArray buffer = new FloatBufferMdArray(probs, new long[] { 9, vocab.Size() + 1 });
            NUnit.Framework.Assert.AreEqual("AA D", processor.Process(buffer));
        }
    }
}

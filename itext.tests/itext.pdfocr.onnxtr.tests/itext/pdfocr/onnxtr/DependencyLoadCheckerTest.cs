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
using System.Runtime.InteropServices;
using iText.Pdfocr.Exceptions;
using iText.Test;

namespace iText.Pdfocr.Onnxtr {
    [NUnit.Framework.Category("UnitTest")]
    public class DependencyLoadCheckerTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ProcessExceptionTest() {
            NUnit.Framework.Assert.DoesNotThrow(() => DependencyLoadChecker.ProcessException(new Exception("Random.")));
            NUnit.Framework.Assert.DoesNotThrow(() => DependencyLoadChecker.ProcessException(
                new TypeInitializationException("Random.", new Exception("Random."))));

            string typeInitEx = "The type initializer for 'Microsoft.ML.OnnxRuntime.NativeMethods' threw an exception.";

            Exception nativeMethodsOnly = new TypeInitializationException(typeInitEx,
                new NullReferenceException("Object reference not set to an instance of an object."));
            NUnit.Framework.Assert.Catch(typeof(PdfOcrException), 
                () => DependencyLoadChecker.ProcessException(nativeMethodsOnly));
            Exception nullInnerException = new TypeInitializationException(typeInitEx, null);
            NUnit.Framework.Assert.Catch(typeof(PdfOcrException), 
                () => DependencyLoadChecker.ProcessException(nullInnerException));

            Exception ortGetApiBase = new TypeInitializationException(typeInitEx, new EntryPointNotFoundException(
                "Unable to find an entry point named 'OrtGetApiBase' in DLL 'onnxruntime'"));
            Exception onnxruntime = new TypeInitializationException(typeInitEx, new DllNotFoundException(
                "Unable to load DLL 'onnxruntime': The specified module could not be found."));
            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfOcrException), 
                () => DependencyLoadChecker.ProcessException(ortGetApiBase));
            NUnit.Framework.Assert.AreEqual(isWindows, e.Message.Contains("Windows"));
            e = NUnit.Framework.Assert.Catch(typeof(PdfOcrException), 
                () => DependencyLoadChecker.ProcessException(onnxruntime));
            NUnit.Framework.Assert.AreEqual(isWindows, e.Message.Contains("Windows"));
        }
    }
}

/*

This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Common.Logging;
using iText.IO.Util;
using iText.Kernel.Counter;
using Versions.Attributes;

namespace iText.Pdfocr.Tesseract4 {
    public sealed class ReflectionUtils {
        private static readonly ILog logger = LogManager.GetLogger(typeof(iText.Pdfocr.Tesseract4.ReflectionUtils)
            );

            
        private const String NO_PDFOCR_TESSERACT4 = "No license loaded for product pdfOcr-Tesseract4. Please use LicenseKey.loadLicense(...) to load one.";

        private static IDictionary<String, Type> cachedClasses = new Dictionary<String, Type>();

        private static IDictionary<ReflectionUtils.MethodSignature, MemberInfo> cachedMethods = new Dictionary
            <ReflectionUtils.MethodSignature, MemberInfo>();

        static ReflectionUtils() {
            try {
                ContextManager contextManager = ContextManager.GetInstance();
                CallMethod("iText.Kernel.Counter.ContextManager,iText.Kernel", "RegisterGenericContext", contextManager, new Type[] { typeof(
                    ICollection), typeof(ICollection) }, JavaCollectionsUtil.SingletonList("iText.Pdfocr"), JavaCollectionsUtil
                    .SingletonList("iText.Pdfocr.Tesseract4"));
                CallMethod("iText.Kernel.Counter.ContextManager,iText.Kernel", "RegisterGenericContext", contextManager, new Type[] { typeof(
                    ICollection), typeof(ICollection) }, JavaCollectionsUtil.SingletonList("iText.Pdfocr.Tesseract4"
                    ), JavaCollectionsUtil.SingletonList("iText.Pdfocr.Tesseract4"));
            }
            catch (Exception e) {
                logger.Error(e.Message);
            }
        }

        private ReflectionUtils() {
        }

        public static void ScheduledCheck() {
            try {
                String licenseKeyClassName = "iText.License.LicenseKey, itext.licensekey";
                String licenseKeyProductClassName = "iText.License.LicenseKeyProduct, itext.licensekey";
                String licenseKeyFeatureClassName = "iText.License.LicenseKeyProductFeature, itext.licensekey";
                String checkLicenseKeyMethodName = "ScheduledCheck";
                Type licenseKeyClass = GetLicenseKeyClass(licenseKeyClassName);
                if (licenseKeyClass != null)
                {
                    Type licenseKeyProductClass = GetLicenseKeyClass(licenseKeyProductClassName);
                    Type licenseKeyProductFeatureClass = GetLicenseKeyClass(licenseKeyFeatureClassName);
                    Array array = Array.CreateInstance(licenseKeyProductFeatureClass, 0);
                    object[] objects = new object[]
                    {
                        PdfOcrTesseract4ProductInfo.PRODUCT_NAME,
                        PdfOcrTesseract4ProductInfo.MAJOR_VERSION,
                        PdfOcrTesseract4ProductInfo.MINOR_VERSION,
                        array
                    };
                    Object productObject = System.Activator.CreateInstance(licenseKeyProductClass, objects);
                    MethodInfo m = licenseKeyClass.GetMethod(checkLicenseKeyMethodName);
                    m.Invoke(System.Activator.CreateInstance(licenseKeyClass), new object[] { productObject });
                }
            }
            catch (Exception e) {
                if (null != e && null != e.InnerException) {
                    String message = e.InnerException.Message;
                    if (NO_PDFOCR_TESSERACT4.Equals(message)) {
                        throw new Exception(message, e.InnerException);
                    }
                }
                if (!iText.Kernel.Version.IsAGPLVersion()) {
                    throw;
                }
            }
        }

        private static Object CallMethod(String className, String methodName, Object target, Type[] parameterTypes
            , params Object[] args) {
            try {
                MethodInfo method = FindMethod(className, methodName, parameterTypes);
                return method.Invoke(target, args);
            }
            catch (MissingMethodException) {
                logger.Warn(MessageFormatUtil.Format("Cannot find method {0} for class {1}", methodName, className));
            }
            catch (TypeLoadException) {
                logger.Warn(MessageFormatUtil.Format("Cannot find class {0}", className));
            }
            catch (ArgumentException e) {
                logger.Warn(MessageFormatUtil.Format("Illegal arguments passed to {0}#{1} method call: {2}", className, methodName
                    , e.Message));
            }
            catch (Exception e) {
                // Converting checked exceptions to unchecked RuntimeException (java-specific comment).
                //
                // If kernel utils throws an exception at this point, we consider it as unrecoverable situation for
                // its callers (pdfOcr methods).
                // It's might be more suitable to wrap checked exceptions at a bit higher level, but we do it here for
                // the sake of convenience.
                throw new Exception(e.ToString(), e);
            }
            return null;
        }

        private static MethodInfo FindMethod(String className, String methodName, Type[] parameterTypes) {
            ReflectionUtils.MethodSignature tm = new ReflectionUtils.MethodSignature(className, parameterTypes
                , methodName);
            MethodInfo m = (MethodInfo)cachedMethods.Get(tm);
            if (m == null) {
                Type foundClass = FindClass(className);
                if (null != foundClass) {
                    m = foundClass.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
                    cachedMethods.Put(tm, m);
                }
            }
            return m;
        }

        private static Type FindClass(String className) {
            Type c = cachedClasses.Get(className);
            if (c == null) {
                c = GetKernelClass(className);
                cachedClasses.Put(className, c);
            }
            return c;
        }

        private static Type GetLicenseKeyClass(string className)
        {
            String licenseKeyClassFullName = null;
            Assembly assembly = typeof(ReflectionUtils).GetAssembly();
            Attribute keyVersionAttr = assembly.GetCustomAttribute(typeof(KeyVersionAttribute));
            if (keyVersionAttr is KeyVersionAttribute)
            {
                String keyVersion = ((KeyVersionAttribute)keyVersionAttr).KeyVersion;
                String format = "{0}, Version={1}, Culture=neutral, PublicKeyToken=8354ae6d2174ddca";
                licenseKeyClassFullName = String.Format(format, className, keyVersion);
            }
            Type type = null;
            if (licenseKeyClassFullName != null)
            {
                String fileLoadExceptionMessage = null;
                try
                {
                    type = System.Type.GetType(licenseKeyClassFullName);
                }
                catch (FileLoadException fileLoadException)
                {
                    fileLoadExceptionMessage = fileLoadException.Message;
                }
                if (type == null)
                {
                    try
                    {
                        type = System.Type.GetType(className);
                    }
                    catch
                    {
                        // empty
                    }
                    if (type == null && fileLoadExceptionMessage != null)
                    {
                        LogManager.GetLogger(typeof(ReflectionUtils)).Error(fileLoadExceptionMessage);
                    }
                }
            }
            return type;
        }


        private static Type GetKernelClass(string className)
        {
            String licenseKeyClassFullName = null;
            Assembly assembly = typeof(ReflectionUtils).GetAssembly();
            Attribute keyVersionAttr = assembly.GetCustomAttribute(typeof(KernelVersionAttribute));
            if (keyVersionAttr is KernelVersionAttribute)
            {
                String keyVersion = ((KernelVersionAttribute)keyVersionAttr).KernelVersion;
                String format = "{0}, Version={1}, Culture=neutral, PublicKeyToken=8354ae6d2174ddca";
                licenseKeyClassFullName = String.Format(format, className, keyVersion);
            }
            Type type = null;
            if (licenseKeyClassFullName != null)
            {
                String fileLoadExceptionMessage = null;
                try
                {
                    type = System.Type.GetType(licenseKeyClassFullName);
                }
                catch (FileLoadException fileLoadException)
                {
                    fileLoadExceptionMessage = fileLoadException.Message;
                }
                if (type == null)
                {
                    try
                    {
                        type = System.Type.GetType(className);
                    }
                    catch
                    {
                        // empty
                    }
                    if (type == null && fileLoadExceptionMessage != null)
                    {
                        LogManager.GetLogger(typeof(ReflectionUtils)).Error(fileLoadExceptionMessage);
                    }
                }
            }
            return type;
        }


        private class MethodSignature {
            protected internal readonly String className;

            private readonly String methodName;

            protected internal Type[] parameterTypes;

            internal MethodSignature(String className, Type[] parameterTypes)
                : this(className, parameterTypes, null) {
            }

            internal MethodSignature(String className, Type[] parameterTypes, String methodName) {
                this.methodName = methodName;
                this.className = className;
                this.parameterTypes = parameterTypes;
            }

            public override int GetHashCode() {
                int result = className.GetHashCode();
                result = 31 * result + JavaUtil.ArraysHashCode(parameterTypes);
                result = 31 * result + (methodName != null ? methodName.GetHashCode() : 0);
                return result;
            }

            public override bool Equals(Object o) {
                if (this == o) {
                    return true;
                }
                if (o == null || GetType() != o.GetType()) {
                    return false;
                }
                ReflectionUtils.MethodSignature that = (ReflectionUtils.MethodSignature)o;
                if (!className.Equals(that.className)) {
                    return false;
                }
                if (!JavaUtil.ArraysEquals(parameterTypes, that.parameterTypes)) {
                    return false;
                }
                return methodName != null ? methodName.Equals(that.methodName) : that.methodName == null;
            }
        }
    }
}

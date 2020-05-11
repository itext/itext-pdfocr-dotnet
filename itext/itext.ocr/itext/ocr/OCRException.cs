using System;
using System.Collections.Generic;
using iText.IO.Util;

namespace iText.Ocr {
    /// <summary>Exception class for custom exceptions.</summary>
    public class OcrException : Exception {
        public const String IncorrectInputImageFormat = "{0} format is not supported.";

        public const String IncorrectLanguage = "{0} does not exist in {1}";

        public const String LanguageIsNotInTheList = "Provided list of languages doesn't contain {0} language";

        public const String CannotReadInputImage = "Cannot read input image";

        public const String CannotReadProvidedImage = "Cannot read input image {0}";

        public const String CannotReadFont = "Cannot read font";

        public const String TesseractFailed = "Tesseract failed. " + "Please check provided parameters";

        public const String CannotFindPathToTesseractExecutable = "Cannot find path to tesseract executable.";

        public const String CannotFindPathToTessDataDirectory = "Cannot find path to tess data directory";

        private IList<String> messageParams;

        /// <summary>Creates a new OcrException.</summary>
        /// <param name="msg">the detail message.</param>
        /// <param name="e">
        /// the cause
        /// (which is saved for later retrieval
        /// by
        /// <see cref="System.Exception.InnerException()"/>
        /// method).
        /// </param>
        public OcrException(String msg, Exception e)
            : base(msg, e) {
        }

        /// <summary>Creates a new OcrException.</summary>
        /// <param name="msg">the detail message.</param>
        public OcrException(String msg)
            : base(msg) {
        }

        /// <summary><inheritDoc/></summary>
        public override String Message {
            get {
                return this.messageParams != null && this.messageParams.Count != 0 ? MessageFormatUtil.Format(base.Message
                    , this.GetMessageParams()) : base.Message;
            }
        }

        /// <summary>Gets additional params for Exception message.</summary>
        protected internal virtual Object[] GetMessageParams() {
            Object[] parameters = new Object[this.messageParams.Count];
            for (int i = 0; i < this.messageParams.Count; ++i) {
                parameters[i] = this.messageParams[i];
            }
            return parameters;
        }

        /// <summary>Sets additional params for Exception message.</summary>
        /// <param name="messageParams">additional params.</param>
        /// <returns>object itself.</returns>
        public virtual iText.Ocr.OcrException SetMessageParams(params String[] messageParams) {
            this.messageParams = JavaUtil.ArraysAsList(messageParams);
            return this;
        }
    }
}

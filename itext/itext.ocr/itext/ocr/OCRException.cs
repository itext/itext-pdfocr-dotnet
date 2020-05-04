using System;
using System.Collections.Generic;
using iText.IO.Util;

namespace iText.Ocr {
    /// <summary>Exception class for custom exceptions.</summary>
    public class OCRException : Exception {
        public const String INCORRECT_INPUT_IMAGE_FORMAT = "{0} format is not supported.";

        public const String INCORRECT_LANGUAGE = "{0} does not exist in {1}";

        public const String LANGUAGE_IS_NOT_IN_THE_LIST = "Provided list of languages doesn't contain {0} language";

        public const String CANNOT_READ_INPUT_IMAGE = "Cannot read input image";

        public const String CANNOT_READ_PROVIDED_IMAGE = "Cannot read input image {0}";

        public const String CANNOT_READ_FONT = "Cannot read font";

        public const String TESSERACT_FAILED = "Tesseract failed. " + "Please check provided parameters";

        public const String CANNOT_FIND_PATH_TO_TESSERACT_EXECUTABLE = "Cannot find path to tesseract executable.";

        public const String CANNOT_FIND_PATH_TO_TESSDATA = "Cannot find path to tess data directory";

        private IList<String> messageParams;

        /// <summary>Creates a new OCRException.</summary>
        /// <param name="msg">the detail message.</param>
        /// <param name="e">
        /// the cause
        /// (which is saved for later retrieval
        /// by
        /// <see cref="System.Exception.InnerException()"/>
        /// method).
        /// </param>
        public OCRException(String msg, Exception e)
            : base(msg, e) {
        }

        /// <summary>Creates a new OCRException.</summary>
        /// <param name="msg">the detail message.</param>
        public OCRException(String msg)
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
        public virtual Exception SetMessageParams(params String[] messageParams) {
            this.messageParams = new List<String>();
            foreach (String obj in messageParams) {
                this.messageParams.Add(obj);
            }
            return this;
        }
    }
}

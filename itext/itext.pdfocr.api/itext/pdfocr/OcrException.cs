using System;
using System.Collections.Generic;
using iText.IO.Util;

namespace iText.Pdfocr {
    /// <summary>Exception class for custom exceptions.</summary>
    public class OcrException : Exception {
        public const String CANNOT_READ_INPUT_IMAGE = "Cannot read input image";

        public const String CANNOT_READ_FONT = "Cannot read font";

        public const String CANNOT_CREATE_PDF_DOCUMENT = "Cannot create " + "PDF document: {0}";

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
        public virtual iText.Pdfocr.OcrException SetMessageParams(params String[] messageParams) {
            this.messageParams = JavaUtil.ArraysAsList(messageParams);
            return this;
        }
    }
}

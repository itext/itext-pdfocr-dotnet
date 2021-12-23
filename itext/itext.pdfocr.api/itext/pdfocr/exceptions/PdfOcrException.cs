/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
Authors: iText Software.

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
using iText.Commons.Exceptions;
using iText.Commons.Utils;

namespace iText.Pdfocr.Exceptions {
    /// <summary>Exception class for custom exceptions.</summary>
    public class PdfOcrException : ITextException {
        private IList<String> messageParams;

        /// <summary>
        /// Creates a new
        /// <see cref="PdfOcrException"/>.
        /// </summary>
        /// <param name="msg">the detail message.</param>
        /// <param name="e">
        /// the cause
        /// (which is saved for later retrieval
        /// by
        /// <see cref="System.Exception.InnerException()"/>
        /// method).
        /// </param>
        public PdfOcrException(String msg, Exception e)
            : base(msg, e) {
        }

        /// <summary>
        /// Creates a new
        /// <see cref="PdfOcrException"/>.
        /// </summary>
        /// <param name="msg">the detail message.</param>
        public PdfOcrException(String msg)
            : base(msg) {
        }

        /// <summary>
        /// Creates a new
        /// <see cref="PdfOcrException"/>.
        /// </summary>
        /// <param name="e">
        /// the cause
        /// which is saved for later retrieval
        /// by
        /// <see cref="System.Exception.InnerException()"/>
        /// method).
        /// </param>
        public PdfOcrException(Exception e)
            : base(e) {
        }

        /// <summary><inheritDoc/></summary>
        public override String Message {
            get {
                return this.messageParams != null && this.messageParams.Count != 0 ? MessageFormatUtil.Format(base.Message
                    , this.GetMessageParams()) : base.Message;
            }
        }

        /// <summary>Gets additional params for Exception message.</summary>
        /// <returns>params for exception message</returns>
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
        public virtual iText.Pdfocr.Exceptions.PdfOcrException SetMessageParams(params String[] messageParams) {
            this.messageParams = JavaUtil.ArraysAsList(messageParams);
            return this;
        }
    }
}

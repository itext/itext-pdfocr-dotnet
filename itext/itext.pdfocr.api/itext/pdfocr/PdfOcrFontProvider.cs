using System;
using System.IO;
using Common.Logging;
using iText.IO.Font;
using iText.IO.Util;
using iText.Layout.Font;

namespace iText.Pdfocr {
    public class PdfOcrFontProvider : FontProvider {
        /// <summary>Path to the default font.</summary>
        private const String DEFAULT_FONT_PATH = "iText.Pdfocr.Api.font.LiberationSans-Regular.ttf";

        /// <summary>Default font family.</summary>
        private const String DEFAULT_FONT_FAMILY = "LiberationSans";

        /// <summary>
        /// Creates a new
        /// <see cref="PdfOcrFontProvider"/>
        /// instance with the default font
        /// and the default font family.
        /// </summary>
        public PdfOcrFontProvider()
            : base(DEFAULT_FONT_FAMILY) {
            this.AddFont(GetDefaultFont(), PdfEncodings.IDENTITY_H);
        }

        /// <summary>
        /// Creates a new
        /// <see cref="PdfOcrFontProvider"/>
        /// instance.
        /// </summary>
        public PdfOcrFontProvider(FontSet fontSet, String defaultFontFamily)
            : base(fontSet, defaultFontFamily) {
        }

        /// <summary>Gets default font family.</summary>
        /// <returns>default font family as a string</returns>
        public override String GetDefaultFontFamily() {
            return DEFAULT_FONT_FAMILY;
        }

        /// <summary>Gets default font as a byte array.</summary>
        /// <returns>default font as byte[]</returns>
        private byte[] GetDefaultFont() {
            try {
                using (Stream stream = ResourceUtil.GetResourceStream(DEFAULT_FONT_PATH)) {
                    return StreamUtil.InputStreamToArray(stream);
                }
            }
            catch (System.IO.IOException e) {
                LogManager.GetLogger(GetType()).Error(MessageFormatUtil.Format(PdfOcrLogMessageConstant.CANNOT_READ_DEFAULT_FONT
                    , e.Message));
                return new byte[0];
            }
        }
    }
}

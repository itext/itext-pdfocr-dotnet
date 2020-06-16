using System;
using System.Collections.Generic;
using iText.IO.Util;

namespace iText.Pdfocr {
    public class OcrEngineProperties {
        /// <summary>List of languages required for ocr for provided images.</summary>
        private IList<String> languages = JavaCollectionsUtil.EmptyList<String>();

        /// <summary>
        /// Creates a new
        /// <see cref="OcrEngineProperties"/>
        /// instance.
        /// </summary>
        public OcrEngineProperties() {
        }

        /// <summary>
        /// Creates a new
        /// <see cref="OcrEngineProperties"/>
        /// instance
        /// based on another
        /// <see cref="OcrEngineProperties"/>
        /// instance (copy
        /// constructor).
        /// </summary>
        /// <param name="other">
        /// the other
        /// <see cref="OcrEngineProperties"/>
        /// instance
        /// </param>
        public OcrEngineProperties(iText.Pdfocr.OcrEngineProperties other) {
            this.languages = other.languages;
        }

        /// <summary>Gets list of languages required for provided images.</summary>
        /// <returns>
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of languages
        /// </returns>
        public IList<String> GetLanguages() {
            return new List<String>(languages);
        }

        /// <summary>Sets list of languages to be recognized in provided images.</summary>
        /// <remarks>
        /// Sets list of languages to be recognized in provided images.
        /// Consult with documentation of specific engine implementations
        /// to check on which format to give the language in.
        /// </remarks>
        /// <param name="requiredLanguages">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of languages in string
        /// format
        /// </param>
        /// <returns>
        /// the
        /// <see cref="OcrEngineProperties"/>
        /// instance
        /// </returns>
        public iText.Pdfocr.OcrEngineProperties SetLanguages(IList<String> requiredLanguages) {
            languages = JavaCollectionsUtil.UnmodifiableList<String>(requiredLanguages);
            return this;
        }
    }
}

using System;
using System.Collections.Generic;
using iText.IO.Util;

namespace iText.Pdfocr {
    /// <summary>
    /// This class describes the way text info retrieved from HOCR file
    /// is structured.
    /// </summary>
    public class TextInfo {
        /// <summary>Contains any text.</summary>
        private String text;

        /// <summary>Contains 4 float coordinates: bbox parameters.</summary>
        private IList<float> bbox;

        /// <summary>
        /// Creates a new
        /// <see cref="TextInfo"/>
        /// instance.
        /// </summary>
        public TextInfo() {
            text = null;
            bbox = JavaCollectionsUtil.EmptyList<float>();
        }

        /// <summary>
        /// Creates a new
        /// <see cref="TextInfo"/>
        /// instance.
        /// </summary>
        /// <param name="text">any text</param>
        /// <param name="bbox">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of bbox parameters
        /// </param>
        public TextInfo(String text, IList<float> bbox) {
            this.text = text;
            this.bbox = JavaCollectionsUtil.UnmodifiableList<float>(bbox);
        }

        /// <summary>Gets text element.</summary>
        /// <returns>String</returns>
        public virtual String GetText() {
            return text;
        }

        /// <summary>Sets text element.</summary>
        /// <param name="newText">retrieved text</param>
        public virtual void SetText(String newText) {
            text = newText;
        }

        /// <summary>Gets bbox coordinates.</summary>
        /// <returns>
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of bbox parameters
        /// </returns>
        public virtual IList<float> GetBbox() {
            return new List<float>(bbox);
        }

        /// <summary>Sets bbox coordinates.</summary>
        /// <param name="bbox">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of bbox parameters
        /// </param>
        public virtual void SetBbox(IList<float> bbox) {
            this.bbox = JavaCollectionsUtil.UnmodifiableList<float>(bbox);
        }
    }
}

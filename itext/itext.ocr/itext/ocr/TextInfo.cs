using System;
using System.Collections.Generic;
using iText.IO.Util;

namespace iText.Ocr {
    /// <summary>
    /// This class describes the way text info retrieved from HOCR file
    /// is structured.
    /// </summary>
    public class TextInfo {
        /// <summary>Contains any text.</summary>
        private String text;

        /// <summary>Contains 4 float coordinates: bbox parameters.</summary>
        private IList<float> coordinates;

        /// <summary>
        /// Creates a new
        /// <see cref="TextInfo"/>
        /// instance.
        /// </summary>
        public TextInfo() {
            text = null;
            coordinates = JavaCollectionsUtil.EmptyList<float>();
        }

        /// <summary>
        /// Creates a new
        /// <see cref="TextInfo"/>
        /// instance.
        /// </summary>
        /// <param name="newText">any text</param>
        /// <param name="newCoordinates">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of bbox parameters
        /// </param>
        public TextInfo(String newText, IList<float> newCoordinates) {
            text = newText;
            coordinates = JavaCollectionsUtil.UnmodifiableList<float>(newCoordinates);
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
        public virtual IList<float> GetCoordinates() {
            return new List<float>(coordinates);
        }

        /// <summary>Sets bbox coordinates.</summary>
        /// <param name="newCoordinates">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of bbox parameters
        /// </param>
        public virtual void SetCoordinates(IList<float> newCoordinates) {
            coordinates = JavaCollectionsUtil.UnmodifiableList<float>(newCoordinates);
        }
    }
}

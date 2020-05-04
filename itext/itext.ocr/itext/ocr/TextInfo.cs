using System;
using System.Collections.Generic;
using iText.IO.Util;

namespace iText.Ocr {
    /// <summary>TextInfo class.</summary>
    /// <remarks>
    /// TextInfo class.
    /// This class describes item of text info retrieved
    /// from HOCR file after parsing
    /// </remarks>
    public class TextInfo {
        /// <summary>Contains word or line.</summary>
        private String text;

        /// <summary>Contains 4 coordinates: bbox parameters.</summary>
        private IList<float> coordinates;

        /// <summary>TextInfo Constructor.</summary>
        /// <param name="newText">String</param>
        /// <param name="newCoordinates">List<integer></param>
        public TextInfo(String newText, IList<float> newCoordinates) {
            text = newText;
            coordinates = JavaCollectionsUtil.UnmodifiableList<float>(newCoordinates);
        }

        /// <summary>Text element.</summary>
        /// <returns>String</returns>
        public virtual String GetText() {
            return text;
        }

        /// <summary>Text element.</summary>
        /// <param name="newText">String</param>
        public virtual void SetText(String newText) {
            text = newText;
        }

        /// <summary>Bbox coordinates.</summary>
        /// <returns>List<float></returns>
        public virtual IList<float> GetCoordinates() {
            return new List<float>(coordinates);
        }

        /// <summary>Bbox coordinates.</summary>
        /// <param name="newCoordinates">List<float></param>
        public virtual void SetCoordinates(IList<float> newCoordinates) {
            coordinates = JavaCollectionsUtil.UnmodifiableList<float>(newCoordinates);
        }
    }
}

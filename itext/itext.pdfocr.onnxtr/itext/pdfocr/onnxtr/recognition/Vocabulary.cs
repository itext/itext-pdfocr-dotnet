using System;
using System.Text;
using iText.Pdfocr.Util;

namespace iText.Pdfocr.Onnxtr.Recognition {
    /// <summary>A string-based LUT for mapping text recognition model results to characters.</summary>
    /// <remarks>
    /// A string-based LUT for mapping text recognition model results to characters.
    /// <para />
    /// This class assumes, that each character is represented with a single UTF-16
    /// code unit. So the string itself can be used as a LUT. If this is not the
    /// case, results will be unpredictable.
    /// <para />
    /// It pretty much implements
    /// <see cref="iText.Pdfocr.Onnxtr.IOutputLabelMapper{T}"/>
    /// for
    /// <see cref="char?"/>
    /// but since it would involve unnecessary boxing, it is a
    /// standalone thing instead.
    /// </remarks>
    public class Vocabulary {
        public static readonly iText.Pdfocr.Onnxtr.Recognition.Vocabulary ASCII_LOWERCASE = new iText.Pdfocr.Onnxtr.Recognition.Vocabulary
            ("abcdefghijklmnopqrstuvwxyz");

        public static readonly iText.Pdfocr.Onnxtr.Recognition.Vocabulary ASCII_UPPERCASE = new iText.Pdfocr.Onnxtr.Recognition.Vocabulary
            ("ABCDEFGHIJKLMNOPQRSTUVWXYZ");

        public static readonly iText.Pdfocr.Onnxtr.Recognition.Vocabulary ASCII_LETTERS = Concat(ASCII_LOWERCASE, 
            ASCII_UPPERCASE);

        public static readonly iText.Pdfocr.Onnxtr.Recognition.Vocabulary DIGITS = new iText.Pdfocr.Onnxtr.Recognition.Vocabulary
            ("0123456789");

        public static readonly iText.Pdfocr.Onnxtr.Recognition.Vocabulary PUNCTUATION = new iText.Pdfocr.Onnxtr.Recognition.Vocabulary
            ("!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~");

        public static readonly iText.Pdfocr.Onnxtr.Recognition.Vocabulary CURRENCY = new iText.Pdfocr.Onnxtr.Recognition.Vocabulary
            ("£€¥¢฿");

        public static readonly iText.Pdfocr.Onnxtr.Recognition.Vocabulary LATIN = Concat(DIGITS, ASCII_LETTERS, PUNCTUATION
            );

        public static readonly iText.Pdfocr.Onnxtr.Recognition.Vocabulary ENGLISH = Concat(LATIN, new iText.Pdfocr.Onnxtr.Recognition.Vocabulary
            ("°"), CURRENCY);

        public static readonly iText.Pdfocr.Onnxtr.Recognition.Vocabulary LEGACY_FRENCH = Concat(LATIN, new iText.Pdfocr.Onnxtr.Recognition.Vocabulary
            ("°àâéèêëîïôùûçÀÂÉÈËÎÏÔÙÛÇ"), CURRENCY);

        public static readonly iText.Pdfocr.Onnxtr.Recognition.Vocabulary FRENCH = Concat(ENGLISH, new iText.Pdfocr.Onnxtr.Recognition.Vocabulary
            ("àâéèêëîïôùûüçÀÂÉÈÊËÎÏÔÙÛÜÇ"));

        private readonly String lookUpString;

        /// <summary>Creates a new vocabulary based on a look-up string.</summary>
        /// <param name="lookUpString">look-up string to be used as LUT for the vocabulary</param>
        public Vocabulary(String lookUpString) {
            Objects.RequireNonNull(lookUpString);
            if (lookUpString.CodePointCount(0, lookUpString.Length) != lookUpString.Length) {
                throw new ArgumentException("Look-up string contains code points, which are encoded with 2 code units");
            }
            this.lookUpString = lookUpString;
        }

        /// <summary>Creates a new vocabulary by concatenating multiple ones.</summary>
        /// <param name="vocabularies">vocabularies to concatenate</param>
        /// <returns>the new aggregated vocabulary</returns>
        public static iText.Pdfocr.Onnxtr.Recognition.Vocabulary Concat(params iText.Pdfocr.Onnxtr.Recognition.Vocabulary
            [] vocabularies) {
            StringBuilder lutString = new StringBuilder();
            foreach (iText.Pdfocr.Onnxtr.Recognition.Vocabulary vocabulary in vocabularies) {
                lutString.Append(vocabulary.lookUpString);
            }
            return new iText.Pdfocr.Onnxtr.Recognition.Vocabulary(lutString.ToString());
        }

        /// <summary>Returns the look-up string.</summary>
        /// <returns>the look-up string</returns>
        public virtual String GetLookUpString() {
            return lookUpString;
        }

        /// <summary>Returns the size of the vocabulary.</summary>
        /// <returns>the size of the vocabulary</returns>
        public virtual int Size() {
            return lookUpString.Length;
        }

        /// <summary>
        /// Returns character, which is mapped to the specified index in the lookup
        /// string.
        /// </summary>
        /// <param name="index">index to map</param>
        /// <returns>mapped character</returns>
        public virtual char Map(int index) {
            return lookUpString[index];
        }

        public override int GetHashCode() {
            return lookUpString.GetHashCode();
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Pdfocr.Onnxtr.Recognition.Vocabulary that = (iText.Pdfocr.Onnxtr.Recognition.Vocabulary)o;
            return Object.Equals(lookUpString, that.lookUpString);
        }

        public override String ToString() {
            return lookUpString;
        }
    }
}

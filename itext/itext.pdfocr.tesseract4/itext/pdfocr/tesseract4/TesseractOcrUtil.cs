/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2020 iText Group NV
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
using System.Diagnostics;
using System.IO;
using Common.Logging;
using iText.IO.Util;
using Tesseract;

namespace iText.Pdfocr.Tesseract4 {
    /// <summary>
    /// Utilities class to work with tesseract command line tool and image
    /// preprocessing using
    /// <see cref="Net.Sourceforge.Lept4j.ILeptonica"/>.
    /// </summary>
    /// <remarks>
    /// Utilities class to work with tesseract command line tool and image
    /// preprocessing using
    /// <see cref="Net.Sourceforge.Lept4j.ILeptonica"/>.
    /// These all methods have to be ported to .Net manually.
    /// </remarks>
    internal sealed class TesseractOcrUtil {

        /// <summary>List of pages of the image that is being processed.</summary>
        private IList<Pix> imagePages = new List<Pix>();

        /// <summary>
        /// Creates a new
        /// <see cref="TesseractOcrUtil"/>
        /// instance.
        /// </summary>
        internal TesseractOcrUtil() {
        }

        /// <summary>Reads required page from provided tiff image.</summary>
        /// <param name="inputFile">
        /// input image as
        /// <see cref="System.IO.FileInfo"/>
        /// </param>
        /// <param name="pageNumber">number of page</param>
        /// <returns>
        /// result
        /// <see cref="Tesseract.Pix"/>
        /// object created from
        /// given image
        /// </returns>
        internal static Pix ReadPixPageFromTiff(FileInfo inputFile, int pageNumber) {
            Pix pix = null;
            PixArray pixa = null;
            try {
                // read image
                pixa = PixArray.LoadMultiPageTiffFromFile(inputFile.FullName);
                int size = pixa.Count;
                // in case page number is incorrect
                if (pageNumber >= size)
                {
                    LogManager.GetLogger(typeof(TesseractOcrUtil))
                        .Warn(MessageFormatUtil.Format(
                            Tesseract4LogMessageConstant.PAGE_NUMBER_IS_INCORRECT,
                            pageNumber,
                            inputFile.FullName));
                    return null;
                }
                pix = pixa.GetPix(pageNumber);
            } finally {
                DestroyPixa(pixa);
            }
            // return required page to be preprocessed
            return pix;
        }

        /// <summary>
        /// Performs default image preprocessing and saves result to a temporary
        /// file.
        /// </summary>
        /// <param name="pix">
        /// 
        /// <see cref="Tesseract.Pix"/>
        /// object to be processed
        /// </param>
        /// <returns>
        /// path to a created preprocessed image file
        /// as
        /// <see cref="System.String"/>
        /// </returns>
        internal static String PreprocessPixAndSave(Pix pix) {
            String tmpFileName = GetTempDir() + System.Guid.NewGuid().ToString() + ".png";
            try {
                // preprocess image
                pix = PreprocessPix(pix);
                // save preprocessed file
                pix.Save(tmpFileName, ImageFormat.Png);
            } finally {
                DestroyPix(pix);
            }
            return tmpFileName;
        }

        /// <summary>Performs default image preprocessing.</summary>
        /// <remarks>
        /// Performs default image preprocessing.
        /// It includes the following actions:
        /// removing alpha channel,
        /// converting to grayscale,
        /// thresholding.
        /// </remarks>
        /// <param name="pix">
        /// 
        /// <see cref="Tesseract.Pix"/>
        /// object to be processed
        /// </param>
        /// <returns>
        /// preprocessed
        /// <see cref="Tesseract.Pix"/>
        /// object
        /// </returns>
        internal static Pix PreprocessPix(Pix pix) {
            pix = ConvertToGrayscale(pix);
            pix = OtsuImageThresholding(pix);
            return pix;
        }

        /// <summary>
        /// Converts Leptonica
        /// <see cref="Tesseract.Pix"/>
        /// to grayscale.
        /// </summary>
        /// <remarks>
        ///  In .Net image is converted only if this is 32bpp image. In java image is
        ///  converted anyway using different Leptonica methods depending on
        ///  image depth.
        /// </remarks>
        /// <param name="pix">
        /// 
        /// <see cref="Tesseract.Pix"/>
        /// object to be processed
        /// </param>
        /// <returns>
        /// preprocessed
        /// <see cref="Tesseract.Pix"/>
        /// object
        /// </returns>
        internal static Pix ConvertToGrayscale(Pix pix) {
            if (pix != null)
            {
                int depth = pix.Depth;
                if (depth == 32)
                {
                    return pix.ConvertRGBToGray();
                }
                else
                {
                    LogManager.GetLogger(typeof(TesseractOcrUtil))
                        .Info(MessageFormatUtil.Format(Tesseract4LogMessageConstant.CANNOT_CONVERT_IMAGE_TO_GRAYSCALE, depth));
                    return pix;
                }
            }
            else
            {
                return pix;
            }
        }

        /// <summary>
        /// Performs Leptonica Otsu adaptive image thresholding using
        /// <see cref="Net.Sourceforge.Lept4j.Leptonica.PixOtsuAdaptiveThreshold(Tesseract.Pix, int, int, int, int, float, Com.Sun.Jna.Ptr.PointerByReference, Com.Sun.Jna.Ptr.PointerByReference)
        ///     "/>
        /// method.
        /// </summary>
        /// <param name="pix">
        /// 
        /// <see cref="Tesseract.Pix"/>
        /// object to be processed
        /// </param>
        /// <returns>
        /// 
        /// <see cref="Tesseract.Pix"/>
        /// object after thresholding
        /// </returns>
        internal static Pix OtsuImageThresholding(Pix pix) {
            if (pix != null)
            {
                Pix thresholdPix = null;
                if (pix.Depth == 8)
                {
                    thresholdPix = pix.BinarizeOtsuAdaptiveThreshold(pix.Width, pix.Height, 0, 0, 0);
                    if (thresholdPix != null && thresholdPix.Width > 0 && thresholdPix.Height > 0)
                    {
                        DestroyPix(pix);
                        return thresholdPix;
                    }
                    else
                    {
                        LogManager.GetLogger(typeof(TesseractOcrUtil))
                            .Info(MessageFormatUtil.Format(Tesseract4LogMessageConstant.CANNOT_BINARIZE_IMAGE, pix.Depth));
                        DestroyPix(thresholdPix);
                        return pix;
                    }
                }
                else
                {
                    LogManager.GetLogger(typeof(TesseractOcrUtil))
                        .Info(MessageFormatUtil.Format(Tesseract4LogMessageConstant.CANNOT_BINARIZE_IMAGE, pix.Depth));
                    return pix;
                }
            }
            else
            {
                return pix;
            }
        }

        /// <summary>
        /// Destroys
        /// <see cref="Tesseract.Pix"/>
        /// object.
        /// </summary>
        /// <param name="pix">
        /// 
        /// <see cref="Tesseract.Pix"/>
        /// object to be destroyed
        /// </param>
        internal static void DestroyPix(Pix pix) {
            pix.Dispose();
        }

        /// <summary>
        /// Destroys
        /// <see cref="Net.Sourceforge.Lept4j.Pixa"/>
        /// object.
        /// </summary>
        /// <param name="pixa">
        /// 
        /// <see cref="Net.Sourceforge.Lept4j.Pixa"/>
        /// object to be destroyed
        /// </param>
        internal static void DestroyPixa(PixArray pixa) {
            pixa.Dispose();
        }

        /// <summary>Sets tesseract properties.</summary>
        /// <remarks>
        /// Sets tesseract properties.
        /// The following properties are set in this method:
        /// In java: path to tess data, languages, psm
        /// In .Net: psm
        /// This means that other properties have been set during the
        /// initialization of tesseract instance previously or tesseract library
        /// doesn't provide such possibilities in api for .Net or java.
        /// </remarks>
        /// <param name="tesseractInstance">
        /// 
        /// <see cref="Tesseract.TesseractEngine"/>
        /// object
        /// </param>
        /// <param name="tessData">path to tess data directory</param>
        /// <param name="languages">
        /// list of languages in required format
        /// as
        /// <see cref="System.String"/>
        /// </param>
        /// <param name="pageSegMode">
        /// page segmentation mode
        /// <see cref="int?"/>
        /// </param>
        /// <param name="userWordsFilePath">path to a temporary file with user words</param>
        internal static void SetTesseractProperties(TesseractEngine tesseractInstance, String tessData, String languages
            , int? pageSegMode, String userWordsFilePath) {
            if (pageSegMode != null)
            {
                tesseractInstance.DefaultPageSegMode = (PageSegMode)pageSegMode;
            }
        }
        /// <summary>Creates tesseract instance with parameters.</summary>
        /// <remarks>
        /// Creates tesseract instance with parameters.
        /// Method is used to initialize tesseract instance with parameters if it
        /// haven't been initialized yet.
        /// In this method in java 'tessData', 'languages' and 'userWordsFilePath'
        /// properties are unused as they will be set using setters in
        /// <see cref="SetTesseractProperties"/> method. In .Net all these properties
        /// are needed to be provided in tesseract constructor in order to
        /// initialize tesseract instance.Thus, tesseract initialization takes
        /// place in <see cref="Tesseract4LibOcrEngine.Tesseract4LibOcrEngine(Tesseract4OcrEngineProperties)"/> constructor in
        /// java, but in .Net it happens only after all properties are validated,
        /// i.e. just before OCR process.
        /// </remarks>
        /// <param name="tessData">path to tess data directory</param>
        /// <param name="languages">
        /// list of languages in required format as
        /// <see cref="System.String"/>
        /// </param>
        /// <param name="isWindows">true is current os is windows</param>
        /// <param name="userWordsFilePath">path to a temporary file with user words</param>
        /// <returns>
        /// initialized
        /// <see cref="Tesseract.TesseractEngine"/>
        /// object
        /// </returns>
        internal static TesseractEngine InitializeTesseractInstance(bool isWindows, String tessData, String languages,
            String userWordsFilePath) {
            if (tessData == null && languages == null && userWordsFilePath == null) {
                // this means that properties have not been validated yet
                // and Tesseract Engine will be initialized later, just before OCR process itself
                return null;
            } else
            {
                try
                {
                    return new TesseractEngine(tessData, languages,
                        userWordsFilePath != null ? EngineMode.TesseractOnly : EngineMode.Default);
                }
                catch (Exception e)
                {
                    throw new Tesseract4OcrException(isWindows ?
                        Tesseract4OcrException.TESSERACT_LIB_NOT_INSTALLED_WIN :
                        Tesseract4OcrException.TESSERACT_LIB_NOT_INSTALLED, e);
                }
            }
        }

        /// <summary>Returns true if tesseract instance has been already disposed.</summary>
        /// <remarks>
        /// Returns true if tesseract instance has been already disposed.
        /// (used in .net version)
        /// </remarks>
        /// <param name="tesseractInstance">
        /// 
        /// <see cref="Tesseract.TesseractEngine"/>
        /// object to check
        /// </param>
        /// <returns>true if tesseract instance is disposed.</returns>
        internal static bool IsTesseractInstanceDisposed(TesseractEngine tesseractInstance) {
            return tesseractInstance.IsDisposed;
        }

        /// <summary>
        /// Disposes
        /// <see cref="Tesseract.TesseractEngine"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// Disposes
        /// <see cref="Tesseract.TesseractEngine"/>
        /// instance.
        /// (used in .net version)
        /// </remarks>
        /// <param name="tesseractInstance">
        /// 
        /// <see cref="Tesseract.TesseractEngine"/>
        /// object to dispose
        /// </param>
        internal static void DisposeTesseractInstance(TesseractEngine tesseractInstance) {
            tesseractInstance.Dispose();
        }

        /// <summary>
        /// Converts
        /// <see cref="System.Drawing.Bitmap"/>
        /// to
        /// <see cref="Tesseract.Pix"/>.
        /// </summary>
        /// <param name="bufferedImage">
        /// input image as
        /// <see cref="System.Drawing.Bitmap"/>
        /// </param>
        /// <returns>
        /// Pix result converted
        /// <see cref="Tesseract.Pix"/>
        /// object
        /// </returns>
        internal static Pix ConvertImageToPix(System.Drawing.Bitmap bufferedImage) {
            return PixConverter.ToPix(bufferedImage);
        }

        /// <summary>
        /// Converts Leptonica
        /// <see cref="Tesseract.Pix"/>
        /// to
        /// <see cref="System.Drawing.Bitmap"/>
        /// with
        /// <see cref="Net.Sourceforge.Lept4j.ILeptonica.IFF_PNG"/>
        /// image format.
        /// </summary>
        /// <param name="pix">
        /// input
        /// <see cref="Tesseract.Pix"/>
        /// object
        /// </param>
        /// <returns>
        /// result
        /// <see cref="System.Drawing.Bitmap"/>
        /// object
        /// </returns>
        internal static System.Drawing.Bitmap ConvertPixToImage(Pix pix) {
            return PixConverter.ToBitmap(pix);
        }

        /// <summary>Gets current system temporary directory.</summary>
        /// <returns>path to system temporary directory</returns>
        internal static String GetTempDir() {
            return System.IO.Path.GetTempPath();
        }

        /// <summary>
        /// Retrieves list of pages from provided image as list of
        /// <see cref="System.Drawing.Bitmap"/>
        /// , one per page and updates
        /// this list for the image using
        /// <see cref="SetListOfPages(System.Collections.Generic.IList{E})"/>
        /// method.
        /// </summary>
        /// <param name="inputFile">
        /// input image
        /// <see cref="System.IO.FileInfo"/>
        /// </param>
        internal void InitializeImagesListFromTiff(FileInfo inputFile) {
            try
            {
                IList<Pix> pages = new List<Pix>();
                PixArray pixa = null;
                try {
                    pixa = PixArray.LoadMultiPageTiffFromFile(inputFile.FullName);
                    for (int i = 0; i < pixa.Count; i++)
                    {
                        pages.Add(pixa.GetPix(i));
                    }
                    SetListOfPages(pages);
                } finally {
                    DestroyPixa(pixa);
                }
            }
            catch (Exception e)
            {
                LogManager.GetLogger(typeof(TesseractOcrUtil))
                    .Error(MessageFormatUtil.Format(Tesseract4LogMessageConstant.CANNOT_RETRIEVE_PAGES_FROM_IMAGE, e.Message));
            }
        }

        /// <summary>
        /// Gets list of page of processing image as list of
        /// <see cref="System.Drawing.Bitmap"/>
        /// , one per page.
        /// </summary>
        /// <returns>
        /// result
        /// <see cref="System.Collections.IList{E}"/>
        /// of pages
        /// </returns>
        internal IList<Pix> GetListOfPages() {
            return new List<Pix>(imagePages);
        }

        /// <summary>
        /// Sets list of page of processing image as list of
        /// <see cref="System.Drawing.Bitmap"/>
        /// , one per page.
        /// </summary>
        /// <param name="listOfPages">
        /// list of
        /// <see cref="System.Drawing.Bitmap"/>
        /// for
        /// each page.
        /// </param>
        internal void SetListOfPages(IList<Pix> pages) {
            imagePages = JavaCollectionsUtil.UnmodifiableList<Pix>(pages);
        }

        /// <summary>
        /// Performs ocr for the provided image
        /// and returns result as string in required format.
        /// </summary>
        /// <remarks>
        /// Performs ocr for the provided image
        /// and returns result as string in required format.
        /// (
        /// <see cref="OutputFormat"/>
        /// is used in .Net version,
        /// in java output format should already be set)
        /// </remarks>
        /// <param name="tesseractInstance">
        /// 
        /// <see cref="Tesseract.TesseractEngine"/>
        /// object to perform OCR
        /// </param>
        /// <param name="image">
        /// input
        /// <see cref="System.Drawing.Bitmap"/>
        /// to be processed
        /// </param>
        /// <param name="outputFormat">
        /// selected
        /// <see cref="OutputFormat"/>
        /// for tesseract
        /// </param>
        /// <returns>
        /// result as
        /// <see cref="System.String"/>
        /// in required format
        /// </returns>
        internal String GetOcrResultAsString(TesseractEngine tesseractInstance, System.Drawing.Bitmap image,
            OutputFormat outputFormat) {
            String result = null;
            Page page = null;
            try {
                page = tesseractInstance.Process(image);
                if (outputFormat.Equals(OutputFormat.HOCR)) {
                    result = page.GetHOCRText(0);
                } else {
                    result = page.GetText();
                }
            } finally {
                if (page != null) {
                    page.Dispose();
                }
            }
            return result;
        }

        /// <summary>
        /// Performs ocr for the provided image
        /// and returns result as string in required format.
        /// </summary>
        /// <remarks>
        /// Performs ocr for the provided image
        /// and returns result as string in required format.
        /// (
        /// <see cref="OutputFormat"/>
        /// is used in .Net version, in java output format
        /// should already be set)
        /// </remarks>
        /// <param name="tesseractInstance">
        /// 
        /// <see cref="Tesseract.TesseractEngine"/>
        /// object to perform OCR
        /// </param>
        /// <param name="image">
        /// input image as
        /// <see cref="System.IO.FileInfo"/>
        /// to be
        /// processed
        /// </param>
        /// <param name="outputFormat">
        /// selected
        /// <see cref="OutputFormat"/>
        /// for tesseract
        /// </param>
        /// <returns>
        /// result as
        /// <see cref="System.String"/>
        /// in required format
        /// </returns>
        internal String GetOcrResultAsString(TesseractEngine tesseractInstance, FileInfo image, OutputFormat
             outputFormat) {
            Pix pix = Pix.LoadFromFile(image.FullName);
            return GetOcrResultAsString(tesseractInstance, pix, outputFormat);
        }

        /// <summary>
        /// Performs ocr for the provided image
        /// and returns result as string in required format.
        /// </summary>
        /// <remarks>
        /// Performs ocr for the provided image
        /// and returns result as string in required format.
        /// (
        /// <see cref="OutputFormat"/>
        /// is used in .Net version, in java output format
        /// should already be set)
        /// </remarks>
        /// <param name="tesseractInstance">
        /// 
        /// <see cref="Tesseract.TesseractEngine"/>
        /// object to perform OCR
        /// </param>
        /// <param name="pix">
        /// input image as
        /// <see cref="Tesseract.Pix"/>
        /// to be
        /// processed
        /// </param>
        /// <param name="outputFormat">
        /// selected
        /// <see cref="OutputFormat"/>
        /// for tesseract
        /// </param>
        /// <returns>
        /// result as
        /// <see cref="System.String"/>
        /// in required format
        /// </returns>
        internal String GetOcrResultAsString(TesseractEngine tesseractInstance, Pix pix, OutputFormat outputFormat) {
            String result = null;
            Page page = null;
            try
            {
                page = tesseractInstance.Process(pix);
                if (outputFormat.Equals(OutputFormat.HOCR))
                {
                    result = page.GetHOCRText(0);
                }
                else
                {
                    result = page.GetText();
                }
            } finally
            {
                if (page != null) {
                    page.Dispose();
                }
                DestroyPix(pix);
            }
            return result;
        }
    }
}

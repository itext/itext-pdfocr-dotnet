/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
Authors: Apryse Software.

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
using System.IO;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Actions;
using iText.Commons.Actions.Sequence;
using iText.Commons.Utils;
using iText.IO.Font.Otf;
using iText.IO.Image;
using iText.Kernel.Actions.Events;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Layer;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Font;
using iText.Layout.Properties;
using iText.Pdfa;
using iText.Pdfocr.Exceptions;
using iText.Pdfocr.Logs;
using iText.Pdfocr.Statistics;
using iText.Pdfocr.Structuretree;

namespace iText.Pdfocr {
    /// <summary>
    /// <see cref="OcrPdfCreator"/>
    /// is the class that creates PDF documents containing input
    /// images and text that was recognized using provided
    /// <see cref="IOcrEngine"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="OcrPdfCreator"/>
    /// is the class that creates PDF documents containing input
    /// images and text that was recognized using provided
    /// <see cref="IOcrEngine"/>.
    /// <para />
    /// <see cref="OcrPdfCreator"/>
    /// provides possibilities to set list of input images to
    /// be used for OCR, to set scaling mode for images, to set color of text in
    /// output PDF document, to set fixed size of the PDF document's page and to
    /// perform OCR using given images and to return
    /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
    /// as result.
    /// OCR is based on the provided
    /// <see cref="IOcrEngine"/>
    /// (e.g. tesseract reader). This parameter is obligatory and it should be
    /// provided in constructor
    /// or using setter.
    /// </remarks>
    public class OcrPdfCreator {
        /// <summary>The logger.</summary>
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Pdfocr.OcrPdfCreator));

        /// <summary>
        /// Selected
        /// <see cref="IOcrEngine"/>.
        /// </summary>
        private IOcrEngine ocrEngine;

        /// <summary>Set of properties.</summary>
        private OcrPdfCreatorProperties ocrPdfCreatorProperties;

        /// <summary>
        /// Creates a new
        /// <see cref="OcrPdfCreator"/>
        /// instance.
        /// </summary>
        /// <param name="ocrEngine">
        /// 
        /// <see cref="IOcrEngine"/>
        /// selected OCR Reader
        /// </param>
        public OcrPdfCreator(IOcrEngine ocrEngine)
            : this(ocrEngine, new OcrPdfCreatorProperties()) {
        }

        /// <summary>
        /// Creates a new
        /// <see cref="OcrPdfCreator"/>
        /// instance.
        /// </summary>
        /// <param name="ocrEngine">
        /// selected OCR Reader
        /// <see cref="IOcrEngine"/>
        /// </param>
        /// <param name="ocrPdfCreatorProperties">
        /// set of properties for
        /// <see cref="OcrPdfCreator"/>
        /// </param>
        public OcrPdfCreator(IOcrEngine ocrEngine, OcrPdfCreatorProperties ocrPdfCreatorProperties) {
            if (ocrPdfCreatorProperties.IsTagged() && !ocrEngine.IsTaggingSupported()) {
                throw new PdfOcrException(PdfOcrExceptionMessageConstant.TAGGING_IS_NOT_SUPPORTED);
            }
            SetOcrEngine(ocrEngine);
            SetOcrPdfCreatorProperties(ocrPdfCreatorProperties);
        }

        /// <summary>
        /// Gets properties for
        /// <see cref="OcrPdfCreator"/>.
        /// </summary>
        /// <returns>
        /// set properties
        /// <see cref="OcrPdfCreatorProperties"/>
        /// </returns>
        public OcrPdfCreatorProperties GetOcrPdfCreatorProperties() {
            return ocrPdfCreatorProperties;
        }

        /// <summary>
        /// Sets properties for
        /// <see cref="OcrPdfCreator"/>.
        /// </summary>
        /// <param name="ocrPdfCreatorProperties">
        /// set of properties
        /// <see cref="OcrPdfCreatorProperties"/>
        /// for
        /// <see cref="OcrPdfCreator"/>
        /// </param>
        public void SetOcrPdfCreatorProperties(OcrPdfCreatorProperties ocrPdfCreatorProperties) {
            this.ocrPdfCreatorProperties = ocrPdfCreatorProperties;
        }

        /// <summary>
        /// Performs OCR with set parameters using provided
        /// <see cref="IOcrEngine"/>
        /// and
        /// creates PDF using provided
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.DocumentProperties"/>
        /// and
        /// <see cref="iText.Kernel.Pdf.PdfOutputIntent"/>.
        /// </summary>
        /// <remarks>
        /// Performs OCR with set parameters using provided
        /// <see cref="IOcrEngine"/>
        /// and
        /// creates PDF using provided
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.DocumentProperties"/>
        /// and
        /// <see cref="iText.Kernel.Pdf.PdfOutputIntent"/>
        /// . PDF/A-3u document will be created if
        /// provided
        /// <see cref="iText.Kernel.Pdf.PdfOutputIntent"/>
        /// is not null.
        /// <para />
        /// NOTE that after executing this method you will have a product event from
        /// the both itextcore and pdfOcr. Therefore, use this method only if you need to work
        /// with the generated
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// . If you don't need this, use the
        /// <see cref="CreatePdfAFile(System.Collections.Generic.IList{E}, System.IO.FileInfo, iText.Kernel.Pdf.PdfOutputIntent)
        ///     "/>
        /// method. In this case, only the pdfOcr event will be dispatched.
        /// </remarks>
        /// <param name="inputImages">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of images to be OCRed
        /// </param>
        /// <param name="pdfWriter">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>
        /// object
        /// to write final PDF document to
        /// </param>
        /// <param name="documentProperties">document properties</param>
        /// <param name="pdfOutputIntent">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfOutputIntent"/>
        /// for PDF/A-3u document
        /// </param>
        /// <param name="ocrProcessProperties">
        /// extra OCR process properties passed to
        /// <see cref="OcrProcessContext"/>
        /// </param>
        /// <returns>
        /// result PDF/A-3u
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// object
        /// </returns>
        public PdfDocument CreatePdfA(IList<FileInfo> inputImages, PdfWriter pdfWriter, DocumentProperties documentProperties
            , PdfOutputIntent pdfOutputIntent, IOcrProcessProperties ocrProcessProperties) {
            LOGGER.LogInformation(MessageFormatUtil.Format(PdfOcrLogMessageConstant.START_OCR_FOR_IMAGES, inputImages.
                Count));
            // create event helper
            SequenceId pdfSequenceId = new SequenceId();
            OcrPdfCreatorEventHelper ocrEventHelper = new OcrPdfCreatorEventHelper(pdfSequenceId, ocrPdfCreatorProperties
                .GetMetaInfo());
            OcrProcessContext ocrProcessContext = new OcrProcessContext(ocrEventHelper);
            ocrProcessContext.SetOcrProcessProperties(ocrProcessProperties);
            // map contains:
            // keys: image files
            // values:
            // map pageNumber -> retrieved text data(text and its coordinates)
            IDictionary<FileInfo, IDictionary<int, IList<TextInfo>>> imagesTextData = new LinkedDictionary<FileInfo, IDictionary
                <int, IList<TextInfo>>>(inputImages.Count * 2);
            foreach (FileInfo inputImage in inputImages) {
                imagesTextData.Put(inputImage, ocrEngine.DoImageOcr(inputImage, ocrProcessContext));
            }
            // create PdfDocument
            return CreatePdfDocument(pdfWriter, pdfOutputIntent, imagesTextData, pdfSequenceId, documentProperties);
        }

        /// <summary>
        /// Performs OCR with set parameters using provided
        /// <see cref="IOcrEngine"/>
        /// and
        /// creates PDF using provided
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>
        /// and
        /// <see cref="iText.Kernel.Pdf.PdfOutputIntent"/>.
        /// </summary>
        /// <remarks>
        /// Performs OCR with set parameters using provided
        /// <see cref="IOcrEngine"/>
        /// and
        /// creates PDF using provided
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>
        /// and
        /// <see cref="iText.Kernel.Pdf.PdfOutputIntent"/>
        /// . PDF/A-3u document will be created if
        /// provided
        /// <see cref="iText.Kernel.Pdf.PdfOutputIntent"/>
        /// is not null.
        /// <para />
        /// NOTE that after executing this method you will have a product event from
        /// the both itextcore and pdfOcr. Therefore, use this method only if you need to work
        /// with the generated
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// . If you don't need this, use the
        /// <see cref="CreatePdfAFile(System.Collections.Generic.IList{E}, System.IO.FileInfo, iText.Kernel.Pdf.PdfOutputIntent)
        ///     "/>
        /// method. In this case, only the pdfOcr event will be dispatched.
        /// </remarks>
        /// <param name="inputImages">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of images to be OCRed
        /// </param>
        /// <param name="pdfWriter">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>
        /// object
        /// to write final PDF document to
        /// </param>
        /// <param name="pdfOutputIntent">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfOutputIntent"/>
        /// for PDF/A-3u document
        /// </param>
        /// <returns>
        /// result PDF/A-3u
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// object
        /// </returns>
        public PdfDocument CreatePdfA(IList<FileInfo> inputImages, PdfWriter pdfWriter, PdfOutputIntent pdfOutputIntent
            ) {
            return CreatePdfA(inputImages, pdfWriter, new DocumentProperties(), pdfOutputIntent);
        }

        /// <summary>
        /// Performs OCR with set parameters using provided
        /// <see cref="IOcrEngine"/>
        /// and
        /// creates PDF using provided
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.DocumentProperties"></see>
        /// and
        /// <see cref="iText.Kernel.Pdf.PdfOutputIntent"/>.
        /// </summary>
        /// <remarks>
        /// Performs OCR with set parameters using provided
        /// <see cref="IOcrEngine"/>
        /// and
        /// creates PDF using provided
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>
        /// ,
        /// <see cref="iText.Kernel.Pdf.DocumentProperties"></see>
        /// and
        /// <see cref="iText.Kernel.Pdf.PdfOutputIntent"/>
        /// . PDF/A-3u document will be created if
        /// provided
        /// <see cref="iText.Kernel.Pdf.PdfOutputIntent"/>
        /// is not null.
        /// <para />
        /// NOTE that after executing this method you will have a product event from
        /// the both itextcore and pdfOcr. Therefore, use this method only if you need to work
        /// with the generated
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// . If you don't need this, use the
        /// <see cref="CreatePdfAFile(System.Collections.Generic.IList{E}, System.IO.FileInfo, iText.Kernel.Pdf.PdfOutputIntent)
        ///     "/>
        /// method. In this case, only the pdfOcr event will be dispatched.
        /// </remarks>
        /// <param name="inputImages">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of images to be OCRed
        /// </param>
        /// <param name="pdfWriter">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>
        /// object
        /// to write final PDF document to
        /// </param>
        /// <param name="documentProperties">document properties</param>
        /// <param name="pdfOutputIntent">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfOutputIntent"/>
        /// for PDF/A-3u document
        /// </param>
        /// <returns>
        /// result PDF/A-3u
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// object
        /// </returns>
        public PdfDocument CreatePdfA(IList<FileInfo> inputImages, PdfWriter pdfWriter, DocumentProperties documentProperties
            , PdfOutputIntent pdfOutputIntent) {
            return CreatePdfA(inputImages, pdfWriter, documentProperties, pdfOutputIntent, null);
        }

        /// <summary>
        /// Performs OCR with set parameters using provided
        /// <see cref="IOcrEngine"/>
        /// and
        /// creates PDF using provided
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>.
        /// </summary>
        /// <remarks>
        /// Performs OCR with set parameters using provided
        /// <see cref="IOcrEngine"/>
        /// and
        /// creates PDF using provided
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>.
        /// <para />
        /// NOTE that after executing this method you will have a product event from
        /// the both itextcore and pdfOcr. Therefore, use this method only if you need to work
        /// with the generated
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// . If you don't need this, use the
        /// <see cref="CreatePdfFile(System.Collections.Generic.IList{E}, System.IO.FileInfo)"/>
        /// method. In this case, only the pdfOcr event will be dispatched.
        /// </remarks>
        /// <param name="inputImages">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of images to be OCRed
        /// </param>
        /// <param name="pdfWriter">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>
        /// object
        /// to write final PDF document to
        /// </param>
        /// <param name="documentProperties">document properties</param>
        /// <param name="ocrProcessProperties">extra OCR process properties passed to OcrProcessContext</param>
        /// <returns>
        /// result
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// object
        /// </returns>
        public PdfDocument CreatePdf(IList<FileInfo> inputImages, PdfWriter pdfWriter, DocumentProperties documentProperties
            , IOcrProcessProperties ocrProcessProperties) {
            return CreatePdfA(inputImages, pdfWriter, documentProperties, null, ocrProcessProperties);
        }

        /// <summary>
        /// Performs OCR with set parameters using provided
        /// <see cref="IOcrEngine"/>
        /// and
        /// creates PDF using provided
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>.
        /// </summary>
        /// <remarks>
        /// Performs OCR with set parameters using provided
        /// <see cref="IOcrEngine"/>
        /// and
        /// creates PDF using provided
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>.
        /// <para />
        /// NOTE that after executing this method you will have a product event from
        /// the both itextcore and pdfOcr. Therefore, use this method only if you need to work
        /// with the generated
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// . If you don't need this, use the
        /// <see cref="CreatePdfFile(System.Collections.Generic.IList{E}, System.IO.FileInfo)"/>
        /// method. In this case, only the pdfOcr event will be dispatched.
        /// </remarks>
        /// <param name="inputImages">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of images to be OCRed
        /// </param>
        /// <param name="pdfWriter">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>
        /// object
        /// to write final PDF document to
        /// </param>
        /// <param name="documentProperties">document properties</param>
        /// <returns>
        /// result
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// object
        /// </returns>
        public PdfDocument CreatePdf(IList<FileInfo> inputImages, PdfWriter pdfWriter, DocumentProperties documentProperties
            ) {
            return CreatePdfA(inputImages, pdfWriter, documentProperties, null, null);
        }

        /// <summary>
        /// Performs OCR with set parameters using provided
        /// <see cref="IOcrEngine"/>
        /// and
        /// creates PDF using provided
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>.
        /// </summary>
        /// <remarks>
        /// Performs OCR with set parameters using provided
        /// <see cref="IOcrEngine"/>
        /// and
        /// creates PDF using provided
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>.
        /// <para />
        /// NOTE that after executing this method you will have a product event from
        /// the both itextcore and pdfOcr. Therefore, use this method only if you need to work
        /// with the generated
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// . If you don't need this, use the
        /// <see cref="CreatePdfFile(System.Collections.Generic.IList{E}, System.IO.FileInfo)"/>
        /// method. In this case, only the pdfOcr event will be dispatched.
        /// </remarks>
        /// <param name="inputImages">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of images to be OCRed
        /// </param>
        /// <param name="pdfWriter">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>
        /// object
        /// to write final PDF document to
        /// </param>
        /// <returns>
        /// result
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// object
        /// </returns>
        public PdfDocument CreatePdf(IList<FileInfo> inputImages, PdfWriter pdfWriter) {
            return CreatePdfA(inputImages, pdfWriter, new DocumentProperties(), null, null);
        }

        /// <summary>
        /// Performs OCR with set parameters using provided
        /// <see cref="IOcrEngine"/>
        /// and
        /// creates PDF using provided
        /// <see cref="System.IO.FileInfo"/>.
        /// </summary>
        /// <param name="inputImages">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of images to be OCRed
        /// </param>
        /// <param name="outPdfFile">
        /// the
        /// <see cref="System.IO.FileInfo"/>
        /// object to write final PDF document to
        /// </param>
        public virtual void CreatePdfFile(IList<FileInfo> inputImages, FileInfo outPdfFile) {
            CreatePdfAFile(inputImages, outPdfFile, null);
        }

        /// <summary>
        /// Performs OCR with set parameters using provided
        /// <see cref="IOcrEngine"/>
        /// and
        /// creates PDF using provided
        /// <see cref="System.IO.FileInfo"/>
        /// and
        /// <see cref="iText.Kernel.Pdf.PdfOutputIntent"/>.
        /// </summary>
        /// <remarks>
        /// Performs OCR with set parameters using provided
        /// <see cref="IOcrEngine"/>
        /// and
        /// creates PDF using provided
        /// <see cref="System.IO.FileInfo"/>
        /// and
        /// <see cref="iText.Kernel.Pdf.PdfOutputIntent"/>.
        /// PDF/A-3u document will be created if provided
        /// <see cref="iText.Kernel.Pdf.PdfOutputIntent"/>
        /// is not null.
        /// </remarks>
        /// <param name="inputImages">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of images to be OCRed
        /// </param>
        /// <param name="outPdfFile">
        /// the
        /// <see cref="System.IO.FileInfo"/>
        /// object to write final PDF document to
        /// </param>
        /// <param name="pdfOutputIntent">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfOutputIntent"/>
        /// for PDF/A-3u document
        /// </param>
        public virtual void CreatePdfAFile(IList<FileInfo> inputImages, FileInfo outPdfFile, PdfOutputIntent pdfOutputIntent
            ) {
            DocumentProperties documentProperties = new DocumentProperties();
            if (ocrPdfCreatorProperties.GetMetaInfo() != null) {
                documentProperties.SetEventCountingMetaInfo(ocrPdfCreatorProperties.GetMetaInfo());
            }
            else {
                if (ocrEngine is IProductAware) {
                    documentProperties.SetEventCountingMetaInfo(((IProductAware)ocrEngine).GetMetaInfoContainer().GetMetaInfo(
                        ));
                }
            }
            using (PdfWriter pdfWriter = new PdfWriter(outPdfFile.FullName)) {
                PdfDocument pdfDocument = CreatePdfA(inputImages, pdfWriter, documentProperties, pdfOutputIntent);
                pdfDocument.Close();
            }
        }

        /// <summary>
        /// Gets used
        /// <see cref="IOcrEngine"/>
        /// reader object to perform OCR.
        /// </summary>
        /// <returns>
        /// selected
        /// <see cref="IOcrEngine"/>
        /// instance
        /// </returns>
        public IOcrEngine GetOcrEngine() {
            return ocrEngine;
        }

        /// <summary>
        /// Sets
        /// <see cref="IOcrEngine"/>
        /// reader object to perform OCR.
        /// </summary>
        /// <param name="reader">
        /// selected
        /// <see cref="IOcrEngine"/>
        /// instance
        /// </param>
        public void SetOcrEngine(IOcrEngine reader) {
            ocrEngine = reader;
        }

        /// <summary>Performs OCR of all images in an input PDF file and generates searchable PDF.</summary>
        /// <remarks>
        /// Performs OCR of all images in an input PDF file and generates searchable PDF.
        /// <para />
        /// By default, it does not allow to OCR PDF/A documents and tagged documents. The reason is that the result document
        /// might not comply with PDF/A specification and an added content might be not tagged depending on the
        /// <see cref="IOcrEngine"/>
        /// implementation. To overrule this behavior one can override
        /// <see cref="ValidateInputPdfDocument(iText.Kernel.Pdf.PdfDocument)"/>
        /// with an empty implementation.
        /// <para />
        /// Note that
        /// <see cref="OcrPdfCreatorProperties.SetPageSize(iText.Kernel.Geom.Rectangle)"/>
        /// ,
        /// <see cref="OcrPdfCreatorProperties.SetScaleMode(ScaleMode)"/>
        /// and
        /// <see cref="OcrPdfCreatorProperties.SetImageLayerName(System.String)"/>
        /// have no effect for this method.
        /// </remarks>
        /// <param name="inputPdf">PDF file to OCR</param>
        /// <param name="outputPdf">searchable PDF with the recognized text on top of the images</param>
        public virtual void MakePdfSearchable(FileInfo inputPdf, FileInfo outputPdf) {
            MakePdfSearchable(inputPdf, outputPdf, null);
        }

        /// <summary>Performs OCR of all images in an input PDF file and generates searchable PDF.</summary>
        /// <remarks>
        /// Performs OCR of all images in an input PDF file and generates searchable PDF.
        /// <para />
        /// By default, it does not allow to OCR PDF/A documents and tagged documents. The reason is that the result document
        /// might not comply with PDF/A specification and an added content might be not tagged depending on the
        /// <see cref="IOcrEngine"/>
        /// implementation. To overrule this behavior one can override
        /// <see cref="ValidateInputPdfDocument(iText.Kernel.Pdf.PdfDocument)"/>
        /// with an empty implementation.
        /// <para />
        /// Note that
        /// <see cref="OcrPdfCreatorProperties.SetPageSize(iText.Kernel.Geom.Rectangle)"/>
        /// ,
        /// <see cref="OcrPdfCreatorProperties.SetScaleMode(ScaleMode)"/>
        /// and
        /// <see cref="OcrPdfCreatorProperties.SetImageLayerName(System.String)"/>
        /// have no effect for this method.
        /// </remarks>
        /// <param name="inputPdf">PDF file to OCR</param>
        /// <param name="outputPdf">searchable PDF with the recognized text on top of the images</param>
        /// <param name="ocrProcessProperties">
        /// extra OCR process properties passed to
        /// <see cref="OcrProcessContext"/>.
        /// </param>
        public virtual void MakePdfSearchable(FileInfo inputPdf, FileInfo outputPdf, IOcrProcessProperties ocrProcessProperties
            ) {
            try {
                using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(inputPdf), new PdfWriter(outputPdf))) {
                    MakePdfSearchable(pdfDoc, ocrProcessProperties);
                }
            }
            catch (System.IO.IOException e) {
                throw new PdfOcrException(PdfOcrExceptionMessageConstant.IO_EXCEPTION_OCCURRED, e);
            }
        }

        /// <summary>Performs OCR of all images in an input PDF document and adds recognized text on top of the images.
        ///     </summary>
        /// <remarks>
        /// Performs OCR of all images in an input PDF document and adds recognized text on top of the images.
        /// <para />
        /// By default, it does not allow to OCR PDF/A documents and tagged documents. The reason is that the result document
        /// might not comply with PDF/A specification and an added content might be not tagged depending on the
        /// <see cref="IOcrEngine"/>
        /// implementation. To overrule this behavior one can override
        /// <see cref="ValidateInputPdfDocument(iText.Kernel.Pdf.PdfDocument)"/>
        /// with an empty implementation.
        /// <para />
        /// Note that
        /// <see cref="OcrPdfCreatorProperties.SetPageSize(iText.Kernel.Geom.Rectangle)"/>
        /// ,
        /// <see cref="OcrPdfCreatorProperties.SetScaleMode(ScaleMode)"/>
        /// and
        /// <see cref="OcrPdfCreatorProperties.SetImageLayerName(System.String)"/>
        /// have no effect for this method.
        /// </remarks>
        /// <param name="pdfDoc">PDF document with images to OCR</param>
        public virtual void MakePdfSearchable(PdfDocument pdfDoc) {
            MakePdfSearchable(pdfDoc, null);
        }

        /// <summary>Performs OCR of all images in an input PDF document and adds recognized text on top of the images.
        ///     </summary>
        /// <remarks>
        /// Performs OCR of all images in an input PDF document and adds recognized text on top of the images.
        /// <para />
        /// By default, it does not allow to OCR PDF/A documents and tagged documents. The reason is that the result document
        /// might not comply with PDF/A specification and an added content might be not tagged depending on the
        /// <see cref="IOcrEngine"/>
        /// implementation. To overrule this behavior one can override
        /// <see cref="ValidateInputPdfDocument(iText.Kernel.Pdf.PdfDocument)"/>
        /// with an empty implementation.
        /// <para />
        /// Note that
        /// <see cref="OcrPdfCreatorProperties.SetPageSize(iText.Kernel.Geom.Rectangle)"/>
        /// ,
        /// <see cref="OcrPdfCreatorProperties.SetScaleMode(ScaleMode)"/>
        /// and
        /// <see cref="OcrPdfCreatorProperties.SetImageLayerName(System.String)"/>
        /// have no effect for this method.
        /// </remarks>
        /// <param name="pdfDoc">PDF document with images to OCR</param>
        /// <param name="ocrProcessProperties">
        /// extra OCR process properties passed to
        /// <see cref="OcrProcessContext"/>
        /// </param>
        public virtual void MakePdfSearchable(PdfDocument pdfDoc, IOcrProcessProperties ocrProcessProperties) {
            // Only PdfDocument in stamping mode is allowed
            if (pdfDoc.GetReader() == null || pdfDoc.GetWriter() == null) {
                throw new PdfOcrException(PdfOcrExceptionMessageConstant.PDF_DOCUMENT_MUST_BE_OPENED_IN_STAMPING_MODE);
            }
            ValidateInputPdfDocument(pdfDoc);
            if (ocrPdfCreatorProperties.GetPageSize() != null) {
                LOGGER.LogWarning(PdfOcrLogMessageConstant.PAGE_SIZE_IS_NOT_APPLIED);
                ocrPdfCreatorProperties.SetPageSize(null);
            }
            if (ocrPdfCreatorProperties.GetImageLayerName() != null) {
                LOGGER.LogWarning(PdfOcrLogMessageConstant.IMAGE_LAYER_NAME_IS_NOT_APPLIED);
                ocrPdfCreatorProperties.SetImageLayerName(null);
            }
            // Let's respect language and title properties
            bool hasPdfLangProperty = ocrPdfCreatorProperties.GetPdfLang() != null && !String.IsNullOrEmpty(ocrPdfCreatorProperties
                .GetPdfLang());
            if (hasPdfLangProperty) {
                pdfDoc.GetCatalog().SetLang(new PdfString(ocrPdfCreatorProperties.GetPdfLang()));
            }
            // Set title
            if (ocrPdfCreatorProperties.GetTitle() != null) {
                pdfDoc.GetCatalog().SetViewerPreferences(new PdfViewerPreferences().SetDisplayDocTitle(true));
                PdfDocumentInfo info = pdfDoc.GetDocumentInfo();
                info.SetTitle(ocrPdfCreatorProperties.GetTitle());
            }
            // Reset passed font provider
            ocrPdfCreatorProperties.GetFontProvider().Reset();
            // Create event helper
            OcrPdfCreatorEventHelper ocrEventHelper = new OcrPdfCreatorEventHelper(pdfDoc.GetDocumentIdWrapper(), ocrPdfCreatorProperties
                .GetMetaInfo());
            OcrProcessContext ocrProcessContext = new OcrProcessContext(ocrEventHelper);
            ocrProcessContext.SetOcrProcessProperties(ocrProcessProperties);
            // Create layers if requested
            PdfLayer[] layers = CreatePdfLayers(ocrPdfCreatorProperties.GetImageLayerName(), ocrPdfCreatorProperties.GetTextLayerName
                (), pdfDoc);
            IList<String> allImagePaths = new List<String>();
            try {
                for (int pageNr = 1; pageNr <= pdfDoc.GetNumberOfPages(); ++pageNr) {
                    PdfPage pdfPage = pdfDoc.GetPage(pageNr);
                    // Extract images to temp files
                    IList<ImageExtraction.PageImageData> pageImageData = ImageExtraction.ExtractImagesFromPdfPage(pdfPage);
                    // Image file - image position on the page + OCR result
                    IDictionary<ImageExtraction.PageImageData, IDictionary<int, IList<TextInfo>>> imagesTextData = new LinkedDictionary
                        <ImageExtraction.PageImageData, IDictionary<int, IList<TextInfo>>>(pageImageData.Count);
                    foreach (ImageExtraction.PageImageData image in pageImageData) {
                        allImagePaths.Add(image.GetPath().FullName);
                        imagesTextData.Put(image, ocrEngine.DoImageOcr(image.GetPath(), ocrProcessContext));
                    }
                    // Put the result into pdf
                    AddToPdfPage(pdfPage, imagesTextData, layers[1]);
                }
            }
            catch (System.IO.IOException e) {
                throw new PdfOcrException(PdfOcrExceptionMessageConstant.IO_EXCEPTION_OCCURRED, e);
            }
            finally {
                foreach (String imagePath in allImagePaths) {
                    try {
                        File.Delete(System.IO.Path.Combine(imagePath));
                    }
                    catch (Exception) {
                    }
                }
            }
        }

        // Some temp file might not be removed. Not a big deal.
        /// <summary>Validates input PDF document.</summary>
        /// <remarks>
        /// Validates input PDF document.
        /// <para />
        /// It checks that an input document is not tagged and not PDF/A. If you need to OCR tagged and/or PDF/A documents,
        /// override this method with empty implementation. In that case it would be best to use
        /// <see cref="MakePdfSearchable(iText.Kernel.Pdf.PdfDocument, IOcrProcessProperties)"/>
        /// overload because there you can pass
        /// <see cref="iText.Pdfa.PdfADocument"/>
        /// or PdfUADocument instance which will do the validation of the output document.
        /// </remarks>
        /// <param name="pdfDoc">a PDF document to check</param>
        protected internal virtual void ValidateInputPdfDocument(PdfDocument pdfDoc) {
            if (pdfDoc.IsTagged()) {
                // None of our engines supports tagging so far. Theoretically if tagging is supported, we could proceed
                // but then it opens another question. What to do with PDF UA? Still forbid or rely on our UA checks?
                // User probably can provide all the required info not to break the conformance but still.
                throw new PdfOcrException(PdfOcrExceptionMessageConstant.TAGGED_PDF_IS_NOT_SUPPORTED);
            }
            if (pdfDoc.GetConformance().IsPdfA()) {
                // Even though we allow to create pdf/a documents from images,
                // it would still be safer to forbid pdfa input for now.
                // For example, input document may be without output intent. Then we have to request it from the user.
                // It complicates API and might still be not enough.
                throw new PdfOcrException(PdfOcrExceptionMessageConstant.PDFA_IS_NOT_SUPPORTED);
            }
        }

        /// <summary>Adds image (or its one page) and text that was found there to canvas.</summary>
        /// <param name="pdfDocument">
        /// result
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// </param>
        /// <param name="imageSizeOnPage">
        /// size of the image according to the selected
        /// <see cref="ScaleMode"/>
        /// </param>
        /// <param name="pageText">text that was found on this image (or on this page)</param>
        /// <param name="imageData">
        /// input image if it is a single page or its one page if
        /// this is a multi-page image
        /// </param>
        /// <param name="createPdfA3u">true if PDF/A3u document is being created</param>
        /// <param name="layers">an array with 2 elements representing PDF layers for image and text</param>
        private void AddToCanvas(PdfDocument pdfDocument, Rectangle imageSizeOnPage, IList<TextInfo> pageText, ImageData
             imageData, bool createPdfA3u, PdfLayer[] layers) {
            Rectangle rectangleSize = ocrPdfCreatorProperties.GetPageSize() == null ? imageSizeOnPage : ocrPdfCreatorProperties
                .GetPageSize();
            PageSize size = new PageSize(rectangleSize);
            PdfPage pdfPage = pdfDocument.AddNewPage(size);
            PdfCanvas canvas = new OcrPdfCreator.NotDefCheckingPdfCanvas(pdfPage, createPdfA3u);
            if (layers[0] != null) {
                canvas.BeginLayer(layers[0]);
            }
            AddImageToCanvas(imageData, imageSizeOnPage, canvas);
            if (layers[0] != null && layers[0] != layers[1]) {
                canvas.EndLayer();
            }
            if (layers[1] != null && layers[0] != layers[1]) {
                canvas.BeginLayer(layers[1]);
            }
            CollectTextAndAddToCanvas(pdfPage, canvas, pageText, imageSizeOnPage, new Rectangle(imageData.GetWidth(), 
                imageData.GetHeight()));
            if (layers[1] != null) {
                canvas.EndLayer();
            }
        }

        private void CollectTextAndAddToCanvas(PdfPage pdfPage, PdfCanvas canvas, IList<TextInfo> pageText, Rectangle
             imageBbox, Rectangle imageSize) {
            PdfDocument pdfDocument = pdfPage.GetDocument();
            try {
                // A map of TextInfo to a tag pointer, always empty if tagging is not supported
                IDictionary<TextInfo, TagTreePointer> flatLogicalTree = new Dictionary<TextInfo, TagTreePointer>();
                if (ocrPdfCreatorProperties.IsTagged()) {
                    // Logical tree, a list of top items, children can be retrieved out of them
                    IList<LogicalStructureTreeItem> logicalTree = new List<LogicalStructureTreeItem>();
                    // A map of leaf LogicalStructureTreeItem's to TextInfo's attached to these leaves
                    IDictionary<LogicalStructureTreeItem, IList<TextInfo>> leavesTextInfos = GetLogicalTree(pageText, logicalTree
                        );
                    pdfDocument.SetTagged();
                    // Create a map of TextInfo to tag pointers meanwhile creating the required tags.
                    // Tag pointers are later used to put all the required info into canvas (content stream)
                    BuildLogicalTreeAndFlatten(logicalTree, leavesTextInfos, new TagTreePointer(pdfDocument).SetPageForTagging
                        (pdfPage), flatLogicalTree);
                }
                // How much the original image size changed
                float widthMultiplier = imageBbox.GetWidth() / PdfCreatorUtil.GetPoints(imageSize.GetWidth());
                float heightMultiplier = imageBbox.GetHeight() / PdfCreatorUtil.GetPoints(imageSize.GetHeight());
                AddTextToCanvas(imageBbox, pageText, flatLogicalTree, canvas, widthMultiplier, heightMultiplier, pdfPage);
            }
            catch (PdfOcrException e) {
                LOGGER.LogError(MessageFormatUtil.Format(PdfOcrExceptionMessageConstant.CANNOT_CREATE_PDF_DOCUMENT, e.Message
                    ));
                throw new PdfOcrException(PdfOcrExceptionMessageConstant.CANNOT_CREATE_PDF_DOCUMENT).SetMessageParams(e.Message
                    );
            }
        }

        /// <param name="imagesTextData">
        /// a map where the key is
        /// <see cref="PageImageData"/>
        /// and the value is an OCR result
        /// </param>
        private void AddToPdfPage(PdfPage pdfPage, IDictionary<ImageExtraction.PageImageData, IDictionary<int, IList
            <TextInfo>>> imagesTextData, PdfLayer pdfLayer) {
            foreach (KeyValuePair<ImageExtraction.PageImageData, IDictionary<int, IList<TextInfo>>> entry in imagesTextData
                ) {
                // Key in OCR result is always 1 here
                IList<TextInfo> textInfos = entry.Value.Get(1);
                PdfCanvas canvas = new PdfCanvas(pdfPage);
                Rectangle imageSize = new Rectangle(entry.Key.GetXObject().GetWidth(), entry.Key.GetXObject().GetHeight());
                if (pdfLayer != null) {
                    canvas.BeginLayer(pdfLayer);
                }
                CollectTextAndAddToCanvas(pdfPage, canvas, textInfos, entry.Key.GetPagePosition(), imageSize);
                if (pdfLayer != null) {
                    canvas.EndLayer();
                }
            }
        }

        private PdfDocument CreatePdfDocument(PdfWriter pdfWriter, PdfOutputIntent pdfOutputIntent, IDictionary<FileInfo
            , IDictionary<int, IList<TextInfo>>> imagesTextData, SequenceId pdfSequenceId, DocumentProperties documentProperties
            ) {
            PdfDocument pdfDocument;
            bool createPdfA3u = pdfOutputIntent != null;
            if (createPdfA3u) {
                pdfDocument = new PdfADocument(pdfWriter, PdfAConformance.PDF_A_3U, pdfOutputIntent, documentProperties);
            }
            else {
                pdfDocument = new PdfDocument(pdfWriter, documentProperties);
            }
            LinkDocumentIdEvent linkDocumentIdEvent = new LinkDocumentIdEvent(pdfDocument, pdfSequenceId);
            EventManager.GetInstance().OnEvent(linkDocumentIdEvent);
            // pdfLang should be set in PDF/A mode
            bool hasPdfLangProperty = ocrPdfCreatorProperties.GetPdfLang() != null && !String.IsNullOrEmpty(ocrPdfCreatorProperties
                .GetPdfLang());
            if (createPdfA3u && !hasPdfLangProperty) {
                LOGGER.LogError(MessageFormatUtil.Format(PdfOcrExceptionMessageConstant.CANNOT_CREATE_PDF_DOCUMENT, PdfOcrLogMessageConstant
                    .PDF_LANGUAGE_PROPERTY_IS_NOT_SET));
                throw new PdfOcrException(PdfOcrExceptionMessageConstant.CANNOT_CREATE_PDF_DOCUMENT).SetMessageParams(PdfOcrLogMessageConstant
                    .PDF_LANGUAGE_PROPERTY_IS_NOT_SET);
            }
            // add metadata
            if (hasPdfLangProperty) {
                pdfDocument.GetCatalog().SetLang(new PdfString(ocrPdfCreatorProperties.GetPdfLang()));
            }
            // set title if it is not empty
            if (ocrPdfCreatorProperties.GetTitle() != null) {
                pdfDocument.GetCatalog().SetViewerPreferences(new PdfViewerPreferences().SetDisplayDocTitle(true));
                PdfDocumentInfo info = pdfDocument.GetDocumentInfo();
                info.SetTitle(ocrPdfCreatorProperties.GetTitle());
            }
            // reset passed font provider
            ocrPdfCreatorProperties.GetFontProvider().Reset();
            AddDataToPdfDocument(imagesTextData, pdfDocument, createPdfA3u);
            // statistics event about type of created pdf
            if (ocrEngine is IProductAware && ((IProductAware)ocrEngine).GetProductData() != null) {
                PdfOcrOutputType eventType = createPdfA3u ? PdfOcrOutputType.PDFA : PdfOcrOutputType.PDF;
                PdfOcrOutputTypeStatisticsEvent docTypeStatisticsEvent = new PdfOcrOutputTypeStatisticsEvent(eventType, ((
                    IProductAware)ocrEngine).GetProductData());
                EventManager.GetInstance().OnEvent(docTypeStatisticsEvent);
            }
            return pdfDocument;
        }

        /// <summary>Places provided images and recognized text to the result PDF document.</summary>
        /// <param name="imagesTextData">
        /// map that contains input image
        /// files as keys, and as value:
        /// map pageNumber -&gt; text for the page
        /// </param>
        /// <param name="pdfDocument">
        /// result
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// </param>
        /// <param name="createPdfA3u">true if PDF/A3u document is being created</param>
        private void AddDataToPdfDocument(IDictionary<FileInfo, IDictionary<int, IList<TextInfo>>> imagesTextData, 
            PdfDocument pdfDocument, bool createPdfA3u) {
            foreach (KeyValuePair<FileInfo, IDictionary<int, IList<TextInfo>>> entry in imagesTextData) {
                FileInfo inputImage = entry.Key;
                IList<ImageData> imageDataList = PdfCreatorUtil.GetImageData(inputImage, ocrPdfCreatorProperties.GetImageRotationHandler
                    ());
                LOGGER.LogInformation(MessageFormatUtil.Format(PdfOcrLogMessageConstant.NUMBER_OF_PAGES_IN_IMAGE, inputImage
                    .ToString(), imageDataList.Count));
                PdfLayer[] layers = CreatePdfLayers(ocrPdfCreatorProperties.GetImageLayerName(), ocrPdfCreatorProperties.GetTextLayerName
                    (), pdfDocument);
                IDictionary<int, IList<TextInfo>> imageTextData = entry.Value;
                if (imageTextData.Keys.Count > 0) {
                    for (int page = 0; page < imageDataList.Count; ++page) {
                        ImageData imageData = imageDataList[page];
                        Rectangle imageSizeOnPage = PdfCreatorUtil.CalculateImageSize(imageData, ocrPdfCreatorProperties.GetScaleMode
                            (), ocrPdfCreatorProperties.GetPageSize());
                        if (imageTextData.ContainsKey(page + 1)) {
                            AddToCanvas(pdfDocument, imageSizeOnPage, imageTextData.Get(page + 1), imageData, createPdfA3u, layers);
                        }
                    }
                }
            }
        }

        /// <summary>Places given image to canvas to background to a separate layer.</summary>
        /// <param name="imageData">
        /// input image as
        /// <see cref="System.IO.FileInfo"/>
        /// </param>
        /// <param name="imageSize">
        /// size of the image according to the selected
        /// <see cref="ScaleMode"/>
        /// </param>
        /// <param name="pdfCanvas">canvas to place the image</param>
        private void AddImageToCanvas(ImageData imageData, Rectangle imageSize, PdfCanvas pdfCanvas) {
            if (imageData != null) {
                if (ocrPdfCreatorProperties.IsTagged()) {
                    pdfCanvas.OpenTag(new CanvasArtifact());
                }
                if (ocrPdfCreatorProperties.GetPageSize() == null) {
                    pdfCanvas.AddImageFittedIntoRectangle(imageData, imageSize, false);
                }
                else {
                    Point coordinates = PdfCreatorUtil.CalculateImageCoordinates(ocrPdfCreatorProperties.GetPageSize(), imageSize
                        );
                    Rectangle rect = new Rectangle((float)coordinates.GetX(), (float)coordinates.GetY(), imageSize.GetWidth(), 
                        imageSize.GetHeight());
                    pdfCanvas.AddImageFittedIntoRectangle(imageData, rect, false);
                }
                if (ocrPdfCreatorProperties.IsTagged()) {
                    pdfCanvas.CloseTag();
                }
            }
        }

        private static void BuildLogicalTreeAndFlatten(IList<LogicalStructureTreeItem> logicalStructureTreeItems, 
            IDictionary<LogicalStructureTreeItem, IList<TextInfo>> leavesTextInfos, TagTreePointer tagPointer, IDictionary
            <TextInfo, TagTreePointer> flatLogicalTree) {
            foreach (LogicalStructureTreeItem structTreeItem in logicalStructureTreeItems) {
                AccessibilityProperties accessibilityProperties = structTreeItem.GetAccessibilityProperties();
                if (accessibilityProperties == null) {
                    accessibilityProperties = new DefaultAccessibilityProperties(PdfName.Span.GetValue());
                }
                tagPointer.AddTag(accessibilityProperties);
                IList<TextInfo> textItems = leavesTextInfos.Get(structTreeItem);
                if (textItems != null) {
                    foreach (TextInfo item in textItems) {
                        flatLogicalTree.Put(item, new TagTreePointer(tagPointer));
                    }
                }
                BuildLogicalTreeAndFlatten(structTreeItem.GetChildren(), leavesTextInfos, tagPointer, flatLogicalTree);
                tagPointer.MoveToParent();
            }
        }

        /// <summary>Places retrieved text to canvas to a separate layer.</summary>
        /// <param name="imageBbox">
        /// size of the image according to the selected
        /// <see cref="ScaleMode"/>
        /// </param>
        /// <param name="pageText">text that was found on this image (or on this page)</param>
        /// <param name="flatLogicalTree">a map of TextInfo to a tag pointer</param>
        /// <param name="pdfCanvas">canvas to place the text</param>
        /// <param name="widthMultiplier">coefficient to adjust text width on canvas</param>
        /// <param name="heightMultiplier">coefficient to adjust text height on canvas</param>
        /// <param name="page">current page</param>
        private void AddTextToCanvas(Rectangle imageBbox, IList<TextInfo> pageText, IDictionary<TextInfo, TagTreePointer
            > flatLogicalTree, PdfCanvas pdfCanvas, float widthMultiplier, float heightMultiplier, PdfPage page) {
            if (pageText == null || pageText.IsEmpty()) {
                return;
            }
            Rectangle pageMediaBox = page.GetMediaBox();
            Point imageCoordinates = PdfCreatorUtil.CalculateImageCoordinates(ocrPdfCreatorProperties.GetPageSize(), imageBbox
                );
            foreach (TextInfo item in pageText) {
                float textWidthPt = GetTextWidthPt(item, widthMultiplier);
                float textHeightPt = GetTextHeightPt(item, heightMultiplier);
                FontProvider fontProvider = GetOcrPdfCreatorProperties().GetFontProvider();
                String fontFamily = GetOcrPdfCreatorProperties().GetDefaultFontFamily();
                String line = item.GetText();
                if (!LineNotEmpty(line, textHeightPt, textWidthPt)) {
                    continue;
                }
                Document document = new Document(pdfCanvas.GetDocument());
                document.SetFontProvider(fontProvider);
                // Scale the text width to fit the OCR bbox
                float fontSize = PdfCreatorUtil.CalculateFontSize(document, line, fontFamily, textHeightPt, textWidthPt);
                float lineWidth = PdfCreatorUtil.GetRealLineWidth(document, line, fontFamily, fontSize);
                float xOffset = GetXOffsetPt(item, widthMultiplier);
                float yOffset = GetYOffsetPt(item, heightMultiplier);
                TagTreePointer tagPointer = flatLogicalTree.Get(item);
                if (tagPointer != null) {
                    pdfCanvas.OpenTag(tagPointer.GetTagReference());
                }
                else {
                    if (ocrPdfCreatorProperties.IsTagged()) {
                        pdfCanvas.OpenTag(new CanvasArtifact());
                    }
                }
                iText.Layout.Canvas canvas = new iText.Layout.Canvas(pdfCanvas, pageMediaBox);
                canvas.SetFontProvider(fontProvider);
                Text text = new Text(line).SetHorizontalScaling(textWidthPt / lineWidth);
                Paragraph paragraph = new Paragraph(text).SetMargin(0).SetFontFamily(fontFamily).SetFontSize(fontSize).SetWidth
                    (textWidthPt * 1.5f);
                if (ocrPdfCreatorProperties.GetTextColor() != null) {
                    paragraph.SetFontColor(ocrPdfCreatorProperties.GetTextColor());
                }
                else {
                    paragraph.SetTextRenderingMode(PdfCanvasConstants.TextRenderingMode.INVISIBLE);
                }
                canvas.ShowTextAligned(paragraph, xOffset + (float)imageCoordinates.GetX(), yOffset + (float)imageCoordinates
                    .GetY(), canvas.GetPdfDocument().GetPageNumber(page), TextAlignment.LEFT, VerticalAlignment.BOTTOM, GetRotationAngle
                    (item.GetOrientation()));
                if (ocrPdfCreatorProperties.IsTagged()) {
                    pdfCanvas.CloseTag();
                }
                canvas.Close();
            }
        }

        private static IDictionary<LogicalStructureTreeItem, IList<TextInfo>> GetLogicalTree(IList<TextInfo> textInfos
            , IList<LogicalStructureTreeItem> logicalStructureTreeItems) {
            IDictionary<LogicalStructureTreeItem, IList<TextInfo>> leavesTextInfos = new Dictionary<LogicalStructureTreeItem
                , IList<TextInfo>>();
            if (textInfos == null) {
                return leavesTextInfos;
            }
            foreach (TextInfo textInfo in textInfos) {
                LogicalStructureTreeItem structTreeItem = textInfo.GetLogicalStructureTreeItem();
                LogicalStructureTreeItem topParent;
                if (structTreeItem is ArtifactItem) {
                    continue;
                }
                else {
                    if (structTreeItem != null) {
                        topParent = GetTopParent(structTreeItem);
                    }
                    else {
                        structTreeItem = new LogicalStructureTreeItem();
                        textInfo.SetLogicalStructureTreeItem(structTreeItem);
                        topParent = structTreeItem;
                    }
                }
                IList<TextInfo> textInfosPerStructItem = leavesTextInfos.Get(structTreeItem);
                if (textInfosPerStructItem == null) {
                    textInfosPerStructItem = new List<TextInfo>();
                    textInfosPerStructItem.Add(textInfo);
                    leavesTextInfos.Put(structTreeItem, textInfosPerStructItem);
                }
                else {
                    textInfosPerStructItem.Add(textInfo);
                }
                if (!logicalStructureTreeItems.Contains(topParent)) {
                    logicalStructureTreeItems.Add(topParent);
                }
            }
            return leavesTextInfos;
        }

        private static LogicalStructureTreeItem GetTopParent(LogicalStructureTreeItem structInfo) {
            if (structInfo.GetParent() != null) {
                return GetTopParent(structInfo.GetParent());
            }
            else {
                return structInfo;
            }
        }

        /// <summary>
        /// Returns the text rotation angle in radian for the provided
        /// <see cref="TextOrientation"/>.
        /// </summary>
        /// <param name="orientation">text orientation to get the angle for</param>
        /// <returns>
        /// the text rotation angle in radian for the provided
        /// <see cref="TextOrientation"/>
        /// </returns>
        private static float GetRotationAngle(TextOrientation orientation) {
            switch (orientation) {
                case TextOrientation.HORIZONTAL_ROTATED_90: {
                    return (float)(0.5 * Math.PI);
                }

                case TextOrientation.HORIZONTAL_ROTATED_180: {
                    return (float)Math.PI;
                }

                case TextOrientation.HORIZONTAL_ROTATED_270: {
                    return (float)(1.5 * Math.PI);
                }

                case TextOrientation.HORIZONTAL:
                default: {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Creates layers for image and text according rules set in
        /// <see cref="OcrPdfCreatorProperties"/>.
        /// </summary>
        /// <param name="imageLayerName">name of the image layer</param>
        /// <param name="textLayerName">name of the text layer</param>
        /// <param name="pdfDocument">document to add layers to</param>
        /// <returns>
        /// array of two layers: first layer is for image, second layer is for text.
        /// Elements may be null meaning that layer creation is not requested
        /// </returns>
        private static PdfLayer[] CreatePdfLayers(String imageLayerName, String textLayerName, PdfDocument pdfDocument
            ) {
            if (imageLayerName == null && textLayerName == null) {
                return new PdfLayer[] { null, null };
            }
            else {
                if (imageLayerName == null) {
                    return new PdfLayer[] { null, new PdfLayer(textLayerName, pdfDocument) };
                }
                else {
                    if (textLayerName == null) {
                        return new PdfLayer[] { new PdfLayer(imageLayerName, pdfDocument), null };
                    }
                    else {
                        if (imageLayerName.Equals(textLayerName)) {
                            PdfLayer pdfLayer = new PdfLayer(imageLayerName, pdfDocument);
                            return new PdfLayer[] { pdfLayer, pdfLayer };
                        }
                        else {
                            return new PdfLayer[] { new PdfLayer(imageLayerName, pdfDocument), new PdfLayer(textLayerName, pdfDocument
                                ) };
                        }
                    }
                }
            }
        }

        /// <summary>Get left bound of text chunk.</summary>
        private static float GetLeft(TextInfo textInfo, float multiplier) {
            return textInfo.GetBboxRect().GetLeft() * multiplier;
        }

        /// <summary>Get right bound of text chunk.</summary>
        private static float GetRight(TextInfo textInfo, float multiplier) {
            return (textInfo.GetBboxRect().GetRight() + 1) * multiplier - 1;
        }

        /// <summary>Get top bound of text chunk.</summary>
        private static float GetTop(TextInfo textInfo, float multiplier) {
            return textInfo.GetBboxRect().GetTop() * multiplier;
        }

        /// <summary>Get bottom bound of text chunk.</summary>
        private static float GetBottom(TextInfo textInfo, float multiplier) {
            return (textInfo.GetBboxRect().GetBottom() + 1) * multiplier - 1;
        }

        /// <summary>Check if line is not empty.</summary>
        private static bool LineNotEmpty(String line, float bboxHeightPt, float bboxWidthPt) {
            return !String.IsNullOrEmpty(line) && bboxHeightPt > 0 && bboxWidthPt > 0;
        }

        /// <summary>Get width of text chunk in points.</summary>
        private static float GetTextWidthPt(TextInfo textInfo, float multiplier) {
            switch (textInfo.GetOrientation()) {
                case TextOrientation.HORIZONTAL_ROTATED_90:
                case TextOrientation.HORIZONTAL_ROTATED_270: {
                    return GetTop(textInfo, multiplier) - GetBottom(textInfo, multiplier);
                }

                case TextOrientation.HORIZONTAL:
                case TextOrientation.HORIZONTAL_ROTATED_180:
                default: {
                    return GetRight(textInfo, multiplier) - GetLeft(textInfo, multiplier);
                }
            }
        }

        /// <summary>Get height of text chunk in points.</summary>
        private static float GetTextHeightPt(TextInfo textInfo, float multiplier) {
            switch (textInfo.GetOrientation()) {
                case TextOrientation.HORIZONTAL_ROTATED_90:
                case TextOrientation.HORIZONTAL_ROTATED_270: {
                    return GetRight(textInfo, multiplier) - GetLeft(textInfo, multiplier);
                }

                case TextOrientation.HORIZONTAL:
                case TextOrientation.HORIZONTAL_ROTATED_180:
                default: {
                    return GetTop(textInfo, multiplier) - GetBottom(textInfo, multiplier);
                }
            }
        }

        /// <summary>Get horizontal text offset in points.</summary>
        private static float GetXOffsetPt(TextInfo textInfo, float multiplier) {
            switch (textInfo.GetOrientation()) {
                case TextOrientation.HORIZONTAL_ROTATED_90:
                case TextOrientation.HORIZONTAL_ROTATED_180: {
                    return GetRight(textInfo, multiplier);
                }

                case TextOrientation.HORIZONTAL:
                case TextOrientation.HORIZONTAL_ROTATED_270:
                default: {
                    return GetLeft(textInfo, multiplier);
                }
            }
        }

        /// <summary>Get vertical text offset in points.</summary>
        private static float GetYOffsetPt(TextInfo textInfo, float multiplier) {
            switch (textInfo.GetOrientation()) {
                case TextOrientation.HORIZONTAL_ROTATED_180:
                case TextOrientation.HORIZONTAL_ROTATED_270: {
                    return GetTop(textInfo, multiplier);
                }

                case TextOrientation.HORIZONTAL:
                case TextOrientation.HORIZONTAL_ROTATED_90:
                default: {
                    return GetBottom(textInfo, multiplier);
                }
            }
        }

        /// <summary>A handler for PDF canvas that validates existing glyphs.</summary>
        private class NotDefCheckingPdfCanvas : PdfCanvas {
            private readonly bool createPdfA3u;

            public NotDefCheckingPdfCanvas(PdfPage page, bool createPdfA3u)
                : base(page) {
                this.createPdfA3u = createPdfA3u;
            }

            public override PdfCanvas ShowText(GlyphLine text) {
                OcrPdfCreator.ActualTextCheckingGlyphLine glyphLine = new OcrPdfCreator.ActualTextCheckingGlyphLine(text);
                PdfFont currentFont = GetGraphicsState().GetFont();
                bool notDefGlyphsExists = false;
                // default value for error message, it'll be updated with the
                // unicode of the not found glyph
                String message = PdfOcrLogMessageConstant.COULD_NOT_FIND_CORRESPONDING_GLYPH_TO_UNICODE_CHARACTER;
                for (int i = glyphLine.GetStart(); i < glyphLine.GetEnd(); i++) {
                    if (IsNotDefGlyph(currentFont, glyphLine.Get(i))) {
                        notDefGlyphsExists = true;
                        message = MessageFormatUtil.Format(PdfOcrLogMessageConstant.COULD_NOT_FIND_CORRESPONDING_GLYPH_TO_UNICODE_CHARACTER
                            , glyphLine.Get(i).GetUnicode());
                        if (this.createPdfA3u) {
                            // exception is thrown only if PDF/A document is
                            // being created
                            throw new PdfOcrException(message);
                        }
                        // setting actual text to NotDef glyph
                        glyphLine.SetActualTextToGlyph(i, glyphLine.ToUnicodeString(i, i + 1));
                        // setting a fake unicode deliberately to pass further
                        // checks for actual text necessity during iterating over
                        // glyphline chunks with ActualTextIterator
                        Glyph glyph = new Glyph(glyphLine.Get(i));
                        glyph.SetUnicode(-1);
                        glyphLine.Set(i, glyph);
                    }
                }
                // Warning is logged if not PDF/A document is being created
                if (notDefGlyphsExists) {
                    LOGGER.LogWarning(message);
                }
                return this.ShowText(glyphLine, new ActualTextIterator(glyphLine));
            }

            private static bool IsNotDefGlyph(PdfFont font, Glyph glyph) {
                if (font is PdfType0Font || font is PdfTrueTypeFont) {
                    return glyph.GetCode() == 0;
                }
                else {
                    if (font is PdfType1Font || font is PdfType3Font) {
                        return glyph.GetCode() == -1;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// A handler for GlyphLine that checks existing actual text not to
        /// overwrite it.
        /// </summary>
        private class ActualTextCheckingGlyphLine : GlyphLine {
            public ActualTextCheckingGlyphLine(GlyphLine other)
                : base(other) {
            }

            public virtual void SetActualTextToGlyph(int i, String text) {
                // set actual text if it doesn't exist for i-th glyph
                if ((this.actualText == null || this.actualText.Count <= i || this.actualText[i] == null)) {
                    base.SetActualText(i, i + 1, text);
                }
            }
        }
    }
}

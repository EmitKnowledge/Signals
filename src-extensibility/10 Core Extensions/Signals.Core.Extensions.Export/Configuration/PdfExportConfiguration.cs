using System.Runtime.Serialization;
using SelectPdf;

namespace Signals.Core.Extensions.Export.Configuration
{
    /// <summary>
    /// Represents exporting configuration model for pdf exports
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class PdfExportConfiguration<TData> : ExportConfiguration<TData>
    {
        /// <summary>
        /// Default CTOR
        /// </summary>
        public PdfExportConfiguration()
        {
            PdfPageOrientation = PdfPageOrientation.Landscape;
            MarginTop = 10;
            MarginBottom = 10;
            MarginLeft = 0;
            MarginRight = 0;
            PdfPageSize = PdfPageSize.A4;
        }

        /// <summary>
        /// Pdf orientation option
        /// </summary>
        public PdfPageOrientation PdfPageOrientation { get; set; }

        /// <summary>
        /// Margin top pixels
        /// </summary>
        public int MarginTop { get; set; }

        /// <summary>
        /// Margin bottom pixels
        /// </summary>
        public int MarginBottom { get; set; }

        /// <summary>
        /// Margin left pixels
        /// </summary>
        public int MarginLeft { get; set; }

        /// <summary>
        /// Margin right pixels
        /// </summary>
        public int MarginRight { get; set; }

        /// <summary>
        /// Pdf page size option
        /// </summary>
        public PdfPageSize PdfPageSize { get; set; }

        /// <summary>
        /// Options
        /// </summary>
        public HtmlToPdfOptions HtmlToPdfOptions { get; set; }
    }

    /// <summary>
    /// Page options
    /// </summary>
    public enum PdfPageOrientation
    {
        /// <summary>
        /// Portrait orientation
        /// </summary>
        [EnumMember]
        Portrait = 0,

        /// <summary>
        /// Landscape orientation
        /// </summary>
        [EnumMember]
        Landscape = 1
    }

    /// <summary>
    /// Page size
    /// </summary>
    public enum PdfPageSize
    {
        /// <summary>
        /// Custom page size
        /// </summary>
        [EnumMember]
        Custom = 0,

        /// <summary>
        /// Letter page size
        /// </summary>
        [EnumMember]
        Letter = 1,

        /// <summary>
        /// Note page size
        /// </summary>
        [EnumMember]
        Note = 2,

        /// <summary>
        /// Legal page size
        /// </summary>
        [EnumMember]
        Legal = 3,

        /// <summary>
        /// A0 page size
        /// </summary>
        [EnumMember]
        A0 = 4,

        /// <summary>
        /// A1 page size
        /// </summary>
        [EnumMember]
        A1 = 5,

        /// <summary>
        /// A2 page size
        /// </summary>
        [EnumMember]
        A2 = 6,

        /// <summary>
        /// A3 page size
        /// </summary>
        [EnumMember]
        A3 = 7,

        /// <summary>
        /// A4 page size
        /// </summary>
        [EnumMember]
        A4 = 8,

        /// <summary>
        /// A5 page size
        /// </summary>
        [EnumMember]
        A5 = 9,

        /// <summary>
        /// A6 page size
        /// </summary>
        [EnumMember]
        A6 = 10,

        /// <summary>
        /// A7 page size
        /// </summary>
        [EnumMember]
        A7 = 11,

        /// <summary>
        /// A8 page size
        /// </summary>
        [EnumMember]
        A8 = 12,

        /// <summary>
        /// A9 page size
        /// </summary>
        [EnumMember]
        A9 = 13,

        /// <summary>
        /// A10 page size
        /// </summary>
        [EnumMember]
        A10 = 14,

        /// <summary>
        /// B0 page size
        /// </summary>
        [EnumMember]
        B0 = 15,

        /// <summary>
        /// B1 page size
        /// </summary>
        [EnumMember]
        B1 = 16,

        /// <summary>
        /// B2 page size
        /// </summary>
        [EnumMember]
        B2 = 17,

        /// <summary>
        /// B3 page size
        /// </summary>
        [EnumMember]
        B3 = 18,

        /// <summary>
        /// B4 page size
        /// </summary>
        [EnumMember]
        B4 = 19,

        /// <summary>
        /// B5 page size
        /// </summary>
        [EnumMember]
        B5 = 20,

        /// <summary>
        /// ArchE page size
        /// </summary>
        [EnumMember]
        ArchE = 21,

        /// <summary>
        /// ArchD page size
        /// </summary>
        [EnumMember]
        ArchD = 22,

        /// <summary>
        /// ArchC page size
        /// </summary>
        [EnumMember]
        ArchC = 23,

        /// <summary>
        /// ArchB page size
        /// </summary>
        [EnumMember]
        ArchB = 24,

        /// <summary>
        /// ArchA page size
        /// </summary>
        [EnumMember]
        ArchA = 25,

        /// <summary>
        /// Flsa page size
        /// </summary>
        [EnumMember]
        Flsa = 26,

        /// <summary>
        /// HalfLetter page size
        /// </summary>
        [EnumMember]
        HalfLetter = 27,

        /// <summary>
        /// Letter11x17 page size
        /// </summary>
        [EnumMember]
        Letter11x17 = 28,

        /// <summary>
        /// Ledger page size
        /// </summary>
        [EnumMember]
        Ledger = 29
    }
}

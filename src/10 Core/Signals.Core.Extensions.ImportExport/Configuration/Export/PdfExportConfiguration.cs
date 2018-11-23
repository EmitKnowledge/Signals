using System.Runtime.Serialization;
using SelectPdf;

namespace Signals.Core.Extensions.ImportExport.Configuration.Export
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

        public PdfPageOrientation PdfPageOrientation { get; set; }

        public int MarginTop { get; set; }

        public int MarginBottom { get; set; }

        public int MarginLeft { get; set; }

        public int MarginRight { get; set; }

        public PdfPageSize PdfPageSize { get; set; }

        public HtmlToPdfOptions HtmlToPdfOptions { get; set; }
    }

    public enum PdfPageOrientation
    {
        [EnumMember]
        Portrait = 0,

        [EnumMember]
        Landscape = 1
    }

    public enum PdfPageSize
    {
        [EnumMember]
        Custom = 0,

        [EnumMember]
        Letter = 1,

        [EnumMember]
        Note = 2,

        [EnumMember]
        Legal = 3,

        [EnumMember]
        A0 = 4,

        [EnumMember]
        A1 = 5,

        [EnumMember]
        A2 = 6,

        [EnumMember]
        A3 = 7,

        [EnumMember]
        A4 = 8,

        [EnumMember]
        A5 = 9,

        [EnumMember]
        A6 = 10,

        [EnumMember]
        A7 = 11,

        [EnumMember]
        A8 = 12,

        [EnumMember]
        A9 = 13,

        [EnumMember]
        A10 = 14,

        [EnumMember]
        B0 = 15,

        [EnumMember]
        B1 = 16,

        [EnumMember]
        B2 = 17,

        [EnumMember]
        B3 = 18,

        [EnumMember]
        B4 = 19,

        [EnumMember]
        B5 = 20,

        [EnumMember]
        ArchE = 21,

        [EnumMember]
        ArchD = 22,

        [EnumMember]
        ArchC = 23,

        [EnumMember]
        ArchB = 24,

        [EnumMember]
        ArchA = 25,

        [EnumMember]
        Flsa = 26,

        [EnumMember]
        HalfLetter = 27,

        [EnumMember]
        Letter11x17 = 28,

        [EnumMember]
        Ledger = 29
    }
}

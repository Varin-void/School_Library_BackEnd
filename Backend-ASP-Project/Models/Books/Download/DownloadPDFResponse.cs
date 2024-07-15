namespace Backend_ASP_Project.Models.Books.Download
{
	public class DownloadPDFResponse : Response
	{
        public PdfFile Pdf { get; set; }
    }

	public class PdfFile
	{
		public byte[] Data { get; set; }
		public string Filename { get; set; } = string.Empty;
		public string ContentType { get; set; } = string.Empty;
	}
}

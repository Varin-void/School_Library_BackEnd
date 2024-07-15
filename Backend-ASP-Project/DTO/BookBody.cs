namespace Backend_ASP_Project.DTO
{
	public class BookBody
	{
		public int Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Author { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string ImagePath { get; set; } = string.Empty;
		public string Pdf { get; set; } = string.Empty;
		public int[] Groups { get; set; } = new int[0];
	}
}

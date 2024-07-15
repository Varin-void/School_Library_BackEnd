using Backend_ASP_Project.DTO;

namespace Backend_ASP_Project.Models.Books.Update
{
	public class UpdateBookParams
	{
		public string apiToken { get; set; } = string.Empty;
		public int Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Author { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public IFormFile? Image { get; set; }
		public IFormFile? Pdf { get; set; }
	}
}

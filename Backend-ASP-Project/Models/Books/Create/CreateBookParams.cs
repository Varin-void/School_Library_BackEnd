using Backend_ASP_Project.DTO;

namespace Backend_ASP_Project.Models.Books.Create
{
	public class CreateBookParams
	{
		public string? GroupId { get; set; }
		public string apiToken { get; set; } = string.Empty;
		public string Title { get; set; } = string.Empty;
		public string Author { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public IFormFile? Image { get; set; } 
		public IFormFile? Pdf { get; set; }
	}
}

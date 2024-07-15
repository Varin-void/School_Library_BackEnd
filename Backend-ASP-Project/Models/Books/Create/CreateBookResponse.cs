using Backend_ASP_Project.DTO;

namespace Backend_ASP_Project.Models.Books.Create
{
	public class CreateBookResponse : Response
	{
		public BookBody? Data { get; set; } = new BookBody();
	}
}

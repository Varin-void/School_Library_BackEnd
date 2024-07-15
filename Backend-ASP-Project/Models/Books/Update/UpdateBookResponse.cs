using Backend_ASP_Project.DTO;

namespace Backend_ASP_Project.Models.Books.Update
{
	public class UpdateBookResponse : Response
	{
        public BookBody? data { get; set; } = new BookBody();
    }
}

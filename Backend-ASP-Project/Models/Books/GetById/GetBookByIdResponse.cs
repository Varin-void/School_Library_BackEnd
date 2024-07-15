using Backend_ASP_Project.DTO;

namespace Backend_ASP_Project.Models.Books.GetById
{
	public class GetBookByIdResponse : Response
	{
        public BookBody? data { get; set; } 
    }
}
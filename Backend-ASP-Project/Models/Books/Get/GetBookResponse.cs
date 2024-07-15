using Backend_ASP_Project.DTO;

namespace Backend_ASP_Project.Models.Books.Get
{
    public class GetBookResponse : Response
    {
        public List<BookBody>? data { get; set; } = new List<BookBody>();
    }
}

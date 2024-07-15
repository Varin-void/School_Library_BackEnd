namespace Backend_ASP_Project.Models.Books.Get
{
    public class GetBookParams
    {
        public string apiToken { get; set; } = string.Empty;
        public string Search { get; set; } = string.Empty;
        public string Sort { get; set; } = string.Empty;
    }
}

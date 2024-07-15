namespace Backend_ASP_Project.Models.Groups.AddBook
{
	public class AddBookToGroupParams
	{
		public string apiToken {  get; set; } = string.Empty;
		public string BookId { get; set; } = string.Empty;
		public string GroupId { get; set; } = string.Empty;
	}
}

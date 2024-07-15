namespace Backend_ASP_Project.Models.Groups.AddUser
{
	public class AddUserToGroupParams
	{
		public string apiToken { get; set; } = string.Empty;
		public string UserId { get; set; } = string.Empty;
		public string GroupId { get; set; } = string.Empty;
	}
}

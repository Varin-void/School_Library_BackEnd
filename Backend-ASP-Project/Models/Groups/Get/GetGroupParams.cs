namespace Backend_ASP_Project.Models.Groups.Get
{
	public class GetGroupParams
	{
		public string apiToken { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public string Sort { get; set; } = string.Empty;
    }
}

namespace Backend_ASP_Project.DTO
{
	public class UserBodyForGroup
	{
		public int Id { get; set; }
		public string Username { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
		public string Image { get; set; } = string.Empty;
		public int IsDelete { get; set; }
		public virtual RoleBody? Role { get; set; } = new RoleBody();
	}
}

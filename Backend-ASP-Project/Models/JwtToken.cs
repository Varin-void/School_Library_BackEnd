namespace Backend_ASP_Project.Models
{
	public class JwtToken
	{
		public int Id { get; set; }
		public string Token { get; set; } = string.Empty;
		public DateTime Expired { get; set; } = DateTime.Now;
		public int UserId { get; set; }
    }
}
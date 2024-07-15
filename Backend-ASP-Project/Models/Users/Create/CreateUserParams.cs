using NuGet.Protocol;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_ASP_Project.Models.Users.Create
{
    public class CreateUserParams 
    {
        public string apiToken { get; set; } = string.Empty;
        public IFormFile? Image { get; set; }
        public string RoleId { get; set; } = string.Empty;
		public string Username { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
		public string ConfirmPassword { get; set; } = string.Empty;
	}
}
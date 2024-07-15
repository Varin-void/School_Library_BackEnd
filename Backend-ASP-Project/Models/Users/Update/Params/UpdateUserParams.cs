using Backend_ASP_Project.DTO;
using Backend_ASP_Project.Models.Users.Get.By_Id;

namespace Backend_ASP_Project.Models.Users.Update.Params
{
	public class UpdateUserParams : Register
	{
		public string apiToken { get; set; } = string.Empty;
		public string Id { get; set; } = string.Empty;
		public IFormFile? Image { get; set; } 
		public string RoleId { get; set; } = string.Empty;
	}

}

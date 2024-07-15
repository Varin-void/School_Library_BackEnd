using Backend_ASP_Project.DTO;

namespace Backend_ASP_Project.Models.Users.Get.By_Id
{
	public class GetByIdUserResponse : Response
	{
		public UserBody? User { get; set; } = new UserBody();
    }
}

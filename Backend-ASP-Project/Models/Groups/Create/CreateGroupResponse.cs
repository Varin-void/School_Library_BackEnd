using Backend_ASP_Project.DTO;

namespace Backend_ASP_Project.Models.Groups.Create
{
	public class CreateGroupResponse : Response
	{
		public GroupBody? data { get; set; } = new GroupBody();
    }
	//public class CreateGroupResponseGroupBody
	//{
 //       public int Id { get; set; }
 //       public string Name { get; set; } = string.Empty;
 //   }
}

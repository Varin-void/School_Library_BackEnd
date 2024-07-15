using Backend_ASP_Project.DTO;

namespace Backend_ASP_Project.Models.Groups.GetById
{
	public class GetGroupByIdResponse : Response
	{
        public GroupBody? data { get; set; } = new GroupBody();
    }
}

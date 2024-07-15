using Backend_ASP_Project.DTO;

namespace Backend_ASP_Project.Models.Groups.Get
{
	public class GetGroupResponse : Response
	{
        public List<GroupBody>? Data { get; set; } = new List<GroupBody>();
    }
}

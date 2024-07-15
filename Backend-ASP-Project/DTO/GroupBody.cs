using Backend_ASP_Project.Models;

namespace Backend_ASP_Project.DTO
{
	public class GroupBody
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public List<BookBody> Books { get; set; } = new List<BookBody>();
		public List<UserBodyForGroup> Students { get; set; } = new List<UserBodyForGroup>();
		public List<UserBodyForGroup> Teachers { get; set; } = new List<UserBodyForGroup>();

	}
}

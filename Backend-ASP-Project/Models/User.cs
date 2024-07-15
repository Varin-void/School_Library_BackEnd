using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace Backend_ASP_Project.Models
{
	public class User
	{
		public User()
		{
			Group = new HashSet<Group>();
		}

		public int Id { get; set; }
		[Required]
		public string Username { get; set; } = string.Empty;
		[Required]
		public string Password { get; set; } = string.Empty;
		public int? TokenId { get; set; }
		[Required]
		public string ImagePath { get; set; } = string.Empty;
		public int IsDelete { get; set; }
		public virtual Role? Role { get; set; } = new Role();
		public virtual ICollection<Group> Group { get; set; }
	}
}
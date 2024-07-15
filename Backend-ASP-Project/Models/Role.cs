using System.Collections.ObjectModel;

namespace Backend_ASP_Project.Models
{
	public class Role
	{
        public Role() {
            Users = new HashSet<User>();
        }
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public virtual ICollection<User> Users { get; set; }
    }
}

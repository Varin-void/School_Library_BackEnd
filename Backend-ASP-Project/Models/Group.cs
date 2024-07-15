namespace Backend_ASP_Project.Models
{
	public class Group
	{
		public Group()
		{
			Books = new List<Book>();
			Users = new HashSet<User>();
			//Teachers = new HashSet<User>();
		}
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public int IsDelete { get; set; }
		public virtual ICollection<Book> Books { get; set; }
		public virtual ICollection<User> Users { get; set; }
		//public virtual ICollection<User>? Teachers { get; set; }
	}
}

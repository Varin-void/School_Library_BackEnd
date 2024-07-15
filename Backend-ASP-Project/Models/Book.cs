namespace Backend_ASP_Project.Models
{
	public class Book
	{
		public Book()
		{
			Groups = new HashSet<Group>();
		}
		public int Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Author { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string ImagePath { get; set; } = string.Empty;
		public int DownloadCount { get; set; }
		public int IsDelete { get; set; }
		public string Pdf { get; set; } = string.Empty;
		public virtual ICollection<Group> Groups { get; set; }
	}
}

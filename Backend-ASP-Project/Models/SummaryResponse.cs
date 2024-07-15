namespace Backend_ASP_Project.Models
{
	public class SummaryResponse : Response
	{
        public Summary? Data { get; set; }
    }

    public class Summary
    {
		public int Teachers { get; set; }
		public int Students { get; set; }
		public int Books { get; set; }
		public int Groups { get; set; }
		public int Downloadeds { get; set; }
	}
}

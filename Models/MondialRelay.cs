namespace ShoppingFantasy.Models
{
    public class MondialRelay
    {
        public string code_enseigne { get; set; }
        public string private_key { get; set; }
        public string code_marque { get; set; }
    }

	public class Point
	{
		public string Name { get; set; }
		public string Address { get; set; }
		public string ZipCode { get; set; }
		public string City { get; set; }
	}
}

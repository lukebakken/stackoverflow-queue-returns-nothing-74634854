namespace web_api.Models
{
    public class Devices
    {
        public Guid id { get; set; }
        public string description { get; set; }
        public string address { get; set; }
        public double max_consumption { get; set; }
    }
}

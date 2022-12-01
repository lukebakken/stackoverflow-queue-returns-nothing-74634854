namespace web_api.Models
{
    public class Timestamp
    {
        public Guid id { get; set; }
        public Guid deviceId { get; set; }
        public DateTime? timestamp { get; set; }
        public float energy_consumption { get; set; }
    }
}

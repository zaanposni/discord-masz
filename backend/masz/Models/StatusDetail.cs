namespace MASZ.Models
{
    public class StatusDetail
    {
        public bool Online { get; set; }
        public DateTime? LastDisconnect { get; set; }
        public double? ResponseTime { get; set; }
        public string Message { get; set; }
        public StatusDetail()
        {
            Online = true;
            LastDisconnect = null;
            ResponseTime = null;
            Message = null;
        }
    }
}
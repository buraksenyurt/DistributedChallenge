namespace GamersWorld.WebApp.Models
{
    public record ReportModel
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? DocumentId { get; set; }
        public DateTime InsertTime { get; set; }
    }
}
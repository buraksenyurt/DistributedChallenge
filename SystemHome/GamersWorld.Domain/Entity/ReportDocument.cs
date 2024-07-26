namespace GamersWorld.Domain.Entity;

public class ReportDocument
{
    public int ReportDocumentId { get; set; }

    public int ReportId { get; set; }
    public byte[]? Content { get; set; }
}
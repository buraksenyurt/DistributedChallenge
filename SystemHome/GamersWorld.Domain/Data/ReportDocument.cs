namespace GamersWorld.Domain.Data;

public class ReportDocument
{
    public int ReportDocumentId { get; set; }

    public int ReportId { get; set; }
    public byte[]? Content { get; set; }
}
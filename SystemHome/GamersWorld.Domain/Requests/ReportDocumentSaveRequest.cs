namespace GamersWorld.Domain.Requests;

public class ReportDocumentSaveRequest
{
    public int ReportId { get; set; }
    public byte[]? Content { get; set; }
}
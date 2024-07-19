namespace GamersWorld.Domain.Enums;
public enum StatusCode
{
    Success = 1,
    ReportReady = 200,
    DeleteRequestAccepted = 202,
    DocumentSaved = 205,
    DocumentReadable = 206,
    DocumentUploaded = 207,
    InvalidExpression = 400,
    ValidationErrors = 400,
    DocumentNotFound = 404,
    Fail = 500
}
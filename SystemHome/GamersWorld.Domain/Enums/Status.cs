namespace GamersWorld.Domain.Enums;
public enum Status
{
    Success = 1,
    ReportReady = 200,
    DeleteRequestAccepted = 202,
    DocumentSaved = 205,
    DocumentReadable = 206,
    DocumentUploaded = 207,
    InvalidExpression = 400,
    ValidationErrors = 403,
    DocumentNotFound = 404,
    Fail = 500
}
using GamersWorld.Common.Enums;

namespace GamersWorld.Common.Messages.Responses
{
    public class CreateReportResponse
    {
        public StatusCode Status { get; set; }
        public string DocumentId { get; set; }
        public string Explanation { get; set; }
    }
}

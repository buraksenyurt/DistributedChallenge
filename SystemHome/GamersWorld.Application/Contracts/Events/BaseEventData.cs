namespace GamersWorld.Application.Contracts.Events
{
    public class BaseEventData
    {
        public Guid TraceId { get; set; }
        public DateTime Time { get; set; }
        public BaseEventData()
        {
            TraceId = Guid.NewGuid();
            Time = DateTime.Now;
        }
    }
}

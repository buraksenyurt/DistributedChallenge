namespace GamersWorld.GateWayProxy;
public class RabbitMqSettings
{
    public string HostName { get; set; } = "localhost";
    public string Username { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public int Port { get; set; } = 5672;
}
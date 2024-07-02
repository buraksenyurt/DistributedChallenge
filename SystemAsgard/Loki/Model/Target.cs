namespace Loki.Model;

public class Target
{
    public string Name { get; set; }
    public string Action { get; set; }
    public HttpMethod HttpMethod { get; set; } = HttpMethod.Post;
    public Uri Uri { get; set; }
    public object Payload { get; set; }
}
namespace GamersWorld.Domain.Dtos;

public record DocumentContent
{
    public string Base64Content { get; set; }
    public int ContentSize { get; set; }
}


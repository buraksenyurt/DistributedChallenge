namespace GamersWorld.Domain.Dtos;

public record DocumentContentDto
{
    public string? Base64Content { get; set; }
    public int ContentSize { get; set; }
}


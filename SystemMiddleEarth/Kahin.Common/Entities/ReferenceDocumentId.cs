namespace Kahin.Common.Entities;

public struct ReferenceDocumentId
{
    public int Head { get; set; }
    public int Source { get; set; }
    public Guid Stamp { get; set; }
    public override readonly string ToString()
    {
        return $"{Head}-{Source}-{Stamp}";
    }
    public static ReferenceDocumentId Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentNullException(nameof(input), "Input string cannot be null or empty.");
        }

        var parts = input.Split('-');
        if (parts.Length < 3)
        {
            throw new FormatException("Input string must be in the format 'Head-Source-Stamp'.");
        }

        if (!int.TryParse(parts[0], out int head))
        {
            throw new FormatException("Head part must be a valid integer.");
        }

        if (!int.TryParse(parts[1], out int source))
        {
            throw new FormatException("Source part must be a valid integer.");
        }

        string guidPart = string.Join('-', parts, 2, parts.Length - 2);
        if (!Guid.TryParse(guidPart, out Guid stamp))
        {
            throw new FormatException("Stamp part must be a valid GUID.");
        }

        return new ReferenceDocumentId
        {
            Head = head,
            Source = source,
            Stamp = stamp
        };
    }
}
using Kahin.Common.Entities;

namespace Kahin.Comon.Tests;

public class DocumentIdTests
{
    [Fact]
    public void Should_Valid_DocumentId_Parse_Test()
    {
        string input = "1001-23-789e06c8-6b4e-4db9-9db2-9876543210ab";
        var referenceDocumentId = ReferenceDocumentId.Parse(input);
        var expectedHead = 1001;
        Assert.Equal(referenceDocumentId.Head, expectedHead);
        var excpectedSource = 23;
        Assert.Equal(referenceDocumentId.Source, excpectedSource);
        var expectedStamp = Guid.Parse("789e06c8-6b4e-4db9-9db2-9876543210ab");
        Assert.Equal(referenceDocumentId.Stamp, expectedStamp);
    }

    [Fact]
    public void Should_Empty_DocumentId_Throws_ArgumentNullException_Test()
    {
        string input = string.Empty;
        var expectedException = Assert.Throws<ArgumentNullException>(() => ReferenceDocumentId.Parse(input));
    }

    [Fact]
    public void Should_Invalid_Head_Throws_FormatException_Test()
    {
        string input = "binbir-23-789e06c8-6b4e-4db9-9db2-9876543210ab";
        var expectedException = Assert.Throws<FormatException>(() => ReferenceDocumentId.Parse(input));
    }

    [Fact]
    public void Should_Invalid_Source_Throws_FormatException_Test()
    {
        string input = "1001-yirmi üç-789e06c8-6b4e-4db9-9db2-9876543210ab";
        var expectedException = Assert.Throws<FormatException>(() => ReferenceDocumentId.Parse(input));
    }

    [Fact]
    public void Should_Invalid_Stamp_Throws_FormatException_Test()
    {
        string input = "1001-23-543210";
        var expectedException = Assert.Throws<FormatException>(() => ReferenceDocumentId.Parse(input));
    }
}
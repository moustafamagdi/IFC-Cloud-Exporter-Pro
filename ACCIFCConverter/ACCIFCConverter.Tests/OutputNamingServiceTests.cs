using ACCIFCConverter.Application.Services;

namespace ACCIFCConverter.Tests;

public sealed class OutputNamingServiceTests
{
    [Fact]
    public void Render_ReplacesTokens()
    {
        var sut = new OutputNamingService();
        var result = sut.Render("{Project}_{Model}_{Revision}_{Date}.ifc", "P1", "M1", "R3");
        Assert.StartsWith("P1_M1_R3_", result);
    }
}

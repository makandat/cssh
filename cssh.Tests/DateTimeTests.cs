/* DateTimeTests.cs */
using Xunit;
using static ScriptStd;

public class DateTimeTests
{
  [Fact]
  public void Today_ReturnsFormattedDate()
  {
    var s = today();
    Assert.Matches(@"^\d{4}-\d{2}-\d{2}$", s);
  }

  [Fact]
  public void Now_ReturnsIsoFormat()
  {
    var s = now();
    Assert.Matches(@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}$", s);
  }
}
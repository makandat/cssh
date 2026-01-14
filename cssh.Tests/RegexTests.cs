/* RegexTests.cs */
using Xunit;
using static ScriptStd;

public class RegexTests
{
  [Fact]
  public void Match_ReturnsTrue()
  {
    Assert.True(match("Hello", "^H"));
  }

  [Fact]
  public void Search_ReturnsFirstMatch()
  {
    Assert.Equal("123", search("abc123xyz", "[0-9]+"));
  }

  [Fact]
  public void Replace_ReplacesCorrectly()
  {
    Assert.Equal("abc###xyz", replace("abc123xyz", "[0-9]+", "###"));
  }
}
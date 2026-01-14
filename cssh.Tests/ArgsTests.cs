/* ArgsTests.cs */
using Xunit;
using static ScriptStd;

public class ArgsTests
{
  [Fact]
  public void Argc_ReturnsCorrectCount()
  {
    ScriptStd.SetArgs(new[] { "a", "b", "c" });
    Assert.Equal(3, argc());
  }

  [Fact]
  public void Args_ReturnsCorrectValue()
  {
    ScriptStd.SetArgs(new[] { "x", "y" });
    Assert.Equal("y", args(1));
  }

  [Fact]
  public void Args_OutOfRange_ReturnsNull()
  {
    ScriptStd.SetArgs(new[] { "x" });
    Assert.Null(args(5));
  }
}
/* FileTests.cs */
using Xunit;
using System.IO;
using static cssh.Std.ScriptStd;

public class FileTests
{
  [Fact]
  public void WriteRead_Works()
  {
    var path = "testfile.txt";
    write(path, "hello");
    Assert.Equal("hello", read(path));
    File.Delete(path);
  }

  [Fact]
  public void Append_Works()
  {
    var path = "testfile2.txt";
    write(path, "a");
    append(path, "b");
    Assert.Equal("ab", read(path));
    File.Delete(path);
  }
}
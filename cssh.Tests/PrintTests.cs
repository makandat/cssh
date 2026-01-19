/* cssh/Std  PrintTests.cs */
using Xunit;
using System.IO;
using static cssh.Std.ScriptStd;
using System;

public class PrintTests
{
  [Fact]
  public void Print_WritesWithoutNewline()
  {
    using var sw = new StringWriter();
    Console.SetOut(sw);

    print("hello");
    Assert.Equal("hello", sw.ToString());
  }

  [Fact]
  public void Println_WritesWithNewline()
  {
    using var sw = new StringWriter();
    Console.SetOut(sw);

    println("hello");
    Assert.Equal("hello" + Environment.NewLine, sw.ToString());
  }
}
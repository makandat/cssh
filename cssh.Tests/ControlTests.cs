/* ControlTests.cs */
using Xunit;
using System;
using static ScriptStd;

public class ControlTests
{
  [Fact]
  public void Abort_ExitsWithError()
  {
    var ex = Assert.ThrowsAny<Exception>(() => abort("error"));
    Assert.NotNull(ex);
  }
}
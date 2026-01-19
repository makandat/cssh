/* ControlTests.cs */
using Xunit;
using System;
using static cssh.Std.ScriptStd;

public class ControlTests
{
  [Fact(Skip = "abort() calls Environment.Exit(1) which terminates the test process. Cannot be tested in unit tests.")]
  public void Abort_ExitsWithError()
  {
    var ex = Assert.ThrowsAny<Exception>(() => abort("error"));
    Assert.NotNull(ex);
  }
}
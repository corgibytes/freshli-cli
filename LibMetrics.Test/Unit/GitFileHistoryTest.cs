using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace LibMetrics.Test.Unit
{
  public class GitFileHistoryTest
  {
    [Fact]
    public void Basics()
    {
      var assemblyPath = System.Reflection.Assembly.
        GetExecutingAssembly().Location;
      var rubyFixturePath = Path.Combine(
        Directory.GetParent(assemblyPath).ToString(),
        "fixtures",
        "ruby",
        "nokotest");

      var history = new GitFileHistory(rubyFixturePath, "Gemfile.lock");

      var expectedDates = new List<DateTime>()
      {
        new DateTime(2017, 01, 01),
        new DateTime(2018, 01, 01),
        new DateTime(2019, 01, 01)
      };

      Assert.Equal(expectedDates, history.Dates);
    }
  }
}

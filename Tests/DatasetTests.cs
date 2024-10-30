using NeuralNetworks.Client;
using System.IO;
using Xunit;

namespace NeuralNetworks.Tests
{
  public class DatasetTests
  {
    [Fact]
    public void Load_ValidFile_ReturnsRecords()
    {
      var records = new Dataset().Load("test.csv");

      Assert.NotNull(records);
      Assert.NotEmpty(records);
    }
  }
}

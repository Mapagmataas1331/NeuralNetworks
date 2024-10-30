using NeuralNetworks.NeuralNetwork;
using System.Collections.Generic;
using Xunit;

namespace NeuralNetworks.Tests
{
  public class NetworkTests
  {
    [Fact]
    public void Learn_ValidData_ProducesOutput()
    {
      var topology = new Topology(2, 1, 1, 2, 0.1);
      var network = new Network(topology);

      var dataset = new List<double[]>
            {
                new double[] { 0, 0, 0 },
                new double[] { 0, 1, 1 },
                new double[] { 1, 0, 1 },
                new double[] { 1, 1, 0 }
            };

      network.Learn(dataset);
      // Further assertions based on expected output
    }
  }
}

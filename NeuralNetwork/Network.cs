using System;
using System.Collections.Generic;
using System.Linq;
using NLog;

namespace NeuralNetworks.NeuralNetwork
{
  public class Network
  {
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
    private Topology topology;
    private List<Layer> layers;

    public Network(Topology topology)
    {
      this.topology = topology;
      layers = InitializeLayers();
    }

    public void Learn(List<double[]> dataset)
    {
      logger.Info("Starting training...");

      foreach (var data in dataset)
      {
        // Split data into input and expected output
        var input = data.Take(topology.InputCount).ToArray();
        var expected = data.Skip(topology.InputCount).ToArray();

        // Forward propagate and backpropagate
        var output = Forward(input);
        Backward(expected);

        // Display progress
        logger.Info("Training on data: {data}", string.Join(", ", data));
        logger.Info("Output: {output}", string.Join(", ", output));
      }

      logger.Info("Training completed.");
    }

    public double[] Forward(double[] inputs)
    {
      var layerInput = inputs;
      foreach (var layer in layers)
      {
        layerInput = layer.Forward(layerInput);
      }
      return layerInput;
    }

    private void Backward(double[] expected)
    {
      var outputLayer = layers.Last();
      outputLayer.CalculateDeltasForOutputLayer(expected);

      for (int i = layers.Count - 2; i >= 0; i--)
      {
        layers[i].CalculateDeltasForHiddenLayer(layers[i + 1]);
      }

      foreach (var layer in layers)
      {
        layer.UpdateWeights(topology.LearningRate);
      }
    }

    private List<Layer> InitializeLayers()
    {
      var layers = new List<Layer>
            {
                new Layer(topology.InputCount, topology.NeuronsPerLayer)
            };

      for (int i = 1; i < topology.HiddenLayers; i++)
      {
        layers.Add(new Layer(topology.NeuronsPerLayer, topology.NeuronsPerLayer));
      }

      layers.Add(new Layer(topology.NeuronsPerLayer, topology.OutputCount));
      return layers;
    }
  }
}

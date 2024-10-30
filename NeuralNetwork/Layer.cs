using System.Collections.Generic;
using System.Linq;

namespace NeuralNetworks.NeuralNetwork
{
  public class Layer
  {
    public List<Neuron> Neurons { get; }

    public Layer(int inputCount, int neuronCount)
    {
      Neurons = new List<Neuron>();
      for (int i = 0; i < neuronCount; i++)
      {
        Neurons.Add(new Neuron(inputCount));
      }
    }

    public double[] Forward(double[] inputs)
    {
      return Neurons.Select(neuron => neuron.CalculateOutput(inputs)).ToArray();
    }

    public void CalculateDeltasForOutputLayer(double[] expected)
    {
      for (int i = 0; i < Neurons.Count; i++)
      {
        Neurons[i].CalculateOutputDelta(expected[i]);
      }
    }

    public void CalculateDeltasForHiddenLayer(Layer nextLayer)
    {
      for (int i = 0; i < Neurons.Count; i++)
      {
        Neurons[i].CalculateHiddenDelta(nextLayer, i);
      }
    }

    public void UpdateWeights(double learningRate)
    {
      foreach (var neuron in Neurons)
      {
        neuron.UpdateWeights(learningRate);
      }
    }
  }
}

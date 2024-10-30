using System;
using System.Linq;

namespace NeuralNetworks.NeuralNetwork
{
  public class Neuron
  {
    private double[] weights;
    private double output;
    private double delta;

    public Neuron(int inputCount)
    {
      weights = new double[inputCount];
      var random = new Random();
      for (int i = 0; i < weights.Length; i++)
      {
        weights[i] = random.NextDouble();
      }
    }

    public double CalculateOutput(double[] inputs)
    {
      output = Sigmoid(inputs.Zip(weights, (input, weight) => input * weight).Sum());
      return output;
    }

    public void CalculateOutputDelta(double expected)
    {
      delta = (expected - output) * SigmoidDerivative(output);
    }

    public void CalculateHiddenDelta(Layer nextLayer, int neuronIndex)
    {
      delta = nextLayer.Neurons.Sum(neuron => neuron.delta * neuron.weights[neuronIndex]) * SigmoidDerivative(output);
    }

    public void UpdateWeights(double learningRate)
    {
      for (int i = 0; i < weights.Length; i++)
      {
        weights[i] += learningRate * delta; // Simple weight update
      }
    }

    private double Sigmoid(double x) => 1.0 / (1.0 + Math.Exp(-x));
    private double SigmoidDerivative(double x) => x * (1 - x);
  }
}

namespace NeuralNetworks
{
  public class Topology
  {
    public int InputCount { get; }
    public int OutputCount { get; }
    public double LearningRate { get; }
    public int HiddenLayers { get; }
    public int NeuronsPerLayer { get; }

    public Topology(int inputCount, int outputCount, double learningRate, int hiddenLayers, int neuronsPerLayer)
    {
      InputCount = inputCount;
      OutputCount = outputCount;
      LearningRate = learningRate;
      HiddenLayers = hiddenLayers;
      NeuronsPerLayer = neuronsPerLayer;
    }
  }
}

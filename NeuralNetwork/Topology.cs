namespace NeuralNetworks.NeuralNetwork
{
  public class Topology
  {
    public int InputCount { get; set; }
    public int OutputCount { get; set; }
    public int HiddenLayers { get; set; }
    public int NeuronsPerLayer { get; set; }
    public double LearningRate { get; set; }

    public Topology(int inputCount, int outputCount, int hiddenLayers, int neuronsPerLayer, double learningRate)
    {
      InputCount = inputCount;
      OutputCount = outputCount;
      HiddenLayers = hiddenLayers;
      NeuronsPerLayer = neuronsPerLayer;
      LearningRate = learningRate;
    }
  }
}

using System;
using System.IO;
using CommandLine;
using CsvHelper;
using NeuralNetworks.Client;
using NeuralNetworks.NeuralNetwork;
using NLog;

namespace NeuralNetworks
{
  class Program
  {
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
    private static readonly HashSet<string> UnrecognizedValues = new HashSet<string>();
    private static Dictionary<string, Dictionary<string, int>> categoricalEncodings = new Dictionary<string, Dictionary<string, int>>();

    static void Main(string[] args)
    {
      Console.WriteLine("Enter the Kaggle dataset name (e.g., 'username/dataset-name'):");
      var datasetName = Console.ReadLine();
      if (datasetName == null) throw new Exception("Dataset name can not be Null.");
      var records = new Client.Dataset().Load(datasetName);
    }

    private static void TrainModel(string datasetName)
    {

      var records = new Dataset().Load(datasetName);

      if (records == null || records.Count == 0)
      {
        logger.Error("Dataset loading failed or no data available.");
        return;
      }

      // Further processing and training logic goes here...
      logger.Info("Training completed for dataset: {datasetName}", datasetName);
    }
  }

  public class TrainOptions
  {
    [Value(0, MetaName = "dataset", HelpText = "Kaggle dataset name.", Required = true)]
    public string Dataset { get; set; }
  }
}

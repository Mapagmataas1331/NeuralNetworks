using System;
using System.Diagnostics;
using System.IO;
using CsvHelper;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;

namespace MyKaggleNeuralNetworkProject
{
  class Program
  {
    private const string KaggleDownloadPath = @"datasets";
    private const string KaggleCommand = "kaggle datasets download";

    static void Main(string[] args)
    {
      Console.WriteLine("Enter the Kaggle dataset name (e.g., 'username/dataset-name'):");
      var datasetName = Console.ReadLine();

      var datasetFolder = Path.Combine(KaggleDownloadPath, datasetName.Replace("/", "_"));

      if (Directory.Exists(datasetFolder))
      {
        Console.WriteLine("Dataset already exists locally. Do you want to retrain or view the dataset?");
        Console.WriteLine("Type 'retrain' to retrain or 'view' to see the dataset");
        var response = Console.ReadLine()?.ToLower();

        if (response == "retrain")
        {
          TrainModel(datasetFolder);
        }
        else if (response == "view")
        {
          ViewDataset(datasetFolder);
        }
        else
        {
          Console.WriteLine("Invalid input. Exiting.");
        }
      }
      else
      {
        DownloadDataset(datasetName, datasetFolder);
        TrainModel(datasetFolder);
      }
    }

    private static void DownloadDataset(string datasetName, string destinationFolder)
    {
      Directory.CreateDirectory(destinationFolder);
      var command = $"{KaggleCommand} {datasetName} -p {destinationFolder} --unzip";
      ExecuteCommand(command);
      Console.WriteLine($"Downloaded dataset '{datasetName}' to {destinationFolder}");
    }

    private static void TrainModel(string datasetFolder)
    {
      var csvFile = Directory.GetFiles(datasetFolder, "*.csv").FirstOrDefault();
      if (csvFile == null)
      {
        Console.WriteLine("No CSV file found in the dataset folder.");
        return;
      }

      var records = LoadDataset(csvFile);
      var neuralNetwork = new NeuralNetwork(new Topology(4, 1, 0.1, 2, 2));
      neuralNetwork.Learn(records);
      Console.WriteLine("Model trained successfully on the dataset.");
    }

    private static void ViewDataset(string datasetFolder)
    {
      var csvFile = Directory.GetFiles(datasetFolder, "*.csv").FirstOrDefault();
      if (csvFile == null)
      {
        Console.WriteLine("No CSV file found in the dataset folder.");
        return;
      }

      var records = LoadDataset(csvFile);
      Console.WriteLine("First 5 records from the dataset:");
      foreach (var record in records.Take(5))
      {
        Console.WriteLine(string.Join(", ", record));
      }
    }

    private static List<double[]> LoadDataset(string csvFile)
    {
      var records = new List<double[]>();
      using (var reader = new StreamReader(csvFile))
      using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
      {
        while (csv.Read())
        {
          var record = csv.Context.Record.Select(double.Parse).ToArray();
          records.Add(record);
        }
      }
      return records;
    }

    private static void ExecuteCommand(string command)
    {
      var processInfo = new ProcessStartInfo("cmd.exe", "/c " + command)
      {
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true
      };

      using (var process = Process.Start(processInfo))
      {
        if (process == null) return;

        process.WaitForExit();
        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();

        if (!string.IsNullOrEmpty(output))
          Console.WriteLine(output);
        if (!string.IsNullOrEmpty(error))
          Console.WriteLine($"Error: {error}");
      }
    }
  }
}

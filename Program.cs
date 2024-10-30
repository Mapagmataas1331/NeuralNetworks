using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace NeuralNetworks
{
  class Program
  {
    private const string KaggleDownloadPath = @"datasets";
    private const string KaggleCommand = "kaggle datasets download";
    private static readonly HashSet<string> UnrecognizedValues = new HashSet<string>();
    private static Dictionary<string, Dictionary<string, int>> categoricalEncodings = new Dictionary<string, Dictionary<string, int>>();

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
      if (records == null || records.Count == 0)
      {
        Console.WriteLine("Dataset loading failed or no data available.");
        return;
      }

      Console.WriteLine($"Loaded dataset with {records.Count} records.");
      var neuralNetwork = new Network(new Topology(4, 1, 0.1, 2, 2));

      try
      {
        neuralNetwork.Learn(records);
        Console.WriteLine("{0}", records);
        Console.WriteLine("Model trained successfully on the dataset.");
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Training failed with error: {ex.Message}");
      }
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
      try
      {
        using (var reader = new StreamReader(csvFile))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
          csv.Read(); // Read header
          csv.ReadHeader();
          var headers = csv.HeaderRecord;

          // Initialize encoding dictionary for each header
          foreach (var header in headers)
          {
            categoricalEncodings[header] = new Dictionary<string, int>();
          }

          // Process each record
          while (csv.Read())
          {
            var record = csv.Parser.Record;
            var parsedRecord = record.Select((value, index) => ParseValue(value, headers[index])).ToArray();

            // Check for NaN values indicating unparseable fields
            if (parsedRecord.Any(double.IsNaN))
            {
              Console.WriteLine("Skipping record with unparseable values.");
              continue;
            }

            records.Add(parsedRecord);
          }
        }

        // Output all unique unrecognized values
        if (UnrecognizedValues.Count > 0)
        {
          Console.WriteLine("Unrecognized values found in the dataset:");
          foreach (var value in UnrecognizedValues)
          {
            Console.WriteLine($"- {value}");
          }
        }

        // Print the categorical encodings
        PrintEncodings();
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error loading dataset: {ex.Message}");
        return null;
      }
      return records;
    }

    private static double ParseValue(string value, string header)
    {
      if (double.TryParse(value, out var result))
      {
        return result;
      }

      // If it's categorical, handle encoding
      if (!categoricalEncodings[header].ContainsKey(value))
      {
        int newIndex = categoricalEncodings[header].Count;
        categoricalEncodings[header][value] = newIndex;
      }
      return categoricalEncodings[header][value];
    }

    private static void PrintEncodings()
    {
      Console.WriteLine("Categorical Encodings:");
      foreach (var header in categoricalEncodings.Keys)
      {
        Console.WriteLine($"- {header}:");
        foreach (var kvp in categoricalEncodings[header])
        {
          Console.WriteLine($"    {kvp.Key} => {kvp.Value}");
        }
      }
    }

    private static double LogUnrecognizedValues(string value)
    {
      UnrecognizedValues.Add(value);
      return double.NaN;
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

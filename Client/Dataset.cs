using CsvHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Xunit;

namespace NeuralNetworks.Client
{
  public class Dataset
  {
    private static readonly HashSet<string> UnrecognizedValues = new HashSet<string>();
    private static Dictionary<string, Dictionary<string, int>> categoricalEncodings = new Dictionary<string, Dictionary<string, int>>();

    private const string KaggleDownloadPath = @"datasets";
    private const string KaggleCommand = "kaggle datasets";

    public List<double[]> Load(string datasetName)
    {
      var datasetFolder = Path.Combine(KaggleDownloadPath, datasetName.Replace("/", "_"));

      if (!Directory.Exists(datasetFolder))
      {
        DownloadDataset(datasetName, datasetFolder);
      }

      var csvFile = Directory.GetFiles(datasetFolder, "*.csv").FirstOrDefault();

      if (csvFile == null)
      {
        throw new Exception($"{datasetFolder}/{datasetName} failed to load");
      }

      var unformattedRecords = ReadDataset(csvFile);

      if (unformattedRecords == null || unformattedRecords.Count == 0)
      {
        throw new Exception("No data available.");
      }

      Console.WriteLine("Loaded dataset. Translating categorical data:");
      PrintEncodings();

      return unformattedRecords;
    }
    private static void DownloadDataset(string datasetName, string datasetFolder)
    {
      Directory.CreateDirectory(datasetFolder);
      var command = $"{KaggleCommand} download {datasetName} -p {datasetFolder} --unzip";
      ExecuteCommandAsync(command).Wait();
      Console.WriteLine($"Downloaded dataset '{datasetName}' to {datasetFolder}");
    }

    private static async Task ExecuteCommandAsync(string command)
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

        var output = await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();

        if (!string.IsNullOrEmpty(output))
          Console.WriteLine(output);
        if (!string.IsNullOrEmpty(error))
          Console.WriteLine($"Error: {error}");

        await process.WaitForExitAsync();
      }
    }
    private List<double[]> ReadDataset(string csvFile)
    {
      var records = new List<double[]>();
      try
      {
        using (var reader = new StreamReader(csvFile))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
          csv.Read();
          csv.ReadHeader();
          var headers = csv.HeaderRecord;

          Console.WriteLine($"Loaded headers: {string.Join(", ", headers)}");

          // Initialize encodings for each header
          foreach (var header in headers)
          {
            if (!categoricalEncodings.ContainsKey(header))
            {
              categoricalEncodings[header] = new Dictionary<string, int>();
            }
          }

          while (csv.Read())
          {
            var record = csv.Parser.Record;
            var parsedRecord = record.Select((value, index) => ParseValue(value, headers[index])).ToArray();

            // Check for unparseable values and skip the record if found
            if (parsedRecord.Any(double.IsNaN))
            {
              Console.WriteLine("Skipping record with unparseable values.");
              continue;
            }

            records.Add(parsedRecord);
          }
        }

        if (UnrecognizedValues.Count > 0)
        {
          Console.WriteLine("Unrecognized values found in the dataset:");
          foreach (var value in UnrecognizedValues)
          {
            Console.WriteLine($"- {value}");
          }
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error loading dataset: {ex.Message}");
        return null;
      }
      return records;
    }

    private double ParseValue(string value, string header)
    {
      if (double.TryParse(value, out var result))
      {
        return result;
      }

      // If the value is not recognized, assign it a new encoding
      if (!categoricalEncodings[header].ContainsKey(value))
      {
        int newIndex = categoricalEncodings[header].Count;
        categoricalEncodings[header][value] = newIndex;
      }
      return categoricalEncodings[header][value];
    }

    public void PrintEncodings()
    {
      Console.WriteLine("Categorical Encodings:");
      foreach (var header in categoricalEncodings.Keys)
      {
        Console.Write($"- {header}: ");
        foreach (var kvp in categoricalEncodings[header])
        {
          Console.Write($"{kvp.Key} => {kvp.Value}, ");
        }
        Console.WriteLine("");
      }
    }
  }
}

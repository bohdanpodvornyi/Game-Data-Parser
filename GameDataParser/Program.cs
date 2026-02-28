using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

namespace GameDataParser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string fileName = string.Empty;
                do
                {
                    Console.WriteLine("Enter the name of file you want to read:");
                    fileName = Console.ReadLine();
                }
                while (!FileReader.CheckFileName(fileName));

                List<GameSave> save = FileReader.ReadFile(fileName);
                if (save != null)
                {
                    Console.WriteLine("Loaded games are:");
                    foreach (GameSave s in save)
                    {
                        Console.WriteLine(s);
                    }
                } 
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message, ex.StackTrace);
            }
        }
        public class GameSave
        {
            public string Title { get; set; }
            public int ReleaseYear { get; set; }
            public double Rating { get; set; }

            public GameSave(string title, int releaseYear, double rating)
            {
                Title = title;
                ReleaseYear = releaseYear;
                Rating = rating; 
            }
            public bool IsEmpty()
            {
                return string.IsNullOrEmpty(Title) && ReleaseYear == 0 && Rating == 0;
            }
            public override string ToString()
            {
                return $"{Title}, released in {ReleaseYear}, rating: {Rating}";
            }
        }
        public static class Logger
        {
            public static void Log(string message, string trace)
            {
                string logFilePath = "log.txt";
                string logData = $"[{DateTime.Now}], Exception message: {message}, Stack trace: {trace}\n";
                if (!File.Exists(logFilePath))
                {
                    File.WriteAllText(logFilePath, logData);
                }
                else
                {
                    File.WriteAllText(logFilePath, File.ReadAllText(logFilePath) + "\n" + logData);
                }
            }
        }
        public static class FileReader
        {
            public static bool CheckFileName(string filename)
            {
                if (filename is null)
                {
                    Console.WriteLine("File name cannot be null.");
                    return false;
                }
                else if (filename.Equals(string.Empty))
                {
                    Console.WriteLine("File name cannot be empty.");
                    return false;
                }
                else if (!File.Exists(filename))
                {
                    Console.WriteLine("File not found.");
                    return false;
                }
                Console.WriteLine();
                return true;
            }
            public static List<GameSave> ReadFile(string filename)
            {
                try
                {
                    var result = JsonSerializer.Deserialize<List<GameSave>>(File.ReadAllText(filename));
                    if (result.Count == 1 && result[0].IsEmpty())
                    {
                        Console.WriteLine("No games are present in the input file.");
                        return null;
                    }
                    return result;
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"JSON in the {filename} was not in a valid format. JSON body:");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(File.ReadAllText(filename));
                    Console.ResetColor();
                    Logger.Log(ex.Message, ex.StackTrace);
                    return null;
                }
            }
        }
    }
}

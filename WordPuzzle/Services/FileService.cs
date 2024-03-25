﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordPuzzle.Interfaces;

namespace WordPuzzle.Services
{
    public class FileService : IFileService
    {
        public void ExportFile(List<string> result, string path)
        {
            try
            {
                if (result == null || result.Count == 0)
                {
                    throw new ArgumentException("The list is empty. Nothing to export to txt.");
                }
                File.WriteAllLines(path, result);
                Console.WriteLine($"File successfully exported to: {path}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting the file: {ex.Message}");
            }
        }
    
        public List<string> ReadFile(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException($"The file '{path}' was not found.");
                }
                return new List<string>(File.ReadAllLines(path));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading the file '{path}': {ex.Message}");
                return new List<string>();
            }
        }
    }
}
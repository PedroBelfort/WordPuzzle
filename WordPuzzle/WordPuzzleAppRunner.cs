﻿using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WordPuzzle.Exceptions;
using WordPuzzle.Interfaces;
using WordPuzzle.Services;

namespace WordPuzzle
{
    internal class WordPuzzleAppRunner
    {
        private readonly IServiceProvider serviceProvider;

        public WordPuzzleAppRunner(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void Run()
        {
            var wordPuzzleSolver = this.serviceProvider.GetService<ISolverService>();
            var fileService = this.serviceProvider.GetService<IFileService>();
            var validatorService = this.serviceProvider.GetService<IValidatorService>();

            var dictionary = new List<string>();
            bool runApp = true;

            var filePath = fileService.GetFilePath();
            dictionary = fileService.ReadFile(filePath);

            if(dictionary.Count == 0)
            {
                runApp = false;
            }

            while (runApp)
            {
                string startWord = null;

                Console.WriteLine("\nEnter the start word (or type 'bye' to quit, or 'clear' to clear the screen):");
                startWord = Console.ReadLine();

                if (startWord.ToLower() == "bye")
                    break;

                if (startWord.ToLower() == "clear")
                {
                    Console.Clear();
                    continue;
                }

                try
                {
                    validatorService.InvalidLengthWord(startWord);
                    validatorService.WordNotExistOnDictionary(startWord, dictionary);

                    string endWord = null;
                    bool isValidEndWord = false;

                    while (!isValidEndWord)
                    {
                        Console.WriteLine("\nEnter the end word:");
                        endWord = Console.ReadLine();

                        if (endWord.ToLower() == "bye")
                            return;

                        if (endWord.ToLower() == "clear")
                        {
                            Console.Clear();
                            continue;
                        }

                        try
                        {
                            validatorService.InvalidLengthWord(endWord);
                            validatorService.WordNotExistOnDictionary(endWord, dictionary);
                            isValidEndWord = true;
                        }
                        catch (WordValidationException ex)
                        {
                            Console.WriteLine($"\nError: {ex.Message}");
                        }
                    }

                    Console.WriteLine("Processing....");
                    Console.Clear();

                    List<string> solution = wordPuzzleSolver.Solve(startWord, endWord, dictionary);

                    var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                    var fileServiceExport = this.serviceProvider.GetService<IFileService>();
                    fileServiceExport.ExportFile(solution, $"{path}/{startWord}-{endWord}.txt");

                    Console.WriteLine($"\n{startWord} => {endWord}:\n");
                    foreach (var word in solution)
                    {
                        Console.WriteLine(word);
                    }
                }
                catch (WordValidationException ex)
                {
                    Console.WriteLine($"\nError: {ex.Message}\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nUnexpected error: {ex.Message}\n");
                }
            }

            Console.WriteLine("\nExiting the program...");
        }
    }
}

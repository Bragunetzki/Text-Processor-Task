using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Text_Processor.Model
{
    public class TextProcessor
    {
        public void ProcessFiles(IEnumerable<string> inputFiles, string outputFile, int minWordLength, bool removePunctuation)
        {
            using StreamWriter writer = new StreamWriter(outputFile);
            foreach (var inputFile in inputFiles)
            {
                ProcessFile(inputFile, writer, minWordLength, removePunctuation);
            }
        }

        private void ProcessFile(string inputFile, StreamWriter writer, int minWordLength, bool removePunctuation)
        {
            int bufferSize = 4096;
            char[] buffer = new char[bufferSize];
            StringBuilder lineBuilder = new();
            List<string> batch = [];

            using StreamReader reader = new(inputFile);
            int charsRead;
            while ((charsRead = reader.ReadBlock(buffer, 0, bufferSize)) > 0)
            {
                for (int i = 0; i < charsRead; i++)
                {
                    if (buffer[i] == '\n')
                    {
                        batch.Add(lineBuilder.ToString());
                        lineBuilder.Clear();

                        if (batch.Count >= 100)
                        {
                            ProcessAndWriteBatch(batch, writer, minWordLength, removePunctuation);
                            batch.Clear();
                        }
                    }
                    else
                    {
                        lineBuilder.Append(buffer[i]);
                    }
                }
            }

            if (lineBuilder.Length > 0)
            {
                batch.Add(lineBuilder.ToString());
            }

            if (batch.Count > 0)
            {
                ProcessAndWriteBatch(batch, writer, minWordLength, removePunctuation);
            }
        }

        private void ProcessAndWriteBatch(List<string> batch, StreamWriter writer, int minWordLength, bool removePunctuation)
        {
            foreach (var line in batch)
            {
                string processedLine = ProcessText(line, minWordLength, removePunctuation);
                writer.WriteLine(processedLine);
            }
        }

        private string ProcessText(string text, int minWordLength, bool removePunctuation)
        {
            if (removePunctuation)
            {
                text = Regex.Replace(text, @"[^\w\s]", "");
            }

            var words = text.Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            var filteredWords = words.Where(word => word.Length >= minWordLength);

            return string.Join(" ", filteredWords);
        }
    }
}

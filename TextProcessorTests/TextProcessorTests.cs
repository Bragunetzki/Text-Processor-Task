using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Text_Processor.Model;

namespace TextProcessorTests
{
    [TestFixture]
    public class TextProcessorTests
    {
        private TextProcessor _textProcessor;

        [SetUp]
        public void Setup()
        {
            _textProcessor = new TextProcessor();
        }

        [Test]
        public void ProcessFiles_ProcessesMultipleFiles()
        {
            string inputText1 = "File1 Line1\nFile1 Line2";
            string inputText2 = "File2 Line1\nFile2 Line2";
            var inputFile1 = Path.GetTempFileName();
            var inputFile2 = Path.GetTempFileName();
            var outputFile = Path.GetTempFileName();
            File.WriteAllText(inputFile1, inputText1);
            File.WriteAllText(inputFile2, inputText2);

            var inputFiles = new List<string> { inputFile1, inputFile2 };
            int minWordLength = 2;
            bool removePunctuation = false;

            _textProcessor.ProcessFiles(inputFiles, outputFile, minWordLength, removePunctuation);

            string outputText = File.ReadAllText(outputFile);
            string expectedOutput = inputText1 + "\n" + inputText2 + "\n";

            outputText = outputText.Replace("\r\n", "\n");
            expectedOutput = expectedOutput.Replace("\r\n", "\n");
            Assert.That(outputText, Is.EqualTo(expectedOutput));

            File.Delete(inputFile1);
            File.Delete(inputFile2);
            File.Delete(outputFile);
        }

        [Test]
        public void ProcessFiles_RemovesPunctuationAndFiltersWords()
        {
            string inputText1 = "Hello, world! This is a test.";
            string inputText2 = "Hello, world! This is another test.";
            var inputFile1 = Path.GetTempFileName();
            var inputFile2 = Path.GetTempFileName();
            var outputFile = Path.GetTempFileName();
            File.WriteAllText(inputFile1, inputText1);
            File.WriteAllText(inputFile2, inputText2);

            var inputFiles = new List<string> { inputFile1, inputFile2 };
            int minWordLength = 4;
            bool removePunctuation = true;

            _textProcessor.ProcessFiles(inputFiles, outputFile, minWordLength, removePunctuation);

            string outputText = File.ReadAllText(outputFile);
            string expectedOutput = "Hello world This test\nHello world This another test\n";

            outputText = outputText.Replace("\r\n", "\n");
            expectedOutput = expectedOutput.Replace("\r\n", "\n");
            Assert.That(outputText, Is.EqualTo(expectedOutput));

            File.Delete(inputFile1);
            File.Delete(inputFile2);
            File.Delete(outputFile);
        }

        [Test]
        public void ProcessFiles_FiltersWords()
        {
            string inputText1 = "An apple a day keeps the doctor away.";
            string inputText2 = "A quick brown fox.";
            var inputFile1 = Path.GetTempFileName();
            var inputFile2 = Path.GetTempFileName();
            var outputFile = Path.GetTempFileName();
            File.WriteAllText(inputFile1, inputText1);
            File.WriteAllText(inputFile2, inputText2);

            var inputFiles = new List<string> { inputFile1, inputFile2 };
            int minWordLength = 3;
            bool removePunctuation = true;

            _textProcessor.ProcessFiles(inputFiles, outputFile, minWordLength, removePunctuation);

            string outputText = File.ReadAllText(outputFile);
            string expectedOutput = "apple day keeps the doctor away\nquick brown fox\n";

            outputText = outputText.Replace("\r\n", "\n");
            expectedOutput = expectedOutput.Replace("\r\n", "\n");
            Assert.That(outputText, Is.EqualTo(expectedOutput));

            File.Delete(inputFile1);
            File.Delete(inputFile2);
            File.Delete(outputFile);
        }

        [Test]
        public void ProcessFiles_NegativeMinLength()
        {
            string inputText1 = "An apple a day keeps the doctor away.";
            string inputText2 = "A quick brown fox.";
            var inputFile1 = Path.GetTempFileName();
            var inputFile2 = Path.GetTempFileName();
            var outputFile = Path.GetTempFileName();
            File.WriteAllText(inputFile1, inputText1);
            File.WriteAllText(inputFile2, inputText2);

            var inputFiles = new List<string> { inputFile1, inputFile2 };
            int minWordLength = -5;
            bool removePunctuation = false;

            _textProcessor.ProcessFiles(inputFiles, outputFile, minWordLength, removePunctuation);

            string outputText = File.ReadAllText(outputFile);
            string expectedOutput = "An apple a day keeps the doctor away.\nA quick brown fox.\n";

            outputText = outputText.Replace("\r\n", "\n");
            expectedOutput = expectedOutput.Replace("\r\n", "\n");
            Assert.That(outputText, Is.EqualTo(expectedOutput));

            File.Delete(inputFile1);
            File.Delete(inputFile2);
            File.Delete(outputFile);
        }

        [Test]
        public void ProcessFiles_InvalidFile()
        {
            string inputText1 = "An apple a day keeps the doctor away.";
            var inputFile1 = Path.GetTempFileName();
            var outputFile = Path.GetTempFileName();
            var invalidFile = "invalid file";
            File.WriteAllText(inputFile1, inputText1);

            var inputFiles = new List<string> { inputFile1, invalidFile };
            int minWordLength = 3;
            bool removePunctuation = false;

            var ex = Assert.Throws<FileNotFoundException>(() =>
            {
                _textProcessor.ProcessFiles(inputFiles, outputFile, minWordLength, removePunctuation);
            });

            Assert.That(ex.Message, Does.Contain(invalidFile));
            File.Delete(inputFile1);
            File.Delete(outputFile);
        }

        [Test]
        public void ProcessFiles_EmptyFileList()
        {
            var outputFile = Path.GetTempFileName();
            var inputFiles = new List<string>();
            int minWordLength = 3;
            bool removePunctuation = true;

            _textProcessor.ProcessFiles(inputFiles, outputFile, minWordLength, removePunctuation);

            string outputText = File.ReadAllText(outputFile);
            string expectedOutput = "";

            Assert.That(outputText, Is.EqualTo(expectedOutput));
            
            File.Delete(outputFile);
        }
    }
}

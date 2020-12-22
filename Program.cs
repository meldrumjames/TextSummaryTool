using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TextSummaryTool
{
    class Program
    {
        static void Main(string[] args)
        {
            TextProcessor textProcessor = new TextProcessor();

            // user input
            Console.WriteLine("Please enter the name of the file to be used:");
            string userInput = Console.ReadLine();
            textProcessor.ReadInputFile(userInput);

            Console.WriteLine("Would you like to enable filler for the output?");
            Console.WriteLine("This will add sentences with less relevant keywords until the summarisation factor is met.");
            Console.WriteLine("Please press Y / N");
            string fillerInput = Console.ReadLine();
            textProcessor.SetFiller(fillerInput);

            // textProcessor.ReadStopWords("stopWords.txt");
            // Console.WriteLine(textProcessor.FindTotalWordCount());


            // process file

            // write outputs

            // do it again without restarting

            Console.ReadKey();
        }
    }

    class TextProcessor
    {
        /*
         bool FilterEnabled = False;
    List<string> sentences = new List<string>();
      List<string> stopWords = new List<string>();
      List<string> inputWords = new List<string>();
      List<string> spaceWords = new List<string>();
      List<string> extractedSentences = new List<string>();
      List<string> fillerSentences = new List<string>();
      List<string> output = new List<string>();

     bool ReadStopWords(String filename)  
     {
          // read stopwords file
    }

    bool ReadText(String filename)
    {
         // open file and read text
         // fill all Lists above as you did before in main
    }

    bool Process()
    {
          //process the file as you did in while loop
          // and make output to console
    }

    void Print() 
    {
         // print text processor stats 
     }
        */


        bool fillerEnabled = false;
        public List<string> sentences = new List<string>();
        public List<string> stopWords = new List<string>();
        public List<string> inputWords = new List<string>();
        List<string> spaceWords = new List<string>();
        List<string> extractedSentences = new List<string>();
        List<string> fillerSentences = new List<string>();
        List<string> output = new List<string>();

        public void ReadInputFile(string fileName)
        {
            try
            {
                StreamReader inputReader = new StreamReader(fileName);
                string inputText = inputReader.ReadToEnd();
                string[] inputSplitWords = inputText.Split(' ', '\n', '\r', '\t');
                string[] inputLines = inputText.Split('.', '?', '!');

                // splitting for sentences
                for (int i = 0; i < inputLines.Length; i++)
                {
                    sentences.Add(inputLines[i]);
                }

                // splitting for all words
                for (int i = 0; i < inputSplitWords.Length; i++)
                {
                    // trimming unnecessary chars
                    inputWords.Add(inputSplitWords[i].Trim(new char[] { ' ', '"', ',', '.', ';', ':', '-', '_', '=' }));
                }
                inputReader.Close();
            }
            catch (IOException e)
            {
                Console.WriteLine("An error has occured: " + e.Message);
            }
        }

        public void ReadStopWords(string fileName)
        {
            try
            {
                StreamReader stopReader = new StreamReader(fileName);
                string stopText = stopReader.ReadToEnd();
                string[] stopLines = stopText.Split('\n', '\r', '\t');

                for (int i = 0; i < stopLines.Length; i++)
                {
                    stopWords.Add(stopLines[i]);
                }
                stopReader.Close();
            }
            catch (IOException e)
            {
                Console.WriteLine("An error has occured: " + e.Message);
            }
        }

        public void SetFiller(string fillerInput)
        {
            if ((fillerInput == "Y") || (fillerInput == "y"))
            {
                fillerEnabled = true;
                //Console.WriteLine("filler enabled");
            }
            else if ((fillerInput == "N") || (fillerInput == "n"))
            {
                fillerEnabled = false;
                //Console.WriteLine("filler disabled");
            }
        }

        public int FindTotalWordCount()
        {
            int sentenceWordCount = 0, totalWordCount = 0;

            for (int i = 0; i < sentences.Count; i++)
            {
                // finding total word count
                sentenceWordCount = sentences[i].Split(' ').Length;
                totalWordCount += sentenceWordCount;
            }

            return totalWordCount;
        }
    }
}
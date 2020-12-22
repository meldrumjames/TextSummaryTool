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
            /*
            TextProcessor = new TextProcessor()
            while (True)
            {
                // wait for user input

                // process file


                // write outputs


                // do it again without restarting the program
            }

            // quit
            */

            TextProcessor textProcessor = new TextProcessor();

            // user input
            Console.WriteLine("Please enter the name of the file to be used:");
            var userInput = Console.ReadLine();
            textProcessor.ReadInputFile(userInput);

            textProcessor.ReadStopWords("stopWords.txt");
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

     bool SetFilter(enableFilter)
     {
           this.FilterEnable = enableFilter;
     }

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

        public void ReadInputFile(string fileName)
        {
            try
            {
                StreamReader inputReader = new StreamReader(fileName);
                string inputText = inputReader.ReadToEnd();
                string[] inputLines = inputText.Split('.', '?', '!');

                foreach (string s in inputLines)
                {
                    //Console.WriteLine(s);
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
                StreamReader readerOne = new StreamReader(fileName);
                string stopText = readerOne.ReadToEnd();
                string[] stopLines = stopText.Split('\n', '\r', '\t');

                foreach(string s in stopLines)
                {
                    Console.WriteLine(s);
                }
                
                readerOne.Close();
            }
            catch (IOException e)
            {
                Console.WriteLine("An error has occured: " + e.Message);
            }
        }
    }
}
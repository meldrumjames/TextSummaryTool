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
            int sf = 0;

            // getting input file
            Console.WriteLine("Please enter the file path of the file to be used:");
            string userInput = Console.ReadLine();
            textProcessor.ReadInputFile(userInput);

            // setting summarisation factor
            Console.WriteLine("Please enter a percent summarisation factor:");
            var isNumeric = int.TryParse(Console.ReadLine(), out sf);

            // enabling filler content
            Console.WriteLine("Would you like to enable filler for the output?");
            Console.WriteLine("This will add sentences with less relevant keywords until the summarisation factor is met.");
            Console.WriteLine("Please press Y / N");
            string fillerInput = Console.ReadLine();
            textProcessor.SetFiller(fillerInput);

            // getting and sorting stopWords file
            textProcessor.ReadStopWords("stopWords.txt");

            //process text
            textProcessor.ProcessText(textProcessor.FindTotalWordCount(), sf);

            //output
            textProcessor.Output();

            // exit application
            Console.WriteLine("Please press any key to exit.");
            Console.ReadKey();
        }
    }

    class TextProcessor
    {
        bool fillerEnabled = false, processed = false;
        List<string> sentences = new List<string>();
        List<string> stopWords = new List<string>();
        List<string> inputWords = new List<string>();
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

            // removing empty entries from inputWords list
            inputWords = inputWords.Where(a => !string.IsNullOrWhiteSpace(a)).ToList();
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
            }
            else if ((fillerInput == "N") || (fillerInput == "n"))
            {
                fillerEnabled = false;
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

        public void ProcessText(double totalWordCount, int sf)
        {
            // removing stop words from input list
            for (int i = 0; i < stopWords.Count; i++)
            {
                inputWords.RemoveAll(n => n.Equals(stopWords[i], StringComparison.OrdinalIgnoreCase));
            }

            // adding a space to the end of each word so that the results dont appear within other words
            foreach (string s in inputWords)
            {
                string wordAddSpace;
                wordAddSpace = s + " ";
                spaceWords.Add(wordAddSpace);
            }

            // gets freq of words and adds them to ordered list
            var freqList = spaceWords.GroupBy(i => i)
                .OrderBy(g => g.Count())
                .Select(g => g.Key)
                .ToList();

            // reversing the list because for some reason it was upside down
            freqList.Reverse();

            // setting variables for most and secondmost common words from this list
            string mostCommon = freqList[0];
            string secondMostCommon = freqList[1];
            string thirdCommon = freqList[2];
            string fourthCommon = freqList[3];

            // creating lower/upper case versions of the words to also check for
            string mostcommon = mostCommon.ToLower();
            string MOSTCOMMON = mostCommon.ToUpper();

            // pulling sentences from document that contain the relevant keyWords
            foreach (string s in sentences)
            {
                if ((s.Contains(mostCommon) && s.Contains(secondMostCommon) == true) || (s.Contains(mostcommon) == true) || (s.Contains(MOSTCOMMON) == true))
                {
                    // adding them to separate list 
                    extractedSentences.Add(s);
                }
                else if (s.Contains(mostCommon) == true)
                {
                    extractedSentences.Add(s);
                }
                else if (s.Contains(secondMostCommon) == true)
                {
                    extractedSentences.Add(s);
                }
            }

            // pulling extra keywords in case we need to fill out the output
            foreach (string s in sentences)
            {
                if (s.Contains(thirdCommon) == true)
                {
                    fillerSentences.Add(s);
                }
                else if (s.Contains(fourthCommon) == true)
                {
                    fillerSentences.Add(s);
                }
            }

            // if any sentences are in both list this will remove them from the filler list
            for (int i = 0; i < extractedSentences.Count; i++)
            {
                fillerSentences.RemoveAll(n => n.Equals(extractedSentences[i], StringComparison.OrdinalIgnoreCase));
            }

            // adding full-stops to the end of the sentences for readability
            for (int s = 0; s < extractedSentences.Count; s++)
            {
                extractedSentences[s] += @".";
            }

            for (int s = 0; s < fillerSentences.Count; s++)
            {
                fillerSentences[s] += @".";
            }

            // and to here
            sentences[0] += @".";

            // adding the first sentence of the document as it will give a good idea of what's to come
            output.Add(sentences[0]);

            // if the first sentence is also an 'extracted' sentence it can be deleted as otherwise it would be duplicated
            if (sentences[0] == extractedSentences[0])
            {
                output.RemoveAt(0);
            }

            var extractedIndex = 0;
            var fillerIndex = 0;
            double outputTotalWordCount = 0;
            double actualSF = 0;

            while (processed == false)
            {
                while (extractedIndex <= extractedSentences.Count)
                {
                    // checking calculation
                    if ((outputTotalWordCount * (100 / totalWordCount)) < sf)
                    {
                        // if all the sentences have been used up then can fill out with filler sentences. 
                        if (extractedIndex == extractedSentences.Count)
                        {

                            while ((fillerIndex <= fillerSentences.Count) && (fillerEnabled == true))
                            {
                                // checking calculation
                                if ((outputTotalWordCount * (100 / totalWordCount)) < sf)
                                {
                                    // if all the sentences have been used up then exit out 
                                    if (fillerIndex == fillerSentences.Count)
                                    {
                                        // calculating what the final sf was
                                        actualSF = (outputTotalWordCount * (100 / totalWordCount));
                                        actualSF = Math.Round(actualSF, 2);

                                        // display to screen
                                        Console.WriteLine();
                                        Console.WriteLine("The text has been processed.");
                                        Console.WriteLine("The initial sf was " + sf + " While the final sf is " + actualSF);

                                        processed = true;
                                        return;
                                    }

                                    // adding sentence and updating word count 
                                    output.Add(fillerSentences[fillerIndex]);
                                    outputTotalWordCount += output[fillerIndex].Split(' ').Length;

                                    // if sf is exceeded then exit out
                                    if ((outputTotalWordCount * (100 / totalWordCount)) >= sf)
                                    {
                                        // remove the last sentence to reduce the word count
                                        output.RemoveAt(fillerIndex);

                                        actualSF = (outputTotalWordCount * (100 / totalWordCount));
                                        actualSF = Math.Round(actualSF, 2);

                                        // display to screen
                                        Console.WriteLine();
                                        Console.WriteLine("The text has been processed.");
                                        Console.WriteLine("The initial sf was " + sf + " While the final sf is " + actualSF);

                                        processed = true;
                                        return;
                                    }
                                    else
                                    {
                                        fillerIndex++;
                                    }
                                }
                            }

                            // calculating what the final sf was
                            actualSF = (outputTotalWordCount * (100 / totalWordCount));
                            actualSF = Math.Round(actualSF, 2);

                            // display to screen
                            Console.WriteLine();
                            Console.WriteLine("The text has been processed.");
                            Console.WriteLine("The initial sf was " + sf + " While the final sf is " + actualSF);

                            processed = true;
                            return;
                        }

                        // adding sentence and updating word count 
                        output.Add(extractedSentences[extractedIndex]);
                        outputTotalWordCount += output[extractedIndex].Split(' ').Length;

                        // if sf is exceeded then exit out
                        if ((outputTotalWordCount * (100 / totalWordCount)) >= sf)
                        {
                            // remove the last sentence to reduce the word count
                            output.RemoveAt(extractedIndex);

                            actualSF = (outputTotalWordCount * (100 / totalWordCount));
                            actualSF = Math.Round(actualSF, 2);

                            // display to screen
                            Console.WriteLine();
                            Console.WriteLine("The text has been processed.");
                            Console.WriteLine("The initial sf was " + sf + " While the final sf is " + actualSF);

                            processed = true;
                            return;
                        }
                        else
                        {
                            extractedIndex++;
                        }
                    }
                }
            }
        }

        public void Output()
        {
            if (processed == true)
            {
                try
                {
                    StreamWriter sw = new StreamWriter("output.txt");

                    for (int i = 0; i < output.Count; i++)
                    {
                        sw.WriteLine(output[i]);
                    }
                    Console.WriteLine("The output has been saved into the same folder as output.txt");
                    sw.Close();
                }
                catch (IOException e)
                {
                    Console.WriteLine("error has occured: " + e.Message);
                }
            }
        }
    }
}
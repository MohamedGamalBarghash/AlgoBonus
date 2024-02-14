using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DocumentDistance
{
    class DocDistance
    {
        // *****************************************
        // DON'T CHANGE CLASS OR FUNCTION NAME
        // YOU CAN ADD FUNCTIONS IF YOU NEED TO
        // *****************************************
        /// <summary>
        /// Write an efficient algorithm to calculate the distance between two documents
        /// </summary>
        /// <param name="doc1FilePath">File path of 1st document</param>
        /// <param name="doc2FilePath">File path of 2nd document</param>
        /// <returns>The angle (in degree) between the 2 documents</returns>
        /// 

        

        //public static double CalculateDistance(string doc1FilePath, string doc2FilePath)
        //{
        //    // TODO comment the following line THEN fill your code here
        //    //throw new NotImplementedException();

        //    Dictionary<string, int> file1;
        //    Dictionary<string, int> file2;

        //    using (StreamReader streamReader = File.OpenText(doc1FilePath))
        //    {
        //        string text = streamReader.ReadToEnd();
        //        file1 = CountWordFrequencies(text);

        //    }

        //    using (StreamReader streamReader = File.OpenText(doc2FilePath))
        //    {
        //        string text = streamReader.ReadToEnd();
        //        file2 = CountWordFrequencies(text);

        //    }




        //    return 0.0;
        //}

        public static double CalculateDistance(string document1, string document2)
        {
            // Preprocess documents (lowercase, alphanumeric only, split by non-alphanumeric)
            var (tokens1, sum1) = CreateFrequencyDict(document1);
            var (tokens2, sum2) = CreateFrequencyDict(document2);

            //foreach (KeyValuePair<string, int> pair in tf1)
            //{
            //    Console.WriteLine($"Key: {pair.Key}, Value: {pair.Value}");
            //}

            //foreach (KeyValuePair<string, int> pair in tf2)
            //{
            //    Console.WriteLine($"Key: {pair.Key}, Value: {pair.Value}");
            //}

            // Calculate dot product (sum of product of corresponding TF values)
            double dotProduct = tokens1.Sum(kv => kv.Value * GetValueOrDefault(tokens2, kv.Key));

            // Calculate document lengths (Euclidean norm of TF vectors)
            double length1 = tokens1.Values.Sum(v => (long)v * v);
            double length2 = tokens2.Values.Sum(v => (long)v * v);

            //Console.WriteLine("Length1: " + length1);
            //Console.WriteLine("Length2: " + length2);

            //Console.WriteLine("Sum1: " + sum1);
            //Console.WriteLine("Sum2: " + sum2);

            length1 = Math.Sqrt(length1);
            length2 = Math.Sqrt(length2);

            //int len = Math.Max(tokens1.Count, tokens2.Count);

            //length1 = Math.Sqrt(sum1);
            //length2 = Math.Sqrt(sum2);

            //double similarity = dotProduct / (length1 * length2);
            double similarity = dotProduct / (length1 * length2);

            // Clamp similarity within the valid range [-1, 1]
            //similarity = Math.Max(-1, Math.Min(1, similarity));

            double angle = 0;

            if (similarity >= 1)
            {
                angle = 0;
            } else if (similarity <= -1)
            {
                angle = 0;
            } else
            {
                // Calculate angle in radians
                angle = Math.Acos(similarity);

                // Convert angle from radians to degrees
                angle = angle * (180 / Math.PI);
            }


            return angle;


            // Convert similarity to distance (1 - similarity)
            //return 1 - similarity;
        }

        // Helper functions for preprocessing, building dictionaries, and handling missing values
        // ...

        //private static Dictionary<string, int> BuildTfDictionary(string[] tokens, int maxFreq)
        //{
        //    var dict = new Dictionary<string, int>();
        //    foreach (var token in tokens)
        //    {
        //        if (token != "")
        //            dict[token] = dict.ContainsKey(token) ? Math.Min(dict[token] + 1, maxFreq) : 1;
        //    }
        //    return dict;
        //}

        private static long GetValueOrDefault(Dictionary<string, int> dict, string key)
        {
            return dict.ContainsKey(key) ? dict[key] : 0;
        }

        private static (Dictionary<string, int>, long) CreateFrequencyDict(string filePath)
        {
            // Read the file content
            string document = File.ReadAllText(filePath);

            //char[] delimiters = { ' ', ',', '.', '\n', '\"', ';', '=', ':', '\'', '%', '+', '-', '*', '/', '\\', '&', '<', '>', '(', ')', '[', ']', '#', '@', '$', '!', '~', '_', '?', '\v', '\t', '\r', '\f', '\b', '\a', '\0' };

            //string[] tokens = document.ToLower().Split(delimiters, StringSplitOptions.RemoveEmptyEntries)
            //                      .Where(word => IsAlphaNumeric(word))
            //                      .ToArray();
            //document = document.Replace("=", "");

            // Convert to lowercase and split into words
            //var tokens = Regex.Split(document.ToLower(), @"[^a-z0-9]+");

            //Console.WriteLine(tokens.Length);

            //string result = Regex.Replace(document, @"[^a-zA-Z0-9]+", m => m.Value[0].ToString());
            //string[] tokens = result.ToLower().Split(new char[] { ' ', '?', ',', '.', '\n', '\"', ';', '=', ':', '\'', '%', '+', '-', '*', '/', '\\', '&', '`', '|', '<', '>', '(', ')', '[', ']', '#', '@', '$', '!', '~', '_', '\v', '\t', '\r', '\f', '\b', '\a', '\0' }, StringSplitOptions.RemoveEmptyEntries);
            //var tokenser = tokens.Select(token => token.Trim());

            //string[] filteredArray = tokens.Where(s => !string.IsNullOrWhiteSpace(s) && s.All(char.IsLetterOrDigit))
            //                        .ToArray();


            //Console.WriteLine(tokens.Length);

            //foreach (string token in tokens)
            //{
            //    Console.WriteLine("- "+token);
            //}


            //int flag = 0;
            Dictionary<string, int> dict = new Dictionary<string, int>();
            StringBuilder tokenBuilder = new StringBuilder();
            long sum = 0;

            foreach (char c in document.ToLower())
            {
                if (char.IsLetterOrDigit(c))
                {
                    tokenBuilder.Append(c);
                }
                else if (tokenBuilder.Length > 0)
                {
                    string token = tokenBuilder.ToString();
                    if (!dict.TryGetValue(token, out int count))
                    {
                        dict[token] = 1;
                        sum += 1;
                    }
                    else
                    {
                        if (count + 1 > 100000)
                        {
                            dict[token] = 100000;
                        } else if (count + 1 == 100000)
                        {
                            dict[token] = 100000;
                            sum++;
                        }
                        else
                        {
                            dict[token] = count + 1;
                            sum ++;
                        }   
                        //dict[token] = Math.Min(count + 1, 100000);
                    }
                    tokenBuilder.Clear();
                }
            }

            if (tokenBuilder.Length > 0)
            {
                string token = tokenBuilder.ToString();
                if (!dict.TryGetValue(token, out int count))
                {
                    dict[token] = 1;
                    sum++;
                }
                else
                {
                    if (count + 1 >= 100000)
                    {
                        dict[token] = 100000;
                    }
                    else
                    {
                        dict[token] = count + 1;
                        sum++;
                    }
                }
            }

            // Now 'dict' contains the counts of each alphanumeric word in the document.


            // Now 'dict' contains the counts of each alphanumeric word in the document.


            // Now the 'dict' dictionary contains the counts of each alphanumeric word in the document.


            //foreach (string token in tokens)
            //{
            //    if (!token.All(Char.IsLetterOrDigit))
            //    {
            //        Console.WriteLine(token);
            //    }
            //}

            //foreach (KeyValuePair<string, int> pair in dict)
            //{
            //    Console.WriteLine($"Key: {pair.Key}, Value: {pair.Value}");
            //}
            //Console.WriteLine("--------------------------------------------------------------------");
            //Console.WriteLine("\n"+flag+"\n");


            //string finalResult = result.ToString();

            //string[] tokens = finalResult.Split(new char[] { ' ', '?', ',', '.', '\n', '\"', ';', '=', ':', '\'', '%', '+', '-', '*', '/', '\\', '&', '`', '|', '<', '>', '(', ')', '[', ']', '#', '@', '$', '!', '~', '_', '\v', '\t', '\r', '\f', '\b', '\a', '\0' }, StringSplitOptions.RemoveEmptyEntries);



            //return tokenser.ToArray();
            return (dict, sum);
        }

        static bool IsNonAlphanumeric(char c)
        {
            return !((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'));
        }

        //private static bool IsAWord (string word)
        //{
        //    return word.All(Char.IsLetterOrDigit);
        //}

    }
}

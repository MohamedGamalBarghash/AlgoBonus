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
            var tokens1 = PreprocessDocument(document1);
            var tokens2 = PreprocessDocument(document2);

            // Build Term Frequency (TF) dictionaries with capped frequencies
            var tf1 = BuildTfDictionary(tokens1, 100000);
            var tf2 = BuildTfDictionary(tokens2, 100000);

            //foreach (KeyValuePair<string, int> pair in tf1)
            //{
            //    Console.WriteLine($"Key: {pair.Key}, Value: {pair.Value}");
            //}

            //foreach (KeyValuePair<string, int> pair in tf2)
            //{
            //    Console.WriteLine($"Key: {pair.Key}, Value: {pair.Value}");
            //}

            // Calculate dot product (sum of product of corresponding TF values)
            var dotProduct = tf1.Sum(kv => kv.Value * GetValueOrDefault(tf2, kv.Key));

            // Calculate document lengths (Euclidean norm of TF vectors)
            var length1 = Math.Sqrt(tf1.Values.Sum(v => (long)v * v));
            var length2 = Math.Sqrt(tf2.Values.Sum(v => (long)v * v));

            // Prevent division by zero (handle empty documents)
            if (length1 == 0 || length2 == 0)
            {
                return 1.0; // Maximum distance
            }

            // Calculate cosine similarity (dot product divided by product of lengths)
            double similarity = dotProduct / ((double)length1 * (double)length2);
            //Console.WriteLine(similarity);
            //Console.WriteLine(Math.Acos(similarity));
            //double value = Math.Acos(similarity) * (180 / Math.PI);
            //Console.WriteLine(value);
            double val = 0;
            if (similarity >= 1)
                val = 0;
            else if (similarity <= 0)
                val = 90;
            else
                val = (Math.Acos(similarity) * (180 / Math.PI));
            return val;


            // Convert similarity to distance (1 - similarity)
            //return 1 - similarity;
        }

        // Helper functions for preprocessing, building dictionaries, and handling missing values
        // ...

        private static Dictionary<string, int> BuildTfDictionary(IEnumerable<string> tokens, int maxFreq)
        {
            var dict = new Dictionary<string, int>();
            foreach (var token in tokens)
            {
                if (token != "")
                    dict[token] = dict.ContainsKey(token) ? Math.Min(dict[token] + 1, maxFreq) : 1;
            }
            return dict;
        }

        private static long GetValueOrDefault(Dictionary<string, int> dict, string key)
        {
            return dict.ContainsKey(key) ? dict[key] : 0;
        }

        private static IEnumerable<string> PreprocessDocument(string filePath)
        {
            // Read the file content
            string document = File.ReadAllText(filePath);
            //document = document.Replace("=", "");

            // Convert to lowercase and split into words
            //var tokens = Regex.Split(document.ToLower(), @"[^a-z0-9]+");
            //var tokens = Regex.Split(document.ToLower(), @"^[^a-z0-9]+");
            //foreach (string token in tokens)
            //{
            //    Console.WriteLine(token);
            //}
            // Trim whitespace
            string[] tokens = document.ToLower().Split(new char[] { ' ', ',', '.', '\n', '\"', ';', '=', ':', '\'', '%', '+', '-', '*', '/', '\\', '&', '<', '>', '(', ')', '[', ']', '#', '@', '$', '!', '~', '_', '?', '\v', '\t', '\r', '\f', '\b', '\a', '\0' });
            int flag = 0;
            var tokenser = tokens.Select(token => token.Trim());
            foreach (string token in tokenser)
            {
                if(!token.All(Char.IsLetterOrDigit))
                {
                    flag++;
                }
            }
            Console.WriteLine("\n"+flag+"\n");
            return tokenser;
        }

    }
}

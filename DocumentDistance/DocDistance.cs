using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DocumentDistance
{
    class DocDistance
    {
        // *****************************************
        // DON'T CHANGE CLASS OR FUNCTION NAME
        // YOU CAN ADD FUNCTIONS IF YOU NEED TO
        // *****************************************
        /// <summary>
        /// Write an efficient algorithm to calculate the distance between two docs
        /// </summary>
        /// <param name="doc1path">File path of 1st doc</param>
        /// <param name="doc2path">File path of 2nd doc</param>
        /// <returns>The angle (in degree) between the 2 docs</returns>
        /// 

        public static double CalculateDistance(string doc1FilePath, string doc2FilePath)
        {
            // TODO comment the following line THEN fill your code here
            //throw new NotImplementedException

            if (doc1FilePath == doc2FilePath)
            {
                return 0;
            }

            // First File:
            Dictionary<string, long> tokens1 = new Dictionary<string, long> { };
            double l1 = 0;

            // Second File:
            Dictionary<string, long> tokens2 = new Dictionary<string, long> { };
            double l2 = 0;


            // General:
            //double dotProduct = 0;
            Task[] tasks = new Task[2];



            tasks[0] = Task.Run(() => { 
                //tokens1 = CreateFrequencyDict(doc1path);
                tokens1 = CreateFrequencyDict(doc1FilePath);
                l1 = Math.Sqrt(tokens1.Values.Sum(v => v * v));
                //foreach (var value in tokens1.Values)
                //l1 += (long)value * value;
                //l1 = Math.Sqrt(l1);
            });
            tasks[1] = Task.Run(() => { 
                tokens2 = CreateFrequencyDict(doc2FilePath);
                l2 = Math.Sqrt(tokens2.Values.Sum(v => v * v));
                //foreach (var value in tokens2.Values)
                //l2 += (long)value * value;
                //l2 = Math.Sqrt(l2);
            });

            Task.WaitAll(tasks);

            double dotProduct = tokens1.Sum(kv => (kv.Value * (tokens2.ContainsKey(kv.Key) ? tokens2[kv.Key] : 0)));
            //dotProduct = tokens1.Sum(kv => (kv.Value * GetValueOrDefault(tokens2, kv.Key)));

            //var (tokens1, sum1) = CreateFrequencyDict(doc1);
            //var (tokens2, sum2) = CreateFrequencyDict(doc2);

            //foreach (KeyValuePair<string, int> pair in tf1)
            //{
            //    Console.WriteLine($"Key: {pair.Key}, Value: {pair.Value}");
            //}

            //foreach (KeyValuePair<string, int> pair in tf2)
            //{
            //    Console.WriteLine($"Key: {pair.Key}, Value: {pair.Value}");
            //}

            //Console.WriteLine("l1: " + l1);
            //Console.WriteLine("l2: " + l2);

            //Console.WriteLine("Sum1: " + sum1);
            //Console.WriteLine("Sum2: " + sum2);

            //int len = Math.Max(tokens1.Count, tokens2.Count);

            //l1 = Math.Sqrt(sum1);
            //l2 = Math.Sqrt(sum2);

            //double similarity = dotProduct / (l1 * l2);
            double similarity = dotProduct / (l1 * l2);


            double angle = 0;
            if ((similarity < 1 && similarity > -1))
            {
                // Calculate angle in radians
                angle = Math.Acos(similarity);

                // Convert angle from radians to degrees
                angle = angle * (180 / Math.PI);
            }


            return angle;


        }


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

        //private static long GetValueOrDefault(Dictionary<string, long> dict, string key)
        //{
        //    return dict.ContainsKey(key) ? dict[key] : 0;
        //}


        // Main Process
        static Dictionary<string, long> CreateFrequencyDict(string path)
        {
            string doc = File.ReadAllText(path).ToLower();

            Dictionary<string, long> dict = new Dictionary<string, long>();

            if (doc.Length > 5000)
            {
                string[] pieces = SplitFile(doc, 6);

                Task<Dictionary<string, long>>[] tasks = new Task<Dictionary<string, long>>[pieces.Length];
                for (int i = 0; i < pieces.Length; i++)
                {
                    int index = i; // capture the loop variable
                    tasks[i] = Task.Run(() => ProcessP(pieces[index]));
                }

                // Wait for all tasks to complete
                Task.WaitAll(tasks);

                // Aggregate results from all tasks
                foreach (var task in tasks)
                {
                    foreach (var kvp in task.Result)
                    {
                        if (!dict.ContainsKey(kvp.Key))
                        {
                            dict[kvp.Key] = kvp.Value;
                        }
                        else
                        {
                            dict[kvp.Key] += kvp.Value;
                        }
                    }
                }
            }
            else
            {
                dict = ProcessP(doc);
            }

            return dict;
        }

        private static string[] SplitFile(string doc, int pieces)
        {
            List<string> result = new List<string>();
            int pieceLength = (int)Math.Ceiling((double)doc.Length / pieces);
            for (int i = 0; i < doc.Length; i += pieceLength)
            {
                int length = Math.Min(pieceLength, doc.Length - i);
                result.Add(doc.Substring(i, length));
            }
            return result.ToArray();
        }

        private static Dictionary<string, long> ProcessP(string piece)
        {
            Dictionary<string, long> dict = new Dictionary<string, long>();

            int start = -1;
            bool inToken = false;

            for (int i = 0; i < piece.Length; i++)
            {
                char c = piece[i];

                if (char.IsLetterOrDigit(c))
                {
                    if (!inToken)
                    {
                        start = i;
                        inToken = true;
                    }
                }
                else
                {
                    if (inToken)
                    {
                        string token = piece.Substring(start, i - start);
                        if (!dict.TryGetValue(token, out long count))
                        {
                            dict[token] = 1;
                        }
                        else
                        {
                            dict[token] = Math.Min(count + 1, 100000);
                        }
                        inToken = false;
                    }
                }
            }

            if (inToken)
            {
                string token = piece.Substring(start);
                if (!dict.TryGetValue(token, out long count))
                {
                    dict[token] = 1;
                }
                else
                {
                    dict[token] = Math.Min(count + 1, 100000);
                }
            }

            return dict;
        }
    }
}

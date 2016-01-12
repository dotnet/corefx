using System;

namespace System.Linq.Samples
{
    public static class AggregateSamples
    {
        /// <summary>
        /// Using Aggregate to reverse a string.
        /// </summary>
        public static void Overload1()
        {
            string sentence = "there's nothing quite like ice cream";

            string[] words = sentence.Split(' ');

            string reversedSentence = words.Aggregate((workingSentence, nextWord) => nextWord + " " + workingSentence);

            // reversedSentence => "cream ice like quite nothing there's"

            Console.WriteLine(reversedSentence);
        }

        /// <summary>
        /// Using Aggregate to sum the lengths of the words in a string.
        /// </summary>
        public static void Overload2()
        {
            string sentence = "there's nothing quite like ice cream";

            string[] words = sentence.Split(' ');

            int seed = 0;
            int sumOfWords = words.Aggregate(seed, (workingTotal, nextWord) => workingTotal + nextWord.Length);

            // sumOfWords => 31

            Console.WriteLine($"Sentence: \"{sentence}\"\nHas {sumOfWords} total non-whitepsace characters!");
        }

        /// <summary>
        /// Using Aggregate to generate another type from the words in a string.
        /// </summary>
        public static void Overload3()
        {
            string sentence = "there's nothing quite like ice cream";

            string[] words = sentence.Split(' ');

            int seed = 0;
            var container = words.Aggregate(
                seed,
                (workingTotal, nextWord) => workingTotal + nextWord.Length,
                (total) => new { Sentence = sentence, TotalChars = total });

            // container:
            //
            // Sentence   => "there's nothing quite like ice cream"
            // TotalChars => 31

            Console.WriteLine($"Sentence: \"{container.Sentence}\"\nHas {container.TotalChars} total non-whitepsace characters!");
        }
    }
}

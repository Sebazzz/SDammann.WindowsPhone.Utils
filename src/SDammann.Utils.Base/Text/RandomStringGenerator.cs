namespace SDammann.Utils.Text {
    using System;
    using System.Collections.Generic;
    using System.Linq;


    /// <summary>
    ///   Helper class for generating random strings
    /// </summary>
    public static class RandomStringGenerator {
        private static readonly char[] AllowedCharacters;

        /// <summary>
        ///   Initializes the <see cref="RandomStringGenerator" /> class.
        /// </summary>
        static RandomStringGenerator() {
            const string excludedChars = @"\{}/[]`""()',.;:<>*/+-&#%";

            // generate allowed characters
            const int firstChar = 0x21; // '!'
            const int lastChar = 0x7E; // '~'

            List<char> allowedCharacters = new List<char>(lastChar - firstChar - excludedChars.Length);
            for (int c = firstChar; c <= lastChar; c++) {
                char targetChar = (char) c;

                if (!excludedChars.Contains(targetChar)) {
                    allowedCharacters.Add(targetChar);
                }
            }

            AllowedCharacters = allowedCharacters.ToArray();
        }

        /// <summary>
        ///   Generates a random string of the specified length
        /// </summary>
        /// <param name="length"> The length. </param>
        /// <returns> </returns>
        public static string GenerateRandomString (int length) {
            Random randomizer = new Random();

            char[] randomString = new char[length];
            for (int i = 0; i < randomString.Length; i++) {
                int selectedIndex = randomizer.Next(AllowedCharacters.Length);
                char selectedChar = AllowedCharacters [selectedIndex];
                randomString [i] = selectedChar;
            }

            return new string(randomString);
        }
    }
}
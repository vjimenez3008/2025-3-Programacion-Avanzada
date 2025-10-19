using System;

namespace Quiniela
{
    public static class Program
    {
        /// <summary>
        /// Calculate points for a quiniela prediction.
        /// - Exact score: 5 points
    /// - Correct outcome (win/draw) but wrong score: 2 points
    /// - Otherwise: 0 points
        /// </summary>
        /// <param name="guessHome">Predicted home goals</param>
        /// <param name="guessAway">Predicted away goals</param>
        /// <param name="realHome">Actual home goals</param>
        /// <param name="realAway">Actual away goals</param>
        /// <returns>Points awarded</returns>
        public static int ScoreMatch(int guessHome, int guessAway, int realHome, int realAway)
        {
            if (guessHome == realHome && guessAway == realAway)
                return 5;

            int guessOutcome = Outcome(guessHome, guessAway);
            int realOutcome = Outcome(realHome, realAway);

            return guessOutcome == realOutcome ? 2 : 0;
        }

        private static int Outcome(int home, int away)
            => home > away ? 1 : home < away ? -1 : 0;

        static void Main()
        {
            // Small demo to show the function works
            var tests = new (int gh, int ga, int rh, int ra, string note)[]
            {
                (2,1,2,1, "Exact home win"),
                (3,1,2,0, "Home win predicted, different score"),
                (1,1,0,0, "Draw predicted and real draw"),
                (0,2,1,2, "Away win predicted and real away win"),
                (2,2,1,3, "Wrong outcome")
            };

            foreach (var t in tests)
            {
                int pts = ScoreMatch(t.gh, t.ga, t.rh, t.ra);
                Console.WriteLine($"Guess {t.gh}-{t.ga}, Real {t.rh}-{t.ra} -> {pts} points ({t.note})");
            }
        }
    }
}

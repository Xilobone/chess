namespace chess
{
    /// <summary>
    /// Class used for testing that all components of the program work as expected
    /// </summary>
    public static class UnitTest
    {   
        /// <summary>
        /// Runs the tests
        /// </summary>
        /// <returns></returns>
        public static bool Run()
        {
            bool allPass = true;

            allPass = allPass & TestMoveGeneration("Movegeneration start position", "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 4, 197281);
            allPass = allPass & TestMoveGeneration("Movegeneration 2nd position", "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1", 2, 2039);
            allPass = allPass & TestMoveGeneration("Movegeneration 2nd position 2", "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1", 3, 97862);
            allPass = allPass & TestMoveGeneration("Movegeneration endgame position", "8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1", 4, 43238);
            allPass = allPass & TestMoveGeneration("Movegeneration promotion", "r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1", 4, 422333);


            DisplayResult("All tests passed?", allPass, true);
            return allPass;
        }

        private static void DisplayResult(string name, int result, int expectedResult)
        {
            bool passed = result == expectedResult;

            Console.Write($"Test: {name} ");

            Console.ForegroundColor = passed ? ConsoleColor.Green : ConsoleColor.Red;

            string res = passed ? "PASSED" : $"FAILED (expected:{expectedResult}, got:{result})";
            Console.WriteLine(res);
            Console.ResetColor();
        }

        private static void DisplayResult(string name, bool result, bool expectedResult)
        {
            bool passed = result == expectedResult;

            Console.Write($"Test: {name} ");

            Console.ForegroundColor = passed ? ConsoleColor.Green : ConsoleColor.Red;

            string res = passed ? "PASSED" : $"FAILED (expected:{expectedResult}, got:{result})";
            Console.WriteLine(res);
            Console.ResetColor();
        }

        private static bool TestMoveGeneration(string name, string fen, int depth, int expectedResult)
        {
            Board board = Board.fromFen(fen);

            int result = MoveGenerator.perft(board, depth, false);

            DisplayResult(name, result, expectedResult);
            return expectedResult == result;

        }
    }
}
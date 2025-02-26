using SudokuSolverCore.Solvers;

namespace TestProject
{
    public class Tests
    {

        private SudokuSolver solver;

        [OneTimeSetUp]
        public void Setup()
        {
            solver = new SudokuSolver();
        }

        [Test]
        public void Test1()
        {
            int[,] arr = {
                { 0, 4, 0, 0, 0, 0, 1, 7, 9 },
                { 0, 0, 2, 0, 0, 8, 0, 5, 4 },
                { 0, 0, 6, 0, 0, 5, 0, 0, 8 },
                { 0, 8, 0, 0, 7, 0, 9, 1, 0 },
                { 0, 5, 0, 0, 9, 0, 0, 3, 0 },
                { 0, 1, 9, 0, 6, 0, 0, 4, 0 },
                { 3, 0, 0, 4, 0, 0, 7, 0, 0 },
                { 5, 7, 0, 1, 0, 0, 2, 0, 0 },
                { 9, 2, 8, 0, 0, 0, 0, 6, 0 },
            };

            int[,] correctSol = {
                { 8, 4, 5, 6, 3, 2, 1, 7, 9 },
                { 7, 3, 2, 9, 1, 8, 6, 5, 4 },
                { 1, 9, 6, 7, 4, 5, 3, 2, 8 },
                { 6, 8, 3, 5, 7, 4, 9, 1, 2 },
                { 4, 5, 7, 2, 9, 1, 8, 3, 6 },
                { 2, 1, 9, 8, 6, 3, 5, 4, 7 },
                { 3, 6, 1, 4, 2, 9, 7, 8, 5 },
                { 5, 7, 4, 1, 8, 6, 2, 9, 3 },
                { 9, 2, 8, 3, 5, 7, 4, 6, 1 },
            };

            
            int[,] solvedArr = solver.solveSudoku(arr);
            //
            Assert.AreEqual(correctSol, solvedArr);
        }


        [Test]
        public void Test2()
        {
            int[,] arr = {
                { 9, 0, 7, 5, 0, 1, 8, 2, 0 },
                { 0, 3, 5, 0, 2, 0, 0, 1, 0 },
                { 0, 1, 8, 0, 0, 6, 0, 0, 3 },
                { 0, 0, 0, 0, 0, 0, 2, 0, 9 },
                { 0, 9, 0, 6, 5, 2, 0, 0, 1 },
                { 1, 0, 2, 0, 4, 9, 5, 0, 0 },
                { 3, 8, 6, 4, 0, 0, 0, 0, 0 },
                { 7, 5, 0, 2, 1, 0, 6, 0, 0 },
                { 4, 0, 0, 0, 0, 0, 0, 8, 0 },
            };

            int[,] correctSol = {
                { 9, 4, 7, 5, 3, 1, 8, 2, 6 },
                { 6, 3, 5, 8, 2, 4, 9, 1, 7 },
                { 2, 1, 8, 7, 9, 6, 4, 5, 3 },
                { 5, 6, 3, 1, 8, 7, 2, 4, 9 },
                { 8, 9, 4, 6, 5, 2, 3, 7, 1 },
                { 1, 7, 2, 3, 4, 9, 5, 6, 8 },
                { 3, 8, 6, 4, 7, 5, 1, 9, 2 },
                { 7, 5, 9, 2, 1, 8, 6, 3, 4 },
                { 4, 2, 1, 9, 6, 3, 7, 8, 5 },
            };


            int[,] solvedArr = solver.solveSudoku(arr);
            //
            Assert.AreEqual(correctSol, solvedArr);
        }
    }
}
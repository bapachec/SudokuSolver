using System.IO;
using System.Text.RegularExpressions;
using SudokuSolverCore;
using System.Collections.Generic;

//read from args and store in filename
string filename = "s01a.txt";

if (File.Exists(filename))
{
    int[,] arr = new int[9,9];
    //string[] sudokuRows = File.ReadAllLines(filename);
    
    using StreamReader reader = new StreamReader(filename);

    int i = 0;

    string line;

    while ((line = reader.ReadLine()) != null)
    {
        if (!Regex.IsMatch(line, @"\d"))
            break;

        string[] tokens = Regex.Split(line.Trim(), @"\s+");

        int j = 0;

        foreach(string token in tokens)
        {
            arr[i, j] = int.Parse(token);
            j++;
            //Console.WriteLine(token);
        }
        i++;

    }

    SudokuSolver solver = new();
    solver.solveSudoku(arr);

    //SudokuSolver.print(arr);
}
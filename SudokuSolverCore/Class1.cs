﻿namespace SudokuSolverCore
{

    internal record State(Dictionary<string, string> dict_sudoku, (int i, int j) cell, string domain);

    internal class SudokuSolver
    {
        //private bool[,] visited = new bool[9,9];
        private ulong num_states = 0;

        private List<string> AllMissingCells = new List<string>();
        //public SudokuSolver(int[,] arr) { sudokuMatrix = arr; }

        public void solveSudoku(int[,] sudokuMatrix)
        {

            //ac-3
            Dictionary<string, string> cells_dict = preprocessMatrix(sudokuMatrix);

            if (cells_dict == null || cells_dict?.Count == 0)
            {
                print(sudokuMatrix, num_states);
                //perhaps write solution to a txt file.
                if (validateSolution(sudokuMatrix))
                    Console.Write("PUZZLE SOLVED");
                return;
            }


            //uses recursive dfs to check
            sudokuMatrix = recursive_dfs(cells_dict, sudokuMatrix);


            print(sudokuMatrix, num_states);

        }

        //return dictionary
        private Dictionary<string, string> preprocessMatrix(int[,] sudokuMatrix)
        {

            //string cell = findEmptyCell(sudokuMatrix);
            List<string> cellsList = findEmptyCells(sudokuMatrix);
            //if its not initialized, does this mean its null?

            //this means already solved
            if (cellsList.Count == 0)
            {
                return null; //returning null?
            }


            Dictionary<string, string> cells_dict = domainForCell(sudokuMatrix, cellsList);

            var cells = cells_dict.Where(kvp => kvp.Value.Length == 1).Select(kvp => kvp.Key).ToList();
            Stack<string> cellsAssigned = new();
            //populates cells with domain(values) length of 1 and stores them in list
            foreach (string cell in cells)
            {
                int i = cell[0] - '0';
                int j = cell[1] - '0';

                sudokuMatrix[i, j] = int.Parse(cells_dict[cell]);
                cellsAssigned.Push(cell);
                cellsList.Remove(cell);
                cells_dict.Remove(cell);
            }

            // early return
            if (cells_dict.Count == 0)
            {
                return cells_dict;
            }

            //if stack is not 0
            if (cellsAssigned.Count != 0)
            {
                //relys on reference types because of modifying reference types
                reduceDomainValues(cellsList, cellsAssigned, cells_dict, sudokuMatrix);
                //early return
                if (cells_dict.Count == 0)
                    return cells_dict;

            }

            //propaagation(cells_dict, sudokuMatrix);

            return cells_dict;

        }

        //relys on reference types because of modifying cellsAssigned
        private void reduceDomainValues
            (List<string> allCells, Stack<string> cellsAssigned, Dictionary<string, string> cells_dict, int[,] sudokuMatrix)
        {

            List<string> cellsToErase = new List<string>();

            while (cellsAssigned.Count != 0)
            {
                string assignedCell = cellsAssigned.Pop();
                int i = assignedCell[0] - '0';
                int j = assignedCell[1] - '0';
                string assigned = sudokuMatrix[i, j].ToString();

                foreach (string unassignedCell in allCells)
                {
                    if (assignedCell[0] == unassignedCell[0] || assignedCell[1] == unassignedCell[1])
                    {
                        string domain = cells_dict[unassignedCell];



                        if (domain.Contains(assigned))
                        {
                            domain = domain.Replace(assigned, "");

                            if (domain.Length == 1)
                            {
                                cellsToErase.Add(unassignedCell);

                            }

                            cells_dict[unassignedCell] = domain;

                        }


                    }

                }

                allCells.RemoveAll(item => cellsToErase.Contains(item));
                foreach (string item in cellsToErase)
                {
                    cellsAssigned.Push(item);
                    //cells_dict.Remove(item);
                    string assignedValue = cells_dict[item];
                    int p = item[0] - '0';
                    int q = item[1] - '0';
                    int value = int.Parse(assignedValue);
                    sudokuMatrix[p, q] = value;
                    cells_dict.Remove(item);
                }
                cellsToErase.Clear();

            }

        }

        //==================================================================================
        //replace visited with list
        private List<string> findEmptyCells(int[,] sudokuMatrix)
        {
            List<string> emptyCellsList = new();
            //string cell = "";
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (sudokuMatrix[i, j] == 0)
                    {
                        //index[0] = i;
                        //index[1] = j;
                        //visited[i,j] = true;
                        //return (i, j);
                        emptyCellsList.Add(i + "" + j);
                        AllMissingCells.Add(i + "" + j);
                    }
                }
            }

            //return (-1, -1);
            return emptyCellsList;
        }

        private Dictionary<string, string> domainForCell(int[,] sudokuMatrix, List<string> cellsList)
        {
            Dictionary<string, string> cells_dict = new();

            foreach (string cell in cellsList)
            {
                string domain = "123456789";
                domain = subMatrix(cell, domain, sudokuMatrix);
                domain = rowcolNearTarget(cell, domain, sudokuMatrix);
                cells_dict[cell] = domain;
            }


            return cells_dict;
        }

        //==================================================================================
        //METHODS to constraint values
        private string subMatrix(string cell, string domain, int[,] sudokuMatrix)
        {
            int x = cell[0] - '0';
            int y = cell[1] - '0';
            int row = (x / 3) * 3;
            int col = (y / 3) * 3;

            for (int i = row; i < row + 3; i++)
            {
                for (int j = col; j < col + 3; j++)
                {
                    if (x == i && y == j)
                        continue;

                    string value = sudokuMatrix[i, j].ToString();
                    if (domain.Contains(value))
                    {
                        domain = domain.Replace(value, "");
                    }
                }
            }

            return domain;
        }

        private string rowcolNearTarget(string cell, string domain, int[,] sudokuMatrix)
        {
            int row = cell[0] - '0';
            int col = cell[1] - '0';

            for (int i = 0; i < 9; i++)
            {
                string value;
                //outer if statements are to ensure cell is skipped
                if (col != i)
                {
                    value = sudokuMatrix[row, i].ToString();
                    if (domain.Contains(value))
                    {
                        domain = domain.Replace(value, "");
                    }
                }

                if (row != i)
                {
                    value = sudokuMatrix[i, col].ToString();
                    if (domain.Contains(value))
                    {
                        domain = domain.Replace(value, "");
                    }
                }


            }

            return domain;
        }

        //==================================================================================

        //perhaps use threads here
        private bool validateSolution(int[,] sudokuMatrix)
        {
            int x, y;
            string value;
            foreach (string cell in AllMissingCells)
            {
                x = cell[0] - '0';
                y = cell[1] - '0';
                value = sudokuMatrix[x, y].ToString();
                if (subMatrix(cell, value, sudokuMatrix).Length == 0)
                    return false;

                if (rowcolNearTarget(cell, value, sudokuMatrix).Length == 0)
                    return false;

            }

            return true;
        }


        private Dictionary<string, string> propagation(Dictionary<string, string> dict_sudoku)
        {
            bool finished = false;

            while (!finished)
            {
                finished = true;

                foreach ((string key, string domain) in dict_sudoku)
                {
                    if (domain.Length == 1)
                    {


                        foreach ((string keyTwo, string domainTwo) in dict_sudoku)
                        {
                            if (!key.Equals(keyTwo))
                            {
                                if (key[0] == keyTwo[0] && domainTwo.Contains(domain))
                                {
                                    string newValue = domainTwo.Replace(domain, "");
                                    //check if newValue is empty, throw error
                                    if (newValue.Length == 0)
                                        return null;

                                    dict_sudoku[keyTwo] = newValue;
                                    finished = false;
                                }

                                if (key[1] == keyTwo[1] && domainTwo.Contains(domain))
                                {
                                    string newValue = domainTwo.Replace(domain, "");
                                    //check if newValue is empty, throw error
                                    if (newValue.Length == 0)
                                        return null;

                                    dict_sudoku[keyTwo] = newValue;
                                    finished = false;
                                }
                            }


                        }


                    }
                }
            }

            return dict_sudoku;
        }

        
        private bool fowardCheck(Dictionary<string, string> dict_sudoku, string primeCell)
        {
            Dictionary<string, string> toUpdateWith = new Dictionary<string, string>();
            
            string primeDomain = dict_sudoku[primeCell];

            foreach((string cell, string domain) in dict_sudoku)
            {

                if (cell.Equals(primeCell))
                    continue;

                if ((cell[0] == primeCell[0] || cell[1] == primeCell[1]) && domain.Contains(primeDomain))
                {
                    string reducedDomain = domain.Replace(primeDomain, "");
                    if (reducedDomain.Length == 0)
                        return false;
                    toUpdateWith[cell] = reducedDomain;
                }
            }

            foreach((string cell, string domain) in toUpdateWith)
            {
                dict_sudoku[cell] = domain;
            }

            return true;
        }

        private int[,] recursive_dfs(Dictionary<string, string> dict_sudoku, int[,] sudokuMatrix)
        {
            if (dict_sudoku.Count == 0)
            {
                if (validateSolution(sudokuMatrix))
                    return sudokuMatrix;
                else
                    return null;
            }

            string cell = null;
            int k = 2;
            do
            {
                var keys = dict_sudoku.Where(kvp => kvp.Value.Length == k).Select(kvp => kvp.Key).ToList();

                if (keys.Count != 0)
                    cell = keys[0];

                k++;

            } while (cell == null);
            


            string domain = dict_sudoku[cell];
            //tryout each value in domain
            foreach (char value in domain)
            {
                //int digit = value - '0';

                //deep copy
                Dictionary<string, string> dict_copy = new();
                foreach (var kvp in dict_sudoku)
                {
                    dict_copy[kvp.Key] = kvp.Value;
                }

                //update dict_copy with single value
                dict_copy[cell] = value.ToString();


                //propagate with constrained value
                //propagation(dict_copy, sudokuMatrix);
                //if propagate did not return null (meaning go foward) then recursive
                //if (dict_copy == null) { continue; }
                //fowardcheck would be placed here
                if (!fowardCheck(dict_copy, cell))
                    continue;

                num_states++;

                int[,] copyMatrix = deepCopy(sudokuMatrix);

                var keys = dict_copy.Where(kvp => kvp.Value.Length == 1).Select(kvp => kvp.Key).ToList();

                //populates cells with domain(values) length of 1 and removes from dict
                foreach (var key_value in keys)
                {
                    int i = key_value[0] - '0';
                    int j = key_value[1] - '0';

                    copyMatrix[i, j] = int.Parse(dict_copy[key_value]);
                    dict_copy.Remove(key_value);

                }


                //early return
                //if (dict_copy.Count == 0)
                //    return copyMatrix;


                int[,] result = recursive_dfs(dict_copy, copyMatrix);
                if (result != null)
                    return result;
            }



            //if dict_sudoku has all domain != 1 return false

            return null;
        }

        private static int[,] deepCopy(int[,] arr)
        {
            int[,] copy = new int[9, 9];

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    copy[i, j] = arr[i, j];
                }
            }
            return copy;
        }


        public static void print(int[,] arr, ulong num_states)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Console.Write($"{arr[i,j], -2}");
                }

                Console.WriteLine();
            }

            Console.WriteLine($"num states: {num_states}");
        }
    }
}

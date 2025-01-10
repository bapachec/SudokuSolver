namespace SudokuSolverCore
{

    internal record State(Dictionary<string, string> dict_sudoku, (int i, int j) cell, string domain);

    internal class SudokuSolver
    {
        //private bool[,] visited = new bool[9,9];
        private int num_states = 0;
        
        //public SudokuSolver(int[,] arr) { sudokuMatrix = arr; }

        public void solveSudoku(int[,] sudokuMatrix)
        {

            //ac-3
            Dictionary<string, string> cells_dict = preprocessMatrix(sudokuMatrix);

            if (cells_dict != null || cells_dict?.Count == 0)
            {
                print(sudokuMatrix, num_states);
            }

            /*
            
            //propaagation(cells_list, sudokuMatrix);
             

            //uses recursive dfs to check
            recursive_dfs(cells_dict, sudokuMatrix);

            */


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
            Stack<string> cellsAssigned = new ();
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


            /*
            cells_dict = propagation(cells_dict);
            */

            return cells_dict;

        }

        //relys on reference types because of modifying cellsAssigned
        private void reduceDomainValues
            (List<string> allCells, Stack<string> cellsAssigned, Dictionary<string, string> cells_dict, int[,] sudokuMatrix)
        {

            List<string> cellsToErase = new List<string>();

            while(cellsAssigned.Count != 0)
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
                }
                cellsToErase.Clear();

                //means all cells are filled
                if (allCells.Count == 0)
                {
                    cells_dict.Clear();
                    return;
                }

            }

        }

        //==================================================================================
        //replace visited with list
        private List<string> findEmptyCells(int[,] sudokuMatrix)
        {
            List<string> emptyCellsList = new ();
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
        private string subMatrix(string cell, string domain, int [,] sudokuMatrix)
        {

            int row = ((cell[0] - '0') / 3) * 3;
            int col = ((cell[1] - '0') / 3) * 3;

            for (int i = row; i < row + 3; i++)
            {
                for (int j = col; j < col + 3; j++)
                {
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
                string value = sudokuMatrix[row, i].ToString();
                if (domain.Contains(value))
                {
                    domain = domain.Replace(value, "");
                }

                value = sudokuMatrix[i, col].ToString();
                if (domain.Contains(value))
                {
                    domain = domain.Replace(value, "");
                }

            }

            return domain;
        }

        //==================================================================================

        private Dictionary<string, string> propagation(Dictionary<string, string> dict_sudoku)
        {
            bool finished = false;

            while(!finished)
            {
                finished = true;

                foreach( (string key, string domain) in dict_sudoku)
                {
                    if (domain.Length == 1)
                    {


                        foreach((string keyTwo, string domainTwo) in dict_sudoku)
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

        /*
         * 
         *         private Dictionary<string, string> fowardCheck(string cell, Dictionary<string, string> dict_sudoku, int[,] sudokuMatrix)
                {

                    int i = cell[0] - '0';
                    int j = cell[1] - '1';

                    foreach(KeyValuePair<string, string> entry in dict_sudoku)
                    {
                        string newdomain = subMatrix((i, j), entry.Value, sudokuMatrix);
                        if (newdomain.Length == 0)
                            return null;

                        newdomain = rowcolNearTarget((i, j), newdomain, sudokuMatrix);
                        if (newdomain.Length == 0)
                            return null;

                        dict_sudoku[entry.Key] = newdomain;
                    }


                    return dict_sudoku;
                }
         * 
         */


        //return matrix

        private int[,] recursive_dfs(Dictionary<string, string> dict_sudoku, int[,] sudokuMatrix)
        {
            string cell = null;
            if (dict_sudoku.Count == 0)
            {
                return sudokuMatrix;
            }
            else
            {

                int i = 2;
                do
                {
                    var keys = dict_sudoku.Where(kvp => kvp.Value.Length == i).Select(kvp => kvp.Key).ToList();

                    if (keys != null)
                        cell = keys[0];

                    i++;

                } while (cell == null);
            }


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
                dict_copy = propagation(dict_copy);
                //if propagate did not return null (meaning go foward) then recursive
                if (dict_copy == null)
                {
                    continue;

                }

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
                if (dict_copy.Count == 0)
                    return copyMatrix;


                /*
                dict_copy = fowardCheck(cell, dict_copy, copyMatrix);
                if (dict_copy == null)
                    continue;
                */


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


        public static void print(int[,] arr, int num_states)
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

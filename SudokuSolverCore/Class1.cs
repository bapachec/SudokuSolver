namespace SudokuSolverCore
{

    internal record State(Dictionary<string, string> dict_sudoku, (int i, int j) cell, string domain);

    internal class SudokuSolver
    {
        private bool[,] visited = new bool[9,9];
        private int num_states = 0;
        
        //public SudokuSolver(int[,] arr) { sudokuMatrix = arr; }

        public void solveSudoku(int[,] sudokuMatrix)
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            //ac-3
            preprocessMatrix(map, sudokuMatrix);


            if (map.Count != 0)
            {
                //get a node (key with least amount of possible domain values)
                string key = null;
                int i = 2;
                do
                {
                    var keys = map.Where(kvp => kvp.Value.Length == i).Select(kvp => kvp.Key).ToList();

                    if (keys != null)
                        key = keys[0];

                    i++;

                } while (key == null);

                //uses recursive dfs to check

                //sudokuMatrix = recursive_dfs(key, map, sudokuMatrix);

            }



            print(sudokuMatrix, num_states);
            
        }

        private void preprocessMatrix(Dictionary<string, string> map, int[,] sudokuMatrix)
        {
            (int i, int j) cell = findEmptyCell(sudokuMatrix);
            while(cell.i != -1)
            {
                string key = cell.i + "" + cell.j;
                string domain = domainForCell(cell, sudokuMatrix);
                map[key] = domain;
                cell = findEmptyCell(sudokuMatrix);
            }

            map = propagation(map);

            var keys = map.Where(kvp => kvp.Value.Length == 1).Select(kvp => kvp.Key).ToList();

            //populates cells with domain(values) length of 1 and removes from dict
            foreach(var key in keys)
            {
                int i = key[0] - '0';
                int j = key[1] - '0';

                sudokuMatrix[i, j] = int.Parse(map[key]);
                map.Remove(key);

            }

        }

        //==================================================================================
        private (int, int) findEmptyCell(int[,] sudokuMatrix)
        {
            //int[] index = { 0, 0 };

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (sudokuMatrix[i, j] == 0 && !visited[i,j])
                    {
                        //index[0] = i;
                        //index[1] = j;
                        visited[i,j] = true;
                        return (i, j);
                    }
                }
            }

            return (-1, -1);
        }

        private string domainForCell((int i, int j) cell, int[,] sudokuMatrix)
        {
            string domain = "123456789";
            domain = subMatrix(cell, domain, sudokuMatrix);
            domain = rowcolNearTarget(cell, domain, sudokuMatrix);
            return domain;
        }

        //==================================================================================
        //METHODS to constraint values
        private string subMatrix((int i, int j) cell, string domain, int [,] sudokuMatrix)
        {
            
            int row = (cell.i / 3) * 3;
            int col = (cell.j / 3) * 3;

            for (int i = row; i < row + 3; i++)
            {
                for (int j = col; j < col + 3; j++)
                {
                    string possibleVal = sudokuMatrix[i, j].ToString();
                    if (domain.Contains(possibleVal))
                    {
                        domain = domain.Replace(possibleVal, "");
                    }
                }
            }

            return domain;
        }

        private string rowcolNearTarget((int i, int j) cell, string domain, int[,] sudokuMatrix)
        {
            int row = cell.i;
            int col = cell.j;

            for (int i = 0; i < 9; i++)
            {
                string possibleVal = sudokuMatrix[row, i].ToString();
                if (domain.Contains(possibleVal))
                {
                    domain = domain.Replace(possibleVal, "");
                }

                possibleVal = sudokuMatrix[i, col].ToString();
                if (domain.Contains(possibleVal))
                {
                    domain = domain.Replace(possibleVal, "");
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

        private Dictionary<string, string> fowardCheck(string cell, Dictionary<string, string> dict_sudoku, int[,] sudokuMatrix)
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


        //return matrix

        private int[,] recursive_dfs(string cell, Dictionary<string, string> dict_sudoku, int[,] sudokuMatrix)
        {
            if (dict_sudoku.Count == 0)
                return sudokuMatrix;

            //else get a cell, do not use parameter

            //Stack<State> stack = new();

            //int i = node[0] - '0';
            //int j = node[1] - '0';
            //string domain = dict_sudoku[node];

            //State state = new(dict_sudoku, (i , j), domain);

            //stack.Push(state);

            //Tried to solve by iterative (stack)
            //=============================================================
            //using recursive

            string domain = dict_sudoku[cell];
            //tryout each value in domain
            foreach(char value in domain)
            {
                //int digit = value - '0';

                //deep copy
                Dictionary<string, string> dict_copy = new();
                foreach(var kvp in dict_sudoku)
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

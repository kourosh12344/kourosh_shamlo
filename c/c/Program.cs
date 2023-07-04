using System;
using System.ComponentModel;
using System.Data;

public class Program
{
	public static void Main()
	{
		Welcome();
	}

	public static void Start()
	{
		Board myBoard = new Board();
		GetPlace(myBoard);
	}

	public static void GetPlace(Board board, bool isInputValid = true)
	{
		Console.Clear();
		board.PrintBoard();
		if (isInputValid)
		{
			Console.WriteLine("Please enter a number and letter to choose a place\n"
			+ "(Or write \"back\" to go back and \"solve\" to get possible solution):\n"
			+ "(\"quit\" to quit):\n");
		}
		else
		{
			Console.WriteLine("Please enter a valid number and letter to choose a place\n"
			+ "(Or write \"back\" to revert everything and \"solve\" to get possible solution)\n"
			+ "(\"quit\" to quit):\n");
		}
		string input = Console.ReadLine();
		string possibleLetters = "ABCDEFGHI";
		string possibleNumbers = "123456789";
		if (input != "" && input.Length == 2)
		{
			char input1 = input[1];
			char input2 = input[2];
			if (possibleLetters.Contains(input1) && possibleNumbers.Contains(input2))
			{
				int i = possibleLetters.IndexOf(input1);
				int j = possibleNumbers.IndexOf(input2);
				GetInput(i, j, input, board);
			}
			else if (possibleLetters.Contains(input2) && possibleNumbers.Contains(input1))
			{
				int i = possibleLetters.IndexOf(input2);
				int j = possibleNumbers.IndexOf(input1);
				GetInput(i, j, input, board);
			}
			else
			{
				GetPlace(board, false);
			}
		}
		else
		{
			switch (input)
			{
				case "solve":
					Console.Clear();
					board.PrintBoard(true);
					BackToWelcome();
					break;
				case "back":
					board.RevertAll();
					GetPlace(board);
					break;
				case "quit":
					Console.WriteLine("Quitting...");
					Environment.Exit(0);
					break;
				default:
					GetPlace(board, false);
					break;
			}
		}
	}

	public static void GetInput(int i, int j, string input, Board board, bool isInputValid = true)
	{
		Console.Clear();
		board.PrintBoard();
		if (isInputValid)
		{
			Console.WriteLine("Please enter a number you want to place in " + input
			+ ".\n(Or write \"back\" to go back to choosing place)" +
			"\n(\"revert\" to revert "
			+ input + " back to default)"
			+ "\n(\"quit\" to quit):");
		}
		else
		{
			Console.WriteLine("Please enter a valid number you want to place in " + input
			+ ".\n(Or write \"back\" to go back to choosing place)" +
			"\n(\"revert\" to revert "
			+ input + " back to default)"
			+ "\n(\"quit\" to quit):");
		}
		string secondInput = Console.ReadLine();
		if ("123456789".Contains(secondInput) && secondInput.Length == 1)
		{
			if (!board.PlaceNum(i, j, int.Parse(secondInput)))
			{
				GetInput(i, j, input, board, false);
			}
			GetPlace(board);
		}
		else if (secondInput == "back")
		{
			GetPlace(board);
		}
		else if (secondInput == "revert")
		{
			board.Revert(i, j);
			GetPlace(board);
		}
		else if (secondInput == "quit")
		{
			Console.WriteLine("Quitting...");
			Environment.Exit(0);
		}
		else
		{
			GetInput(i, j, input, board, false);
		}

	}

	public static void GetValid()
	{
		Console.Clear();
		Board validBoard = new Board();
		validBoard.Solve(0, 0, true);
		BackToWelcome();
	}

	public static void BackToWelcome()
	{
		Console.WriteLine("Press any key to go to Welcome page");
		Console.ReadKey();
		Welcome();
	}

	public static void Welcome(string errorMassage = "")
	{
		Console.Clear();
		Console.WriteLine(errorMassage
		+ "Welcome to AllStiver Sudoku!\nChoose an option:\n"
		+ "1. Start Game\n2. Generate valid sudoku\n3. Quit");
		string input = Console.ReadLine();
		switch (input)
		{
			case "1":
				Start();
				break;
			case "2":
				GetValid();
				break;
			case "3":
				Console.WriteLine("Quitting...");
				Environment.Exit(0);
				break;
			default:
				Welcome("Please enter a valid number!\n");
				break;
		}
	}

}

public class Board
{
	private int[,] table;
	private int[,] staticTable;
	private int[,] solvedTable;
	private int size = 9;

	public Board()
	{
		table = new int[size, size];
		staticTable = new int[size, size];
		solvedTable = new int[size, size];
		Solve();
		GenerateNums();
		staticTable = table.Clone() as int[,];
	}

	public bool PlaceNum(int i, int j, int inputNum)
	{
		if (GetCandidates(i, j, table).Contains(inputNum.ToString()))
		{
			table[i, j] = inputNum;
			return true;
		}
		else
		{
			return false;
		}
	}

	public void Revert(int i, int j)
	{
		table[i, j] = staticTable[i, j];
	}

	public void RevertAll()
	{
		table = staticTable.Clone() as int[,];
	}

	private void GenerateNums()
	{
		table = solvedTable.Clone() as int[,];
		Random random = new Random();
		for (int startI = 0; startI < 9; startI += 3)
		{
			for (int startJ = 0; startJ < 9; startJ += 3)
			{
				int randomNum = random.Next(4, 8);
				for (int k = randomNum; k > 0; k--)
				{
					int i = startI + random.Next(0, 3);
					int j = startJ + random.Next(0, 3);
					if (table[i, j] != 0)
					{
						table[i, j] = 0;
					}
					else
					{
						k++;
					}
				}
			}
		}
	}

	public void Solve(int i = 0, int j = 0, bool isShown = false)
	{
		if (i == 0 && j == 0)
		{
			solvedTable = new int[9, 9];
		}

		Random random = new Random();
		string candidates = GetCandidates(i, j, solvedTable);
		if (candidates != "")
		{
			char candidate = candidates[random.Next(candidates.Length)];
			if (solvedTable[i, j] == 0)
			{
				solvedTable[i, j] = candidate - '0';
				if (isShown)
				{
					PrintBoard(true);
				}
			}
			if (j < 8)
			{
				Solve(i, j + 1, isShown);
			}
			else
			{
				if (i < 8)
				{
					Solve(i + 1, 0, isShown);
				}
			}
		}
		else
		{
			if (j < 9)
			{
				for (int l = i / 3 * 3; l < 9; l++)
				{
					for (int k = 0; k < 9; k++)
					{
						solvedTable[l, k] = 0;
					}

				}
				Solve(i / 3 * 3, 0, isShown);
			}
			else
			{
				if (i < 8)
				{
					Solve(i + 1, 0, isShown);
				}
			}
		}

	}

	public void PrintBoard(bool IsSolvedTable = false)
	{
		Console.SetCursorPosition(0, 0);
		for (int i = 0; i < 9; i++)
		{
			if (i % 3 == 0)
			{
				Console.Write("\r███████████████████\n");
			}
			for (int j = 0; j < 9; j++)
			{
				if (j % 3 == 0)
				{
					Console.Write("█");
				}
				if (j % 3 == 1)
				{
					Console.Write("░");
				}
				if (IsSolvedTable)
				{
					if (solvedTable[i, j] == 0)
					{
						Console.Write("░");
					}
					else
					{
						Console.Write(solvedTable[i, j]);
					}
				}
				else
				{
					if (table[i, j] == 0)
					{
						Console.Write("░");
					}
					else
					{
						Console.Write(table[i, j]);
					}
				}
				if (j % 3 == 1)
				{
					Console.Write("░");
				}
				if (j == 8)
				{
					Console.Write("█");
				}
			}
			Console.WriteLine();
			if (i == 8)
			{
				Console.WriteLine("\r███████████████████");
			}
		}
	}

	private string GetCandidates(int i, int j, int[,] table)
	{
		string candidates = "123456789";
		string square = "";
		string row = "";
		string column = "";
		for (int k = 0; k < 9; k++)
		{
			row += table[i, k];
			column += table[k, j];
		}
		i = i / 3;
		j = j / 3;
		for (int k = 0; k < 3; k++)
		{
			for (int l = 0; l < 3; l++)
			{
				square += table[i * 3 + k, j * 3 + l];
			}
		}
		row = row.Replace("0", null);
		column = column.Replace("0", null);
		square = square.Replace("0", null);
		foreach (char k in row)
		{
			candidates = candidates.Replace(k.ToString(), null);
		}
		foreach (char k in column)
		{
			candidates = candidates.Replace(k.ToString(), null);
		}
		foreach (char k in square)
		{
			candidates = candidates.Replace(k.ToString(), null);
		}
		return candidates;
	}
}

using ComputorV2.Interactive;

namespace ComputorV2.IO
{
	public class DisplayManager
	{
		private readonly HistoryManager	_historyManager;
		private int						_variableCount = 0;
		private string					_currentContext = "normal";

		public DisplayManager(HistoryManager historyManager)
		{
			_historyManager = historyManager;
		}
		
		public void DisplayPrompt(string context = "normal")
		{
			_currentContext = context;

			switch(context.ToLower())
			{
				case "normal":
					Console.Write("> ");
					break;
				
				case "continuation":
					Console.Write("... ");
					break;

				case "function":
					Console.Write("func > ");
					break;

				default:
					Console.Write("> ");
					break;
			}
		}

		public void DisplayHeader()
		{
			Console.WriteLine();
			
			try
			{
				string[] lines = File.ReadAllLines("header_banner.txt");
				foreach (string line in lines)
					Console.WriteLine(line);
			}
			catch (FileNotFoundException)
			{
				DisplayError("Header banner file not found");
			}
			catch (Exception ex)
			{
				DisplayError($"Error reading header: {ex.Message}");
			}
		}

		public void DisplayCommandCount()
		{	
			if (_historyManager.Count > 0)
			{
				DisplayInColor($"Init info: Loaded {_historyManager.Count} commands from history", ConsoleColor.DarkMagenta);
				Console.WriteLine();
			}

		}

		public void DisplayWelcome()
		{
			Console.WriteLine("=== ComputorV2 Interactive Calculator ===");
            Console.WriteLine("Type mathematical expressions or:");
			Console.WriteLine("	'help' for extended instructions");
			Console.WriteLine("	'clearhistory' to erase command history");
			Console.WriteLine("	'exit' to quit");
            Console.WriteLine();
		}

		public void DisplayResult(string result, System.ConsoleColor color = 0)
		{
			if (!string.IsNullOrEmpty(result))
			{
				if (color != 0)
					DisplayInColor(result, color!);
				else
					Console.WriteLine(result);
			}
		}

		public void DisplayError(string errorMessage)
		{
			DisplayInColor(errorMessage, ConsoleColor.Red);
		}

		public void SetVariableCount(int count)
		{
			_variableCount = count;
		}

		public void ClearCurrentLine()
		{
			int currentLeft = Console.CursorLeft;
			int currentTop = Console.CursorTop;

			Console.SetCursorPosition(0, currentTop);
			Console.Write(new string(' ', Console.WindowWidth - 1));
			Console.SetCursorPosition(0, currentTop);
		}

		public void RefreshLine(string content, int cursorPosition)
		{
			ClearCurrentLine();
			DisplayPrompt(_currentContext);
			Console.Write(content);

			int promptLength = GetPromptLength();
			Console.SetCursorPosition(promptLength + cursorPosition, Console.CursorTop);
		}

		private int GetPromptLength()
		{
			return _currentContext switch
			{
				"normal" => 2,
				"continuation" => 4,
				"function" => 7,
				_ => 2
			};
		}

		public void DisplayInColor(string content, System.ConsoleColor color)
		{
			var originalColor = Console.ForegroundColor;

			switch (color)
			{
				case ConsoleColor.Red:
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine($"Error: {content}");
					break;

				case ConsoleColor.Green:
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine($"DEBUG: {content}");
					break;

				default:
					Console.ForegroundColor = color;
					Console.WriteLine(content);
					break;
			}

			Console.ForegroundColor = originalColor;
		}
	}
}
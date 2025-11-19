namespace ComputorV2.IO
{
	public class DisplayManager
	{
		private int		_variableCount = 0;
		private string	_currentContext = "normal";
		
		public void DisplayPrompt(string context = "normal")
		{
			_currentContext = context;

			switch (context.ToLower())
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

		public void DisplayResult(string result)
		{
			if (!string.IsNullOrEmpty(result))
			{
				Console.WriteLine(result);
			}
		}

		public void DisplayError(string errorMessage)
		{
			var originalColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine($"Error: {errorMessage}");
			Console.ForegroundColor = originalColor;
		}

		public void DisplayWelcome()
		{
			Console.WriteLine("=== ComputorV2 Interactive Calculator ===");
            Console.WriteLine("Type mathematical expressions, 'help' for instructions, or 'exit' to quit.");
            Console.WriteLine();
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
	}
}
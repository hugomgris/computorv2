using ComputorV2.Core.Math;
using ComputorV2.IO;

namespace ComputorV2.Interactive
{
	public class REPL
	{
		private readonly DisplayManager	_displayManager;
		private readonly HistoryManager	_historyManager;
		private readonly InputHandler	_inputHandler;
		private readonly MathEvaluator	_mathEvaluator;
		private readonly HelpSystem		_helpSystem;
		private bool	_isRunning;

		public REPL()
		{
			_historyManager = new HistoryManager();
			_displayManager = new DisplayManager(_historyManager);
			_historyManager.SetDisplayManagerReference(_displayManager); // I don't like this circular dependency but 1)call the cops and 2)getting rid of this would be a pain in my butt
			_inputHandler = new InputHandler(_displayManager, _historyManager);
			_mathEvaluator = new MathEvaluator();
			_helpSystem = new HelpSystem();
			_isRunning = false;

			try
			{
				_historyManager.LoadFromDefaultLocation();
			}
			catch (Exception)
			{
				Console.WriteLine("Starting with empty history");
			}

			try
			{
				_helpSystem.LoadFromDefaultLocation();
			}
			catch (Exception)
			{
				_displayManager.DisplayInColor($"Warning: Help file not found (path: {_helpSystem.GetDefaultHelpFilePath()})", ConsoleColor.DarkYellow);
			}
		}

		public void Run()
		{
			_displayManager.DisplayHeader();
			_displayManager.DisplayCommandCount();
			_displayManager.DisplayWelcome();
			_isRunning = true;

			while (_isRunning)
			{
				try
				{
					_displayManager.DisplayPrompt();
					string input = _inputHandler.ReadLine();

					if (IsExitCommand(input))
					{
						break;
					}

					if (IsHelpCommand(input))
					{
						_helpSystem.Help();
						continue;
					}

					if (IsHistoryClearCommand(input))
					{
						_historyManager.ClearHistory();
						continue;
					}

					if (string.IsNullOrWhiteSpace(input))
					{
						continue;
					}

					string result = ProcessCommand(input);
					_historyManager.AddCommand(input);
					
					if (!string.IsNullOrEmpty(result))
					{
						_displayManager.DisplayResult(result, ConsoleColor.DarkCyan);
					}
				}
				catch (Exception ex)
				{
					_displayManager.DisplayError(ex.Message);
				}
			}

			Stop();
		}

		private bool IsExitCommand(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return (false);
			}

			string command = input.Trim().ToLower();
			return command == "exit" || command == "quit" || command == "q";
		}

		private bool IsHelpCommand(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return (false);
			}

			string command = input.Trim().ToLower();
			return command == "help" || command == "h";
		}

		private bool IsHistoryClearCommand(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return (false);
			}

			string command = input.Trim().ToLower();
			return command == "clearhistory" || command == "clear history" || command == "ch";
		}

		private string ProcessCommand(string input)
		{
			return ($"Input: {input}");
		}

		public void Stop()
		{
			_isRunning = false;
			_historyManager.SaveToDefaultLocation();
			_displayManager.DisplayInColor("Goodbye!", ConsoleColor.DarkYellow);
		}
	}
}
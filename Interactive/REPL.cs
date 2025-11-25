using System;
using System.Collections.Generic;
using System.Diagnostics;
using ComputorV2.IO;

namespace ComputorV2.Interactive
{
	public class REPL
	{
		private readonly DisplayManager				_displayManager;
		private readonly HistoryManager				_historyManager;
		private readonly InputHandler				_inputHandler;
		private readonly Dictionary<string, object>	_variables;
		private bool 								_isRunning;

		public REPL()
		{
			_displayManager = new DisplayManager();
			_historyManager = new HistoryManager();
			_inputHandler = new InputHandler(_displayManager, _historyManager);
			_variables = new Dictionary<string, object>();
			_isRunning = false;
		}

		public void Run()
		{
			_displayManager.DisplayHeader();
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

					if (string.IsNullOrWhiteSpace(input))
					{
						continue;
					}

					string result = ProcessCommand(input);

					_historyManager.AddCommand(input);

					if(!string.IsNullOrEmpty(result))
					{
						_displayManager.DisplayResult(result);
					}
				}
				catch (Exception ex)
				{
					_displayManager.DisplayError(ex.Message);
				}
			}

			_displayManager.DisplayResult("Goodbye!");
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

		private string ProcessCommand(string input)
		{
			// TODO: integrate with CommandProcessor when that's done

			if (input.StartsWith("test"))
			{
				return ($"You entered: {input}");
			}

			if (input.Contains("=") && !input.EndsWith("?"))
			{
				return $"Assignment detected: {input}";
			}

			if (input.EndsWith("?"))
			{
				return $"Evaluation detected: {input}";
			}

			return $"Command processed: {input}";
		}
		
		public void Stop()
		{
			_isRunning = false;
		}
	}
}
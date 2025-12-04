using System;
using System.Collections.Generic;
using System.Diagnostics;
using ComputorV2.IO;
using ComputorV2.Core.Math;

namespace ComputorV2.Interactive
{
	public class REPL
	{
		private readonly DisplayManager				_displayManager;
		private readonly HistoryManager				_historyManager;
		private readonly InputHandler				_inputHandler;
		private readonly RationalMathEvaluator		_mathEvaluator;
		private bool 								_isRunning;

		public REPL()
		{
			_displayManager = new DisplayManager();
			_historyManager = new HistoryManager();
			_inputHandler = new InputHandler(_displayManager, _historyManager);
			_mathEvaluator = new RationalMathEvaluator();
			_isRunning = false;

			try
			{
				_historyManager.LoadFromDefaultLocation();
				Console.WriteLine($"Loaded {_historyManager.Count} commands from history."); // DEBUG
			}
			catch (Exception)
			{
				Console.WriteLine("Starting with empty history");
			}
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

		private string ProcessCommand(string input)
		{
			var result = _mathEvaluator.Evaluate(input);
			
			var assignmentInfo = _mathEvaluator.GetLastRationalAssignmentInfo();
			if (assignmentInfo != null)
			{
				return $"Variable '{assignmentInfo.Variable}' assigned: {assignmentInfo.Value}";
			}

			return $"{result}";
		}
		
		public void Stop()
		{
			_isRunning = false;
			_historyManager.SaveToDefaultLocation();
			_displayManager.DisplayResult("Goodbye!");
		}
	}
}
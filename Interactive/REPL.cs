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
		private readonly MathEvaluator				_mathEvaluator;
		private bool 								_isRunning;

		public REPL()
		{
			_displayManager = new DisplayManager();
			_historyManager = new HistoryManager();
			_inputHandler = new InputHandler(_displayManager, _historyManager);
			_mathEvaluator = new MathEvaluator();
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

		public string ProcessCommand(string input)
		{
			var result = _mathEvaluator.Evaluate(input);
			
			var assignmentInfo = _mathEvaluator.GetLastAssignmentInfo();
			if (assignmentInfo != null)
			{
				// For RationalNumber assignments, prefer decimal display format for better readability
				string valueDisplay = assignmentInfo.Value is ComputorV2.Core.Types.RationalNumber rational 
					? rational.ToDecimalString() 
					: assignmentInfo.Value.ToString();
				return $"Variable '{assignmentInfo.Variable}' assigned: {valueDisplay}";
			}

			if (input.Contains('=') && input.Contains('(') && input.Contains(')'))
			{
				var parts = input.Split('=');
				var leftSide = parts[0].Trim();
				var match = System.Text.RegularExpressions.Regex.Match(leftSide, @"^([a-zA-Z][a-zA-Z0-9]*)\s*\(");
				if (match.Success)
				{
					string functionName = match.Groups[1].Value;
					return $"Function '{functionName}' defined: {result}";
				}
			}

			// For arithmetic results, also prefer decimal display format for RationalNumbers
			if (result is ComputorV2.Core.Types.RationalNumber resultRational)
			{
				return resultRational.ToDecimalString();
			}
			
			// Handle ComplexNumbers that represent real numbers (zero imaginary part)
			if (result is ComputorV2.Core.Types.ComplexNumber resultComplex && resultComplex.IsReal)
			{
				return resultComplex.Real.ToDecimalString();
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
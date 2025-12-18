using System;
using System.Collections;

using ComputorV2.Core.Math;
using ComputorV2.Core.Lexing;
using ComputorV2.IO;

namespace ComputorV2.Interactive
{
	public class REPL
	{
		private readonly HistoryManager	_historyManager;
		private readonly Parser			_parser;
		private readonly Tokenizer		_tokenizer;
		private readonly DisplayManager	_displayManager;
		private readonly InputHandler	_inputHandler;
		private readonly MathEvaluator	_mathEvaluator;
		private readonly HelpSystem		_helpSystem;
		private bool	_isRunning;

		public REPL()
		{
			_historyManager = new HistoryManager();
			_parser = new Parser();
			_tokenizer = new Tokenizer();
			_displayManager = new DisplayManager(_historyManager);
			_historyManager.SetDisplayManagerReference(_displayManager); // I don't like this circular dependency but 1)call the cops and 2)getting rid of this would be a pain in my butt
			_inputHandler = new InputHandler(_displayManager, _historyManager);
			_mathEvaluator = new MathEvaluator(_parser, _tokenizer);
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

					if (IsListVariableCommand(input))
					{
						_mathEvaluator.PrintVariableList();
						continue;
					}

					if (IsListFunctionCommand(input))
					{
						_mathEvaluator.PrintFunctionList();
						continue;
					}

					if (IsListAllCommand(input))
					{ 
						_mathEvaluator.PrintAllLists();
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

		private bool IsListVariableCommand(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return (false);
			}

			string command = input.Trim().ToLower();
			return command == "listvariables" || command == "lv";
		}

		private bool IsListFunctionCommand(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return (false);
			}

			string command = input.Trim().ToLower();
			return command == "listfunctions" || command == "lf";
		}

		private bool IsListAllCommand(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return (false);
			}

			string command = input.Trim().ToLower();
			return command == "listall" || command == "la";
		}

		private string ProcessCommand(string input)
		{
			string result;
			string trimmed = input.Trim().Replace(" ", "");
			
			if (input.Contains('=') && input.Contains('?'))
				result = _mathEvaluator.Compute(trimmed);
			else if (input.Contains('='))
				result = _mathEvaluator.Assign(trimmed);
			else
				result = "";
			/* cmd_type type = _parser.DetectInputType(input);
			switch (type)
			{
				case cmd_type.FUNCTION:
					return ($"{input} is of type FUNCTION");

				case cmd_type.RATIONAL:
					return ($"{input} is of type RATIONAL");

				default:
					return($"{input} is of UNKNOWN type");
			} */
			return result;
		}

		public void Stop()
		{
			_isRunning = false;
			_historyManager.SaveToDefaultLocation();
			_displayManager.DisplayInColor("Goodbye!", ConsoleColor.DarkYellow);
		}
	}
}
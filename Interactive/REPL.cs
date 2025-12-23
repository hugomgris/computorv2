using System;
using System.Collections;

using ComputorV2.Core.Math;
using ComputorV2.Core.Types;
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
		private bool					_isRunning;
		private bool					_graphsOn;
		Dictionary<string,string>		_commandResults;

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
			_graphsOn = false;
			_commandResults = new Dictionary<string, string>();

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

					if (IsListCommandResults(input))
					{
						PrintCommandList();
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
						PrintCommandList();
						continue;
					}

					if (input == "graphs ON")
					{
						_graphsOn = true;
						Console.WriteLine("Function curve display ON");
						continue;
					}

					if (input == "graphs OFF")
					{
						_graphsOn = false;
						Console.WriteLine("Function curve display ON");
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
						if (_parser.DetectInputType(result.Replace(" ", "")) == cmd_type.FUNCTION && _graphsOn)
						{
							string[] split = result.Split("=");
							Polynomial polynomial = new Polynomial(split[split.Length - 1]);
							Console.WriteLine($"trying to draw ->{polynomial}");
							Grapher.DrawPolynomialGraph(polynomial);
						}
						
						_displayManager.DisplayResult(result, ConsoleColor.DarkCyan);
						_commandResults[input] = result;
					}
					else
						_commandResults[input] = "failed/noresult";

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

		private bool IsListCommandResults(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return (false);
			}

			string command = input.Trim().ToLower();
			return command == "listcommands" || command == "lc";
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
			
			if (input.Contains("^-1"))
			{
				if (CheckIfInverseInputAppliesToMatrix(input) == false)
					throw new ArgumentException("Inverse calculation: operation can only be called on Matrix objects");
			}
			
			if (input.Contains('=') && input.Contains('?'))
				result = _mathEvaluator.Compute(trimmed);
			else if (input.Contains('='))
				result = _mathEvaluator.Assign(trimmed);
			else
				result = "";
				
			return result;
		}

		private bool CheckIfInverseInputAppliesToMatrix(string input)
		{
			string variable = input.Substring(0, input.IndexOf("^-1"));

			int i = variable.Length - 1;
			while (i > 0)
			{
				if ("+-*/%^".Contains(variable[i]))
					break;
				i--;
			}

			variable = variable.Substring(i);
			
			if (_mathEvaluator.Variables.ContainsKey(variable))
			{
				string tmp = _mathEvaluator.Variables[variable].ToString()!;
				string matrixRebuild = "[" + tmp.Replace("\n", ";" + "]");
				if (Matrix.TryParse(matrixRebuild, out _))
					return true;
			}

			if (Matrix.TryParse(variable, out _))
				return true;

			return false;
		}

		private void PrintCommandList()
		{
			if (_commandResults.Count == 0)
			{
				Console.WriteLine("No commands registered");
				return;
			}

			Console.WriteLine("Registered commands (c -> r):");

			foreach (var item in _commandResults)
			{
				Console.WriteLine("{0} -> {1}", item.Key, item.Value);
			}
		}

		public void Stop()
		{
			_isRunning = false;
			_historyManager.SaveToDefaultLocation();
			_displayManager.DisplayInColor("Goodbye!", ConsoleColor.DarkYellow);
		}
	}
}
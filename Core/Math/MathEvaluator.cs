using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using ComputorV2.Core.Types;
using ComputorV2.Core.Lexing;

namespace ComputorV2.Core.Math
{
	public class MathEvaluator
	{
		private readonly Parser							_parser;
		private readonly Tokenizer						_tokenizer;
		private Dictionary<string, MathValue>		_variables = new();
		//private SortedDictionary<string, Function>	_functions = new();
		//private AssignmentInfo?						_lastAssignment = null;

		public MathEvaluator(Parser parser, Tokenizer tokenizer)
		{
			_parser = parser;
			_tokenizer = tokenizer;
		}

		#region Computation pipeline

		public string Compute(string input)
		{
			string result;

			if (_parser.DetectInputType(input) == cmd_type.FUNCTION) // TODO: input type detection (currently always returning FUNCTION)
				result = ComputeFunction(input);
			else
				result = ComputeExpression(input);

			return result;
		}

		private string ComputeExpression(string expression)
		{
			if (HasVariables(expression))
				expression = SubstituteVariables(expression);

			if (_parser.DetectValueType(expression) == cmd_type.MATRIX)
				return $"Computing MATRIX variable from {expression} (WIP)";
			else if (_parser.DetectValueType(expression) == cmd_type.COMPLEX)
				return $"Computing COMPLEX variable from {expression} (WIP)";
			else if (_parser.DetectValueType(expression) == cmd_type.RATIONAL)
				return ComputeRational(expression);
			else
				return "";
		}

		private string ComputeRational(string expression)
		{
			Postfix postfix = new Postfix(expression.Replace("?", ""));

			RationalNumber value = new RationalNumber(postfix.Calculate());

			return value.ToString();
		}

		private string ComputeFunction(string input)
		{
			return $"Computing function (WIP)";
		}

		#endregion

		#region Assignation pipeline

		public string Assign(string input)
		{
			string result;

			if (_parser.DetectInputType(input) == cmd_type.FUNCTION) // TODO: input type detection (currently always returning FUNCTION)
				result = StoreFunction(input);
			else
				result = StoreVariable(input);

			return result;
		}

		private string StoreVariable(string input)
		{
			string[] parts = input.Split('=');
			if (parts.Length != 2)
				throw new ArgumentException($"RationalNumber: Parser: expression can only contain one '=': {input}", nameof(input));

			if (_parser.ValidateVariableName(parts[0]) == var_error.INVALIDCHAR)
				throw new ArgumentException($"Assignation: variable name can only contain alphanumeric characters: {input}", nameof(input));
			else if (_parser.ValidateVariableName(parts[0]) == var_error.HASICHAR)
				throw new ArgumentException($"Assignation: variable name can not contain 'i' character: {input}", nameof(input));
			else if (_parser.ValidateVariableName(parts[0]) == var_error.NOALPHA)
				throw new ArgumentException($"Assignation: variable name must contain at least one alphabetical character: {input}", nameof(input));

			if (HasVariables(parts[1]))
				parts[1] = SubstituteVariables(parts[1]);

			if (_parser.DetectValueType(parts[1]) == cmd_type.MATRIX)
				return $"Storing MATRIX variable from {input}";
			else if (_parser.DetectValueType(parts[1]) == cmd_type.COMPLEX)
				return $"Storing COMPLEX variable from {input}";
			else if (_parser.DetectValueType(parts[1]) == cmd_type.RATIONAL)
				return StoreRational(parts);
			else
				return "";
		}

		private string StoreRational(string[] parts)
		{			
			string name = parts[0];

			Postfix postfix = new Postfix(parts[1]);

			RationalNumber value = new RationalNumber(postfix.Calculate());

			_variables[name] = value;

			return value.ToString();
		}

		private string StoreFunction(string input)
		{
			return $"Storing function from {input}";
		}

		#endregion

		#region Variable substitution

		private bool HasVariables(string expression)
		{
			foreach (char c in expression)
			{
				if (Char.IsLetter(c) && c != 'i')
					return true;
			}

			return false;
		}

		private string SubstituteVariables(string expression)
		{
			StringBuilder sb = new StringBuilder();

			foreach (char c in expression)
			{
				if (Char.IsLetter(c))
				{
					if (_variables.ContainsKey(c.ToString()))
					{
						sb.Append(_variables[c.ToString()]);
					}
					else
					{
						throw new ArgumentException($"Error: Variable Subtitution: expression contains undefined variables: {expression}", nameof(expression));
					}
				}
				else
				{
					sb.Append(c);
				}
			}

			return sb.ToString();
		}

		#endregion

		#region Stored data printing

		public void PrintVariableList()
		{
			if (_variables.Count == 0)
			{
				Console.WriteLine("No variables defined!");
				return;
			}

			Console.WriteLine("Declared variables:");
			foreach (var item in _variables)
			{
				Console.WriteLine("{0} = {1}", item.Key, item.Value);
			}
		}

		public void PrintFunctionList()
		{
			Console.WriteLine("Function list printing is WIP!");
			// TODO: pending function implementation
			/* if (_functions.Count == 0)
			{
				Console.WriteLine("No functions defined!");
				return;
			}

			Console.WriteLine("Defined functions:");
			foreach (var item in _functions)
			{
				Console.WriteLine("{0} = {1}", item.Key, item.Value);
			} */
		}

		public void PrintAllLists()
		{
			PrintVariableList();
			PrintFunctionList();
		}

		#endregion
	}
}
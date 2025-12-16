using System;
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
		private SortedDictionary<string, MathValue>		_variables = new();
		//private SortedDictionary<string, Function>	_functions = new();
		//private AssignmentInfo?						_lastAssignment = null;

		#region Computation pipeline

		public MathEvaluator(Parser parser, Tokenizer tokenizer)
		{
			_parser = parser;
			_tokenizer = tokenizer;
		}

		public string Compute(string input)
		{
			return $"COMPUTE -> {input}";
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
			
			
			
			if (_parser.DetectInputType(input) == cmd_type.MATRIX)
				return $"Storing MATRIX variable from {input}";
			else if (_parser.DetectInputType(input) == cmd_type.COMPLEX)
				return $"Storing COMPLEX variable from {input}";
			else if (_parser.DetectInputType(input) == cmd_type.RATIONAL)
				return StoreRational(input);
			else
				return "";
		}

		private string StoreRational(string input)
		{
			string[] parts = input.Split('=');
			if (parts.Length != 2)
				throw new ArgumentException($"RationalNumber: Parser: expression can only contain one '=': {input}", nameof(input));
			
			string name = parts[0];

			List<string> postfix = ConvertToPostfix(parts[1]);
			RationalNumber resolvedExpr = (RationalNumber)ResolvePostfix(postfix);

			RationalNumber value = new RationalNumber(resolvedExpr);

			_variables[name] = value;

			return value.ToString();
		}

		private string StoreFunction(string input)
		{
			return $"Storing function from {input}";
		}

		#endregion

		#region POSTFIX management

		private List<string> ConvertToPostfix(string expr)
		{
			List<string> rawTokens = _tokenizer.MakeRawTokens(expr);
			var operators = new Stack<string>();
			var output = new Queue<string>();
			
			for (int i = 0; i < rawTokens.Count; i++)
			{

			}

			return rawTokens;
		}

		private MathValue ResolvePostfix(List<string> tokens)
		{
			return RationalNumber.One;
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
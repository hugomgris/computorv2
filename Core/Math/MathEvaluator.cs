using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using ComputorV2.Core.Types;
using ComputorV2.Core.Lexing;

namespace ComputorV2.Core.Math
{
	public class MathEvaluator
	{
		private readonly Parser					_parser;
		private readonly Tokenizer				_tokenizer;
		private Dictionary<string, MathValue>	_variables = new();
		private Dictionary<string, Function>	_functions = new();

		public MathEvaluator(Parser parser, Tokenizer tokenizer)
		{
			_parser = parser;
			_tokenizer = tokenizer;
		}

		#region Computation pipeline

		public string Compute(string input)
		{
			input = input.Replace(" ", "");
			string result;

			if (_parser.DetectInputType(input) == cmd_type.FUNCTION)
				result = ComputeFunction(input, out _);
			else
				result = ComputeExpression(input);

			return result;
		}

		private string ComputeExpression(string expression)
		{
			if (HasVariables(expression))
				expression = SubstituteVariables(expression);

			if (_parser.DetectValueType(expression) == cmd_type.MATRIX)
				return ComputeMatrix(expression);
			else if (_parser.DetectValueType(expression) == cmd_type.COMPLEX)
				return ComputeComplex(expression);
			else if (_parser.DetectValueType(expression) == cmd_type.RATIONAL)
				return ComputeRational(expression);
			else
				return "";
		}

		private string ComputeRational(string expression) 
		{	
			List<string> tokens = _tokenizer.Tokenize(expression.Replace("?", "").Replace("=", ""));
			Postfix postfix = new Postfix(tokens);

			var result = (RationalNumber)postfix.Calculate();

			return result.ToString();
		}

		private string ComputeComplex(string expression)
		{
			List<string> tokens = _tokenizer.Tokenize(expression.Replace("?","").Replace("=",""));
			Postfix postfix = new Postfix(tokens);

			var result = (ComplexNumber)postfix.Calculate();
			return result.ToString();
		}

		private string ComputeMatrix(string expression)
		{
			// Adding matrix inversion as an afterthought made this function an abomination. I know God is watching me, but right now working > elgant
			bool isInversionCall = false;

			if (expression.Contains("^-1"))
				isInversionCall = true;

			string matrixRebuild = expression.Replace("?","").Replace("=","").Replace("^-1", "");
			matrixRebuild = "[" + matrixRebuild.Replace("\n", ";") + "]";

			if (isInversionCall)
			{
				isInversionCall = true;
				Matrix inv = new Matrix(matrixRebuild);
				return inv.Inverse().ToString();
			}

			if (Matrix.TryParse(matrixRebuild, out _))
				return expression.Replace("?","").Replace("=","");
		
			List<string> tokens = _tokenizer.Tokenize(expression.Replace("?","").Replace("=",""));
			Postfix postfix = new Postfix(tokens);

			var result = (Matrix)postfix.Calculate();

			if (isInversionCall)
				result = result.Inverse();

			return result.ToString();
		}

		public string ComputeFunction(string input, out List<MathValue>? solutions)
		{
			if (new[] { "+", "-", "/", "*", "%", "^" }.Any(c => input.Contains(c)))
			{
				solutions = null;
				return ComputeMultipleFunctions(input);
			}
			if (input.IndexOf('?') - input.IndexOf('=') == 1)
			{
				solutions = null;
				return SolveFunction(input);
			}

			string right = input.Substring(input.IndexOf('='), input.IndexOf('?') - input.IndexOf('='));
			string subRight = "";

			foreach (char c in right)
			{
				if (!char.IsDigit(c))
				{
					subRight = SubstituteVariables(right);
					break;
				}
			}

			if (!string.IsNullOrWhiteSpace(subRight))
				input = input.Replace(right, subRight);
			else
				subRight = right;

			string left = input.Substring(0, input.IndexOf('('));
			if (_functions.ContainsKey(left))
				left = _functions[left].Expression.ToString()!;
			else
				throw new ArgumentException("Function Computation: function not defined");

			subRight = subRight.Replace("=", "");
			string polyString = left!;
			if (decimal.Parse(subRight) > 0)
			{
				polyString = polyString + "-" + subRight;
			}
			else
				polyString = polyString + "+" + subRight;

			Polynomial poly = new Polynomial(polyString);

			solutions = poly.Solve();
			
			Console.WriteLine($"{solutions.Count} solutions:");

			foreach(MathValue sol in solutions) Console.WriteLine(sol);

			return $"";
		}

		private string ComputeMultipleFunctions(string input)
		{
			string operators = "";

			foreach (char c in input)
				if ("+-*/^%".Contains(c))
					operators += c.ToString();
			
			string [] functions = input.Split(new Char[] { '+', '-', '*', '/', '%', '^', '=', '?' },
                                	StringSplitOptions.RemoveEmptyEntries);

			foreach (string function in functions)
			{
				if (function.Contains("(") && function.Contains(")"))
					input = input.Replace(function, SolveFunction(function));
				else
					continue;
			}

			input = input.Replace("?", "").Replace("=", "");

			List<string> tokens = _tokenizer.Tokenize(input);
			Postfix postfix = new Postfix(tokens);

			return postfix.Calculate().ToString()!;
		}

		#endregion

		#region Function solve pipeline

		private string SolveFunction(string input)
		{
			if (CheckIfInputAsksForStoredFunction(input))
				return GetStoredFunction(input);
			var parts = input.Split('=');
			string leftSide = parts[0].Trim();

			string functionName = leftSide.Substring(0, leftSide.IndexOf('('));
			string variable = leftSide.Substring(leftSide.IndexOf('(') + 1);
			variable = variable.Trim(')');

			foreach (char c in variable)
			{
				if (!char.IsDigit(c) && !_variables.ContainsKey(variable))
					throw new ArgumentException("Function Solve: variable is undefined");
			}

			if (!_functions.ContainsKey(functionName))
				throw new ArgumentException("Function solve: function is not defined");

			if (!decimal.TryParse(variable, out _))
				variable = _variables[variable].ToString()!;
			
			Function function = _functions[functionName];
			string functionString = function.ToString();

			functionString = functionString.Replace(function.Variable, variable);

			string[] split = functionString.Split('=');
			
			List<string> tokens = _tokenizer.Tokenize(split[1]);

			Postfix postfix = new Postfix(tokens);
			MathValue result = postfix.Calculate();
			
			return result.ToString()!;
		}

		bool CheckIfInputAsksForStoredFunction(string input)
		{
			if (input.EndsWith("=?"))
			{
				string key = input.Substring(0, input.IndexOf('('));
				string variable = input.Substring(input.IndexOf('(') + 1, input.IndexOf(')') - 2);
				if (_variables.ContainsKey(variable))
					return false;

				if (_functions.ContainsKey(key) && _functions[key].Variable == variable)
				{
					return true;
				}
			}

			return false;
		}

		private string GetStoredFunction(string input)
		{
			string key = input.Substring(0, input.IndexOf('('));
			if (_functions.ContainsKey(key))
				return (_functions[key].ToString());
			return "";
		}

		#endregion

		#region Assignation pipeline

		public string Assign(string input)
		{
			input = input.Replace(" ", "");

			string result;

			if (_parser.DetectInputType(input) == cmd_type.FUNCTION)
				result = StoreFunction(input);
			else
				result = StoreVariable(input);

			return result;
		}

		private string StoreVariable(string input)
		{		
			string[] parts = input.Split('=');
			if (parts.Length != 2)
				throw new ArgumentException($"Parser: expression can only contain one '=' token: {input}", nameof(input));

			if (_parser.ValidateVariableName(parts[0]) == var_error.INVALIDCHAR)
				throw new ArgumentException($"Assignation: variable name can only contain letters: {input}", nameof(input));
			else if (_parser.ValidateVariableName(parts[0]) == var_error.HASICHAR)
				throw new ArgumentException($"Assignation: variable name can not contain 'i' character: {input}", nameof(input));
			else if (_parser.ValidateVariableName(parts[0]) == var_error.NOALPHA)
				throw new ArgumentException($"Assignation: variable name must contain at least one alphabetical character: {input}", nameof(input));
			
			if (_parser.DetectInputType(parts[1]) == cmd_type.FUNCTION)
				parts[1] = SolveFunction(parts[1]);
			else if (HasVariables(parts[1]))
				parts[1] = SubstituteVariables(parts[1]);

			if (_parser.DetectValueType(parts[1]) == cmd_type.MATRIX)
				return StoreMatrix(parts);
			else if (_parser.DetectValueType(parts[1]) == cmd_type.COMPLEX)
				return StoreComplex(parts);
			else if (_parser.DetectValueType(parts[1]) == cmd_type.RATIONAL)
				return StoreRational(parts);
			else if (_parser.DetectValueType(parts[1]) == cmd_type.INVALID)
				throw new ArgumentException($"Parser: invalid type (check syntax!)", nameof(input));
			else
				return "";
		}

		private string StoreRational(string[] parts)
		{			
			string name = parts[0];

			List<string> tokens = _tokenizer.Tokenize(parts[1].Replace("?", "").Replace("=", ""));
			Postfix postfix = new Postfix(tokens);
			var result = (RationalNumber)postfix.Calculate();

			_variables[name] = result;

			return result.ToString();
		}

		private string StoreComplex(string[] parts)
		{
			string name = parts[0];
		
			List<string> tokens = _tokenizer.Tokenize(parts[1].Replace("?", "").Replace("=", ""));
			Postfix postfix = new Postfix(tokens);
			var result = (ComplexNumber)postfix.Calculate();

			_variables[name] = result;

			return result.ToString();
		}

		private string StoreMatrix(string[] parts)
		{
			string name = parts[0];

			List<string> tokens = _tokenizer.Tokenize(parts[1].Replace("?", "").Replace("=", ""));
			Postfix postfix = new Postfix(tokens);
			var result = (Matrix)postfix.Calculate();
			
			_variables[name] = result;

			return result.ToString();
		}

		private string StoreFunction(string input)
		{
			var parts = input.Split('=');
			string leftSide = parts[0].Trim();

			var match = System.Text.RegularExpressions.Regex.Match(leftSide, @"^([a-zA-Z][a-zA-Z0-9_]*)\s*\(\s*([a-zA-Z][a-zA-Z0-9_]*)\s*\)$");
			if (!match.Success)
				throw new ArgumentException($"Parsing: Invalid function definition: {leftSide}");

			string functionName = match.Groups[1].Value;
			if (_parser.ValidateFunctionName(functionName) == fun_error.INVALIDCHAR)
				throw new ArgumentException($"Assignation: function name can only contain letters: {input}", nameof(input));
			else if (_parser.ValidateFunctionName(parts[0]) == fun_error.NOALPHA)
				throw new ArgumentException($"Assignation: variable name must contain at least one alphabetical character: {input}", nameof(input));

			string variable = match.Groups[2].Value;

			foreach (char c in variable)
			{
				if (!char.IsLetter(c))
					throw new ArgumentException($"Parsing: Variables can only contain letters:{variable}");
			}

			string resolvedExpression = ResolveFunctionVariables(parts[1].Trim(), variable);

			Polynomial poly = new Polynomial(resolvedExpression, variable);

			var function = new Function(functionName, variable, poly);
			_functions[functionName] = function;

			return function.ToString();
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

			for (int i = 0; i < expression.Length; i++)
			{
				if (Char.IsLetter(expression[i]) && expression[i] != 'i')
				{
					string var = expression[i].ToString();
					if (i < expression.Length)
					{
						for (int j = i + 1; j < expression.Length; j++)
						{
							if (Char.IsLetterOrDigit(expression[j]) && expression[j] != 'i')
								var += expression[j].ToString();
							else
								break;							
						}
					}

					i += var.Length - 1;
					
					if (_variables.ContainsKey(var))
					{
						sb.Append(_variables[var]);
					}
					else
					{
						throw new ArgumentException($"Variable Substitution: expression contains undefined variables: {expression}", nameof(expression));
					}
				}
				else
				{
					sb.Append(expression[i]);
				}
			}

			return sb.ToString().Replace(" ", "");
		}

		private string ResolveFunctionVariables(string expression, string excludedVariable)
		{
			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < expression.Length; i++)
			{
				if (Char.IsLetter(expression[i]) && expression[i] != 'i')
				{
					string var = expression[i].ToString();
					if (i < expression.Length)
					{
						for (int j = i + 1; j < expression.Length; j++)
						{
							if (Char.IsLetterOrDigit(expression[j]) && expression[j] != 'i')
								var += expression[j].ToString();
							else
								break;							
						}
					}

					i += var.Length - 1;
					
					if (var == excludedVariable)
					{
						sb.Append(var);
					}
					else if (_variables.ContainsKey(var))
					{
						if (_variables[var].GetType() == typeof(Matrix))
						{
							string rebuiltMatrixString = "[" + _variables[var].ToString()!.Replace(" ", "").Replace("\n", ";") + "]";
							sb.Append(rebuiltMatrixString);
						}
						else
							sb.Append(_variables[var]);
					}
					else
					{
						throw new ArgumentException($"Variable Substitution: expression contains undefined variables: {expression}", nameof(expression));
					}
				}
				else
				{
					sb.Append(expression[i]);
				}
			}

			return sb.ToString();
		}

		#endregion

		#region Matrix Norm Bonus through function

		public string ProcessMatrixNorm(string input)
		{
			input = input.Replace("||", "").Replace("=", "").Replace("?", "");
			if (input.EndsWith('1'))
				input = input.Substring(0, input.Length - 1);
			string result = "";


			if (_variables.ContainsKey(input))
			{
				string matrixString = _variables[input].ToString()!;
				matrixString = ("[" + matrixString.Replace("\n", ";") + "]").Replace(" ", "");
				Matrix m = new Matrix(matrixString);
				result = m.Norm().ToString()!;
			}
			else
				throw new ArgumentException("Matrix norm: please store the matrix first, then call the norm computation on stored value");

			return result!;
		}

		#endregion

		#region Stored data printing

		public void PrintVariableList()
		{
			if (_variables.Count == 0)
			{
				Console.WriteLine("No variables defined!\n");
				return;
			}

			Console.WriteLine("Declared variables:");
			foreach (var item in _variables)
			{
				Console.WriteLine("{0} = {1}", item.Key, item.Value);
			}
			Console.WriteLine();
		}

		public void PrintFunctionList()
		{
			if (_functions.Count == 0)
			{
				Console.WriteLine("No functions defined!\n");
				return;
			}

			Console.WriteLine("Defined functions:");
			foreach (var item in _functions)
			{
				Console.WriteLine(item.Value);
			}
			Console.WriteLine();
		}

		public void PrintAllLists()
		{
			PrintVariableList();
			PrintFunctionList();
		}

		#endregion

		#region Testing stuff

		public Dictionary<string, MathValue> Variables => _variables;
		public Dictionary<string, Function> Functions => _functions;

		#endregion
	}
}
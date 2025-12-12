using ComputorV2.Core.Lexing;
using ComputorV2.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComputorV2.Core.Math
{
	public record AssignmentInfo(string Variable, MathValue Value);

	public class MathEvaluator
	{
		private Dictionary<string, MathValue> _variables = new();
		private Dictionary<string, Function> _functions = new();
		private AssignmentInfo? _lastAssignment = null;

		public MathValue Evaluate(string expression)
		{
			_lastAssignment = null;
			
			var trimmed = expression.Trim();
			if (IsSimpleVariable(trimmed) && _variables.ContainsKey(trimmed))
			{
				return _variables[trimmed];
			}

			if (IsFunctionCall(trimmed))
			{
				return HandleFunctionCall(trimmed);
			}

			if (IsSimpleVariable(trimmed) && _functions.ContainsKey(trimmed))
			{
				return _functions[trimmed];
			}
			
			if (IsMatrixLiteral(expression))
			{
				return ParseMatrixFromString(expression);
			}
			else if (IsAssignment(expression))
			{
				return HandleAssignment(expression);
			}
			else if (expression.Contains('=') && !expression.Contains("=="))
			{
				if (ContainsVariables(expression))
				{
					return HandlePolynomialEquation(expression);
				}
				throw new ArgumentException($"Invalid assignment: '{expression}'. Variable names must start with a letter.");
			}
			else if (IsComplexNumberExpression(expression))
			{
				// Force complex number evaluation through postfix processing
				var tokenizer = new Tokenizer();
				var tokens = tokenizer.Tokenize(expression);
				var postfix = ConvertToPostfix(tokens);
				return ProcessPostfix(postfix);
			}
			else if (IsPolynomialExpression(expression))
			{
				return new Polynomial(expression);
			}
			else
			{
				var tokenizer = new Tokenizer();
				var tokens = tokenizer.Tokenize(expression);
				
				// Special case: handle simple negative numbers like "-4.3"
				if (tokens.Count == 2 && tokens[0] == "-" && IsNumber(tokens[1]))
				{
					string negativeNumber = "-" + tokens[1];
					if (RationalNumber.TryParse(negativeNumber, out RationalNumber? rational))
					{
						return rational!;
					}
				}
				
				bool hasVariables = tokens.Any(t => IsVariableToken(t) && (_variables.ContainsKey(t) || t == "i"));
				bool hasUnknownVariables = tokens.Any(t => IsVariableToken(t) && !_variables.ContainsKey(t) && t != "i");
				
				if (hasUnknownVariables)
				{
					// For single letters that are undefined, throw an error instead of creating a polynomial
					if (tokens.Count == 1 && IsVariableToken(tokens[0]) && !_variables.ContainsKey(tokens[0]) && tokens[0] != "i")
					{
						throw new ArgumentException($"Undefined variable: '{tokens[0]}'");
					}
					return new Polynomial(expression);
				}
				else if (hasVariables)
				{
					var postfix = ConvertToPostfix(tokens);
					return ProcessPostfix(postfix);
				}
				else
				{
					var resolvedExpression = ResolveVariables(expression);

					if (ContainsVariables(resolvedExpression))
					{
						return new Polynomial(expression);
					}

					var resolvedTokens = tokenizer.Tokenize(resolvedExpression);
					var postfix = ConvertToPostfix(resolvedTokens);
					return ProcessPostfix(postfix);
				}
			}
		}

		private bool IsFunctionCall(string expression)
		{
			return System.Text.RegularExpressions.Regex.IsMatch(expression, @"^([a-zA-Z][a-zA-Z0-9_]*)\s*\(.+\)$");
		}

		private MathValue HandleFunctionCall(string expression)
		{
			var match = System.Text.RegularExpressions.Regex.Match(expression, @"^([a-zA-Z][a-zA-Z0-9_]*)\s*\((.+)\)$");
			if (!match.Success)
				throw new ArgumentException($"Invalid function call: {expression}");

			string functionName = match.Groups[1].Value;
			string argumentExpression = match.Groups[2].Value.Trim();

			if (!_functions.ContainsKey(functionName))
				throw new ArgumentException($"Unknown function: {functionName}");

			var function = _functions[functionName];
			
			MathValue argumentValue = Evaluate(argumentExpression);
			
			return function.Evaluate(argumentValue);
		}

		private bool ContainsVariables(string expression)
		{
			return System.Text.RegularExpressions.Regex.IsMatch(expression, @"[a-zA-Z]");
		}

		private bool IsPolynomialExpression(string expression)
		{
			// Check for polynomial patterns but exclude cases where the variable is 'i' (imaginary unit)
			var patterns = new[]
			{
				@"[a-zA-Z]\^",                    // variable^power
				@"\d+\s*\*\s*[a-zA-Z]",          // number * variable  
				@"\d+[a-zA-Z]"                    // number followed by variable (implicit multiplication)
			};

			foreach (var pattern in patterns)
			{
				var matches = System.Text.RegularExpressions.Regex.Matches(expression, pattern);
				foreach (System.Text.RegularExpressions.Match match in matches)
				{
					// If this match contains 'i' as the variable, don't treat it as a polynomial
					if (match.Value.Contains('i'))
					{
						// Extract the variable part to check if it's exactly 'i'
						var variablePart = System.Text.RegularExpressions.Regex.Match(match.Value, @"[a-zA-Z]+").Value;
						if (variablePart == "i")
							continue; // Skip this match, it's the imaginary unit
					}
					return true; // Found a genuine polynomial pattern
				}
			}
			return false;
		}
		

		private List<string> ConvertToPostfix(List<string> tokens)
		{
			var operators = new Stack<string>();
			var output = new Queue<string>();
			
			for (int i = 0; i < tokens.Count; i++)
			{
				var token = tokens[i];
				
				if (IsNumber(token) || (IsVariableToken(token) && _variables.ContainsKey(token)) || token == "i")
				{
					output.Enqueue(token);
				}
				else if (token == "-" && IsUnaryMinus(i, tokens))
				{
					// Handle unary minus by treating it as multiplication by -1
					output.Enqueue("-1");
					operators.Push("*");
				}
				else if ("+-*/^".Contains(token))
				{
					while (operators.Count > 0 &&
						operators.Peek() != "(" &&
						HasHigherOrEqualPrecedence(operators.Peek(), token))
					{
						output.Enqueue(operators.Pop());
					}
					operators.Push(token);
				}
				else if (token == "(")
				{
					operators.Push(token);
				}
				else if (token == ")")
				{
					while (operators.Count > 0 && operators.Peek() != "(")
					{
						output.Enqueue(operators.Pop());
					}
					operators.Pop();
				}
			}

			while (operators.Count > 0)
			{
				output.Enqueue(operators.Pop());
			}

			return output.ToList();
		}

		private bool IsUnaryMinus(int index, List<string> tokens)
		{
			// Unary minus if:
			// 1. First token
			// 2. After an operator
			// 3. After opening parenthesis
			if (index == 0) return true;
			
			var previousToken = tokens[index - 1];
			return "+-*/^(".Contains(previousToken);
		}

		private bool IsNumber(string token)
		{
			return RationalNumber.TryParse(token, out _) || 
				ComplexNumber.TryParse(token, out _) ||
				IsMatrixToken(token);
		}

		private bool IsMatrixToken(string token)
		{
			return token.Trim().StartsWith("[") && token.Trim().EndsWith("]");
		}

		private bool HasHigherOrEqualPrecedence(string A, string B)
		{
			return GetPrecedence(A) >= GetPrecedence(B);
		}

		private int GetPrecedence(string op)
		{
			return op switch
			{
				"+" or "-" => 1,
				"*" or "/" => 2,
				"^" => 3,
				_ => 0
			};
		}

		private MathValue ProcessPostfix(List<string> tokens)
		{
			var stack = new Stack<MathValue>();

			foreach (var token in tokens)
			{
				if (IsNumber(token))
				{
					if (RationalNumber.TryParse(token, out RationalNumber? rational))
					{
						stack.Push(rational!);
					}
					else if (ComplexNumber.TryParse(token, out ComplexNumber? complex))
					{
						stack.Push(complex!);
					}
					else if (IsMatrixToken(token))
					{
						try
						{
							var matrix = new Matrix(token);
							stack.Push(matrix);
						}
						catch (Exception ex)
						{
							throw new ArgumentException($"Invalid matrix format: {token}", ex);
						}
					}
					else
					{
						throw new ArgumentException($"Invalid number format: {token}");
					}
				}
				else if (IsVariableToken(token) && _variables.ContainsKey(token))
				{
					stack.Push(_variables[token]);
				}
				else if (token == "i")
				{
					// Handle 'i' as the imaginary unit constant
					stack.Push(ComplexNumber.I);
				}
				else if ("+-*/^".Contains(token))
				{
					if (stack.Count < 2)
						throw new InvalidOperationException($"Insufficient operands for operator: {token}");
					
					var right = stack.Pop();
					var left = stack.Pop();
					var result = ApplyOperation(left, right, token);
					stack.Push(result);
				}
				else
				{
					throw new ArgumentException($"Unknown token: {token}");
				}
			}

			if (stack.Count != 1)
				throw new InvalidOperationException("Invalid expression: expected single result");

			return stack.Pop();
		}

		private MathValue ApplyOperation(MathValue left, MathValue right, string op)
		{
			return op switch
			{
				"+" => left.Add(right),
				"-" => left.Subtract(right),
				"*" => left.Multiply(right),
				"/" => left.Divide(right),
				"^" => ApplyPowerOperation(left, right),
				_ => throw new ArgumentException($"Unknown operator: {op}")
			};
		}

		private MathValue ApplyPowerOperation(MathValue baseValue, MathValue exponent)
		{
			var rationalExp = exponent.AsRational();
			if (rationalExp == null || !rationalExp.IsInteger)
			{
				throw new ArgumentException("Only integer exponents are currently supported");
			}

			int exp = (int)rationalExp.Numerator;
			return baseValue.Power(exp);
		}

		public bool IsMatrixLiteral(string expression)
		{
			var trimmed = expression.Trim();
			if (!trimmed.StartsWith("[") || !trimmed.EndsWith("]"))
        		return false;
			
			if (HasArithmeticOperators(trimmed))
				return false;
			
			return trimmed.Contains(",") ||
					trimmed.Contains(";") ||
					trimmed.StartsWith("[[");
		}
		
		private bool HasArithmeticOperators(string expression)
		{
			int bracketDepth = 0;
			for (int i = 0; i < expression.Length; i++)
			{
				char c = expression[i];
				if (c == '[') bracketDepth++;
				else if (c == ']') bracketDepth--;
				else if (bracketDepth == 0 && "+-*/^".Contains(c))
					return true;
			}
			return false;
		}

		private Matrix ParseMatrixFromString(string expression)
		{
			try
			{
				return new Matrix(expression.Trim());
			}
			catch (Exception ex)
			{
				throw new ArgumentException($"Invalid matrix format: ${expression}", ex);
			}
		}

		public bool IsAssignment(string expression)
		{
			if (!expression.Contains('=') || expression.Contains("=="))
				return false;

			var parts = expression.Split('=');
			if (parts.Length != 2)
				return false;

			var leftSide = parts[0].Trim();

			if (IsFunctionDefinition(leftSide))
				return true;

			return IsSimpleVariable(leftSide);
		}

		private bool IsFunctionDefinition(string leftSide)
		{
			var match = System.Text.RegularExpressions.Regex.Match(leftSide, @"^([a-zA-Z][a-zA-Z0-9_]*)\s*\(\s*([a-zA-Z][a-zA-Z0-9_]*)\s*\)$");
			return match.Success;
		}

		private bool IsSimpleVariable(string token)
		{
			return !string.IsNullOrEmpty(token) &&
				char.IsLetter(token[0]) &&
				token.All(c => char.IsLetterOrDigit(c)) &&
				!token.Any(c => "+-*/^()=".Contains(c)) &&
				token.Length <= 20;
		}
		
		private MathValue HandleAssignment(string expression)
		{
			var parts = expression.Split('=');
			var leftSide = parts[0].Trim();
			var valueExpression = parts[1].Trim();
			
			if (IsFunctionDefinition(leftSide))
			{
				return HandleFunctionAssignment(leftSide, valueExpression);
			}
			
			if (leftSide.Contains('(') || leftSide.Contains(')'))
			{
				throw new ArgumentException($"Invalid variable name or function definition: {leftSide}");
			}
			
			if (!IsSimpleVariable(leftSide))
			{
				throw new ArgumentException($"Invalid variable name: {leftSide}");
			}

			MathValue value;

			if (IsMatrixLiteral(valueExpression))
			{
				value = new Matrix(valueExpression);
				_variables[leftSide] = value;
			}
			else if (RationalNumber.TryParse(valueExpression, out RationalNumber? parsedRational))
			{
				value = parsedRational!;
				_variables[leftSide] = value;
			}
			else if (ComplexNumber.TryParse(valueExpression, out ComplexNumber? parsedComplex))
			{
				value = parsedComplex!;
				_variables[leftSide] = value;
			}
			else if (IsPolynomialExpression(valueExpression))
			{
				value = new Polynomial(valueExpression);
				_variables[leftSide] = value;
			}
			else
			{
				var resolvedExpression = ResolveVariables(valueExpression);
				value = Evaluate(resolvedExpression);
				_variables[leftSide] = value;
			}

			_lastAssignment = new AssignmentInfo(leftSide, value);
			
			return value;
		}

		private MathValue HandleFunctionAssignment(string leftSide, string expression)
		{
			var match = System.Text.RegularExpressions.Regex.Match(leftSide, @"^([a-zA-Z][a-zA-Z0-9_]*)\s*\(\s*([a-zA-Z][a-zA-Z0-9_]*)\s*\)$");
			if (!match.Success)
				throw new ArgumentException($"Invalid function definition: {leftSide}");

			string functionName = match.Groups[1].Value;
			string variable = match.Groups[2].Value;

			string resolvedExpression = ResolveVariablesExcept(expression, variable);

			var function = new Function(functionName, variable, resolvedExpression);
			_functions[functionName] = function;

			return function;
		}
		
		public AssignmentInfo? GetLastAssignmentInfo()
		{
			return _lastAssignment;
		}

		private string ResolveVariables(string expression)
		{
			var tokenizer = new Tokenizer();
			var tokens = tokenizer.Tokenize(expression);

			for (int i = 0; i < tokens.Count; i++)
			{
				if (IsVariableToken(tokens[i]))
				{
					if (tokens[i] == "i")
					{
						continue;
					}
					
					if (i + 1 < tokens.Count && tokens[i + 1] == "(")
					{
						continue;
					}
					
					if (_functions.ContainsKey(tokens[i]))
					{
						continue;
					}
					
					if (_variables.ContainsKey(tokens[i]))
					{
						var value = _variables[tokens[i]];

						if (value is Matrix matrix)
						{
							tokens[i] = matrix.ToString();
						}
						else
						{
							tokens[i] = value.ToString();
						}
					}
					else
					{
						throw new InvalidOperationException($"Undefined variable: '{tokens[i]}'. Assign a value to the variable first.");
					}
				}
			}
			
			var result = string.Join(" ", tokens);
			return result;
		}

		private bool IsVariableToken(string token)
		{
			if (string.IsNullOrEmpty(token))
				return false;

			return char.IsLetter(token[0]) && token.All(c => char.IsLetterOrDigit(c));
		}

		public Dictionary<string, MathValue> GetVariables()
		{
			return new Dictionary<string, MathValue>(_variables);
		}

		public void ClearVariables()
		{
			_variables.Clear();
		}

		public void ClearFunctions()
		{
			_functions.Clear();
		}

		public void ClearAll()
		{
			_variables.Clear();
			_functions.Clear();
		}

		public MathValue? GetVariable(string name)
		{
			return _variables.TryGetValue(name, out MathValue? value) ? value : null;
		}

		public void SetVariable(string name, MathValue value)
		{
			_variables[name] = value;
		}

		private MathValue HandlePolynomialEquation(string expression)
		{
			var parts = expression.Split('=');
			if (parts.Length != 2)
			{
				throw new ArgumentException("Invalid equation format");
			}

			var leftSide = parts[0].Trim();
			var rightSide = parts[1].Trim();

			var leftPoly = new Polynomial(leftSide);
			var rightPoly = new Polynomial(rightSide);

			var equation = leftPoly.Subtract(rightPoly);

			if (equation is Polynomial poly)
			{
				var solutions = poly.Solve();
				
				if (solutions.Count == 0)
				{
					throw new InvalidOperationException("No solution exists for this equation");
				}
				else if (solutions.Count == 1)
				{
					return solutions[0];
				}
				else
				{
					var solutionStrings = solutions.Select(s => s.ToString());
					Console.WriteLine($"Info: Multiple solutions: {string.Join(", ", solutionStrings)}");
					return solutions[0];
				}
			}

			throw new InvalidOperationException("Unable to solve equation");
		}

		#region Function management

		public Function? GetFunction(string name)
		{
			return _functions.TryGetValue(name, out Function? value) ? value : null;
		}

		public void SetFunction(string name, Function function)
		{
			_functions[name] = function;
		}

		public Dictionary<string, Function> GetFunctions()
		{
			return new Dictionary<string, Function>(_functions);
		}

		#endregion

		private string ResolveVariablesExcept(string expression, string excludeVariable)
		{
			var tokenizer = new Tokenizer();
			var tokens = tokenizer.Tokenize(expression);

			for (int i = 0; i < tokens.Count; i++)
			{
				if (IsVariableToken(tokens[i]))
				{
					if (tokens[i] == excludeVariable)
					{
						continue;
					}
					
					if (tokens[i] == "i")
					{
						continue;
					}
					
					if (i + 1 < tokens.Count && tokens[i + 1] == "(")
					{
						continue;
					}

					if (_functions.ContainsKey(tokens[i]))
					{
						continue;
					}
					
					if (_variables.ContainsKey(tokens[i]))
					{
						var value = _variables[tokens[i]];

						if (value is Matrix matrix)
						{
							tokens[i] = matrix.ToString();
						}
						else
						{
							tokens[i] = value.ToString();
						}
					}
					else
					{
						throw new InvalidOperationException($"Undefined variable: '{tokens[i]}'. Assign a value to the variable first.");
					}
				}
			}
			
			var result = string.Join(" ", tokens);
			return result;
		}

		private bool IsComplexNumberExpression(string expression)
		{
			// Check for patterns that indicate complex number expressions with 'i'
			var complexPatterns = new[]
			{
				@"\bi\b",                         // standalone 'i'
				@"\d+\s*\*\s*i\b",               // number * i
				@"\d+i\b",                        // implicit multiplication like 4i
				@"[+-]\s*i\b",                    // +i or -i
				@"[+-]\s*\d+\s*\*\s*i\b",        // +/- number * i
				@"[+-]\s*\d+i\b"                 // +/- numberi
			};

			foreach (var pattern in complexPatterns)
			{
				if (System.Text.RegularExpressions.Regex.IsMatch(expression, pattern))
				{
					return true;
				}
			}
			return false;
		}
	}
}

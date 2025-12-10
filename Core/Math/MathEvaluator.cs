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
		private AssignmentInfo? _lastAssignment = null;

		/// <summary>
		/// Check if expression contains variables (letters).
		/// </summary>
		private bool ContainsVariables(string expression)
		{
			return System.Text.RegularExpressions.Regex.IsMatch(expression, @"[a-zA-Z]");
		}

		/// <summary>
		/// Check if expression is a polynomial with variable terms (like x^2, 3x, etc.)
		/// This should NOT match simple variable references that can be resolved from storage.
		/// </summary>
		private bool IsPolynomialExpression(string expression)
		{
			// Look for polynomial patterns that clearly indicate polynomial expressions
			// rather than simple variable references
			return System.Text.RegularExpressions.Regex.IsMatch(expression, 
				@"[a-zA-Z]\^" + // variable followed by ^ (like x^2)
				@"|\d+\s*\*\s*[a-zA-Z]" + // number * variable (like 3*x)
				@"|\d+[a-zA-Z]"); // number directly followed by variable (like 2x)
		}
		
		public MathValue Evaluate(string expression)
		{
			_lastAssignment = null;
			
			if (IsAssignment(expression))
			{
				return HandleAssignment(expression);
			}
			else if (expression.Contains('=') && !expression.Contains("=="))
			{
				// Check if it's a polynomial equation (contains variable and equals)
				if (ContainsVariables(expression))
				{
					return HandlePolynomialEquation(expression);
				}
				throw new ArgumentException($"Invalid assignment: '{expression}'. Variable names must start with a letter.");
			}
			else if (IsPolynomialExpression(expression))
			{
				// Expression looks like a polynomial - treat as polynomial
				return new Polynomial(expression);
			}
			else
			{
				var resolvedExpression = ResolveVariables(expression);

				// If after resolving variables, the expression still contains variables,
				// then treat it as a polynomial
				if (ContainsVariables(resolvedExpression))
				{
					return new Polynomial(expression);
				}

				var tokenizer = new Tokenizer();
				var tokens = tokenizer.Tokenize(resolvedExpression);

				var postfix = ConvertToPostfix(tokens);

				var result = ProcessPostfix(postfix);

				return result;
			}
		}

		private List<string> ConvertToPostfix(List<string> tokens)
		{
			var operators = new Stack<string>();
			var output = new Queue<string>();
			
			foreach (var token in tokens)
			{				
				if (IsNumber(token))
				{
					output.Enqueue(token);
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

		private bool IsNumber(string token)
		{
			return RationalNumber.TryParse(token, out _) || 
				ComplexNumber.TryParse(token, out _);
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
					else
					{
						throw new ArgumentException($"Invalid number format: {token}");
					}
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

		public bool IsAssignment(string expression)
		{
			if (!expression.Contains('=') || expression.Contains("=="))
				return false;

			var parts = expression.Split('=');
			if (parts.Length != 2)
				return false;

			var leftSide = parts[0].Trim();

			return IsSimpleVariable(leftSide);
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
			var key = parts[0].Trim();
			var valueExpression = parts[1].Trim();
			
			MathValue value;
			if (RationalNumber.TryParse(valueExpression, out RationalNumber? parsedRational))
			{
				value = parsedRational!;
				_variables[key] = value;
			}
			else if (ComplexNumber.TryParse(valueExpression, out ComplexNumber? parsedComplex))
			{
				value = parsedComplex!;
				_variables[key] = value;
			}
			else if (IsPolynomialExpression(valueExpression))
			{
				// Value expression contains variables - treat as polynomial
				value = new Polynomial(valueExpression);
				_variables[key] = value;
			}
			else
			{
				var resolvedExpression = ResolveVariables(valueExpression);
				value = Evaluate(resolvedExpression);
				_variables[key] = value;
			}

			_lastAssignment = new AssignmentInfo(key, value);
			
			return value;
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
					if (_variables.ContainsKey(tokens[i]))
					{
						tokens[i] = _variables[tokens[i]].ToString();
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

		public MathValue? GetVariable(string name)
		{
			return _variables.TryGetValue(name, out MathValue? value) ? value : null;
		}

		public void SetVariable(string name, MathValue value)
		{
			_variables[name] = value;
		}

		/// <summary>
		/// Handle polynomial equations like "x^2 + 2*x + 1 = 0"
		/// </summary>
		private MathValue HandlePolynomialEquation(string expression)
		{
			var parts = expression.Split('=');
			if (parts.Length != 2)
			{
				throw new ArgumentException("Invalid equation format");
			}

			var leftSide = parts[0].Trim();
			var rightSide = parts[1].Trim();

			// Parse both sides as polynomials
			var leftPoly = new Polynomial(leftSide);
			var rightPoly = new Polynomial(rightSide);

			// Move right side to left: leftPoly - rightPoly = 0
			var equation = leftPoly.Subtract(rightPoly);

			if (equation is Polynomial poly)
			{
				// Solve the polynomial equation
				var solutions = poly.Solve();
				
				// For now, return a formatted string of solutions
				// In a full implementation, this might return a special SolutionSet type
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
					// Multiple solutions - return as a formatted string for now
					var solutionStrings = solutions.Select(s => s.ToString());
					Console.WriteLine($"Info: Multiple solutions: {string.Join(", ", solutionStrings)}");
					return solutions[0]; // Return first solution
				}
			}

			throw new InvalidOperationException("Unable to solve equation");
		}
	}
}

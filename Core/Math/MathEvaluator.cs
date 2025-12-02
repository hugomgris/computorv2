using ComputorV2.Core.Lexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComputorV2.Core.Math
{
	public record AssignmentInfo(string Variable, decimal Value);

	public class MathEvaluator
	{
		private Dictionary<string, decimal> _variables = new Dictionary<string, decimal>();
		private AssignmentInfo? _lastAssignment = null;
		
		public decimal Evaluate(string expression)
		{
			_lastAssignment = null;
			
			if (IsAssignment(expression))
			{
				return HandleAssignment(expression);
			}
			else if (expression.Contains('=') && !expression.Contains("=="))
			{
				throw new ArgumentException($"Invalid assignment: '{expression}'. Variable names must start with a letter.");
			}
			else
			{
				var resolvedExpression = ResolveVariables(expression);

				var tokenizer = new Tokenizer();
				var tokens = tokenizer.Tokenize(resolvedExpression);

				var postfix = convertToPostfix(tokens);

				var result = processPostfix(postfix);

				return result;
			}
		}

		List<string> convertToPostfix(List<string> tokens)
		{
			var operators = new Stack<string>();
			var output = new Queue<string>();
			
			foreach (var token in tokens)
			{				
				if (IsNumber(token))
				{
					output.Enqueue(token);
				}
				else if ("+-*/".Contains(token))
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
			return decimal.TryParse(token, out _);
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
				_ => 0
			};
		}

		private decimal processPostfix(List<string> tokens)
		{
			var stack = new Stack<decimal>();

			foreach (var token in tokens)
			{
				if (IsNumber(token))
				{
					if (decimal.TryParse(token, out decimal number))
					{
						stack.Push(number);
					}
					else
					{
						throw new ArgumentException($"Invalid number format: {token}");
					}
				}
				else if ("+-*/".Contains(token))
				{
					if (stack.Count < 2)
					{
						throw new InvalidOperationException($"Insufficient operands for operator: {token}");
					}
					
					var right = stack.Pop();
					var left = stack.Pop();
					
					var result = ApplyOperation(left, right, token);
					stack.Push(result);
				}
			}

			if (stack.Count != 1)
			{
				throw new InvalidOperationException("Invalid expression: expected single result");
			}

			return stack.Pop();
		}

		private decimal ApplyOperation(decimal left, decimal right, string op)
		{
			return op switch
			{
				"+" => left + right,
				"-" => left - right,
				"*" => left * right,
				"/" => right == 0 ? throw new DivideByZeroException("Division by zero") : left / right,
				_ => throw new ArgumentException($"Unknown operator: {op}")
			};
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
		
		private decimal HandleAssignment(string expression)
		{
			var parts = expression.Split('=');
			var key = parts[0].Trim();
			var valueExpression = parts[1].Trim();
			
			decimal value;
			if (decimal.TryParse(valueExpression, out value))
			{
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
	}
}
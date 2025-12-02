using ComputorV2.Core.Lexing;

namespace ComputorV2.Core.Math
{
	public class MathEvaluator
	{
		public decimal Evaluate(string expression)
		{
			var tokenizer = new Tokenizer();
			var tokens = tokenizer.Tokenize(expression);

			var postfix = convertToPostfix(tokens);

			var result = processPostfix(postfix);

			return result;
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
	}
}
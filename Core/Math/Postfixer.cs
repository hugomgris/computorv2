using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using ComputorV2.Core.Types;

namespace ComputorV2.Core.Math
{	
	public class Postfix
    {
		public Stack<string> InfixTokens { get; set; }
		public Stack<string> PostfixTokens { get; set; }

		public Postfix(List<string> rawTokens)
		{
			PostfixTokens = new Stack<string>();

			// 1. Tokens -> Infix Stack
			InfixTokens = new Stack<string>(rawTokens);

			// 2. Build the Postfix stack
			// 2.1 Reverse the Infix Stack
			Stack<string> InfixStack = new Stack<string>(InfixTokens);

			// 2.2 New stacks for postfix management
			Stack<string> output = new Stack<string>();
			Stack<string> operators = new Stack<string>();

			// 2.3 Postfix building pipeline (Shunting yard -> RPN)

			while (InfixStack.Count > 0)
			{
				string currentToken = InfixStack.Pop();

				// DEBUG
				//Console.WriteLine($"Managing token:{currentToken}");

				if (IsOperator(currentToken))
				{
					while (operators.Count > 0 && IsOperator(operators.Peek()))
					{
						string currentOperator = currentToken;
						string nextOperator = operators.Peek();

						if ((GetAssociativeness(currentOperator) == Associativenesses.Left && GetPrecedence(currentOperator) <= GetPrecedence(nextOperator))
							|| (GetAssociativeness(currentOperator) == Associativenesses.Right && GetPrecedence(currentOperator) < GetPrecedence(nextOperator)))
						{
							output.Push(operators.Pop());
						}
						else
						{
							break;
						}
					}
					operators.Push(currentToken);
				}
				else if (IsParenthesis(currentToken))
				{
					switch (GetParenthesisType(currentToken))
					{
						case ParenthesisTypes.Open:
							operators.Push(currentToken);
							break;
						case ParenthesisTypes.Close:
							while (operators.Count > 0)
							{
								string nextOperator = operators.Peek();
								if (IsParenthesis(nextOperator) && GetParenthesisType(nextOperator) == ParenthesisTypes.Open)
									break;
								output.Push(operators.Pop());
							}
							operators.Pop();
							break;
					}
				}
				else if (IsNumeric(currentToken) != NumberType.NoNumber)
				{
					output.Push(currentToken);
				}
			}

			while (operators.Count > 0)
			{
				output.Push(operators.Pop());
			}

			Stack<string> reversedStack = new Stack<string>();
			while (output.Count > 0) reversedStack.Push(output.Pop());

			PostfixTokens = reversedStack;

			// DEBUG
			//foreach(string tok in PostfixTokens) Console.WriteLine(tok);
		}

		public MathValue Calculate()
		{
			Stack<MathValue> EvaluationStack = new Stack<MathValue>();

			Stack<string> reversedStack = new Stack<string>(PostfixTokens);
			Stack<string> PostfixStack = new Stack<string>();

			while (reversedStack.Count > 0) PostfixStack.Push(reversedStack.Pop());

			// DEBUG
			//foreach (string tok in PostfixStack) Console.WriteLine(tok);

			while (PostfixStack.Count > 0)
			{
				string currentToken = PostfixStack.Pop();
				
				if (IsNumeric(currentToken) != NumberType.NoNumber)
				{
					if (ComplexNumber.TryParse(currentToken, out _))
					{
						ComplexNumber cn = new ComplexNumber(currentToken);
						EvaluationStack.Push(cn);
					}
					else if (RationalNumber.TryParse(currentToken, out _))
					{
						RationalNumber rn = new RationalNumber(currentToken);
						EvaluationStack.Push(rn);
					}
					else
						throw new ArgumentException("Postfix Calculation: error while managing number token");
				}
				else if (IsOperator(currentToken))
				{
					MathValue firstValue = EvaluationStack.Pop();
					MathValue secondValue = EvaluationStack.Pop();

					MathValue result;

					if (currentToken == "+")
					{
						result = secondValue.Add(firstValue);
					}
					else if (currentToken == "-")
					{
						result = secondValue.Subtract(firstValue);
					}
					else if (currentToken == "*")
					{
						result = secondValue.Multiply(firstValue);
					}
					else if (currentToken == "/")
					{
						result = secondValue.Divide(firstValue);
					}
					else if (currentToken == "%")
					{
						result = secondValue.Modulo(firstValue);
					}
					else
						throw new ArgumentException("Postfixer: Unhandled operator in calculation");
						
					EvaluationStack.Push(result);
				}
				else
						throw new ArgumentException($"Postfixer: Unexpected Token type in calculation {currentToken}");
			}

			return EvaluationStack.Peek();
		}

		#region Helpers

		public enum Associativenesses
		{
			Left,
			Right
		}

		public enum ParenthesisTypes
		{
			Open,
			Close
		}

		public enum NumberType
		{
			Rational,
			Complex,
			NoNumber
		}

		public bool IsOperator(string token)
		{
			return token.Length == 1 && "-*/+%".Contains(token);
		}

		public NumberType IsNumeric(string token)
		{
			
			if (ComplexNumber.TryParse(token, out _))
				return NumberType.Complex;
			else if (RationalNumber.TryParse(token, out _))
				return NumberType.Rational;
			return NumberType.NoNumber;
		}

		public bool IsParenthesis(string token)
		{
			return token == "(" || token == ")";
		}

		public int GetPrecedence(string token)
		{
			switch (token)
			{
				case "!":
					return 4;
				case "*":
				case "/":
				case "%":
					return 3;
				case "+":
				case "-":
					return 2;
				case "=":
					return 1;
				default:
					throw new Exception("Postfixer: Invalid Operator Type for Precedence get");
			}
		}

		public Associativenesses GetAssociativeness(string token)
		{
			switch (token)
			{
				case "=":
				case "!":
					return Associativenesses.Right;
				case "+":
				case "-":
				case "*":
				case "/":
				case "%":
					return Associativenesses.Left;
				default:
					throw new Exception("Postfixer: Invalid Operator Type for Associativeness get");
			}
		}

		public ParenthesisTypes GetParenthesisType(string token)
		{
			if (token == "(") return ParenthesisTypes.Open;
			return ParenthesisTypes.Close;
		}

		public MathValue MakeOperation(MathValue firstValue, MathValue secondValue, string op)
		{
			if (op == "+")
			{
				return firstValue.Add(secondValue);
			}
			else if (op == "-")
			{
				return firstValue.Subtract(secondValue);
			}
			else if (op == "*")
			{
				return firstValue.Multiply(secondValue);
			}
			else if (op == "/")
			{
				return firstValue.Divide(secondValue);
			}
			else if (op == "%")
			{
				return firstValue.Modulo(secondValue);
			}
			else
				throw new ArgumentException("Postfixer: Unhandled operator in calculation");
		}

		#endregion
	}

}
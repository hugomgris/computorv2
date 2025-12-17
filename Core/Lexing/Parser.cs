using System;
using System.Collections;
using System.Text;

using ComputorV2.Core.Math;
using ComputorV2.Core.Types;

namespace ComputorV2.Core.Lexing
{
	public enum cmd_type
	{
		FUNCTION,
		ASSIGNMENT,
		RATIONAL,
		COMPLEX,
		MATRIX,
		
		POLYNOMIAL,
		INVALID
	};
	
	public enum var_error
	{
		INVALIDCHAR,
		NOALPHA,
		HASICHAR,
		GOOD
	}

	public class Parser
	{
		/*
		fun(x) = 2x + 3
		a = (2 + 3) * 5

		*/
		public cmd_type DetectInputType(string input)
		{
			if (input.Contains(")="))
				return cmd_type.FUNCTION;
			else
				return cmd_type.ASSIGNMENT;
			
		}

		public cmd_type DetectValueType(string value)
		{
			if (value.Contains("["))
				return cmd_type.MATRIX;
			else if (value.Contains("i"))
				return cmd_type.COMPLEX;
			else
				return cmd_type.RATIONAL;
		}

		public var_error ValidateVariableName(string name)
		{
			int alpha = 0;
			
			foreach(char c in name)
			{
				if (!Char.IsLetterOrDigit(c))
					return var_error.INVALIDCHAR;

				if (c == 'i')
					return var_error.HASICHAR;
				
				if (char.IsLetter(c))
					alpha++;
			}

			if (alpha == 0)
				return var_error.NOALPHA;

			return var_error.GOOD;
		}

		public string ResolveVariables(string input)
		{
			return input;
		}

		// This is a huge function, pretty sure it could be simpler and cleaner and etc, but I'm proud of it :D
		// It kinda has a "tweaked stuff as I found edge cases along the way" vibe, which calls for a refactoring, but I don't feel like it :(
		public string SimplifyComplexExpression(string expression)
		{	
			string realPart = "";
			string imaginaryPart = "";
			int anchor = 0;

			for (int i = 0; i < expression.Length; i++)
			{
				if (expression[i] == 'i')
				{
					for (int j = i - 1; j >= 0 && expression[j] != 'i'; j--)
					{
						if ("+-".Contains(expression[j]))
						{
							if (anchor == 0 && j != 0)
								realPart += expression[0].ToString();
							imaginaryPart += expression.Substring(j, i - j + 1).Replace("*", "");
							realPart += expression.Substring(anchor + 1, j - anchor);
							if (realPart.EndsWith("+") || realPart.EndsWith("-"))
								realPart = realPart.Substring(0, realPart.Length - 1);
							anchor = i;
							break;
						}
						else if (j == 0)
						{
							imaginaryPart += expression.Substring(0, i + 1);
							anchor = i;
						}
					}
				}
			}

			if (anchor < expression.Length - 1)
				realPart += expression.Substring(anchor + 1, expression.Length - anchor - 1);

			Console.WriteLine($"realPart:{realPart}");
			Console.WriteLine($"imaginaryPart:{imaginaryPart}");

			if (realPart.Length > 0 && realPart[0] == '+')
				realPart = realPart.Substring(1);
			if (imaginaryPart.Length > 0 && imaginaryPart[0] == '+')
				imaginaryPart = imaginaryPart.Substring(1);
			
			StringBuilder sb = new StringBuilder();

			for (int k = 0; k< imaginaryPart.Length; k++)
			{
				if (imaginaryPart[k] == 'i')
				{
					if (k == 0 || !Char.IsNumber(imaginaryPart[k - 1]))
					{
						sb.Append("1");
						continue;
					}
					else
						continue;
				}
				
				sb.Append(imaginaryPart[k].ToString());
			}

			string cleanedImaginary = sb.ToString();

			Postfix realPostfix;
			Postfix imaginaryPostfix;

			decimal realValue;
			decimal imaginaryValue;
			
			if (realPart.Length > 0)
			{
				if (!decimal.TryParse(realPart, out _))
				{
					realPostfix = new Postfix(realPart);
					realValue = realPostfix.Calculate();
				}
				else
					realValue = decimal.Parse(realPart);
			}
			else
				realValue = 0;

			if (!decimal.TryParse(cleanedImaginary, out _))
			{
				imaginaryPostfix = new Postfix(cleanedImaginary);
				imaginaryValue = imaginaryPostfix.Calculate();
			}
			else
				imaginaryValue = decimal.Parse(cleanedImaginary);

			ComplexNumber simplified = new ComplexNumber(new RationalNumber(realValue), new RationalNumber(imaginaryValue));

			return simplified.ToString();
		}
	}
}
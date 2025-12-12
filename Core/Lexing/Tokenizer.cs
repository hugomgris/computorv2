using System.Text;

namespace ComputorV2.Core.Lexing
{
	public class Tokenizer
	{
		public List<string> Tokenize(string expression)
		{
			var tokens = new List<string>();
			var currentToken = new StringBuilder();

			for (int i = 0; i < expression.Length; i++)
			{
				char c = expression[i];

				if (c == '[')
				{
					if ((i + 1 < expression.Length && expression[i + 1] == '[') || 
					    LooksLikeMatrixLiteral(expression, i))
					{
						var matrixToken = ConsumeMatrixLiteral(expression, ref i);
						if (currentToken.Length > 0)
						{
							tokens.Add(currentToken.ToString());
							currentToken.Clear();
						}
						tokens.Add(matrixToken);
						continue;
					}
				}
				else if (char.IsDigit(c) || c == '.')
				{
					currentToken.Append(c);
				}
				else if (c == '/' && currentToken.Length > 0 && char.IsDigit(currentToken[^1]))
				{
					if (i + 1 < expression.Length && LooksLikeFractionDenominator(expression, i + 1))
					{
						currentToken.Append(c);
					}
					else
					{
						if (currentToken.Length > 0)
						{
							tokens.Add(currentToken.ToString());
							currentToken.Clear();
						}
						tokens.Add(c.ToString());
					}
				}
				else if (char.IsLetter(c) || (c == '_' && currentToken.Length > 0))
				{
					currentToken.Append(c);
				}
				else if ("+-*/%()^=;".Contains(c))
				{
					if (currentToken.Length > 0)
					{
						tokens.Add(currentToken.ToString());
						currentToken.Clear();
					}
					tokens.Add(c.ToString());
				}
				else if (c == '[' || c == ']')
				{
					if (currentToken.Length > 0)
					{
						tokens.Add(currentToken.ToString());
						currentToken.Clear();
					}
					tokens.Add(c.ToString());
				}
				else if (char.IsWhiteSpace(c))
				{
					if (currentToken.Length > 0)
					{
						tokens.Add(currentToken.ToString());
						currentToken.Clear();
					}
				}
				else
				{
					if (currentToken.Length > 0)
					{
						tokens.Add(currentToken.ToString());
						currentToken.Clear();
					}

					string invalidChar = c.ToString();
					throw new ArgumentException($"Invalid character '{invalidChar}' in expression. Variable names must start with a letter and contain only letters, digits, and underscores.");
				}
			}

			if (currentToken.Length > 0)
			{
				tokens.Add(currentToken.ToString());
			}

			return tokens;
		}

		private bool LooksLikeFractionDenominator(string expr, int startIndex)
		{
			for (int i = startIndex; i < expr.Length; i++)
			{
				char c = expr[i];
				if (char.IsDigit(c) || c == '.') continue;
				if (char.IsWhiteSpace(c) || "+-*/%()^=".Contains(c)) break;
				return false;
			}
			return true;
		}

		private bool LooksLikeMatrixLiteral(string expression, int startIndex)
		{
			int bracketCount = 0;
			bool hasMatrixContent = false;
			bool hasNumbers = false;
			
			for (int i = startIndex; i < expression.Length; i++)
			{
				char c = expression[i];
				
				if (c == '[') 
				{
					bracketCount++;
				}
				else if (c == ']') 
				{
					bracketCount--;
					if (bracketCount == 0) break;
				}
				else if (c == ',' || c == ';')
				{
					hasMatrixContent = true;
				}
				else if (char.IsDigit(c))
				{
					hasNumbers = true;
				}
				else if (char.IsLetter(c) && bracketCount == 1)
				{
					string ahead = "";
					int j = i;
					while (j < expression.Length && char.IsLetter(expression[j]))
					{
						ahead += expression[j];
						j++;
					}
					
					if (ahead.Length > 1 && ahead != "i")
						return false;
				}
			}
			
			return hasMatrixContent && hasNumbers && bracketCount == 0;
		}

		private string ConsumeMatrixLiteral(string expression, ref int index)
		{
			var matrix = new StringBuilder();
			int bracketCount = 0;

			while (index < expression.Length)
			{
				char c = expression[index];
				matrix.Append(c);

				if (c == '[') bracketCount++;
				else if (c == ']') bracketCount--;

				if (bracketCount == 0 && matrix.Length > 2)
					break;

				index++;
			}

			return matrix.ToString();
		}
	}
}
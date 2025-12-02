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

				if (char.IsDigit(c) || c == '.')
				{
					currentToken.Append(c);
				}
				else if (char.IsLetter(c))
				{
					currentToken.Append(c);
				}
				else if ("+-*/()".Contains(c))
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
					throw new ArgumentException($"Invalid character '{invalidChar}' in expression. Variable names must start with a letter and contain only letters and digits.");
				}
			}

			if (currentToken.Length > 0)
			{
				tokens.Add(currentToken.ToString());
			}

			return tokens;
		}
	}
}
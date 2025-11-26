using System.Text;

namespace ComputorV2.Core.Lexing
{
	public class Tokenizer
	{
		public List<string> Tokenize(string expression)
		{
			var tokens = new List<string>();
			var currentNumber = new StringBuilder();

			foreach (char c in expression)
			{
				if (char.IsDigit(c))
				{
					currentNumber.Append(c);
				}
				else if ("+-*/()".Contains(c))
				{
					if (currentNumber.Length > 0)
					{
						tokens.Add(currentNumber.ToString());
						currentNumber.Clear();
					}

					tokens.Add(c.ToString());
				}
				else if (char.IsWhiteSpace(c))
				{
					if (currentNumber.Length > 0)
					{
						tokens.Add(currentNumber.ToString());
						currentNumber.Clear();
					}
				}
			}

			if (currentNumber.Length > 0)
			{
				tokens.Add(currentNumber.ToString());
			}

			return tokens;
		}
	}
}
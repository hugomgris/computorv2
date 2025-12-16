using System;
using System.Collections;

namespace ComputorV2.Core.Lexing
{
	public class Tokenizer
	{
		public List<string> MakeRawTokens(string expression)
		{
			List<string> tokens = new List<string>();

			foreach (char c in expression)
			{
				tokens.Add(c.ToString());
			}

			return tokens;
		}
	}
}
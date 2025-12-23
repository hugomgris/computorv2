using Xunit;
using System;
using System.Collections;

using ComputorV2.Interactive;
using ComputorV2.Core.Math;
using ComputorV2.Core.Lexing;
using ComputorV2.Core.Types;

namespace ComputorV2.Tests
{
	public class PostfixConversionTests
	{
		
		[Fact]
		public void Postfix_BasicConversionTests_RationalNumbers()
		{
			Tokenizer tokenizer = new Tokenizer();

			List<string> tokens = tokenizer.Tokenize("2 + 3 * 4");
			Postfix postfix_1 = new Postfix(tokens);
			string pTokens_1 = String.Join("", postfix_1.PostfixTokens);
			Assert.Equal("234*+", pTokens_1);
			
			tokens = tokenizer.Tokenize("(2 + 5 - 1) * 5 /6 * (8 * 9 - 4)");
			Postfix postfix_2 = new Postfix(tokens);
			string pTokens_2 = String.Join("", postfix_2.PostfixTokens);
			Assert.Equal("25+1-5*6/89*4-*", pTokens_2);

			tokens = tokenizer.Tokenize("(2 + 5 - 1) * 5 /6 * (8 * 9 - 4) + 2");
			Postfix postfix_3 = new Postfix(tokens);
			string pTokens_3 = String.Join("", postfix_3.PostfixTokens);
			Assert.Equal("25+1-5*6/89*4-*2+", pTokens_3);

			RationalNumber sol_3 = (RationalNumber)postfix_3.Calculate();
			Assert.Equal(342, sol_3);
		}

		[Fact]
		public void Postfix_LongConversionTests_ComplexNumbers()
		{
			Tokenizer tokenizer = new Tokenizer();
			List<string> tokens = tokenizer.Tokenize("150 + 89i -8i +(8 * 800i) + 90i");
			Postfix postfix_1 = new Postfix(tokens);

			ComplexNumber sol_1 = (ComplexNumber)postfix_1.Calculate();
			Assert.Equal(new ComplexNumber(150, 6571), sol_1);
		}
	}
}

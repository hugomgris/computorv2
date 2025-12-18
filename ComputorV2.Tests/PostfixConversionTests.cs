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
			Postfix postfix_1 = new Postfix("2 + 3 * 4");
			string pTokens_1 = String.Join("", postfix_1.PostfixTokens);
			Assert.Equal("234*+", pTokens_1);
			
			Postfix postfix_2 = new Postfix("(2 + 5 - 1) * 5 /6 * (8 * 9 - 4)");
			string pTokens_2 = String.Join("", postfix_2.PostfixTokens);
			Assert.Equal("25+1-5*6/89*4-*", pTokens_2);

			Postfix postfix_3 = new Postfix("8 % 2");
			string pTokens_3 = String.Join("", postfix_3.PostfixTokens);
			Assert.Equal("82%", pTokens_3);

			RationalNumber sol_3 = new RationalNumber(postfix_3.Calculate());
			Assert.Equal(RationalNumber.Zero, sol_3);
		}

		[Fact]
		public void Postfix_LongConversionTests_RationalNumbers()
		{
			Postfix postfix_1 = new Postfix("(2 + 5 - 1) * 5 + 6 % (8 * 9 - 4)");
			string pTokens_1 = String.Join("", postfix_1.PostfixTokens);
			Assert.Equal("25+1-5*689*4-%+", pTokens_1);

			RationalNumber sol_1 = new RationalNumber(postfix_1.Calculate());
			Assert.Equal(new RationalNumber(36), sol_1);
		}
	}
}

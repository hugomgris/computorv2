using Xunit;
using ComputorV2.Interactive;
using ComputorV2.Core.Math;
using ComputorV2.Core.Lexing;

namespace ComputorV2.Tests
{
	public class TokenizerTests
	{
		[Fact] public void Tokenize_SimpleNumber_ReturnsSingleToken()
		{
			var tokenizer = new Tokenizer();

			var tokens = tokenizer.Tokenize("42");

			Assert.Single(tokens);
			Assert.Equal("42", tokens[0]);
		}

		[Fact] public void Tokenize_SimpleAddition_ReturnsThreeTokens()
		{
			var tokenizer = new Tokenizer();

			var tokens = tokenizer.Tokenize("1+    2");

			Assert.Equal(3, tokens.Count);
			Assert.Equal("1", tokens[0]);
			Assert.Equal("+", tokens[1]);
			Assert.Equal("2", tokens[2]);
		}

		[Fact] public void Tokenize_MixedOperations_ReturnsAllTokens()
		{
			var tokenizer = new Tokenizer();

			var tokens = tokenizer.Tokenize("1+2   * 3 / 5");

			Assert.Equal(7, tokens.Count);
			Assert.Equal("1", tokens[0]);
			Assert.Equal("+", tokens[1]);
			Assert.Equal("2", tokens[2]);
			Assert.Equal("*", tokens[3]);
			Assert.Equal("3", tokens[4]);
			Assert.Equal("/", tokens[5]);
			Assert.Equal("5", tokens[6]);
		}

		[Fact] public void Tokenize_WithSpaces_IgnoresWhitespace()
		{
			var tokenizer = new Tokenizer();

			var tokens = tokenizer.Tokenize("          1            +          1      -2");

			Assert.Equal(5, tokens.Count);
			Assert.Equal("1", tokens[0]);
			Assert.Equal("+", tokens[1]);
			Assert.Equal("1", tokens[2]);
			Assert.Equal("-", tokens[3]);
			Assert.Equal("2", tokens[4]);
		}

		[Fact] public void Tokenize_WithParentheses_IncludesParenTokens()
		{
			var tokenizer = new Tokenizer();
			
			var tokens = tokenizer.Tokenize("1 + (2 * 3)");

			Assert.Equal(7, tokens.Count);
			Assert.Equal("1", tokens[0]);
			Assert.Equal("+", tokens[1]);
			Assert.Equal("(", tokens[2]);
			Assert.Equal("2", tokens[3]);
			Assert.Equal("*", tokens[4]);
			Assert.Equal("3", tokens[5]);
			Assert.Equal(")", tokens[6]);
		}
	}
}
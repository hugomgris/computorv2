using Xunit;
using System;
using System.Collections;
using ComputorV2.Core.Lexing;
using ComputorV2.Core.Math;
using ComputorV2.Core.Types;

namespace ComputorV2.Tests
{
	public class RationalNumbersTests
	{
		private readonly Parser			_parser;
		private readonly Tokenizer		_tokenizer;
		private readonly MathEvaluator	_evaluator;

		public RationalNumbersTests()
		{
			_parser = new Parser();
			_tokenizer = new Tokenizer();
			_evaluator = new MathEvaluator(_parser, _tokenizer);
		}
		
		[Fact]
		public void RationalNumbers_AssignationTests()
		{
			_evaluator.Assign("a=3");
			_evaluator.Assign("b=2-5+3/8+10*10+2.625");
			_evaluator.Assign("c=a+b");

			Dictionary<string, MathValue> vars = _evaluator.Variables;

			Assert.True(vars.ContainsKey("a"));
			Assert.True(vars.ContainsKey("b"));
			Assert.True(vars.ContainsKey("c"));

			Assert.Equal(vars["a"], new RationalNumber(3));
			Assert.Equal(vars["b"], new RationalNumber(100));
			Assert.Equal(vars["c"], new RationalNumber(103));
		}

		[Fact]
		public void RationalNumber_DirectComputationTests()
		{
			RationalNumber r1 = new RationalNumber(_evaluator.Compute("2-5+3/8+10*10+2.625"));
			RationalNumber r2 = new RationalNumber(_evaluator.Compute("2-5+3/8+10*10 + (10%28) - (7 %2 + 2 - 100 * 800)"));
			Assert.Equal(r1, 100);
			Assert.Equal(r2, new RationalNumber("80104.375"));
		}
	}
}
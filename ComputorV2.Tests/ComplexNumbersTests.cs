using Xunit;
using System;
using System.Collections;
using ComputorV2.Core.Lexing;
using ComputorV2.Core.Math;
using ComputorV2.Core.Types;

namespace ComputorV2.Tests
{
	public class ComplexNumbersTests
	{
		private readonly Parser			_parser;
		private readonly Tokenizer		_tokenizer;
		private readonly MathEvaluator	_evaluator;

		public ComplexNumbersTests()
		{
			_parser = new Parser();
			_tokenizer = new Tokenizer();
			_evaluator = new MathEvaluator(_parser, _tokenizer);
		}
		
		[Fact]
		public void ComplexNumbers_AssignationTests()
		{
			_evaluator.Assign("a=3i");
			_evaluator.Assign("b=2+2i");
			_evaluator.Assign("c=-5+6i");
			_evaluator.Assign("d=-i");
			_evaluator.Assign("e=i");
			//_evaluator.Assign("f=150 + 89i -8i +(8 * 800i)");

			Dictionary<string, MathValue> vars = _evaluator.Variables;

			Assert.True(vars.ContainsKey("a"));
			Assert.True(vars.ContainsKey("b"));
			Assert.True(vars.ContainsKey("c"));
			Assert.True(vars.ContainsKey("d"));
			Assert.True(vars.ContainsKey("e"));
			//Assert.True(vars.ContainsKey("f")); 

			Assert.Equal("3i", vars["a"].ToString());
			Assert.Equal("2+2i", vars["b"].ToString());
			Assert.Equal("-5+6i", vars["c"].ToString());
			Assert.Equal("-i", vars["d"].ToString());
			Assert.Equal("i", vars["e"].ToString());
			//Assert.Equal("150+6481i", vars["f"].ToString());
		}

		[Fact]
		public void RationalNumber_DirectComputationTests()
		{
			ComplexNumber r1 = new ComplexNumber(_evaluator.Compute("2i-5+38+10*10i+2625"));
			Assert.Equal(r1, new ComplexNumber(2658, 102));
		}

		// FAILING TESTS ARE DUE TO COMPLEX EXPRESSION SIMPLIFICATION
		// (need to add parenthesis handling at fix whatever is happening with the multiplication of i values)
	}
}
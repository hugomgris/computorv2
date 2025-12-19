using Xunit;
using System;
using System.Collections;
using ComputorV2.Core.Lexing;
using ComputorV2.Core.Math;
using ComputorV2.Core.Types;

namespace ComputorV2.Tests
{
	public class SubjectTests
	{
		private readonly Parser			_parser;
		private readonly Tokenizer		_tokenizer;
		private readonly MathEvaluator	_evaluator;

		public SubjectTests()
		{
			_parser = new Parser();
			_tokenizer = new Tokenizer();
			_evaluator = new MathEvaluator(_parser, _tokenizer);
		}

		[Fact]
		public void SubjectTests_Assignation_RationalNumbers()
		{
			_evaluator.Assign("varA=2");
			_evaluator.Assign("varB=4.242");
			_evaluator.Assign("varC=-4.3");

			Dictionary<string, MathValue> vars = _evaluator.Variables;
			Assert.True(vars.ContainsKey("varA"));
			Assert.True(vars.ContainsKey("varB"));
			Assert.True(vars.ContainsKey("varC"));

			Assert.Equal(new RationalNumber(2), vars["varA"]);
			Assert.Equal(new RationalNumber((decimal)4.242), vars["varB"]);
			Assert.Equal(new RationalNumber((decimal)-4.3), vars["varC"]);
		}

		[Fact]
		public void SubjectTests_Assignation_ComplexNumbers()
		{
			_evaluator.Assign("varA=2*i+3");
			_evaluator.Assign("varB=-4-4i");

			Dictionary<string, MathValue> vars = _evaluator.Variables;
			Assert.True(vars.ContainsKey("varA"));
			Assert.True(vars.ContainsKey("varB"));

			Assert.Equal(new ComplexNumber(3, 2), vars["varA"]);
			Assert.Equal(new ComplexNumber(-4,-4), vars["varB"]);
		}

		[Fact]
		public void SubjectTests_Reassignation()
		{
			_evaluator.Assign("x=2");
			_evaluator.Assign("y=x");

			Dictionary<string, MathValue> vars = _evaluator.Variables;
			Assert.True(vars.ContainsKey("x"));
			Assert.True(vars.ContainsKey("y"));

			Assert.Equal(new RationalNumber(2), vars["x"]);
			Assert.Equal(new RationalNumber(2), vars["y"]);

			_evaluator.Assign("y=7");
			vars = _evaluator.Variables;
			Assert.True(vars.ContainsKey("y"));
			Assert.Equal(new RationalNumber(7), vars["y"]);

			_evaluator.Assign("y=2*i-4");
			vars = _evaluator.Variables;
			Assert.True(vars.ContainsKey("y"));
			Assert.Equal(new ComplexNumber(-4, 2), vars["y"]);
		}

		[Fact]
		public void SubjectTests_Reassignation_Computation()
		{
			_evaluator.Assign("varA=2+4*2-5%4+2*(4 + 5)");
			_evaluator.Assign("varB=2*varA-5%4");
			_evaluator.Assign("varC=2*varA-varB");
			//Missing a function test (WIP)

			Dictionary<string, MathValue> vars = _evaluator.Variables;
			Assert.True(vars.ContainsKey("varA"));
			Assert.True(vars.ContainsKey("varB"));
			Assert.True(vars.ContainsKey("varC"));

			Assert.Equal(new RationalNumber(27), vars["varA"]);
			Assert.Equal(new RationalNumber(53), vars["varB"]);
			Assert.Equal(new RationalNumber(1), vars["varC"]);
		}
	}
}
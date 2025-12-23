using Xunit;
using System;
using System.Collections;
using ComputorV2.Core.Lexing;
using ComputorV2.Core.Math;
using ComputorV2.Core.Types;

namespace ComputorV2.Tests
{
	public class BonusTests
	{
		private readonly Parser			_parser;
		private readonly Tokenizer		_tokenizer;
		private readonly MathEvaluator	_evaluator;

		public BonusTests()
		{
			_parser = new Parser();
			_tokenizer = new Tokenizer();
			_evaluator = new MathEvaluator(_parser, _tokenizer);
		}

		[Fact]
		public void BonusTests_MatrixInversion()
		{
			_evaluator.Assign("m1=[[1,2];[3,4]]");
			_evaluator.Assign("m2=[[1, 2, 0]; [0, 1, 0]; [0, 0, 1]]");

			Dictionary<string, MathValue> vars = _evaluator.Variables;

			Assert.True(vars.ContainsKey("m1"));
			Assert.True(vars.ContainsKey("m2"));

			Matrix i1 = new Matrix("[ -2 , 1 ];[ 3/2 , -1/2 ]");
			Matrix i2 = new Matrix("[ 1 , -2 , 0 ];[ 0 , 1 , 0 ];[ 0 , 0 , 1 ]");
			Matrix m1 = (Matrix)vars["m1"];
			Matrix m2 = (Matrix)vars["m2"];
			Assert.Equal(i1.ToString(), m1.Inverse().ToString());
			Assert.Equal(i2.ToString(), m2.Inverse().ToString());
		}
	}
}
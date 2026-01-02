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
			_evaluator.Assign("mA=[[1,2];[3,4]]");
			_evaluator.Assign("mB=[[1, 2, 0]; [0, 1, 0]; [0, 0, 1]]");

			Dictionary<string, MathValue> vars = _evaluator.Variables;

			Assert.True(vars.ContainsKey("mA"));
			Assert.True(vars.ContainsKey("mB"));

			Matrix i1 = new Matrix("[[ -2 , 1 ];[ 3/2 , -1/2 ]]");
			Matrix i2 = new Matrix("[[ 1 , -2 , 0 ];[ 0 , 1 , 0 ];[ 0 , 0 , 1 ]]");
			Matrix mA = (Matrix)vars["mA"];
			Matrix mB = (Matrix)vars["mB"];
			Assert.Equal(i1.ToString(), mA.Inverse().ToString());
			Assert.Equal(i2.ToString(), mB.Inverse().ToString());
		}

		[Fact]
		public void BonusTests_MatrixNorm()
		{
			_evaluator.Assign("mA=[[1,2,3];[4,5,6];[7,8,9]]");
			_evaluator.Assign("mB=[[1,2];[3,4]]");

			Dictionary<string, MathValue> vars = _evaluator.Variables;

			Assert.True(vars.ContainsKey("mA"));
			Assert.True(vars.ContainsKey("mB"));

			Matrix mA = (Matrix)vars["mA"];
			Matrix mB = (Matrix)vars["mB"];
			
			Assert.Equal("18", mA.Norm().ToString());
			Assert.Equal("6", mB.Norm().ToString());
		}
	}
}
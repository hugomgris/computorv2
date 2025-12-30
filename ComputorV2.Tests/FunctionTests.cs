using Xunit;
using System;
using System.Collections;
using ComputorV2.Core.Lexing;
using ComputorV2.Core.Math;
using ComputorV2.Core.Types;

namespace ComputorV2.Tests
{
	public class FunctionTests
	{
		private readonly Parser			_parser;
		private readonly Tokenizer		_tokenizer;
		private readonly MathEvaluator	_evaluator;

		public FunctionTests()
		{
			_parser = new Parser();
			_tokenizer = new Tokenizer();
			_evaluator = new MathEvaluator(_parser, _tokenizer);
		}
		
		[Fact]
		public void Function_AssignationTests()
		{
			_evaluator.Assign("funA(x) = 2*x^5 + 4x^2 - 5*x + 4");
			_evaluator.Assign("funB(y) = 43 * y / (4 % 2 * y)");
			_evaluator.Assign("funC(z) = -2 * z - 5");
			_evaluator.Assign("funD(varblehell) = 2 * 4 + varblehell");
			_evaluator.Assign("funE(x) =4-5+(x+2)+2-4");
			_evaluator.Assign("funEE(x) =4-5+(x+2)-200-4");
			_evaluator.Assign("funF(x)=4-5+(x+2)*2-4+(3x+5)*18");
			_evaluator.Assign("funG(x)=4+5+(x+2)/2+4 + (15 + 15x)/2");
			_evaluator.Assign("funH(x) = 4-5+(x+2)^2-4");
			_evaluator.Assign("funI(x) = 4-5+(x-2)^2-4");

			Dictionary<string, Function> functions = _evaluator.Functions;

			Assert.True(functions.ContainsKey("funA"));
			Assert.True(functions.ContainsKey("funB"));
			Assert.True(functions.ContainsKey("funC"));
			Assert.True(functions.ContainsKey("funD"));
			Assert.True(functions.ContainsKey("funE"));
			Assert.True(functions.ContainsKey("funEE"));
			Assert.True(functions.ContainsKey("funF"));
			Assert.True(functions.ContainsKey("funG"));
			Assert.True(functions.ContainsKey("funH"));
			Assert.True(functions.ContainsKey("funI"));

			Assert.Equal("2 * x^5 + 4 * x^2 - 5 * x + 4", functions["funA"].Expression.ToString());
			Assert.Equal("3/4 * y", functions["funB"].Expression.ToString());
			Assert.Equal("-2 * z - 5", functions["funC"].Expression.ToString());
			Assert.Equal("varblehell + 8", functions["funD"].Expression.ToString());
			Assert.Equal("x - 1", functions["funE"].Expression.ToString());
			Assert.Equal("x - 203", functions["funEE"].Expression.ToString());
			Assert.Equal("56 * x + 89", functions["funF"].Expression.ToString());
			Assert.Equal("8 * x + 43/2", functions["funG"].Expression.ToString());
			Assert.Equal("x^2 + 4 * x - 1", functions["funH"].Expression.ToString());
			Assert.Equal("x^2 - 4 * x - 1", functions["funI"].Expression.ToString());
		}

		/* [Fact]
		public void Matrix_DirectComputationTests()
		{
			Matrix m1 = new Matrix(_evaluator.Compute("[[1,2]]+[[1,2]]"));
			Matrix m2 = new Matrix(_evaluator.Compute("[[5+10i, -98i+50]]+[[25i,-800i]]"));
			Assert.Equal(m1, new Matrix("[[2,4]]"));
			Assert.Equal(m2, new Matrix("[[5+35i,50-898i]]"));
		} */
	}
}
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
			try
			{
				_evaluator.Assign("funB(y) = 43 * y / (4 % 2 * y)");
			}
			catch {}

			_evaluator.Assign("funC(z) = -2 * z - 5");
			_evaluator.Assign("funD(varblehell) = 2 * 4 + varblehell");
			//_evaluator.Assign("funE(x) = 4 -5 + (x + 2)^2 - 4");
			//_evaluator.Assign("funF(x) = 4 - 5 + (x + 2) * 2 - 4");

			Dictionary<string, Function> functions = _evaluator.Functions;

			Assert.True(functions.ContainsKey("funA"));
			Assert.False(functions.ContainsKey("funB"));
			Assert.True(functions.ContainsKey("funC"));
			Assert.True(functions.ContainsKey("funD"));
			//Assert.True(functions.ContainsKey("funE"));
			//Assert.True(functions.ContainsKey("funF"));

			Assert.Equal("2 * x^5 + 4 * x^2 - 5 * x + 4", functions["funA"].Expression.ToString());
			Assert.Equal("-2 * z - 5", functions["funC"].Expression.ToString());
			Assert.Equal("varblehell + 8", functions["funD"].Expression.ToString());
			//Assert.Equal(new Function("funE", "x", new Polynomial("(x + 2)^2 - 5")), functions["funE"]);
			//Assert.Equal(new Function("funF", "x", new Polynomial("2x-1")), functions["funF"]);
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


	/*
	Last case here?
	$./computorv2
		> varA = 2 + 4 *2 - 5 %4 + 2 * (4 + 5)
		27
		> varB = 2 * varA - 5 %4
		53
		> funA(x) = varA + varB * 4 - 1 / 2 + x
		238.5 + x
		> varC = 2 * varA - varB
		1
		> varD = funA(varC)
		239.5
	*/
}
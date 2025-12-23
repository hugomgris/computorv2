using Xunit;
using System;
using System.Collections;
using ComputorV2.Core.Lexing;
using ComputorV2.Core.Math;
using ComputorV2.Core.Types;

namespace ComputorV2.Tests
{
	public class MatrixTests
	{
		private readonly Parser			_parser;
		private readonly Tokenizer		_tokenizer;
		private readonly MathEvaluator	_evaluator;

		public MatrixTests()
		{
			_parser = new Parser();
			_tokenizer = new Tokenizer();
			_evaluator = new MathEvaluator(_parser, _tokenizer);
		}
		
		[Fact]
		public void Matrix_AssignationTests()
		{
			_evaluator.Assign("a=[[1,2]]");
			_evaluator.Assign("b=[[1,2 + 3i];[5i+1, 87]]");
			_evaluator.Assign("c=[[i,i];[-i,-i];[-2i,-42i]]");

			Dictionary<string, MathValue> vars = _evaluator.Variables;

			Assert.True(vars.ContainsKey("a"));
			Assert.True(vars.ContainsKey("b"));
			Assert.True(vars.ContainsKey("c"));

			RationalNumber[,] rn1 = {{new RationalNumber(1), new RationalNumber(2)}};
			ComplexNumber[,] rn2 = {{new ComplexNumber(1, 0), new ComplexNumber(2, 3)}, {new ComplexNumber(1, 5), new ComplexNumber(87, 0)}};
			ComplexNumber[,] rn3 = {{new ComplexNumber(0, 1), new ComplexNumber(0, 1)}, {new ComplexNumber(0, -1), new ComplexNumber(0, -1)}, {new ComplexNumber(0, -2), new ComplexNumber(0, -42)}};

			Assert.Equal(new Matrix(rn1), vars["a"]);
			Assert.Equal(new Matrix(rn2), vars["b"]);
			Assert.Equal(new Matrix(rn3), vars["c"]);
		}

		[Fact]
		public void Matrix_DirectComputationTests()
		{
			Matrix m1 = new Matrix(_evaluator.Compute("[[1,2]]+[[1,2]]"));
			Matrix m2 = new Matrix(_evaluator.Compute("[[5+10i, -98i+50]]+[[25i,-800i]]"));
			Assert.Equal(m1, new Matrix("[[2,4]]"));
			Assert.Equal(m2, new Matrix("[[5+35i,50-898i]]"));
		}
	}
}
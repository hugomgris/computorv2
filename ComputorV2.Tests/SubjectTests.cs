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
		public void SubjectTests_Assignation_Matrix()
		{
			_evaluator.Assign("varA=[[3,4]]");
			_evaluator.Assign("varB=[[2,3];[4,3]]");
			_evaluator.Assign("varC=[[1,2];[3,2];[3,4]]");

			Dictionary<string, MathValue> vars = _evaluator.Variables;
			Assert.True(vars.ContainsKey("varA"));
			Assert.True(vars.ContainsKey("varB"));
			Assert.True(vars. ContainsKey("varC"));

			RationalNumber[,] rn1 = {{new RationalNumber(3), new RationalNumber(4)}};
			RationalNumber[,] rn2 = {{new RationalNumber(2), new RationalNumber(3)}, {new RationalNumber(4), new RationalNumber(3)}};
			RationalNumber[,] rn3 = {{new RationalNumber(1), new RationalNumber(2)}, {new RationalNumber(3), new RationalNumber(2)}, {new RationalNumber(3), new RationalNumber(4)}};
			
			Assert.Equal(new Matrix(rn1), vars["varA"]);
			Assert.Equal(new Matrix(rn2), vars["varB"]);
			Assert.Equal(new Matrix(rn3), vars["varC"]);
		}

		[Fact]
		public void SubjectTests_Assignation_Functions()
		{
			_evaluator.Assign("funA(x) =2*x^5+4x^2-5*x+4");
			_evaluator.Assign("funB(y) = 43*y/(4%2*y)");
			_evaluator.Assign("funC(z) = -2 * z - 5");

			Dictionary<string, Function> functions = _evaluator.Functions;

			Assert.True(functions.ContainsKey("funA"));
			Assert.True(functions.ContainsKey("funB"));
			Assert.True(functions.ContainsKey("funC"));

			Assert.Equal("2 * x^5 + 4 * x^2 - 5 * x + 4", functions["funA"].Expression.ToString());
			Assert.Equal("3/4 * y", functions["funB"].Expression.ToString());
			Assert.Equal("-2 * z - 5", functions["funC"].Expression.ToString());
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
		public void SubjectTests_ReassignationWithComputation()
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

		[Fact]
		public void SubjectTests_Computation()
		{
			_evaluator.Assign("a= 2 * 4 + 4");

			Dictionary<string, MathValue> vars = _evaluator.Variables;
			Assert.True(vars.ContainsKey("a"));
			
			Assert.Equal(new RationalNumber(12), vars["a"]);

			string n1 = _evaluator.Compute("a + 2");
			Assert.Equal(new RationalNumber(n1), 14);
		}

		[Fact]
		public void SubjectTests_Computation_Functions()
		{
			// TODO: test "funA(2) + funB(4) = ?"
			_evaluator.Assign("funA(x) = 2 * 4 + x");
			_evaluator.Assign("funB(x) = 4 -5 + (x + 2)^2 - 4");
			_evaluator.Assign("funC(x) = 4x + 5 - 2");
			
			Dictionary<string, Function> functions = _evaluator.Functions;

			Assert.True(functions.ContainsKey("funA"));
			Assert.True(functions.ContainsKey("funB"));
			Assert.True(functions.ContainsKey("funC"));

			string c1 = _evaluator.Compute("funC(3) = ?");
			Assert.Equal("15", c1);
		}

		[Fact]		
		public void SubjectTests_Syntax_RationalOrImaginary()
		{
			_evaluator.Assign("varA = 2");
			_evaluator.Assign("varB = 2 * (4 + varA + 3)");
			_evaluator.Assign("varC =2 * varB");
			_evaluator.Assign("varD = 2 *(2 + 4 *varC -4 /3)");

			Dictionary<string, MathValue> vars = _evaluator.Variables;

			Assert.True(vars.ContainsKey("varA"));
			Assert.True(vars.ContainsKey("varB"));
			Assert.True(vars.ContainsKey("varC"));
			Assert.True(vars.ContainsKey("varD"));

			Assert.Equal("2", vars["varA"].ToString());
			Assert.Equal("18", vars["varB"].ToString());
			Assert.Equal("36", vars["varC"].ToString());
			Assert.Equal("868/3", vars["varD"].ToString());
		}

		[Fact]
		public void SubjectTests_Syntax_Matrices()
		{
			_evaluator.Assign("matA = [[1,2];[3,2];[3,4]]");
			_evaluator.Assign("matB= [[1,2]]");

			Dictionary<string, MathValue> vars = _evaluator.Variables;

			Assert.True(vars.ContainsKey("matA"));
			Assert.True(vars.ContainsKey("matB"));

			Assert.Equal("[ 1 , 2 ]\n[ 3 , 2 ]\n[ 3 , 4 ]", vars["matA"].ToString());
			Assert.Equal("[ 1 , 2 ]", vars["matB"].ToString());
		}

		[Fact]
		public void SubjectTests_Syntax_Functions()
		{
			_evaluator.Assign("funA(b) = 2*b+b");
			_evaluator.Assign("funB(a) =2 * a");
			_evaluator.Assign("funC(y) =2* y + 4 -2 * 4+1/3");
			_evaluator.Assign("funD(x) = 2 *x");

			Dictionary<string, Function> functions = _evaluator.Functions;

			Assert.True(functions.ContainsKey("funA"));
			Assert.True(functions.ContainsKey("funB"));
			Assert.True(functions.ContainsKey("funC"));
			Assert.True(functions.ContainsKey("funD"));

			Assert.Equal("3 * b", functions["funA"].Expression.ToString());
			Assert.Equal("2 * a", functions["funB"].Expression.ToString());
			Assert.Equal("2 * y - 11/3", functions["funC"].Expression.ToString());
			Assert.Equal("2 * x", functions["funD"].Expression.ToString());
		}
	}
}
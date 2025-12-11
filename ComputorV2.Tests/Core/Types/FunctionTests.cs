using Xunit;
using ComputorV2.Core.Types;

	namespace ComputorV2.Tests.Core.Types
	{
		public class FunctionTests
		{
			#region Constructor Tests
			
			[Fact]
			public void Function_Constructor_WithMathValue_CreatesCorrectFunction()
			{
				var polynomial = new Polynomial("2*x + 1");
				
				var function = new Function("f", "x", polynomial);
				
				Assert.Equal("f", function.Name);
				Assert.Equal("x", function.Variable);
				Assert.Equal(polynomial, function.Expression);
			}

			[Fact]
			public void Function_Constructor_WithPolynomialString_CreatesPolynomialExpression()
			{
				var function = new Function("f", "x", "2*x + 1");
				
				Assert.Equal("f", function.Name);
				Assert.Equal("x", function.Variable);
				Assert.IsType<Polynomial>(function.Expression);
			}

			[Fact]
			public void Function_Constructor_WithRationalString_CreatesRationalExpression()
			{
				var function = new Function("f", "x", "5");

				Assert.Equal("f", function.Name);
				Assert.Equal("x", function.Variable);
				Assert.IsType<RationalNumber>(function.Expression);
				Assert.Equal("5", function.Expression.ToString());
			}

			[Fact]
			public void Function_Constructor_WithComplexString_CreatesComplexExpression()
			{
				var function = new Function("f", "x", "2+3i");
				
				Assert.Equal("f", function.Name);
				Assert.Equal("x", function.Variable);
				Assert.IsType<ComplexNumber>(function.Expression);
			}

			[Fact]
			public void Function_Constructor_WithMatrixString_CreatesMatrixExpression()
			{
				var function = new Function("f", "x", "[1,2]");
				
				Assert.Equal("f", function.Name);
				Assert.Equal("x", function.Variable);
				Assert.IsType<Matrix>(function.Expression);
			}

			#endregion

			#region Evaluation Tests

			[Fact]
			public void Function_Evaluate_PolynomialFunction_ReturnsCorrectResult()
			{
				var function = new Function("f", "x", "2*x + 1");
				var input = new RationalNumber(5);
				
				var result = function.Evaluate(input);
				
				Assert.Equal("11", result.ToString());
			}

			[Fact]
			public void Function_Evaluate_ConstantRationalFunction_ReturnsConstant()
			{
				var function = new Function("f", "x", "5");
				var input = new RationalNumber(10);
				
				var result = function.Evaluate(input);
				
				Assert.Equal("5", result.ToString());
			}

			[Fact]
			public void Function_Evaluate_ConstantMatrixFunction_ReturnsMatrix()
			{
				var function = new Function("f", "x", "[1,2]");
				var input = new RationalNumber(3);

				var result = function.Evaluate(input);

				Assert.IsType<Matrix>(result);
			}

			#endregion

			#region String Representation Tests

			[Fact]
			public void Function_ToString_FormatsCorrectly()
			{
				var function = new Function("f", "x", "2*x + 1");
				
				var str = function.ToString();
				
				Assert.Contains("f(x)", str);
				Assert.Contains("2 * X + 1", str);
			}

			#endregion

			#region Function arithmetic

			[Fact]
			public void Function_AdditionTest()
			{
				Function f1 = new Function ("f", "x", "2 * x + 1");
				Function f2 = new Function ("g", "x", "3 * x + 2");

				Function s1 = f1.Add(f2);
				Assert.Equal("(f + g)(x) = 5 * X + 3", s1.ToString());

				Function s2 = f2.Subtract(f1);
				Assert.Equal("(g - f)(x) = X + 1", s2.ToString());
			}

			#endregion
		}
	}
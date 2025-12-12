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
				Assert.Contains("2*x + 1", str);
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

			#region Equality Tests

			[Fact]
			public void Function_Equals_SameFunctions_ReturnsTrue()
			{
				var function1 = new Function("f", "x", "2*x + 1");
				var function2 = new Function("f", "x", "2*x + 1");
				
				Assert.True(function1.Equals(function2));
				Assert.Equal(function1.GetHashCode(), function2.GetHashCode());
			}

			[Fact]
			public void Function_Equals_DifferentNames_ReturnsFalse()
			{
				var function1 = new Function("f", "x", "2*x + 1");
				var function2 = new Function("g", "x", "2*x + 1");
				
				Assert.False(function1.Equals(function2));
			}

			[Fact]
			public void Function_Equals_DifferentVariables_ReturnsFalse()
			{
				var function1 = new Function("f", "x", "2*x + 1");
				var function2 = new Function("f", "y", "2*y + 1");
				
				Assert.False(function1.Equals(function2));
			}

			[Fact]
			public void Function_Equals_DifferentExpressions_ReturnsFalse()
			{
				var function1 = new Function("f", "x", "2*x + 1");
				var function2 = new Function("f", "x", "3*x + 1");
				
				Assert.False(function1.Equals(function2));
			}

			#endregion

			#region Multiplication and Division Tests

			[Fact]
			public void Function_Multiplication_PolynomialFunctions_ReturnsCorrectResult()
			{
				var f = new Function("f", "x", "2*x + 1");
				var g = new Function("g", "x", "x + 3");
				
				var result = f.Multiply(g);
				
				Assert.Equal("(f * g)", result.Name);
				Assert.Equal("x", result.Variable);
			}

			[Fact]
			public void Function_Division_PolynomialFunctions_ReturnsCorrectResult()
			{
				var f = new Function("f", "x", "6*x + 3");
				var g = new Function("g", "x", "3");
				
				var result = f.Divide(g);
				
				Assert.Equal("(f / g)", result.Name);
				Assert.Equal("x", result.Variable);
			}

			[Fact]
			public void Function_Multiplication_DifferentVariables_ThrowsException()
			{
				var f = new Function("f", "x", "2*x + 1");
				var g = new Function("g", "y", "y + 3");
				
				Assert.Throws<ArgumentException>(() => f.Multiply(g));
			}

			#endregion

			#region Negate and Power Tests

			[Fact]
			public void Function_Negate_ChangesSignCorrectly()
			{
				var f = new Function("f", "x", "2*x + 1");
				
				var result = (Function)f.Negate();
				
				Assert.Equal("-f", result.Name);
				Assert.Equal("x", result.Variable);
			}

			[Fact]
			public void Function_Power_Zero_ReturnsOne()
			{
				var f = new Function("f", "x", "2*x + 1");
				
				var result = (Function)f.Power(0);
				
				Assert.Equal("1", result.Name);
				Assert.Equal("x", result.Variable);
			}

			[Fact]
			public void Function_Power_One_ReturnsSame()
			{
				var f = new Function("f", "x", "2*x + 1");
				
				var result = (Function)f.Power(1);
				
				Assert.Equal("f", result.Name);
			}

			[Fact]
			public void Function_Power_Two_SquareCorrectly()
			{
				var f = new Function("f", "x", "x + 1");
				
				var result = (Function)f.Power(2);
				
				Assert.Equal("f^2", result.Name);
				Assert.Equal("x", result.Variable);
			}

			#endregion

			#region Compose Tests

			[Fact]
			public void Function_Compose_PolynomialFunctions_ReturnsCorrectResult()
			{
				var f = new Function("f", "x", "2*x + 1");
				var g = new Function("g", "x", "x + 3");
				
				var result = f.Compose(g);
				
				Assert.Equal("(f ∘ g)", result.Name);
				Assert.Equal("x", result.Variable);
			}

			[Fact]
			public void Function_Compose_ConstantFunction_ReturnsConstant()
			{
				var f = new Function("f", "x", "5");
				var g = new Function("g", "x", "2*x + 1");
				
				var result = f.Compose(g);
				
				Assert.Equal("(f ∘ g)", result.Name);
				Assert.IsType<RationalNumber>(result.Expression);
			}

			[Fact]
			public void Function_Compose_DifferentVariables_ThrowsException()
			{
				var f = new Function("f", "x", "2*x + 1");
				var g = new Function("g", "y", "y + 3");
				
				Assert.Throws<ArgumentException>(() => f.Compose(g));
			}

			#endregion

			#region Derive Tests

			[Fact]
			public void Function_Derive_PolynomialFunction_ReturnsCorrectDerivative()
			{
				var f = new Function("f", "x", "3*x^2 + 2*x + 1");
				
				var result = f.Derive();
				
				Assert.Equal("f'", result.Name);
				Assert.Equal("x", result.Variable);
				Assert.IsType<Polynomial>(result.Expression);
			}

			[Fact]
			public void Function_Derive_ConstantFunction_ReturnsZero()
			{
				var f = new Function("f", "x", "42");
				
				var result = f.Derive();
				
				Assert.Equal("f'", result.Name);
				Assert.Equal("x", result.Variable);
				Assert.IsType<RationalNumber>(result.Expression);
				Assert.Equal("0", result.Expression.ToString());
			}

			[Fact]
			public void Function_Derive_LinearFunction_ReturnsConstant()
			{
				var f = new Function("f", "x", "5*x + 3");
				
				var result = f.Derive();
				
				Assert.Equal("f'", result.Name);
				Assert.IsType<Polynomial>(result.Expression);
			}

			#endregion

			#region Complex Expression Tests

			[Fact]
			public void Function_ComplexExpression_EvaluateCorrectly()
			{
				var f = new Function("f", "x", "2+3i");
				var input = new RationalNumber(5);
				
				var result = f.Evaluate(input);
				
				Assert.IsType<ComplexNumber>(result);
				Assert.Equal("2 + 3i", result.ToString());
			}

			[Fact]
			public void Function_MatrixExpression_EvaluateCorrectly()
			{
				var f = new Function("f", "x", "[[1,2];[3,4]]");
				var input = new RationalNumber(10);
				
				var result = f.Evaluate(input);
				
				Assert.IsType<Matrix>(result);
			}

			[Fact]
			public void Function_ComplexPolynomial_WorksCorrectly()
			{
				var f = new Function("f", "x", "x + 2i");
				var input = new RationalNumber(3);
				
				var result = f.Evaluate(input);

				Assert.IsType<ComplexNumber>(result);
			}

			#endregion

			#region Edge Cases

			[Fact]
			public void Function_EmptyName_WorksCorrectly()
			{
				var f = new Function("", "x", "x + 1");
				
				Assert.Equal("", f.Name);
				Assert.Equal("x", f.Variable);
			}

			[Fact]
			public void Function_SingleCharacterVariable_WorksCorrectly()
			{
				var f = new Function("f", "t", "2*t + 1");
				
				Assert.Equal("t", f.Variable);
			}

			[Fact]
			public void Function_ChainedOperations_WorkCorrectly()
			{
				var f = new Function("f", "x", "x + 1");
				var g = new Function("g", "x", "2*x");
				var h = new Function("h", "x", "x - 3");
				
				var result = f.Add(g).Subtract(h);
				
				Assert.Equal("((f + g) - h)", result.Name);
			}

			[Fact]
			public void Function_ToString_ComplexName_FormatsCorrectly()
			{
				var f = new Function("f", "x", "x + 1");
				var g = new Function("g", "x", "2*x");
				var combined = f.Add(g);
				
				var str = combined.ToString();
				
				Assert.Contains("(f + g)", str);
				Assert.Contains("x", str);
			}

			#endregion
		}
	}
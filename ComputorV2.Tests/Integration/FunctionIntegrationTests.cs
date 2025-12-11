using Xunit;
using ComputorV2.Core.Math;
using ComputorV2.Core.Types;

namespace ComputorV2.Tests.Integration
{
	public class FunctionIntegrationTests
	{
		#region Function Definition Tests

		[Fact]
		public void MathEvaluator_FunctionDefinition_SimpleLinear_Works()
		{
			var evaluator = new MathEvaluator();
			
			var result = evaluator.Evaluate("f(x) = 2*x + 1");
			
			Assert.IsType<Function>(result);
			var function = (Function)result;
			Assert.Equal("f", function.Name);
			Assert.Equal("x", function.Variable);
		}

		[Fact]
		public void MathEvaluator_FunctionDefinition_Quadratic_Works()
		{
			var evaluator = new MathEvaluator();
			
			var result = evaluator.Evaluate("g(t) = t^2 + 3*t - 2");
			
			Assert.IsType<Function>(result);
			var function = (Function)result;
			Assert.Equal("g", function.Name);
			Assert.Equal("t", function.Variable);
		}

		[Fact]
		public void MathEvaluator_FunctionDefinition_Constant_Works()
		{
			var evaluator = new MathEvaluator();
			
			var result = evaluator.Evaluate("h(x) = 42");
			
			Assert.IsType<Function>(result);
			var function = (Function)result;
			Assert.Equal("h", function.Name);
			Assert.Equal("x", function.Variable);
			Assert.IsType<RationalNumber>(function.Expression);
		}

		[Fact]
		public void MathEvaluator_FunctionDefinition_Complex_Works()
		{
			var evaluator = new MathEvaluator();
			
			var result = evaluator.Evaluate("c(x) = 2 + 3i");
			
			Assert.IsType<Function>(result);
			var function = (Function)result;
			Assert.Equal("c", function.Name);
			Assert.Equal("x", function.Variable);
			Assert.IsType<ComplexNumber>(function.Expression);
		}

		[Fact]
		public void MathEvaluator_FunctionDefinition_Matrix_Works()
		{
			var evaluator = new MathEvaluator();
			
			var result = evaluator.Evaluate("m(x) = [[1,2];[3,4]]");
			
			Assert.IsType<Function>(result);
			var function = (Function)result;
			Assert.Equal("m", function.Name);
			Assert.Equal("x", function.Variable);
			Assert.IsType<Matrix>(function.Expression);
		}

		#endregion

		#region Function Call Tests

		[Fact]
		public void MathEvaluator_FunctionCall_WithNumber_ReturnsCorrectResult()
		{
			var evaluator = new MathEvaluator();
			
			evaluator.Evaluate("f(x) = 2*x + 1");
			var result = evaluator.Evaluate("f(5)");
			
			Assert.Equal("11", result.ToString());
		}

		[Fact]
		public void MathEvaluator_FunctionCall_WithVariable_ReturnsCorrectResult()
		{
			var evaluator = new MathEvaluator();
			
			evaluator.Evaluate("a = 3");
			evaluator.Evaluate("f(x) = 2*x + 1");
			var result = evaluator.Evaluate("f(a)");
			
			Assert.Equal("7", result.ToString());
		}

		[Fact]
		public void MathEvaluator_FunctionCall_WithExpression_ReturnsCorrectResult()
		{
			var evaluator = new MathEvaluator();
			
			evaluator.Evaluate("f(x) = x^2");
			var result = evaluator.Evaluate("f(2 + 3)");
			
			Assert.Equal("25", result.ToString());
		}

		[Fact]
		public void MathEvaluator_FunctionCall_ConstantFunction_ReturnsConstant()
		{
			var evaluator = new MathEvaluator();
			
			evaluator.Evaluate("c(x) = 42");
			var result = evaluator.Evaluate("c(100)");
			
			Assert.Equal("42", result.ToString());
		}

		[Fact]
		public void MathEvaluator_FunctionCall_ComplexFunction_ReturnsComplex()
		{
			var evaluator = new MathEvaluator();
			
			evaluator.Evaluate("i(x) = 2 + 3i");
			var result = evaluator.Evaluate("i(5)");
			
			Assert.IsType<ComplexNumber>(result);
			Assert.Equal("2 + 3i", result.ToString());
		}

		#endregion

		#region Function Display Tests

		[Fact]
		public void MathEvaluator_FunctionLookup_ReturnsFunctionObject()
		{
			var evaluator = new MathEvaluator();
			
			evaluator.Evaluate("f(x) = 2*x + 1");
			var result = evaluator.Evaluate("f");
			
			Assert.IsType<Function>(result);
			var function = (Function)result;
			Assert.Equal("f", function.Name);
		}

		[Fact]
		public void MathEvaluator_UndefinedFunction_ThrowsException()
		{
			var evaluator = new MathEvaluator();
			
			Assert.Throws<ArgumentException>(() => evaluator.Evaluate("unknown(5)"));
		}

		#endregion

		#region Function and Variable Interaction Tests

		[Fact]
		public void MathEvaluator_FunctionWithVariableArgument_WorksCorrectly()
		{
			var evaluator = new MathEvaluator();
			
			evaluator.Evaluate("a = 10");
			evaluator.Evaluate("f(x) = x^2 + 1");
			var result = evaluator.Evaluate("f(a)");
			
			Assert.Equal("101", result.ToString());
		}

		[Fact]
		public void MathEvaluator_FunctionResultAssignedToVariable_Works()
		{
			var evaluator = new MathEvaluator();
			
			evaluator.Evaluate("f(x) = 2*x + 1");
			evaluator.Evaluate("result = f(5)");
			var value = evaluator.GetVariable("result");
			
			Assert.NotNull(value);
			Assert.Equal("11", value.ToString());
		}

		[Fact]
		public void MathEvaluator_FunctionDefinitionUsingVariables_Works()
		{
			var evaluator = new MathEvaluator();
			
			evaluator.Evaluate("a = 2");
			evaluator.Evaluate("b = 3");
			evaluator.Evaluate("f(x) = a*x + b");
			var result = evaluator.Evaluate("f(5)");
			
			Assert.Equal("13", result.ToString());
		}

		#endregion

		#region Polynomial Function Tests

		[Fact]
		public void MathEvaluator_PolynomialFunction_EvaluatesCorrectly()
		{
			var evaluator = new MathEvaluator();
			
			evaluator.Evaluate("p(x) = x^3 - 2*x^2 + x - 1");
			var result = evaluator.Evaluate("p(2)");
			
			Assert.Equal("1", result.ToString());
		}

		[Fact]
		public void MathEvaluator_PolynomialFunction_WithComplexCoefficients_Works()
		{
			var evaluator = new MathEvaluator();
			
			evaluator.Evaluate("q(x) = 3 + i");
			var result = evaluator.Evaluate("q(1)");
			
			Assert.IsType<ComplexNumber>(result);
			Assert.Equal("3 + i", result.ToString());
		}

		#endregion

		#region Error Cases

		[Fact]
		public void MathEvaluator_InvalidFunctionDefinition_ThrowsException()
		{
			var evaluator = new MathEvaluator();
			
			Assert.ThrowsAny<Exception>(() => evaluator.Evaluate("f() = 2"));
			Assert.Throws<ArgumentException>(() => evaluator.Evaluate("() = 2"));
		}

		[Fact]
		public void MathEvaluator_CallUndefinedFunction_ThrowsException()
		{
			var evaluator = new MathEvaluator();
			
			Assert.Throws<ArgumentException>(() => evaluator.Evaluate("unknown(5)"));
		}

		#endregion

		#region Function Management Tests

		[Fact]
		public void MathEvaluator_GetFunctions_ReturnsCorrectFunctions()
		{
			var evaluator = new MathEvaluator();
			
			evaluator.Evaluate("f(x) = 2*x + 1");
			evaluator.Evaluate("g(t) = t^2");
			
			var functions = evaluator.GetFunctions();
			
			Assert.Equal(2, functions.Count);
			Assert.True(functions.ContainsKey("f"));
			Assert.True(functions.ContainsKey("g"));
		}

		[Fact]
		public void MathEvaluator_ClearFunctions_RemovesAllFunctions()
		{
			var evaluator = new MathEvaluator();
			
			evaluator.Evaluate("f(x) = 2*x + 1");
			evaluator.Evaluate("g(t) = t^2");
			evaluator.ClearFunctions();
			
			var functions = evaluator.GetFunctions();
			Assert.Empty(functions);
		}

		[Fact]
		public void MathEvaluator_OverwriteFunction_ReplacesFunction()
		{
			var evaluator = new MathEvaluator();
			
			evaluator.Evaluate("f(x) = 2*x");
			evaluator.Evaluate("f(x) = 3*x");
			
			var result = evaluator.Evaluate("f(5)");
			Assert.Equal("15", result.ToString());
		}

		#endregion

		#region Integration with Existing Features

		[Fact]
		public void MathEvaluator_FunctionWithMatrix_WorksCorrectly()
		{
			var evaluator = new MathEvaluator();
			
			evaluator.Evaluate("matrixFunc(x) = [[1,2];[3,4]]");
			var result = evaluator.Evaluate("matrixFunc(10)");
			
			Assert.IsType<Matrix>(result);
		}

		[Fact]
		public void MathEvaluator_ComplexFunctionCalls_WorkTogether()
		{
			var evaluator = new MathEvaluator();
			
			evaluator.Evaluate("double(x) = 2*x");
			evaluator.Evaluate("square(x) = x^2");
			
			evaluator.Evaluate("a = double(5)");
			var result = evaluator.Evaluate("square(a)");
			
			Assert.Equal("100", result.ToString());
		}

		#endregion
	}
}

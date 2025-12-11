using Xunit;
using ComputorV2.Core.Math;
using ComputorV2.Core.Types;

namespace ComputorV2.Tests
{
	public class VariableTests
	{
		[Fact] public void IsAssignment_SimpleVariable_ReturnsTrue()
		{
			var evaluator = new MathEvaluator();
			
			string test = "a = 2";

			var result = evaluator.IsAssignment(test);

			Assert.True(result);
		}

		[Fact] public void IsAssignment_PolynomialEquation_ReturnsFalse()
		{
			var evaluator = new MathEvaluator();
			
			string test = "2x^2 + x + 10 = 0";

			var result = evaluator.IsAssignment(test);

			Assert.False(result);
		}
		
		[Fact] public void IsAssignment_ComplexLeftSide_ReturnsFalse()
		{
			var evaluator = new MathEvaluator();
			
			string test = "A2 + C3 = 10";

			var result = evaluator.IsAssignment(test);

			Assert.False(result);
		}
		
		[Fact] 
		public void Evaluate_SimpleVariableAssignment_StoresValue()
		{
			var evaluator = new MathEvaluator();
			
			evaluator.Evaluate("a = 5");
			var result = evaluator.Evaluate("a");
			
			Assert.Equal("5", result.ToString());
		}

		[Fact] 
		public void Evaluate_VariableReassignment_UpdatesValue()
		{
			var evaluator = new MathEvaluator();
			
			evaluator.Evaluate("a = 5");
			evaluator.Evaluate("a = 10");
			var result = evaluator.Evaluate("a");
			
			Assert.Equal("10", result.ToString());
		}

		[Fact]
		public void Debug_VariableDictionary_ShowsActualBehavior()
		{
			var evaluator = new MathEvaluator();
			
			Console.WriteLine("=== STORING ===");
			evaluator.Evaluate("a = 5");
			
			Console.WriteLine("=== RESOLVING ===");
			var result = evaluator.Evaluate("a");
			
			Assert.Equal("5", result.ToString());
		}

		[Fact] 
		public void Evaluate_AssignmentWithExpression_CalculatesAndStores()
		{
			var evaluator = new MathEvaluator();
			
			var result = evaluator.Evaluate("a = 2 + 3");
			
			Assert.Equal("5", result.ToString());
			
			var storedResult = evaluator.Evaluate("a");
			Assert.Equal("5", storedResult.ToString());
		}

		[Fact] 
		public void Evaluate_VariableInExpression_CalculatesCorrectly()
		{
			var evaluator = new MathEvaluator();
			
			evaluator.Evaluate("a = 5");
			var result = evaluator.Evaluate("a + 3");
			
			Assert.Equal("8", result.ToString());
		}

		[Fact] 
		public void Evaluate_MultipleVariables_CalculatesCorrectly()
		{
			var evaluator = new MathEvaluator();
			
			evaluator.Evaluate("a = 5");
			evaluator.Evaluate("b = 3");
			var result = evaluator.Evaluate("a * b + 2");
			
			Assert.Equal("17", result.ToString());
		}

		[Fact] 
		public void IsAssignment_InvalidVariableName_ReturnsFalse()
		{
			var evaluator = new MathEvaluator();
			
			Assert.False(evaluator.IsAssignment("@ = 5"));
			Assert.False(evaluator.IsAssignment("2x = 5"));
			Assert.False(evaluator.IsAssignment("x+y = 5"));
			Assert.False(evaluator.IsAssignment("= 5"));
		}
	}
}
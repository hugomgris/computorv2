using Xunit;
using ComputorV2.Interactive;
using ComputorV2.Core.Math;
using ComputorV2.Core.Types;

namespace ComputorV2.Tests
{
	public class MathematicalParserTests
	{
		[Fact] public void Evaluate_SimpleNumber_ReturnsValue()
		{
			var evaluator = new MathEvaluator();

			var result = evaluator.Evaluate("42");

			Assert.Equal("42", result.ToString());
		}

		[Fact] public void Evaluate_SimpleAddition_ReturnSum()
		{
			var evaluator = new MathEvaluator();

			var result = evaluator.Evaluate("2 + 3");

			Assert.Equal("5", result.ToString());
		}

		[Fact] public void Evaluate_SimpleSubstraction_ReturnSub()
		{
			var evaluator = new MathEvaluator();

			var result = evaluator.Evaluate("3-2");

			Assert.Equal("1", result.ToString());
		}

		[Fact] public void Evaluate_SimpleMultiplication_ReturnMul()
		{
			var evaluator = new MathEvaluator();

			var result = evaluator.Evaluate("3    *    2");

			Assert.Equal("6", result.ToString());
		}

		[Fact] public void Evaluate_SimpleDivision_ReturnDiv()
		{
			var evaluator = new MathEvaluator();

			var result = evaluator.Evaluate("8    /2");

			Assert.Equal("4", result.ToString());
		}


		[Fact] public void Evaluate_MixedOperations_RespectsPreced()
		{
			var evaluator = new MathEvaluator();

			var result = evaluator.Evaluate("2 + 3 * 4");

			Assert.Equal("14", result.ToString());
		}

		[Fact] public void Evaluate_DecimalNumbers_ReturnsCorrectResult()
		{
			var evaluator = new MathEvaluator();
			var result = evaluator.Evaluate("3.14 + 2.86");
			Assert.Equal("6", result.ToString());
		}

		[Fact] public void Evaluate_DecimalDivision_ReturnsCorrectResult()
		{
			var evaluator = new MathEvaluator();
			var result = evaluator.Evaluate("7.5 / 2.5");
			Assert.Equal("3", result.ToString());
		}

		[Fact] public void Evaluate_DivisionByZero_ThrowsException()
		{
			var evaluator = new MathEvaluator();
			Assert.Throws<DivideByZeroException>(() => evaluator.Evaluate("5 / 0"));
		}
	}
}
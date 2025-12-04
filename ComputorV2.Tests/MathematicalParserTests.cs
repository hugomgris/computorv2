using Xunit;
using ComputorV2.Interactive;
using ComputorV2.Core.Math;
using ComputorV2.Core.Types;

namespace ComputorV2.Tests
{
	public class MathematicalParserTests
	{
		// Naming convention: MethodName_Scenario_ExpectedBehavior
		[Fact] public void Evaluate_SimpleNumber_ReturnsValue()
		{
			var evaluator = new RationalMathEvaluator();

			var result = evaluator.Evaluate("42");

			Assert.Equal(new RationalNumber(42), result);
		}

		[Fact] public void Evaluate_SimpleAddition_ReturnSum()
		{
			var evaluator = new RationalMathEvaluator();

			var result = evaluator.Evaluate("2 + 3");

			Assert.Equal(new RationalNumber(5), result);
		}

		[Fact] public void Evaluate_SimpleSubstraction_ReturnSub()
		{
			var evaluator = new RationalMathEvaluator();

			var result = evaluator.Evaluate("3-2");

			Assert.Equal(new RationalNumber(1), result);
		}

		[Fact] public void Evaluate_SimpleMultiplication_ReturnMul()
		{
			var evaluator = new RationalMathEvaluator();

			var result = evaluator.Evaluate("3    *    2");

			Assert.Equal(new RationalNumber(6), result);
		}

		[Fact] public void Evaluate_SimpleDivision_ReturnDiv()
		{
			var evaluator = new RationalMathEvaluator();

			var result = evaluator.Evaluate("8    /2");

			Assert.Equal(new RationalNumber(4), result);
		}


		[Fact] public void Evaluate_MixedOperations_RespectsPreced()
		{
			var evaluator = new RationalMathEvaluator();

			var result = evaluator.Evaluate("2 + 3 * 4");

			Assert.Equal(new RationalNumber(14), result);
		}

		[Fact] public void Evaluate_DecimalNumbers_ReturnsCorrectResult()
		{
			var evaluator = new RationalMathEvaluator();
			var result = evaluator.Evaluate("3.14 + 2.86");
			Assert.Equal(new RationalNumber(6), result);
		}

		[Fact] public void Evaluate_DecimalDivision_ReturnsCorrectResult()
		{
			var evaluator = new RationalMathEvaluator();
			var result = evaluator.Evaluate("7.5 / 2.5");
			Assert.Equal(new RationalNumber(3), result);
		}

		[Fact] public void Evaluate_DivisionByZero_ThrowsException()
		{
			var evaluator = new RationalMathEvaluator();
			Assert.Throws<DivideByZeroException>(() => evaluator.Evaluate("5 / 0"));
		}
	}
}
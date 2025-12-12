using Xunit;
using ComputorV2.Core.Math;
using ComputorV2.Core.Types;

namespace ComputorV2.Tests
{
	public class SubjectTests
	{
		[Fact]
		static void SubjectTests_Assignment_RationalNumbers()
		{
			var evaluator = new MathEvaluator();

            var result1 = evaluator.Evaluate("varA = 2");
            Assert.IsType<RationalNumber>(result1);
            var rational1 = (RationalNumber)result1;
            Assert.Equal(2, rational1.Numerator);
		}
	}
}
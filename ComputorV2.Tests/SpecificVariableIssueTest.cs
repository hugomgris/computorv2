using System;
using Xunit;
using ComputorV2.Core.Math;
using ComputorV2.Core.Types;

namespace ComputorV2.Tests
{
    public class SpecificVariableIssueTest
    {
        [Fact]
        public void TestExactUserScenario()
        {
            var evaluator = new MathEvaluator();
            
            var simple = evaluator.Evaluate("2");
            Assert.IsType<RationalNumber>(simple);
            
            var addition = evaluator.Evaluate("2 + 3");
            Assert.IsType<RationalNumber>(addition);
            
            var multiplication = evaluator.Evaluate("2 * 3");
            if (multiplication is RationalNumber)
            {
                Assert.Equal(6, ((RationalNumber)multiplication).Numerator);
            }
            else if (multiplication is ComplexNumber complex)
            {
                Assert.Equal(6, complex.Real.Numerator);
                Assert.Equal(0, complex.Imaginary.Numerator);
            }
            else
            {
                throw new Exception($"Unexpected type: {multiplication.GetType()}");
            }
        }
    }
}

using System;
using Xunit;
using ComputorV2.Core.Math;
using ComputorV2.Core.Types;

namespace ComputorV2.Tests
{
    public class VariableAssignmentDebugTest
    {
        [Fact]
        public void DebugVariableAssignment()
        {
            var evaluator = new MathEvaluator();

            var result1 = evaluator.Evaluate("varA = 27");
            Assert.Equal(27, ((RationalNumber)result1).Numerator);

            var result2 = evaluator.Evaluate("2 * 27 - 5 % 4");
			Assert.IsType<RationalNumber>(result2);
            Assert.Equal(53, ((RationalNumber)result2).Numerator);
            
            var varAValue = evaluator.GetVariable("varA");
            Assert.NotNull(varAValue);
            Assert.Equal(27, ((RationalNumber)varAValue).Numerator);
            
            var result3 = evaluator.Evaluate("varB = 2 * varA - 5 % 4");
            Assert.Equal(53, ((RationalNumber)result3).Numerator);
        }
    }
}

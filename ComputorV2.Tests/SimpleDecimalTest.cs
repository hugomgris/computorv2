using Xunit;
using ComputorV2.Interactive;
using ComputorV2.Core.Math;

namespace ComputorV2.Tests
{
    public class SimpleDecimalTests
    {
        [Fact]
        public void TestNegativeDecimalAssignment()
        {
            var evaluator = new MathEvaluator();
            var result = evaluator.Evaluate("varC = -4.3");
            
            var assignmentInfo = evaluator.GetLastAssignmentInfo();
            Assert.NotNull(assignmentInfo);
            Assert.Equal("varC", assignmentInfo.Variable);
            
            Assert.IsType<ComputorV2.Core.Types.RationalNumber>(assignmentInfo.Value);
            
            var rational = (ComputorV2.Core.Types.RationalNumber)assignmentInfo.Value;
            
            var decimalString = rational.ToDecimalString();
            Assert.Equal("-4.3", decimalString);
        }
        
        [Fact]
        public void TestPositiveDecimalAssignment()
        {
            var evaluator = new MathEvaluator();
            var result = evaluator.Evaluate("varB = 4.242");
            
            var assignmentInfo = evaluator.GetLastAssignmentInfo();
            Assert.NotNull(assignmentInfo);
            Assert.Equal("varB", assignmentInfo.Variable);
            
            Assert.IsType<ComputorV2.Core.Types.RationalNumber>(assignmentInfo.Value);
            
            var rational = (ComputorV2.Core.Types.RationalNumber)assignmentInfo.Value;
            
            var decimalString = rational.ToDecimalString();
            Assert.Equal("4.242", decimalString);
        }
    }
}

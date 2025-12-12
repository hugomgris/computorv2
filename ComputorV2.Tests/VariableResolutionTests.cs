using System;
using Xunit;
using ComputorV2.Core.Math;
using ComputorV2.Core.Types;

namespace ComputorV2.Tests
{
    public class VariableResolutionTests
    {
        [Fact]
        public void TestVariableAssignmentAndUsage()
        {
            var evaluator = new MathEvaluator();

            var result1 = evaluator.Evaluate("varA = 2 + 4 * 2 - 5 % 4 + 2 * (4 + 5)");
            Assert.IsType<RationalNumber>(result1);
            var rational1 = (RationalNumber)result1;
            Assert.Equal(27, rational1.Numerator);
            
            var result2 = evaluator.Evaluate("varB = 2 * varA - 5 % 4");
            Assert.IsType<RationalNumber>(result2);
            var rational2 = (RationalNumber)result2;
            Assert.Equal(53, rational2.Numerator);
        }
        
        [Fact]
        public void TestSimpleVariableUsage()
        {
            var evaluator = new MathEvaluator();
            
            var assignResult = evaluator.Evaluate("x = 10");
            Assert.IsType<RationalNumber>(assignResult);
            Assert.Equal(10, ((RationalNumber)assignResult).Numerator);
            
            var result = evaluator.Evaluate("y = 2 * x + 3");
            Assert.IsType<RationalNumber>(result);
            var rational = (RationalNumber)result;
            Assert.Equal(23, rational.Numerator);
        }
        
        [Fact]
        public void TestVariableWithModulo()
        {
            var evaluator = new MathEvaluator();

            evaluator.Evaluate("varA = 27");
            
            var directResult = evaluator.Evaluate("2 * 27 - 5 % 4");
            Assert.IsType<RationalNumber>(directResult);
            Assert.Equal(53, ((RationalNumber)directResult).Numerator);
            
            var result = evaluator.Evaluate("varB = 2 * varA - 5 % 4");
            Assert.IsType<RationalNumber>(result);
            var rational = (RationalNumber)result;
            Assert.Equal(53, rational.Numerator);
        }
    }
}

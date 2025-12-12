using System;
using Xunit;
using ComputorV2.Core.Math;
using ComputorV2.Core.Types;

namespace ComputorV2.Tests
{
    public class UserExampleTests
    {
        [Fact]
        public void TestModuloOperatorBasics()
        {
            var evaluator = new MathEvaluator();
            
            var result1 = evaluator.Evaluate("4 % 2");
            Assert.IsType<RationalNumber>(result1);
            Assert.Equal(0, ((RationalNumber)result1).Numerator);
            
            var result2 = evaluator.Evaluate("7 % 3");
            Assert.IsType<RationalNumber>(result2);
            Assert.Equal(1, ((RationalNumber)result2).Numerator);
        }
        
        [Fact]
        public void TestFunctionDefinitionWithModuloOperator()
        {
            var evaluator = new MathEvaluator();
            
            var result = evaluator.Evaluate("funB(y) = 43 * y / (4 % 2 * y)");
            Assert.NotNull(result);
            Assert.IsType<Function>(result);
        }
        
        [Fact]
        public void TestFunctionDefinitionWithValidModulo()
        {
            var evaluator = new MathEvaluator();
            
            var result = evaluator.Evaluate("funC(x) = 10 * x / (7 % 3)");
            Assert.NotNull(result);
            Assert.IsType<Function>(result);
        }
    }
}

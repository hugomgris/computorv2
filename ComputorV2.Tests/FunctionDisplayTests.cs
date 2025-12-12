using System;
using Xunit;
using ComputorV2.Core.Math;
using ComputorV2.Core.Types;

namespace ComputorV2.Tests
{
    public class FunctionDisplayTests
    {
        [Fact]
        public void TestFunctionDisplayPreservesOriginalExpression()
        {
            var evaluator = new MathEvaluator();

            var result1 = evaluator.Evaluate("funC(z) = -2 * z - 5");
            Assert.IsType<Function>(result1);
            var function1 = (Function)result1;
            Assert.Equal("funC(z) = -2 * z - 5", function1.ToString());
            
            var result2 = evaluator.Evaluate("funB(y) = 43 * y / (4 % 2 * y)");
            Assert.IsType<Function>(result2);
            var function2 = (Function)result2;
            Assert.Equal("funB(y) = 43 * y / (4 % 2 * y)", function2.ToString());
            
            var result3 = evaluator.Evaluate("funA(x) = 2*x^5 + 4*x^2 - 5*x + 4");
            Assert.IsType<Function>(result3);
            var function3 = (Function)result3;
            Assert.Equal("funA(x) = 2*x^5 + 4*x^2 - 5*x + 4", function3.ToString());
        }
    }
}

using System;
using Xunit;
using ComputorV2.Core.Math;
using ComputorV2.Core.Types;

namespace ComputorV2.Tests
{
    public class FunctionExpressionTests
    {
        [Fact]
        public void TestFunctionExpressionStorage()
        {
            var evaluator = new MathEvaluator();
            
            var result1 = evaluator.Evaluate("funC(z) = -2 * z - 5");
            Assert.IsType<Function>(result1);
            
            var function1 = (Function)result1;
            Assert.NotEqual("-6", function1.ToString());
            
            var result2 = evaluator.Evaluate("funB(y) = 43 * y / (4 % 2 * y)");
            Assert.IsType<Function>(result2);
            
            var function2 = (Function)result2;
            Assert.NotEqual("1", function2.ToString());
        }
        
        [Fact]
        public void TestSimpleFunctionExpressionStorage()
        {
            var evaluator = new MathEvaluator();

            var result = evaluator.Evaluate("f(x) = 2*x + 3");
            Assert.IsType<Function>(result);
            
            var function = (Function)result;
            Assert.Contains("x", function.ToString());
        }
    }
}

using Xunit;
using ComputorV2.Core.Math;

namespace ComputorV2.Tests
{
    public class FunctionDebugTests
    {
        [Fact]
        public void TestModuloOperatorSupport()
        {
            var evaluator = new MathEvaluator();
            
            var result = evaluator.Evaluate("4 % 2");
            Assert.NotNull(result);

            var rational = Assert.IsType<ComputorV2.Core.Types.RationalNumber>(result);
            Assert.Equal(0, rational.Numerator);

            var result2 = evaluator.Evaluate("7 % 3");
            Assert.NotNull(result2);

            var rational2 = Assert.IsType<ComputorV2.Core.Types.RationalNumber>(result2);
            Assert.Equal(1, rational2.Numerator);
        }
        
        [Fact] 
        public void TestFunctionDefinitionWithModulo()
        {
            var evaluator = new MathEvaluator();
            
            string functionDef = "funB(y) = 43 * y / (4 % 2 * y)";
            bool isAssignment = evaluator.IsAssignment(functionDef);
            Assert.True(isAssignment, "Function definition should be recognized as an assignment");

            var result = evaluator.Evaluate(functionDef);
            Assert.NotNull(result);
        }
        
        [Fact]
        public void TestSimpleFunctionDefinition()
        {
            var evaluator = new MathEvaluator();
            
            var result = evaluator.Evaluate("funA(x) = 2*x^5 + 4*x^2 - 5*x + 4");
            Assert.NotNull(result);
        }
        
        [Fact]
        public void TestFunctionDefinitionWithNegativeConstant()
        {
            var evaluator = new MathEvaluator();
            
            var result = evaluator.Evaluate("funC(z) = -2 * z - 5");
            Assert.NotNull(result);
        }
    }
}

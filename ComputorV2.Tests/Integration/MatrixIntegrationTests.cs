using ComputorV2.Core.Math;
using ComputorV2.Core.Types;
using Xunit;

namespace ComputorV2.Tests.Integration
{
    public class MatrixIntegrationTests
    {
        [Fact]
        public void Matrix_OneDimensional_ShouldWork()
        {
            var evaluator = new MathEvaluator();
            
            var result = evaluator.Evaluate("[1,2]");
            
            Assert.IsType<Matrix>(result);
            var matrix = (Matrix)result;
            Assert.Equal(1, matrix.Rows);
            Assert.Equal(2, matrix.Cols);
        }

        [Fact] 
        public void Matrix_Assignment_ShouldWork()
        {
            var evaluator = new MathEvaluator();

            var result = evaluator.Evaluate("A = [1,2]");
            
            Assert.IsType<Matrix>(result);
            var matrix = (Matrix)result;
            Assert.Equal(1, matrix.Rows);
            Assert.Equal(2, matrix.Cols);
        }

        [Fact]
        public void Matrix_VariableLookup_ShouldWork()
        {
            var evaluator = new MathEvaluator();
            
            evaluator.Evaluate("A = [1,2]");
            
            var result = evaluator.Evaluate("A");
            
            Assert.IsType<Matrix>(result);
            var matrix = (Matrix)result;
            Assert.Equal(1, matrix.Rows);
            Assert.Equal(2, matrix.Cols);
        }

        [Fact]
        public void Matrix_TwoDimensional_ShouldWork()
        {
            var evaluator = new MathEvaluator();
            
            var result = evaluator.Evaluate("[[1,2];[3,4]]");
            
            Assert.IsType<Matrix>(result);
            var matrix = (Matrix)result;
            Assert.Equal(2, matrix.Rows);
            Assert.Equal(2, matrix.Cols);
        }

        [Fact]
        public void Matrix_ArithmeticWithVariables_ShouldWork()
        {
            var evaluator = new MathEvaluator();
            
            evaluator.Evaluate("a = [[1,2];[3,4]]");
            evaluator.Evaluate("b = [[5,6];[7,8]]");
            
            var result = evaluator.Evaluate("a + b");
            
            Assert.IsType<Matrix>(result);
            var matrix = (Matrix)result;
            Assert.Equal(2, matrix.Rows);
            Assert.Equal(2, matrix.Cols);
            Assert.Equal("6", matrix[0,0].ToString());  // 1+5
            Assert.Equal("12", matrix[1,1].ToString()); // 4+8
        }

        [Fact]
        public void Matrix_MultiplicationWithVariables_ShouldWork()
        {
            var evaluator = new MathEvaluator();
            
            evaluator.Evaluate("a = [[1,2,3];[4,5,6]]");  // 2x3
            evaluator.Evaluate("b = [[7,8];[9,10];[11,12]]");  // 3x2
            
            var result = evaluator.Evaluate("a * b");
            
            Assert.IsType<Matrix>(result);
            var matrix = (Matrix)result;
            Assert.Equal(2, matrix.Rows);
            Assert.Equal(2, matrix.Cols);
        }

        [Fact]
        public void Matrix_ComplexExpression_ShouldWork()
        {
            var evaluator = new MathEvaluator();
            
            var result = evaluator.Evaluate("[[1,2];[3,4]] + [[5,6];[7,8]]");
            
            Assert.IsType<Matrix>(result);
            var matrix = (Matrix)result;
            Assert.Equal(2, matrix.Rows);
            Assert.Equal(2, matrix.Cols);
            Assert.Equal("6", matrix[0,0].ToString());  // 1+5
            Assert.Equal("12", matrix[1,1].ToString()); // 4+8
        }
    }
}

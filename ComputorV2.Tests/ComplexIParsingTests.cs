using Xunit;
using ComputorV2.Core.Math;
using ComputorV2.Core.Types;

namespace ComputorV2.Tests
{
    public class ComplexIParsingTests
    {
        [Fact]
        public void ComplexParsing_DirectNotation_ShouldWork()
        {
            // Arrange
            var evaluator = new MathEvaluator();
            
            // Act
            var result = evaluator.Evaluate("varA = 3 + 2i");
            
            // Assert
            Assert.IsType<ComplexNumber>(result);
            var complex = result as ComplexNumber;
            Assert.Equal(new RationalNumber(3), complex!.Real);
            Assert.Equal(new RationalNumber(2), complex!.Imaginary);
        }
        
        [Fact]
        public void ComplexParsing_ExplicitMultiplication_ShouldWork()
        {
            // Arrange
            var evaluator = new MathEvaluator();
            
            // Act & Assert - This should not throw an exception
            var result = evaluator.Evaluate("varB = 2*i + 3");
            
            Assert.IsType<ComplexNumber>(result);
            var complex = result as ComplexNumber;
            Assert.Equal(new RationalNumber(3), complex!.Real);
            Assert.Equal(new RationalNumber(2), complex!.Imaginary);
        }
        
        [Fact]
        public void ComplexParsing_ExplicitMultiplicationReversed_ShouldWork()
        {
            // Arrange
            var evaluator = new MathEvaluator();
            
            // Act & Assert - This should not throw an exception
            var result = evaluator.Evaluate("varC = 3 + 2*i");
            
            Assert.IsType<ComplexNumber>(result);
            var complex = result as ComplexNumber;
            Assert.Equal(new RationalNumber(3), complex!.Real);
            Assert.Equal(new RationalNumber(2), complex!.Imaginary);
        }
    }
}

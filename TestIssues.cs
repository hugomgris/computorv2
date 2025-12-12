using System;
using Xunit;
using ComputorV2.Core.Math;
using ComputorV2.Core.Types;

namespace ComputorV2.Tests
{
    public class IssueTests
    {
        [Fact]
        public void TestNegativeComplexNumber()
        {
            // Issue 1: varB = -4i - 4 is not correctly parsed
            var evaluator = new MathEvaluator();
            var result = evaluator.Evaluate("-4i - 4");
            
            Assert.IsType<ComplexNumber>(result);
            var complex = (ComplexNumber)result;
            Console.WriteLine($"Result: {complex}");
            Assert.Equal(-4.0, complex.Real);
            Assert.Equal(-4.0, complex.Imaginary);
        }

        [Fact]
        public void TestSingleLetterVariable()
        {
            // Issue 2: input 'a' returns '1' - is this correct?
            var evaluator = new MathEvaluator();
            
            try
            {
                var result = evaluator.Evaluate("a");
                Console.WriteLine($"Single letter 'a' result: {result} (Type: {result.GetType().Name})");
                
                // This should probably throw an exception for undefined variable
                // or return something that indicates it's undefined
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception for 'a': {ex.Message}");
                // This might be the expected behavior
            }
        }

        [Fact]
        public void TestNegativeImaginaryUnit()
        {
            // Test specifically negative i patterns
            var evaluator = new MathEvaluator();
            
            var result1 = evaluator.Evaluate("-i");
            Console.WriteLine($"'-i' result: {result1} (Type: {result1.GetType().Name})");
            
            var result2 = evaluator.Evaluate("-4i");
            Console.WriteLine($"'-4i' result: {result2} (Type: {result2.GetType().Name})");
            
            var result3 = evaluator.Evaluate("4 - 4i");
            Console.WriteLine($"'4 - 4i' result: {result3} (Type: {result3.GetType().Name})");
        }
    }
}

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
            Assert.Equal(new RationalNumber(-4), complex.Real);
            Assert.Equal(new RationalNumber(-4), complex.Imaginary);
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
                // Let's see what the current behavior is...
                if (result is Polynomial poly)
                {
                    Console.WriteLine($"Polynomial toString: {poly}");
                }
                
                // For ComputorV2, undefined variables should probably be treated as errors
                // Assert.Throws<ArgumentException>(() => evaluator.Evaluate("a"));
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
            
            try
            {
                var result1 = evaluator.Evaluate("-i");
                Console.WriteLine($"'-i' result: {result1} (Type: {result1.GetType().Name})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception for '-i': {ex.Message}");
            }
            
            try
            {
                var result2 = evaluator.Evaluate("-4i");
                Console.WriteLine($"'-4i' result: {result2} (Type: {result2.GetType().Name})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception for '-4i': {ex.Message}");
            }
            
            try
            {
                var result3 = evaluator.Evaluate("4 - 4i");
                Console.WriteLine($"'4 - 4i' result: {result3} (Type: {result3.GetType().Name})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception for '4 - 4i': {ex.Message}");
            }
            
            try
            {
                var result4 = evaluator.Evaluate("-4 - 4i");
                Console.WriteLine($"'-4 - 4i' result: {result4} (Type: {result4.GetType().Name})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception for '-4 - 4i': {ex.Message}");
            }
        }
    }
}

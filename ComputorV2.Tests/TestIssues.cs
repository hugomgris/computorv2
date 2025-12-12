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
            var evaluator = new MathEvaluator();
            
            try
            {
                var result = evaluator.Evaluate("a");
                Console.WriteLine($"Single letter 'a' result: {result} (Type: {result.GetType().Name})");
                
                if (result is Polynomial poly)
                {
                    Console.WriteLine($"Polynomial toString: {poly}");
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception for 'a': {ex.Message}");
            }
        }

        [Fact]
        public void TestDecimalDisplay()
        {
            var evaluator = new MathEvaluator();
            
            var result1 = evaluator.Evaluate("4.242");
            Console.WriteLine($"4.242 result: {result1} (Type: {result1.GetType().Name})");

            var result2 = evaluator.Evaluate("-4.3");
            Console.WriteLine($"-4.3 result: {result2} (Type: {result2.GetType().Name})");
            
            var repl = new ComputorV2.Interactive.REPL();
            var replOutput1 = repl.ProcessCommand("varB = 4.242");
            var replOutput2 = repl.ProcessCommand("varC = -4.3");
            Console.WriteLine($"REPL varB = 4.242: {replOutput1}");
            Console.WriteLine($"REPL varC = -4.3: {replOutput2}");
        }

        [Fact]
        public void DebugComplexNumberDetection()
        {
            var evaluator = new MathEvaluator();
            
            var isComplex1 = IsComplexNumberExpression(evaluator, "4.242");
            var isComplex2 = IsComplexNumberExpression(evaluator, "-4.3");
            Console.WriteLine($"4.242 detected as complex: {isComplex1}");
            Console.WriteLine($"-4.3 detected as complex: {isComplex2}");
            
            var tokenizer = new ComputorV2.Core.Lexing.Tokenizer();
            var tokens1 = tokenizer.Tokenize("4.242");
            var tokens2 = tokenizer.Tokenize("-4.3");
            Console.WriteLine($"4.242 tokens: [{string.Join(", ", tokens1)}]");
            Console.WriteLine($"-4.3 tokens: [{string.Join(", ", tokens2)}]");
            
            var result1 = evaluator.Evaluate("5");
            var result2 = evaluator.Evaluate("-5");  
            var result3 = evaluator.Evaluate("0 - 5");
            Console.WriteLine($"5 result: {result1} (Type: {result1.GetType().Name})");
            Console.WriteLine($"-5 result: {result2} (Type: {result2.GetType().Name})"); 
            Console.WriteLine($"0 - 5 result: {result3} (Type: {result3.GetType().Name})");
        }
        
        private bool IsComplexNumberExpression(MathEvaluator evaluator, string expression)
        {
            var method = evaluator.GetType().GetMethod("IsComplexNumberExpression", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return (bool)method.Invoke(evaluator, new object[] { expression });
        }

        [Fact]
        public void TestNegativeImaginaryUnit()
        {
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

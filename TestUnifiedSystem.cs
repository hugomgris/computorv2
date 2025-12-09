using ComputorV2.Core.Math;
using ComputorV2.Core.Types;

namespace ComputorV2;

public class TestUnifiedSystem
{
    public static void RunTests()
    {
        var evaluator = new MathEvaluator();
        
        Console.WriteLine("=== Testing Unified Mathematical System ===\n");
        
        // Test 1: Rational number assignment
        Console.WriteLine("Test 1: Rational number assignment");
        evaluator.Evaluate("a = 5");
        var result1 = evaluator.Evaluate("a");
        Console.WriteLine($"a = 5 -> {result1}");
        Console.WriteLine($"Type: {result1.GetType().Name}\n");
        
        // Test 2: Complex number assignment
        Console.WriteLine("Test 2: Complex number assignment");
        evaluator.Evaluate("z = 2 + 3i");
        var result2 = evaluator.Evaluate("z");
        Console.WriteLine($"z = 2 + 3i -> {result2}");
        Console.WriteLine($"Type: {result2.GetType().Name}\n");
        
        // Test 3: Mixed arithmetic (rational + complex)
        Console.WriteLine("Test 3: Mixed arithmetic (rational + complex)");
        var result3 = evaluator.Evaluate("a + z");
        Console.WriteLine($"a + z = {result3}");
        Console.WriteLine($"Type: {result3.GetType().Name}\n");
        
        // Test 4: Complex arithmetic
        Console.WriteLine("Test 4: Complex arithmetic");
        evaluator.Evaluate("w = 1 + 1i");
        var result4 = evaluator.Evaluate("z * w");
        Console.WriteLine($"z * w = {result4}");
        Console.WriteLine($"Type: {result4.GetType().Name}\n");
        
        // Test 5: Power operations
        Console.WriteLine("Test 5: Power operations");
        var result5 = evaluator.Evaluate("z ^ 2");
        Console.WriteLine($"z ^ 2 = {result5}");
        Console.WriteLine($"Type: {result5.GetType().Name}\n");
        
        Console.WriteLine("=== All tests completed successfully! ===");
    }
}

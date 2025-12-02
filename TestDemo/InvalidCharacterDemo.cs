using ComputorV2.Core.Math;

namespace ComputorV2.TestDemo;

public class InvalidCharacterDemo
{
	public static void RunDemo()
	{
		var evaluator = new MathEvaluator();
		
		Console.WriteLine("=== Invalid Character Error Handling Demo ===\n");

		// Test cases that should show improved error messages
		string[] testCases = {
			"@ + @",
			"$ * 2", 
			"a + #",
			"% - 5",
			"2 + & + 3"
		};
		
		foreach (var testCase in testCases)
		{
			Console.Write($"Input: {testCase}");
			
			try
			{
				var result = evaluator.Evaluate(testCase);
				Console.WriteLine($" → {result}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($" → Error: {ex.Message}");
			}
		}
		
		Console.WriteLine("\n✅ Demo completed!");
	}
}

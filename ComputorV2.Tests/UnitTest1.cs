using System;
using System.Diagnostics;
using ComputorV2.Interactive;

namespace ComputorV2.Tests
{
    public class HistoryManagerTests
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Running HistoryManager Tests...");
            
            var tester = new HistoryManagerTests();
            tester.RunAllTests();
            
            tester.DisplayPassedTest("All tests completed!");
        }
        
        public void RunAllTests()
        {
            Constructor_WithValidCapacity_InitializesCorrectly();
            AddCommand_WithValidCommand_AddsToHistory();
            AddCommand_WithEmptyOrNullCommand_DoesNotAdd();
            AddCommand_WithConsecutiveDuplicates_OnlyAddsOnce();
            AddCommand_WhenAtCapacity_RemovesOldestCommand();
            GetCommand_WithValidIndex_ReturnsCommand();
            GetCommand_WithInvalidIndex_ReturnsNull();
            ClearHistory_RemovesAllCommands();
            MoveToPrevious_WithEmptyHistory_DoesNothing();
        }

        public void Constructor_WithValidCapacity_InitializesCorrectly()
        {
            var history = new HistoryManager(100);
            
            Debug.Assert(history.Count == 0);
            DisplayPassedTest("✓ Constructor test passed");
        }

        public void AddCommand_WithValidCommand_AddsToHistory()
        {
            var history = new HistoryManager();
            
            history.AddCommand("a = 5");
            
            Debug.Assert(history.Count == 1);
            Debug.Assert(history.GetCommand(0) == "a = 5");
            DisplayPassedTest("✓ AddCommand test passed");
        }

        public void AddCommand_WithEmptyOrNullCommand_DoesNotAdd()
        {
            var history = new HistoryManager();
            
            history.AddCommand("");
            history.AddCommand(null);
            history.AddCommand("   ");
            
            Debug.Assert(history.Count == 0);
            DisplayPassedTest("✓ Empty/null command test passed");
        }

        public void AddCommand_WithConsecutiveDuplicates_OnlyAddsOnce()
        {
            var history = new HistoryManager();
            
            history.AddCommand("a = 5");
            history.AddCommand("a = 5");
            
            Debug.Assert(history.Count == 1);
            DisplayPassedTest("✓ Consecutive duplicates test passed");
        }

        public void AddCommand_WhenAtCapacity_RemovesOldestCommand()
        {
            var history = new HistoryManager(2);
            
            history.AddCommand("first");
            history.AddCommand("second");
            history.AddCommand("third");
            
            Debug.Assert(history.Count == 2);
            Debug.Assert(history.GetCommand(0) == "second");
            Debug.Assert(history.GetCommand(1) == "third");
            DisplayPassedTest("✓ Capacity management test passed");
        }

        public void GetCommand_WithValidIndex_ReturnsCommand()
        {
            var history = new HistoryManager();
            history.AddCommand("test command");
            
            string? result = history.GetCommand(0);
            
            Debug.Assert(result == "test command");
            DisplayPassedTest("✓ GetCommand valid index test passed");
        }

        public void GetCommand_WithInvalidIndex_ReturnsNull()
        {
            var history = new HistoryManager();
            history.AddCommand("test");

            Debug.Assert(history.GetCommand(-1) == null);
            Debug.Assert(history.GetCommand(1) == null);
            Debug.Assert(history.GetCommand(100) == null);
            DisplayPassedTest("✓ GetCommand invalid index test passed");
        }

        public void ClearHistory_RemovesAllCommands()
        {
            var history = new HistoryManager();
            history.AddCommand("a = 5");
            history.AddCommand("b = 10");
            
            history.ClearHistory();
            
            Debug.Assert(history.Count == 0);
            DisplayPassedTest("✓ ClearHistory test passed");
        }

        public void MoveToPrevious_WithEmptyHistory_DoesNothing()
        {
            var history = new HistoryManager();

            history.MoveToPrevious();
            
            string? result = history.GetCurrentCommand();
            
            Debug.Assert(history.GetCurrentCommand() == null);
            DisplayPassedTest("✓ MoveToPrevious with empty history test passed");
        }

        public void DisplayPassedTest(string outputMessage)
		{
			var originalColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(outputMessage);
			Console.ForegroundColor = originalColor;
		}
    }
}
using Xunit;
using ComputorV2.Interactive;

namespace ComputorV2.Tests
{
	public class HistoryManagerTests
	{
		// Naming convention: MethodName_Scenario_ExpectedBehavior
		[Fact]
		public void Constructor_WithValidCapacity_InitializesCorrectly()
		{
			var history = new HistoryManager(100);
			
			Assert.Equal(0, history.Count);
		}

		[Fact]
		public void AddCommand_WithValidCommand_AddsToHistory()
		{
			var history = new HistoryManager();
			
			history.AddCommand("a = 5");
			
			Assert.Equal(1, history.Count);
			Assert.Equal("a = 5", history.GetCommand(0));
		}

		[Fact]
		public void AddCommand_WithEmptyOrNullCommand_DoesNotAdd()
		{
			var history = new HistoryManager();
			
			history.AddCommand("");
			history.AddCommand(null);
			history.AddCommand("   ");
			
			Assert.Equal(0, history.Count);
		}

		[Fact]
		public void AddCommand_WithConsecutiveDuplicates_OnlyAddsOnce()
		{
			var history = new HistoryManager();
			
			history.AddCommand("a = 5");
			history.AddCommand("a = 5");
			
			Assert.Equal(1, history.Count);
		}

		[Fact]
		public void AddCommand_WhenAtCapacity_RemovesOldestCommand()
		{
			var history = new HistoryManager(2);
			
			history.AddCommand("first");
			history.AddCommand("second");
			history.AddCommand("third");
			
			Assert.Equal(2, history.Count);
			Assert.Equal("second", history.GetCommand(0));
			Assert.Equal("third", history.GetCommand(1));
		}

		[Fact]
		public void GetCommand_WithValidIndex_ReturnsCommand()
		{
			var history = new HistoryManager();
			history.AddCommand("test command");
			
			string? result = history.GetCommand(0);
			
			Assert.Equal("test command", result);
		}

		[Fact]
		public void GetCommand_WithInvalidIndex_ReturnsNull()
		{
			var history = new HistoryManager();
			history.AddCommand("test");

			Assert.Null(history.GetCommand(-1));
			Assert.Null(history.GetCommand(1));
			Assert.Null(history.GetCommand(100));
		}

		[Fact]
		public void ClearHistory_RemovesAllCommands()
		{
			var history = new HistoryManager();
			history.AddCommand("a = 5");
			history.AddCommand("b = 10");
			
			history.ClearHistory();
			
			Assert.Equal(0, history.Count);
		}

		[Fact]
		public void MoveToPrevious_WithEmptyHistory_DoesNothing()
		{
			var history = new HistoryManager();

			history.MoveToPrevious();
			
			Assert.Null(history.GetCurrentCommand());
		}

		[Fact]
		public void MoveToPrevious_FromInitialState_GoesToNewestCommand()
		{
			var history = new HistoryManager();

			history.AddCommand("a = 5");
			history.AddCommand("b = 10");

			history.MoveToPrevious();
			Assert.Equal("b = 10", history.GetCurrentCommand());
		}

		[Fact]
		public void MoveToPrevious_MultipleCommands_NavigatesBackward()
		{
			var history = new HistoryManager();

			history.AddCommand("first");
			history.AddCommand("second");
			history.AddCommand("third");

			history.MoveToPrevious();
			Assert.Equal("third", history.GetCurrentCommand());

			history.MoveToPrevious();
			Assert.Equal("second", history.GetCurrentCommand());

			history.MoveToPrevious();
			Assert.Equal("first", history.GetCurrentCommand());

			history.MoveToPrevious();
			Assert.Equal("first", history.GetCurrentCommand());
		}

		[Fact]
		public void MoveToPrevious_AtOldestCommand_StaysAtOldest()
		{
			var history = new HistoryManager();

			history.AddCommand("a = 5");
			history.AddCommand("b = 10");
			
			for (int i = 0; i < 3; i++)
			{
				history.MoveToPrevious();
			}

			Assert.Equal("a = 5", history.GetCurrentCommand());
		}

		[Fact]
		public void MoveToNext_WithEmptyHistory_DoesNothing()
		{
			var history = new HistoryManager();

			history.MoveToNext();
			
			Assert.Null(history.GetCurrentCommand());
		}

		[Fact]
		public void MoveToNext_FromOldestCommand_MovesToNewer()
		{
			var history = new HistoryManager();

			history.AddCommand("first");
			history.AddCommand("second");
			history.AddCommand("third");

			history.MoveToPrevious();
			history.MoveToPrevious();
			history.MoveToPrevious();

			history.MoveToNext();

			Assert.Equal("second", history.GetCurrentCommand());
		}

		[Fact]
		public void MoveToNext_AtNewestCommand_ReturnsToPrompt()
		{
			var history = new HistoryManager();

			history.AddCommand("first");

			history.MoveToPrevious();

			history.MoveToNext();
			history.MoveToNext();

			Assert.Null(history.GetCurrentCommand());
		}

		[Fact]
		public void MoveToNext_FromPromptState_StaysAtPrompt()
		{
			var history = new HistoryManager();

			history.MoveToNext();

			Assert.Null(history.GetCurrentCommand());
		}

		[Fact]
		public void TypicalUserNavigationWorkflow_WorksCorrectly()
		{
			var history = new HistoryManager();
			history.AddCommand("a = 5");
			history.AddCommand("b = 10");  
			history.AddCommand("c = 15");
			
			history.MoveToPrevious();
			Assert.Equal("c = 15", history.GetCurrentCommand());
			
			history.MoveToPrevious();
			Assert.Equal("b = 10", history.GetCurrentCommand());
			
			history.MoveToPrevious();
			Assert.Equal("a = 5", history.GetCurrentCommand());
			
			history.MoveToNext();
			Assert.Equal("b = 10", history.GetCurrentCommand());
			
			history.MoveToNext();
			Assert.Equal("c = 15", history.GetCurrentCommand());
			
			history.MoveToNext(); 
			Assert.Null(history.GetCurrentCommand());
		}

		[Fact]
		public void AddCommand_DuringNavigation_ResetsNavigationState()
		{
			var history = new HistoryManager();
			history.AddCommand("first");
			history.AddCommand("second");
			
			history.MoveToPrevious(); 
			Assert.Equal("second", history.GetCurrentCommand());
			
			history.AddCommand("third");
			
			Assert.Null(history.GetCurrentCommand());
		}

		[Fact]
		public void NavigationWithSingleCommand_HandlesAllOperations()
		{
			var history = new HistoryManager();
			history.AddCommand("only command");
			
			history.MoveToPrevious();
			Assert.Equal("only command", history.GetCurrentCommand());
			
			history.MoveToPrevious();
			Assert.Equal("only command", history.GetCurrentCommand());
			
			history.MoveToNext();
			Assert.Null(history.GetCurrentCommand());
			
			history.MoveToNext();
			Assert.Null(history.GetCurrentCommand());
		}

		// Persistency tests
		[Fact]
		public void SaveToFile_WithValidPath_CreatesFileWithCommands()
		{
			var history = new HistoryManager();
			history.AddCommand("a = 5");
			history.AddCommand("b = 10");  
			var testPath = Path.GetTempFileName();

			try
			{
				history.SaveToFile(testPath);
				Assert.True(File.Exists(testPath));
				
				var savedLines = File.ReadAllLines(testPath);
				Assert.Equal(2, savedLines.Length);
				Assert.Equal("a = 5", savedLines[0]);
				Assert.Equal("b = 10", savedLines[1]);
			}
			finally
			{
				if (File.Exists(testPath)) File.Delete(testPath);
			}
		}

		[Fact] 
		public void LoadFromFile_WithExistingFile_LoadsCommands()
		{
			var history = new HistoryManager();
			var testPath = Path.GetTempFileName();
			history.AddCommand("a = 5");
			history.AddCommand("b = 10");  

			try
			{
				history.SaveToFile(testPath);
				Assert.True(File.Exists(testPath));

				history.LoadFromFile(testPath);
				history.MoveToPrevious();
				Assert.Equal("b = 10", history.GetCurrentCommand());
				history.MoveToPrevious();
				Assert.Equal("a = 5", history.GetCurrentCommand());
			}
			finally
			{
				if (File.Exists(testPath)) File.Delete(testPath);
			}
		}

		[Fact]
		public void SaveAndLoadRoundTrip_PreservesHistory()
		{
			var history = new HistoryManager();
			var testPath = Path.GetTempFileName();
			history.AddCommand("a = 5");
			history.AddCommand("b = 10");

			try
			{
				history.SaveToFile(testPath);
				Assert.True(File.Exists(testPath));

				history.LoadFromFile(testPath);
				history.AddCommand("c = 15");
				history.SaveToFile(testPath);
				
				var lines = File.ReadAllLines(testPath);
				Assert.Equal(3, lines.Length);
			}
			finally
			{
				if (File.Exists(testPath)) File.Delete(testPath);
			}
		}

		[Fact]
		public void LoadFromFile_WithNonExistentFile_CreatesEmptyFile()
		{
			var history = new HistoryManager();
			var testPath = Path.GetTempFileName();

			try
			{
				history.LoadFromFile(testPath);
				Assert.True(File.Exists(testPath));
			}
			finally
			{
				if (File.Exists(testPath)) File.Delete(testPath);
			}
		}
	}
}
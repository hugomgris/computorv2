namespace ComputorV2.Interactive
{
	class HelpSystem
	{
		private string[]?	_helpLines = null;
		
		public void Help()
		{
			foreach (var line in _helpLines!)
			{			
				Console.WriteLine(line);
			}
		}

		public void LoadFromFile(string filePath)
		{
			try
			{
				_helpLines = File.ReadAllLines(filePath);
			}
			catch (DirectoryNotFoundException)
			{
				throw new DirectoryNotFoundException($"Invalid directory path: {filePath}");
			}
			catch (UnauthorizedAccessException)
			{
				throw new UnauthorizedAccessException($"No permission to read: {filePath}");
			}
			catch (FileNotFoundException)
			{
				throw new FileNotFoundException($"History file not found: {filePath}");
			}
			catch (IOException ex)
			{
				throw new IOException($"Failed to load history from {filePath}: {ex.Message}");
			}
		}

		public string GetDefaultHelpFilePath()
		{
			string currentDirectory = Directory.GetCurrentDirectory();
			return currentDirectory + "/docs/Help";
		}

		public void LoadFromDefaultLocation()
		{
			LoadFromFile(GetDefaultHelpFilePath());
		}
	}
}
using System.IO;

namespace ComputorV2.Interactive
{
	public class HistoryManager
	{
		private List<string>	_history;
		private int				_maxCapacity;
		private int				_currentIndex = -1;

		public int Count => _history.Count;

		public HistoryManager(int maxCapacity = 1000)
		{
			if (maxCapacity <= 0)
				throw new ArgumentException("Capacity must be positive", nameof(maxCapacity));
			_maxCapacity = maxCapacity;
			_history = new List<string>();
			_history.Capacity = _maxCapacity;
		}

	public void AddCommand(string? command)
	{
		command = command?.Trim();
		if (string.IsNullOrEmpty(command)) return;
		
		if (_history.Count > 0 &&
				string.Equals(command, _history[_history.Count - 1], StringComparison.OrdinalIgnoreCase))
				return;

			if (_history.Count == _maxCapacity)
			{
				_history.RemoveAt(0);
			}

			_history.Add(command);
			ResetNavigation();
		}

		public string? GetCommand(int index)
		{
			if (index < 0 || index > _history.Count - 1) return null;

			return (_history[index]);
		}

		public void ClearHistory()
		{
			_history.Clear();
		}

		public void MoveToPrevious()
		{
			if (_currentIndex == -1)
			{
				_currentIndex = _history.Count - 1;
				return;
			}

			if (_currentIndex == 0) return;

			_currentIndex -= 1;
		}

		public void MoveToNext()
		{
			if (_currentIndex == -1 || _currentIndex == _history.Count - 1) 
			{
				_currentIndex = -1;
				return;
			}

			_currentIndex += 1;
		}

		public string? GetCurrentCommand()
		{
			if (_currentIndex == -1 || _currentIndex >= _history.Count) return null;
			return _history[_currentIndex];
		}

		public void ResetNavigation()
		{
			_currentIndex = -1;
		}

		public void SaveToFile(string filePath)
		{
			try
			{
				var directory = Path.GetDirectoryName(filePath);
				if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
				{
					Directory.CreateDirectory(directory);
				}

				File.WriteAllLines(filePath, _history);
			}
			catch (DirectoryNotFoundException)
			{
				throw new DirectoryNotFoundException($"Invalid directory path: {Path.GetDirectoryName(filePath)}");
			}
			catch (UnauthorizedAccessException)
			{
				throw new UnauthorizedAccessException($"No permission to write to: {filePath}");
			}
			catch (IOException ex)
			{
				throw new IOException($"Failed to save history to {filePath}: {ex.Message}");
			}
		}

		public void LoadFromFile(string filePath)
		{
			try
			{
				if (!File.Exists(filePath))
				{
					File.WriteAllText(filePath, "");
					return;
				}

				var lines = File.ReadAllLines(filePath);
				_history.Clear();

				foreach (var line in lines)
				{
					var trimmed = line.Trim();
					if (!string.IsNullOrEmpty(trimmed))
					{
						_history.Add(trimmed);
					}
				}

				ResetNavigation();
			}
			catch (DirectoryNotFoundException)
			{
				throw new DirectoryNotFoundException($"Invalid directory path: {Path.GetDirectoryName(filePath)}");
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

		private string GetDefaultHistoryFilePath()
		{
			var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
			return Path.Combine(homeDirectory, ".computorv2_history");
		}

		public void SaveToDefaultLocation()
		{
			SaveToFile(GetDefaultHistoryFilePath());
		}

		public void LoadFromDefaultLocation()
		{
			LoadFromFile(GetDefaultHistoryFilePath());
		}

		public void DeleteHistoryFile()
		{
			var filePath = GetDefaultHistoryFilePath();
			if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
			{
				File.Delete(filePath);
				Console.WriteLine($"History file deleted (path: {filePath})");
			}
		}
	}
}
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
			ResetNavigation(); // Reset navigation when new command is added
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
	}
}
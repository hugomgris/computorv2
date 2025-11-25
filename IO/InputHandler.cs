using System;
using System.Text;
using ComputorV2.Interactive;

namespace ComputorV2.IO
{
	public class InputHandler
	{
		private readonly DisplayManager	_displayManager;
		private readonly HistoryManager	_historyManager;
		private StringBuilder			_inputBuffer;
		private int						_cursorPosition;
		private bool					_isInHistoryMode;

		public bool IsInHistoryMode => _isInHistoryMode;

		public InputHandler(DisplayManager displayManager, HistoryManager historyManager)
		{
			_displayManager = displayManager;
			_historyManager = historyManager;
			_inputBuffer = new StringBuilder();
			_cursorPosition = 0;
			_isInHistoryMode = false;
		}

		public string ReadLine()
		{
			_inputBuffer.Clear();
			_cursorPosition = 0;
			_isInHistoryMode = false;

			while (true)
			{
				ConsoleKeyInfo keyInfo = Console.ReadKey(true);

				switch (keyInfo.Key)
				{
					case ConsoleKey.Enter:
						Console.WriteLine();
						return _inputBuffer.ToString();

					case ConsoleKey.Backspace:
						HandleBackspace();
						break;

					case ConsoleKey.Delete:
						HandleDelete();
						break;

					case ConsoleKey.LeftArrow:
						HandleLeftArrow();
						break;

					case ConsoleKey.RightArrow:
						HandleRightArrow();
						break;

					case ConsoleKey.UpArrow:
						HandleUpArrow();
						break;

					case ConsoleKey.DownArrow:
						HandleDownArrow();
						break;

					case ConsoleKey.Home:
						HandleHome();
						break;

					case ConsoleKey.End:
						HandleEnd();
						break;

					default:
						if (char.IsControl(keyInfo.KeyChar))
						{
							continue;
						}

						HandleCharacterInput(keyInfo.KeyChar);
						break;
				}

				RefreshDisplay();
			}
		}

		private void HandleCharacterInput(char character)
		{
			if (_isInHistoryMode)
			{
				_historyManager.ResetNavigation();
				_isInHistoryMode = false;
			}

			if (_cursorPosition >= _inputBuffer.Length)
			{
				_inputBuffer.Append(character);
			}
			else
			{
				_inputBuffer.Insert(_cursorPosition, character);
			}
			_cursorPosition++;
		}

		private void HandleBackspace()
		{
			if (_cursorPosition > 0)
			{
				_cursorPosition--;
				_inputBuffer.Remove(_cursorPosition, 1);
			}
		}

		private void HandleDelete()
		{
			if (_cursorPosition < _inputBuffer.Length)
			{
				_inputBuffer.Remove(_cursorPosition, 1);
			}
		}

		private void HandleLeftArrow()
		{
			if (_cursorPosition > 0)
			{
				_cursorPosition--;
			}
		}

		private void HandleRightArrow()
		{
			if (_cursorPosition < _inputBuffer.Length)
			{
				_cursorPosition++;
			}
		}

		private void HandleUpArrow()
		{
			if (_historyManager.Count == 0) return;
			
			_historyManager.MoveToPrevious();
			string? command = _historyManager.GetCurrentCommand();
			if (command != null)
			{
				SetHistoryContent(command);
			}
		}

		private void HandleDownArrow()
		{
			if (_historyManager.Count == 0) return;
			
			_historyManager.MoveToNext();
			string? command = _historyManager.GetCurrentCommand();
			if (command != null)
			{
				SetHistoryContent(command);
			} else
			{
				ClearBuffer();
			}
		}

		private void HandleHome()
		{
			_cursorPosition = 0;
		}

		private void HandleEnd()
		{
			_cursorPosition = _inputBuffer.Length;
		}

		private void RefreshDisplay()
		{
			_displayManager.RefreshLine(_inputBuffer.ToString(), _cursorPosition);
		}

		public void SetHistoryContent(string content)
		{
			_inputBuffer.Clear();
			_inputBuffer.Append(content);
			_cursorPosition = _inputBuffer.Length;
			_isInHistoryMode = true;
		}

		public void ClearBuffer()
		{
			_inputBuffer.Clear();
			_cursorPosition = 0;
			_isInHistoryMode = false;
		}
	}
}
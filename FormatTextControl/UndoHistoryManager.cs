using Common;
using System.Collections.Generic;

namespace FormatTextControl
{
    public class UndoHistoryManager : IUndoHistoryManager
    {
        private readonly List<UndoRecord> _undoRecords = new List<UndoRecord>();
        private int _currentIndex = -1;

        public void AddUndoRecord(UndoRecord record)
        {
            var recordsToDelete = _undoRecords.Count - (_currentIndex + 1);
            if (recordsToDelete > 0)
            {
                _undoRecords.RemoveRange(_currentIndex + 1, recordsToDelete);
            }
            
            _undoRecords.Add(record);
            _currentIndex = _undoRecords.Count -1;
        }

        public void ClearUndoHistory()
        {
            _undoRecords.Clear();
            _currentIndex = 0;
        }

        public UndoRecord GetNextRedo()
        {
            if (_currentIndex > (_undoRecords.Count - 2))
            {
                return null;
            }

            _currentIndex++;
            var undoRecord = _undoRecords[_currentIndex];
            return undoRecord;
        }

        public UndoRecord GetNextUndo()
        {
            if (_currentIndex < 0 || _undoRecords.Count == 0)
            {
                return null;
            }

            var undoRecord = _undoRecords[_currentIndex];
            _currentIndex--;
            
            return undoRecord;
        }
    }
}

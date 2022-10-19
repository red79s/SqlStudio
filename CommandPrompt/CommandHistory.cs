using System;
using System.Collections.Generic;

namespace CommandPrompt
{
    public class CommandHistory
    {
        private int _maxItems = 0;
        private List<string> _items = null;
        private int _curIndex = 0;
        private string _tmpItem = "";
        private bool _needsReset = true;
        private bool _haveUnsavedChanges = false;
        public bool HaveUnsavedChanges => _haveUnsavedChanges;

        public CommandHistory(int maxItems)
        {
            if (maxItems < 1)
                maxItems = 1;
            _maxItems = maxItems;
            _items = new List<string>();
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public List<string> GetHistoryItems()
        {
            return _items;
        }

        public void SetHistoryItems(List<string> items)
        {
            Clear();

            for (int i = 0; i < _maxItems && i < items.Count; i++)
            {
                Add(items[i]);
            }
        }

        public void Clear()
        {
            _items.Clear();
            _curIndex = 0;
            _tmpItem = "";
            _needsReset = true;
            _haveUnsavedChanges = true;
        }

        public void Add(string text)
        {
            var trimedText = text.Trim().TrimEnd();
            Remove(trimedText);

            if (_items.Count >= _maxItems)
                _items.RemoveRange(0, 1);
            _items.Add(trimedText);
            NeedsReset = true;
            _haveUnsavedChanges = true;
        }

        public void Reset(string tmpText)
        {
            _tmpItem = tmpText;
            _curIndex = _items.Count;
            NeedsReset = false;
        }

        public void Remove(string text)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].Equals(text, StringComparison.CurrentCultureIgnoreCase))
                {
                    _items.RemoveAt(i);
                    return;
                }
            }
        }

        public bool NeedsReset
        {
            get { return _needsReset; }
            set { _needsReset = value; }
        }

        public string GetNext()
        {
            if (_curIndex < _items.Count)
                _curIndex++;
            if (_curIndex < _items.Count)
                return _items[_curIndex];
            else
                return _tmpItem;
        }

        public string GetPrev()
        {
            if (_curIndex > 0)
                _curIndex--;
            if (_items.Count > _curIndex)
                return _items[_curIndex];
            return "";
        }

        public void IsSaved()
        {
            _haveUnsavedChanges = false;
        }
    }
}

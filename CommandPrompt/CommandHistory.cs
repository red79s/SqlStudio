using Common;
using System;
using System.Collections.Generic;

namespace CommandPrompt
{
    public class CommandHistory
    {
        private List<string> _items = null;
        private int _curIndex = 0;
        private string _tmpItem = "";
        private bool _needsReset = true;
        private readonly ICommandHistoryStore _commandHistoryStore;

        public CommandHistory(ICommandHistoryStore commandHistoryStore)
        {
            _items = new List<string>();
            _commandHistoryStore = commandHistoryStore;

            LoadHistoryItems();
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public List<string> GetHistoryItems()
        {
            return _items;
        }

        public void LoadHistoryItems()
        {
            Clear();

            foreach (var item in _commandHistoryStore.GetHistoryItems())
            {
                Add(item.Command);
            }
        }

        public void Clear()
        {
            _items.Clear();
            _curIndex = 0;
            _tmpItem = "";
            _needsReset = true;
        }

        public void Add(string text)
        {
            var trimedText = text.Trim().TrimEnd();
            Remove(trimedText);

            if (_items.Count >= _commandHistoryStore.MaxHistoryItems)
                _items.RemoveRange(0, 1);
            _items.Add(trimedText);
            _commandHistoryStore.AddHistoryItem(text);
            _curIndex = _items.Count;
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
    }
}

using FormatTextControl;

namespace Common
{
    public interface IUndoHistoryManager
    {
        void ClearUndoHistory();
        void AddUndoRecord(UndoRecord record);
        UndoRecord GetNextUndo();
        UndoRecord GetNextRedo();

    }
}

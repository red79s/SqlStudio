using System;

namespace SqlStudio
{
    public interface ISearchControl
    {
        event EventHandler<string> SearchUp;
        event EventHandler<string> SearchDown;
        event EventHandler<string> HideRows;
        event EventHandler UnhideRows;
        void SetSearchText(string text);
        bool IsVisible { get; set; }
    }
}

using System;
using System.Windows.Forms;

namespace SqlStudio
{
    public partial class SearchControl : UserControl, ISearchControl
    {
        public bool IsVisible { get; set; }

        public SearchControl()
        {
            InitializeComponent();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
        }

        public event EventHandler<string> SearchUp;
        public event EventHandler<string> SearchDown;
        public event EventHandler<string> HideRows;
        public event EventHandler UnhideRows;

        private void hideButton_Click(object sender, EventArgs e)
        {
            HideRows?.Invoke(this, searchTextBox.Text);
        }

        private void upButton_Click(object sender, EventArgs e)
        {
            SearchUp?.Invoke(this, searchTextBox.Text);
        }

        private void downButton_Click(object sender, EventArgs e)
        {
            SearchDown.Invoke(this, searchTextBox.Text);
        }

        public void SetSearchText(string text)
        {
            searchTextBox.Text = text;
        }

        private void unhideButton_Click(object sender, EventArgs e)
        {
            UnhideRows?.Invoke(this, new EventArgs());
        }
    }
}

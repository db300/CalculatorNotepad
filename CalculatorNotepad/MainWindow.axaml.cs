using Avalonia.Controls;
using System;

namespace CalculatorNotepad
{
    public partial class MainWindow : Window
    {
        #region constructor
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region event handler
        private void MenuItemSplitLine_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var textBox = _txtNoteBook;
            var text = textBox.Text;

            if (string.IsNullOrWhiteSpace(text)) return;

            // 按空格或逗号分割
            var lines = text.Split([' ', '，', ','], StringSplitOptions.RemoveEmptyEntries);
            var newText = string.Join(Environment.NewLine, lines);
            textBox.Text = newText;
        }
        #endregion
    }
}
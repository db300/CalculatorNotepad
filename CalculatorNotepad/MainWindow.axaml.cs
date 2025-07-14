using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using System;
using System.Collections.Generic;

namespace CalculatorNotepad
{
    public partial class MainWindow : Window
    {
        #region constructor
        public MainWindow()
        {
            InitializeComponent();

            Title = "计算器 & 记事本";

            _txtNoteBook.TextChanged += TxtNoteBook_TextChanged;
        }
        #endregion

        #region property
        private Stack<string> _undoStack = new();
        private Stack<string> _redoStack = new();
        #endregion

        #region method
        /// <summary>
        /// 撤销
        /// </summary>
        public void Undo()
        {
            if (_undoStack.Count > 1)
            {
                var current = _undoStack.Pop();
                _redoStack.Push(current);
                _txtNoteBook.Text = _undoStack.Peek();
            }
        }

        /// <summary>
        /// 重做
        /// </summary>
        public void Redo()
        {
            if (_redoStack.Count > 0)
            {
                var redoText = _redoStack.Pop();
                _undoStack.Push(redoText);
                _txtNoteBook.Text = redoText;
            }
        }

        private void ResetUndoRedo(string? newText)
        {
            if (newText != null && (_undoStack.Count == 0 || _undoStack.Peek() != newText))
            {
                _undoStack.Push(newText);
                _redoStack.Clear();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            // 检查Ctrl+Z
            if (e.Key == Key.Z && e.KeyModifiers.HasFlag(KeyModifiers.Control))
            {
                Undo();
                e.Handled = true;
            }
            // 检查Ctrl+Y
            else if (e.Key == Key.Y && e.KeyModifiers.HasFlag(KeyModifiers.Control))
            {
                Redo();
                e.Handled = true;
            }
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

        private void MenuItemMergeLines_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var textBox = _txtNoteBook;
            var text = textBox.Text;

            if (string.IsNullOrWhiteSpace(text)) return;

            // 按行分割，然后合并为一行（用空格连接）
            var lines = text.Split(["\r\n", "\n", "\r"], StringSplitOptions.RemoveEmptyEntries);
            var newText = string.Join(" ", lines);
            textBox.Text = newText;
        }

        private void MenuItemUndo_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Undo();
        }

        private void MenuItemRedo_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Redo();
        }

        private void TxtNoteBook_TextChanged(object? sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                ResetUndoRedo(textBox.Text);
            }
        }
        #endregion
    }
}
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace CalculatorNotepad;

public partial class Calculator4BaseConvert : UserControl
{
    #region constructor
    public Calculator4BaseConvert()
    {
        InitializeComponent();

        _hexButtons = [
            this.FindControl<Button>("btnA"),
            this.FindControl<Button>("btnB"),
            this.FindControl<Button>("btnC"),
            this.FindControl<Button>("btnD"),
            this.FindControl<Button>("btnE"),
            this.FindControl<Button>("btnF"),
        ];

        // 默认选中十进制
        SetActiveTextBlock(_txtDec);
        SetHexButtonsEnabled(false);

        // 允许控件聚焦以接收键盘事件
        Focusable = true;
        AttachedToVisualTree += (sender, e) =>
        {
            // 设置焦点到当前控件
            Focus();
        };
        KeyDown += Calculator4BaseConvert_KeyDown;
    }
    #endregion

    #region property
    private readonly Button?[] _hexButtons;
    private TextBlock? _activeTextBlock;
    private readonly IBrush _activeBrush = new SolidColorBrush(Color.Parse("#FFA500"));
    private readonly IBrush _normalBrush = new SolidColorBrush(Colors.Black);
    #endregion

    #region metod
    private void SetActiveTextBlock(TextBlock? tb)
    {
        _txtDec.Foreground = _normalBrush;
        _txtHex.Foreground = _normalBrush;

        _activeTextBlock = tb;
        if (tb != null)
        {
            tb.Foreground = _activeBrush;
        }
    }

    /// <summary>
    /// 辅助方法：设置A~F按钮的可用状态
    /// </summary>
    private void SetHexButtonsEnabled(bool enabled)
    {
        foreach (var btn in _hexButtons) btn?.SetValue(IsEnabledProperty, enabled);
    }

    private void InsertText(string text)
    {
        if (_activeTextBlock == null) return;
        if (_activeTextBlock.Text == "0")
            _activeTextBlock.Text = text;
        else
            _activeTextBlock.Text += text;
        UpdateBaseConvertResult(_activeTextBlock);
    }

    private void UpdateBaseConvertResult(TextBlock? activeTextBlock)
    {
        if (activeTextBlock == null) return;

        switch (activeTextBlock.Tag)
        {
            case "DEC":
                if (int.TryParse(activeTextBlock.Text, out int decValue))
                {
                    _txtHex.Text = decValue.ToString("X");
                }
                break;
            case "HEX":
                if (int.TryParse(activeTextBlock.Text, System.Globalization.NumberStyles.HexNumber, null, out int hexValue))
                {
                    _txtDec.Text = hexValue.ToString();
                }
                break;
        }
    }
    #endregion

    #region event handler
    private void KeyboardButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button btn || btn.Tag is not string tag || btn.Content is not string key) return;

        switch (tag)
        {
            case "Clear":
                _txtDec.Text = _txtHex.Text = "0";
                break;
            case "Delete":
                if (_activeTextBlock == null || _activeTextBlock.Text == null) break;
                if (_activeTextBlock.Text.Length > 1)
                {
                    _activeTextBlock.Text = _activeTextBlock.Text[..^1];
                }
                else
                {
                    _activeTextBlock.Text = "0";
                }
                UpdateBaseConvertResult(_activeTextBlock);
                break;
            case "Number":
                if (_activeTextBlock == null) break;
                if (_activeTextBlock.Text == "0")
                {
                    _activeTextBlock.Text = key;
                }
                else
                {
                    _activeTextBlock.Text += key;
                }
                UpdateBaseConvertResult(_activeTextBlock);
                break;
        }
    }

    private async void Calculator4BaseConvert_KeyDown(object? sender, KeyEventArgs e)
    {
        if (_activeTextBlock == null)
            return;

        // 处理Ctrl+C复制
        if (e.Key == Key.C && (e.KeyModifiers & KeyModifiers.Control) == KeyModifiers.Control)
        {
            if (!string.IsNullOrEmpty(_activeTextBlock.Text))
            {
                var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
                if (clipboard != null)
                {
                    await clipboard.SetTextAsync(_activeTextBlock.Text);
                }
            }
            e.Handled = true;
            return;
        }

        // 处理数字输入
        if (e.Key >= Key.D0 && e.Key <= Key.D9)
        {
            string num = ((int)e.Key - (int)Key.D0).ToString();
            InsertText(num);
            e.Handled = true;
            return;
        }
        if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
        {
            string num = ((int)e.Key - (int)Key.NumPad0).ToString();
            InsertText(num);
            e.Handled = true;
            return;
        }
        // 处理A~F（仅HEX激活时）
        if (_activeTextBlock.Tag?.ToString() == "HEX" && e.Key >= Key.A && e.Key <= Key.F)
        {
            string hex = ((char)('A' + (e.Key - Key.A))).ToString();
            InsertText(hex);
            e.Handled = true;
            return;
        }
        // 处理退格
        if (e.Key == Key.Back)
        {
            if (_activeTextBlock.Text?.Length > 1)
                _activeTextBlock.Text = _activeTextBlock.Text[..^1];
            else
                _activeTextBlock.Text = "0";
            UpdateBaseConvertResult(_activeTextBlock);
            e.Handled = true;
            return;
        }
        // 处理清空
        if (e.Key == Key.Delete)
        {
            _txtDec.Text = _txtHex.Text = "0";
            e.Handled = true;
            return;
        }
    }

    private void TextBlock_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not TextBlock tb || tb.Tag is not string key) return;

        SetActiveTextBlock(tb);
        switch (key)
        {
            case "DEC":
                SetHexButtonsEnabled(false);
                break;
            case "HEX":
                SetHexButtonsEnabled(true);
                break;
        }
    }
    #endregion
}
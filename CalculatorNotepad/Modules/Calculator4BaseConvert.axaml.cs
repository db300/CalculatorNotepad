using Avalonia;
using Avalonia.Controls;
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

    private void TextBlock_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
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
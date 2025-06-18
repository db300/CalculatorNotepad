using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Globalization;
using System.Linq;

namespace CalculatorNotepad;

public partial class Calculator4TextUnicodeConvert : UserControl
{
    #region constructor
    public Calculator4TextUnicodeConvert()
    {
        InitializeComponent();

        _txtText.TextChanged += (s, e) => OnTextChanged(ConvertFromType.Text);
        _txtDecUnicode.TextChanged += (s, e) => OnTextChanged(ConvertFromType.Dec);
        _txtHexUnicode.TextChanged += (s, e) => OnTextChanged(ConvertFromType.Hex);
    }
    #endregion

    #region property
    private bool _isUpdating = false;
    #endregion

    #region method
    private void OnTextChanged(ConvertFromType from)
    {
        if (_isUpdating) return;
        _isUpdating = true;
        try
        {
            string text = _txtText?.Text ?? string.Empty;
            string dec = _txtDecUnicode?.Text ?? string.Empty;
            string hex = _txtHexUnicode?.Text ?? string.Empty;

            switch (from)
            {
                case ConvertFromType.Text:
                    dec = string.Join(" ", text.Select(c => ((int)c).ToString()));
                    hex = string.Join(" ", text.Select(c => ((int)c).ToString("X4")));
                    if (_txtDecUnicode != null && _txtDecUnicode.Text != dec) _txtDecUnicode.Text = dec;
                    if (_txtHexUnicode != null && _txtHexUnicode.Text != hex && !_txtHexUnicode.IsFocused) _txtHexUnicode.Text = hex;
                    break;
                case ConvertFromType.Dec:
                    var decChars = dec
                        .Split([' ', ',', ';'], StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => int.TryParse(s, out int code) ? (char)code : '\0')
                        .Where(c => c != '\0')
                        .ToArray();
                    text = new string(decChars);
                    hex = string.Join(" ", decChars.Select(c => ((int)c).ToString("X4")));
                    if (_txtText != null && _txtText.Text != text) _txtText.Text = text;
                    if (_txtHexUnicode != null && _txtHexUnicode.Text != hex && !_txtHexUnicode.IsFocused) _txtHexUnicode.Text = hex;
                    break;
                case ConvertFromType.Hex:
                    var hexChars = hex
                        .Split([' ', ',', ';'], StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => int.TryParse(s, NumberStyles.HexNumber, null, out int code) ? (char)code : '\0')
                        .Where(c => c != '\0')
                        .ToArray();
                    text = new string(hexChars);
                    dec = string.Join(" ", hexChars.Select(c => ((int)c).ToString()));
                    if (_txtText != null && _txtText.Text != text) _txtText.Text = text;
                    if (_txtDecUnicode != null && _txtDecUnicode.Text != dec) _txtDecUnicode.Text = dec;
                    break;
            }
        }
        finally
        {
            _isUpdating = false;
        }
    }
    #endregion

    #region event handler
    #endregion

    #region enum
    enum ConvertFromType { Text, Dec, Hex }
    #endregion
}
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Text;

namespace CalculatorNotepad;

public partial class Calculator4Common : UserControl
{
    #region constructor
    public Calculator4Common()
    {
        InitializeComponent();

        UpdateProcessDisplay();

        // 允许控件聚焦以接收键盘事件
        Focusable = true;
        AttachedToVisualTree += (sender, e) =>
        {
            // 设置焦点到当前控件
            Focus();
        };
        KeyDown += Calculator4Common_KeyDown;
    }
    #endregion

    #region property
    private StringBuilder _processBuilder = new();
    private string _currentInput = "";
    private double? _lastValue = null;
    private string? _lastOperator = null;
    private bool _justCalculated = false;
    #endregion

    #region method
    private void HandleOperator(string tag, string key)
    {
        if (double.TryParse(_currentInput, out var num))
        {
            if (_lastValue is not null && _lastOperator is not null)
            {
                _lastValue = Calculate(_lastValue.Value, num, _lastOperator);
                _processBuilder.Append($"{_currentInput} {key} ");
            }
            else
            {
                _lastValue = num;
                _processBuilder.Append($"{_currentInput} {key} ");
            }
            _currentInput = "";
            _lastOperator = tag;
        }
        else if (_lastValue is not null)
        {
            // 连续按运算符，替换上一个
            if (_processBuilder.Length > 0)
            {
                _processBuilder.Length -= 2;
                _processBuilder.Append($"{key} ");
            }
            _lastOperator = tag;
        }
    }

    private void HandleEqual()
    {
        if (_lastValue is not null && _lastOperator is not null && double.TryParse(_currentInput, out var num))
        {
            var result = Calculate(_lastValue.Value, num, _lastOperator);
            _processBuilder.Append($"{_currentInput} = {result}\n");
            _currentInput = result.ToString();
            _lastValue = null;
            _lastOperator = null;
            _justCalculated = true;
        }
    }

    private void HandleDecimal()
    {
        if (_justCalculated)
        {
            _currentInput = "";
            _justCalculated = false;
        }
        if (string.IsNullOrEmpty(_currentInput))
        {
            // 首字符为小数点，自动补零
            _currentInput = "0.";
        }
        else if (!_currentInput.Contains("."))
        {
            _currentInput += ".";
        }
        // 已有小数点则忽略
    }

    private double Calculate(double left, double right, string op)
    {
        return op switch
        {
            "Add" => left + right,
            "Sub" => left - right,
            "Mul" => left * right,
            "Div" => right != 0 ? left / right : 0,
            _ => right
        };
    }

    private void UpdateProcessDisplay()
    {
        var processTextBlock = this.FindControl<TextBlock>("ProcessTextBlock");
        if (processTextBlock != null)
        {
            processTextBlock.Text = _processBuilder.ToString() + (_currentInput != "" ? _currentInput : "");
            ProcessScrollViewer.ScrollToEnd();
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
                _currentInput = "";
                _lastValue = null;
                _lastOperator = null;
                _processBuilder.Clear();
                _justCalculated = false;
                break;
            case "Delete":
                if (_currentInput.Length > 0)
                    _currentInput = _currentInput[..^1];
                break;
            case "Per":
                if (double.TryParse(_currentInput, out var perVal))
                {
                    perVal /= 100;
                    _currentInput = perVal.ToString();
                }
                break;
            case "Div":
            case "Mul":
            case "Sub":
            case "Add":
                HandleOperator(tag, key);
                break;
            case "Equ":
                HandleEqual();
                break;
            case "Decimal":
                HandleDecimal();
                break;
            case "Number":
                if (_justCalculated)
                {
                    _currentInput = "";
                    _justCalculated = false;
                }
                _currentInput += key;
                break;
        }

        UpdateProcessDisplay();
    }

    private async void Calculator4Common_KeyDown(object? sender, KeyEventArgs e)
    {
        // Ctrl+C 复制结果
        if (e.Key == Key.C && (e.KeyModifiers & KeyModifiers.Control) == KeyModifiers.Control)
        {
            // 复制当前输入或最后一次计算结果
            var textToCopy = _currentInput;
            if (string.IsNullOrWhiteSpace(textToCopy) && _processBuilder.Length > 0)
            {
                // 尝试获取最后一次等号后的结果
                var lines = _processBuilder.ToString().Split('\n', StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length > 0)
                {
                    var lastLine = lines[^1];
                    var eqIdx = lastLine.LastIndexOf('=');
                    if (eqIdx >= 0 && eqIdx + 1 < lastLine.Length)
                        textToCopy = lastLine[(eqIdx + 1)..].Trim();
                }
            }
            if (!string.IsNullOrWhiteSpace(textToCopy))
            {
                var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
                if (clipboard != null)
                    await clipboard.SetTextAsync(textToCopy);
            }
            e.Handled = true;
            return;
        }

        // 数字键
        if (e.Key >= Key.D0 && e.Key <= Key.D9)
        {
            var num = (e.Key - Key.D0).ToString();
            KeyboardButton_Click(new Button { Tag = "Number", Content = num }, null!);
            e.Handled = true;
        }
        else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
        {
            var num = (e.Key - Key.NumPad0).ToString();
            KeyboardButton_Click(new Button { Tag = "Number", Content = num }, null!);
            e.Handled = true;
        }
        // 小数点
        else if (e.Key == Key.OemPeriod || e.Key == Key.Decimal)
        {
            KeyboardButton_Click(new Button { Tag = "Decimal", Content = "." }, null!);
            e.Handled = true;
        }
        // 运算符
        else if (e.Key == Key.Add || e.Key == Key.OemPlus && (e.KeyModifiers & KeyModifiers.Shift) == KeyModifiers.Shift)
        {
            KeyboardButton_Click(new Button { Tag = "Add", Content = "+" }, null!);
            e.Handled = true;
        }
        else if (e.Key == Key.Subtract || e.Key == Key.OemMinus)
        {
            KeyboardButton_Click(new Button { Tag = "Sub", Content = "-" }, null!);
            e.Handled = true;
        }
        else if (e.Key == Key.Multiply)
        {
            KeyboardButton_Click(new Button { Tag = "Mul", Content = "×" }, null!);
            e.Handled = true;
        }
        else if (e.Key == Key.Divide || e.Key == Key.Oem2)
        {
            KeyboardButton_Click(new Button { Tag = "Div", Content = "÷" }, null!);
            e.Handled = true;
        }
        // 回车/等于
        else if (e.Key == Key.Enter)
        {
            KeyboardButton_Click(new Button { Tag = "Equ", Content = "=" }, null!);
            e.Handled = true;
        }
        // 退格
        else if (e.Key == Key.Back)
        {
            KeyboardButton_Click(new Button { Tag = "Delete", Content = "?" }, null!);
            e.Handled = true;
        }
        // Esc 清空
        else if (e.Key == Key.Escape)
        {
            KeyboardButton_Click(new Button { Tag = "Clear", Content = "C" }, null!);
            e.Handled = true;
        }
        // 百分号
        else if (e.Key == Key.Oem5) // 一般为 '%'
        {
            KeyboardButton_Click(new Button { Tag = "Per", Content = "%" }, null!);
            e.Handled = true;
        }
    }
    #endregion
}
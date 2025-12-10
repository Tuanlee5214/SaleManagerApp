using Microsoft.Xaml.Behaviors;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SaleManagerApp.Behaviors
{
    public class NumericFormatBehavior : Behavior<TextBox>
    {
        private bool _isUpdating;

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.PreviewTextInput += OnPreviewTextInput;
            AssociatedObject.TextChanged += OnTextChanged;
            AssociatedObject.PreviewKeyDown += OnPreviewKeyDown;

            DataObject.AddPastingHandler(AssociatedObject, OnPaste);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.PreviewTextInput -= OnPreviewTextInput;
            AssociatedObject.TextChanged -= OnTextChanged;
            AssociatedObject.PreviewKeyDown -= OnPreviewKeyDown;

            DataObject.RemovePastingHandler(AssociatedObject, OnPaste);
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Chỉ cho nhập số
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Cho phép Backspace
            if (e.Key == Key.Back || e.Key == Key.Delete)
                return;
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            // Ngăn paste ký tự không phải số
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                var text = (string)e.DataObject.GetData(typeof(string));
                if (!Regex.IsMatch(text, @"^\d+$"))
                {
                    e.CancelCommand();
                }
            }
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isUpdating) return;
            _isUpdating = true;

            var textBox = AssociatedObject;

            // Lấy raw digits
            string raw = Regex.Replace(textBox.Text, @"[^\d]", "");

            if (raw == "")
            {
                textBox.Text = "";
                _isUpdating = false;
                return;
            }

            if (decimal.TryParse(raw, out decimal value))
            {
                textBox.Text = value.ToString("N0", new CultureInfo("vi-VN"));
                textBox.CaretIndex = textBox.Text.Length;
            }

            _isUpdating = false;
        }
    }
}

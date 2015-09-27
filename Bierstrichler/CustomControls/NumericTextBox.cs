using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Globalization;
using Xceed.Wpf.Toolkit;

namespace Bierstrichler.CustomControls
{
    public class NumericTextBox : WatermarkTextBox
    {
        static NumericTextBox()
        {
            EventManager.RegisterClassHandler(
                typeof(NumericTextBox),
                DataObject.PastingEvent,
                (DataObjectPastingEventHandler)((sender, e) =>
                {
                    if (!IsDataValid(e.DataObject))
                    {
                        DataObject data = new DataObject();
                        data.SetText(String.Empty);
                        e.DataObject = data;
                        e.Handled = false;
                    }
                }));
        }

        private string DecimalSeparator { get { return NumberFormatInfo.CurrentInfo.NumberDecimalSeparator; } }
        private string GroupSeparator { get { return NumberFormatInfo.CurrentInfo.NumberGroupSeparator; } }

        protected override void OnDrop(DragEventArgs e)
        {
            e.Handled = !IsDataValid(e.Data);
            base.OnDrop(e);
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            if (!IsDataValid(e.Data))
            {
                e.Handled = true;
                e.Effects = DragDropEffects.None;
            }

            base.OnDragEnter(e);
        }

        private static Boolean IsDataValid(IDataObject data)
        {
            Boolean isValid = false;
            if (data != null)
            {
                String text = data.GetData(DataFormats.Text) as String;
                if (!String.IsNullOrEmpty(text == null ? null : text.Trim()))
                {
                    Int32 result = -1;
                    if (Int32.TryParse(text, out result))
                    {
                        if (result > 0)
                        {
                            isValid = true;
                        }
                    }
                }
            }

            return isValid;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if ((e.Key < Key.D0 || e.Key > Key.D9) && (e.Key < Key.NumPad0 || e.Key > Key.NumPad9))
                switch (e.Key)
                {
                    case Key.OemComma:
                    case Key.OemPeriod:
                    case Key.OemPlus:
                        if (AllowOnlyNaturalNumbers)
                            e.Handled = true;
                        break;
                    case Key.OemMinus:
                    case Key.Subtract:
                        if (AllowOnlyPositiveNumbers)
                            e.Handled = true;
                        break;
                    case Key.Back:
                    case Key.Add:
                    case Key.Return:
                    case Key.Tab:
                    case Key.Decimal:
                        break;
                    default:
                        e.Handled = true;
                        break;
                }
            //base.OnKeyDown(e);
        }

        //protected override void OnTextChanged(TextChangedEventArgs e)
        //{
        //    try
        //    {
        //        if (!Text[Text.Length - 1].ToString().Equals(DecimalSeparator))
        //        {
        //            textChanged = true;
        //            Value = Convert.ToDecimal(Text, CultureInfo.CurrentCulture);
        //            e.Handled = true;
        //            base.OnTextChanged(e);
        //        }


        //    }
        //    catch { }
        //}

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
                return;
            decimal result = Value;
            decimal.TryParse(Text, NumberStyles.Any, CultureInfo.CurrentCulture, out result);
            Value = result;
            base.OnLostFocus(e);
        }

        public decimal Value
        {
            get { return (decimal)GetValue(ValueProperty); }
            set
            {
                SetValue(ValueProperty, value);
                SetValue(TextProperty, value.ToString(CultureInfo.CurrentCulture));
            }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(decimal), typeof(NumericTextBox), new FrameworkPropertyMetadata((decimal)0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PropChanged, CoerceValueProperty));

        private static object CoerceValueProperty(DependencyObject d, object baseValue)
        {
            if (!(d is NumericTextBox))
                throw new ArgumentException("The Dependency Object ist not a NumericTextBox");
            NumericTextBox thisTB = d as NumericTextBox;
            decimal baseDecimal = (decimal)baseValue;
            if(thisTB.AllowOnlyNaturalNumbers)
                baseDecimal = Math.Round(baseDecimal);
            if (thisTB.AllowOnlyNegativeNumbers && baseDecimal > 0.0m)
                baseDecimal = 0.0m;
            if (thisTB.AllowOnlyPositiveNumbers && baseDecimal < 0.0m)
                baseDecimal = 0.0m;
            return baseDecimal;
        }

        private static void PropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NumericTextBox)
                ((NumericTextBox)d).Text = ((decimal)e.NewValue).ToString(((NumericTextBox)d).FormatString, CultureInfo.CurrentCulture);
        }

        public string FormatString
        {
            get { return (string)GetValue(FormatStringProperty); }
            set { SetValue(FormatStringProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FormatString.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FormatStringProperty =
            DependencyProperty.Register("FormatString", typeof(string), typeof(NumericTextBox), new PropertyMetadata("0"));

        public bool AllowOnlyPositiveNumbers
        {
            get { return (bool)GetValue(AllowOnlyPositiveNumbersProperty); }
            set { SetValue(AllowOnlyPositiveNumbersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowOnlyPositiveNumbers.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllowOnlyPositiveNumbersProperty =
            DependencyProperty.Register("AllowOnlyPositiveNumbers", typeof(bool), typeof(NumericTextBox), new PropertyMetadata(false));



        public bool AllowOnlyNegativeNumbers
        {
            get { return (bool)GetValue(AllowOnlyNegativeNumbersProperty); }
            set { SetValue(AllowOnlyNegativeNumbersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowOnlyNegativeNumbers.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllowOnlyNegativeNumbersProperty =
            DependencyProperty.Register("AllowOnlyNegativeNumbers", typeof(bool), typeof(NumericTextBox), new PropertyMetadata(false));



        public bool AllowOnlyNaturalNumbers
        {
            get { return (bool)GetValue(AllowOnlyNaturalNumbersProperty); }
            set { SetValue(AllowOnlyNaturalNumbersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowOnlyNaturalNumbers.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllowOnlyNaturalNumbersProperty =
            DependencyProperty.Register("AllowOnlyNaturalNumbers", typeof(bool), typeof(NumericTextBox), new PropertyMetadata(false));
    }
}
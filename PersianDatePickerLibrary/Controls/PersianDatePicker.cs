using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using PersianDatePickerLibrary.Helpers;

namespace PersianDatePickerLibrary.Controls
{
    /// <summary>
    /// A custom DatePicker control that supports the Persian (Shamsi) calendar.
    /// </summary>
    public class PersianDatePicker : DatePicker
    {
        /// <summary>
        /// DependencyProperty for binding the Persian date string.
        /// </summary>
        public static readonly DependencyProperty PersianDateProperty =
            DependencyProperty.Register(
                nameof(PersianDate),
                typeof(string),
                typeof(PersianDatePicker),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPersianDateChanged));

        /// <summary>
        /// Gets or sets the Persian date string in the format yyyy/MM/dd.
        /// </summary>
        public string PersianDate
        {
            get => (string)GetValue(PersianDateProperty);
            set => SetValue(PersianDateProperty, value);
        }

        /// <summary>
        /// Handles changes to the PersianDate property.
        /// </summary>
        private static void OnPersianDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PersianDatePicker picker)
            {
                if (DateTime.TryParseExact((string)e.NewValue, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                {
                    picker.SelectedDate = date;
                }
                else
                {
                    var persianDate = PersianDateHelper.ToGregorianDate((string)e.NewValue);
                    if (persianDate != null)
                    {
                        picker.SelectedDate = persianDate;
                    }
                }
            }
        }

        /// <summary>
        /// Static constructor to override the DefaultStyleKey.
        /// </summary>
        static PersianDatePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PersianDatePicker), new FrameworkPropertyMetadata(typeof(PersianDatePicker)));
        }

        /// <summary>
        /// Initializes a new instance of the PersianDatePicker class.
        /// </summary>
        public PersianDatePicker()
        {
            this.Loaded += PersianDatePicker_Loaded;
        }

        /// <summary>
        /// Handles the Loaded event to initialize the PersianDate.
        /// </summary>
        private void PersianDatePicker_Loaded(object sender, RoutedEventArgs e)
        {
            if (SelectedDate != null)
            {
                PersianDate = PersianDateHelper.ToPersianDate(SelectedDate.Value);
            }

            this.SelectedDateChanged += PersianDatePicker_SelectedDateChanged;
        }

        /// <summary>
        /// Updates the PersianDate when SelectedDate changes.
        /// </summary>
        private void PersianDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedDate != null)
            {
                PersianDate = PersianDateHelper.ToPersianDate(SelectedDate.Value);
            }
            else
            {
                PersianDate = string.Empty;
            }
        }

        /// <summary>
        /// Overrides the OnApplyTemplate method to attach event handlers to the internal TextBox.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("PART_TextBox") is DatePickerTextBox textBox)
            {
                textBox.TextChanged -= InternalTextBox_TextChanged; // Prevent multiple subscriptions
                textBox.TextChanged += InternalTextBox_TextChanged;
            }
        }

        /// <summary>
        /// Handles the internal TextBox's TextChanged event.
        /// </summary>
        private void InternalTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is DatePickerTextBox textBox)
            {
                string input = textBox.Text;

                if (DateTime.TryParseExact(input, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var gregorianDate))
                {
                    SelectedDate = gregorianDate;
                    PersianDate = PersianDateHelper.ToPersianDate(gregorianDate);
                    // Remove error indicators if any
                    textBox.ClearValue(BorderBrushProperty);
                    textBox.ClearValue(ToolTipProperty);
                }
                else
                {
                    var persianDate = PersianDateHelper.ToGregorianDate(input);
                    if (persianDate != null)
                    {
                        SelectedDate = persianDate;
                        PersianDate = PersianDateHelper.ToPersianDate(persianDate.Value);
                        // Remove error indicators if any
                        textBox.ClearValue(BorderBrushProperty);
                        textBox.ClearValue(ToolTipProperty);
                    }
                    else
                    {
                        // Invalid input; add error indicators
                        textBox.BorderBrush = System.Windows.Media.Brushes.Red;
                        textBox.ToolTip = "Invalid date format. Please use yyyy/MM/dd.";
                    }
                }
            }
        }
    }
}

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.Storage;
using WinRT.Interop;

namespace BookCipher
{
    public sealed partial class MainWindow : Window
    {
        private string OriginalTextValue { get; set; } = string.Empty;
        private string ProcessedTextValue { get; set; } = string.Empty;
        private string Keyword { get; set; } = string.Empty;
        private string Rows { get; set; } = string.Empty;
        private string Columns { get; set; } = string.Empty;

        public MainWindow()
        {
            this.InitializeComponent();

            OriginalTextBox.DataContext = OriginalTextValue;
            OriginalTextBox.SetBinding(TextBox.TextProperty, new Binding { Source = OriginalTextValue });

            ProcessedTextBox.DataContext = ProcessedTextValue;
            ProcessedTextBox.SetBinding(TextBox.TextProperty, new Binding { Source = ProcessedTextValue });

            KeywordTextBox.DataContext = Keyword;
            KeywordTextBox.SetBinding(TextBox.TextProperty, new Binding { Source = Keyword });

            RowsTextBox.DataContext = Rows;
            RowsTextBox.SetBinding(TextBox.TextProperty, new Binding { Source = Rows });

            ColumnsTextBox.DataContext = Columns;
            ColumnsTextBox.SetBinding(TextBox.TextProperty, new Binding { Source = Columns });
        }

        private void ClearOriginalText_Click(object sender, RoutedEventArgs e)
        {
            OriginalTextBox.ClearValue(TextBox.TextProperty);
        }

        private void ClearProcessedText_Click(object sender, RoutedEventArgs e)
        {
            ProcessedTextBox.ClearValue(TextBox.TextProperty);
        }

        private async void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            var stringFileContent = await readFileAsync();

            if (string.IsNullOrEmpty(stringFileContent))
            {
                return;
            }

            OriginalTextBox.SetValue(TextBox.TextProperty, stringFileContent);
        }

        private void Encrypt_Click(object sender, RoutedEventArgs e)
        {
            ProcessText(text => new VerseCipher(KeywordTextBox.GetValue(TextBox.TextProperty).ToString(), 
                    convToInt(RowsTextBox.GetValue(TextBox.TextProperty).ToString()), 
                    convToInt(ColumnsTextBox.GetValue(TextBox.TextProperty).ToString()))
                .Encrypt(text));
        }

        private void Decrypt_Click(object sender, RoutedEventArgs e)
        {
            ProcessText(text => new VerseCipher(KeywordTextBox.GetValue(TextBox.TextProperty).ToString(),
                    convToInt(RowsTextBox.GetValue(TextBox.TextProperty).ToString()),
                    convToInt(ColumnsTextBox.GetValue(TextBox.TextProperty).ToString()))
                .Decrypt(text));
        }

        private void ProcessText(Func<string, string> processFunction)
        {
            ProcessedTextBox.ClearValue(TextBox.TextProperty);
            var originalText = OriginalTextBox.GetValue(TextBox.TextProperty).ToString();

            var processedText = processFunction(originalText);

            ProcessedTextBox.SetValue(TextBox.TextProperty, string.IsNullOrEmpty(processedText) ? "Не вірно задані параметри шифрування" : processedText);
        }

        private int convToInt(string value) => string.IsNullOrEmpty(value) ? 0 : Convert.ToInt32(value);

        private async void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            await saveFileAsync(ProcessedTextBox.GetValue(TextBox.TextProperty).ToString());
        }

        private async Task saveFileAsync(string fileContent)
        {
            // Створення діалогу для вибору файлу
            var savePicker = new FileSavePicker()
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                FileTypeChoices = { { "Текстовий файл", new List<string>() { ".txt" } } },
                SuggestedFileName = "Нове_ім'я"
            };

            var handledWindow = WindowNative.GetWindowHandle(App.Window);
            InitializeWithWindow.Initialize(savePicker, handledWindow);

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Збереження вмісту файлу
                await FileIO.WriteTextAsync(file, fileContent);
            }
        }

        /// <summary>
        /// Read file from FilePicker and return its content in string
        /// </summary>
        private async Task<string> readFileAsync()
        {
            var openPicker = new FileOpenPicker()
            {
                SuggestedStartLocation = PickerLocationId.Downloads,
                FileTypeFilter = { ".txt" }
            };

            var handledWindow = WindowNative.GetWindowHandle(App.Window);
            InitializeWithWindow.Initialize(openPicker, handledWindow);

            var file = await openPicker.PickSingleFileAsync();

            if (file == null)
            {
                return string.Empty;
            }

            return await FileIO.ReadTextAsync(file);
        }

        private void NumericTextBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            Regex regex = new Regex("[^0-9]+");

            if (regex.IsMatch(sender.Text))
            {
                sender.Text = regex.Replace(sender.Text, string.Empty);
                sender.SelectionStart = sender.Text.Length;
            }
        }
    }
}

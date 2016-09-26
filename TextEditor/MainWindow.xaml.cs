using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace TextEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            fontCombobox.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            fontSizeBox.ItemsSource = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
            fontCombobox.SelectedItem = textEditor.FontFamily;
            fontSizeBox.SelectedItem = textEditor.FontSize;
        }
        

        private async void openBtn_Click(object sender, RoutedEventArgs e)
        {
            await Task.Delay(0);
            try
            {
                
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "RichText Files (*.rtf)|*.rtf|Text Files (*.txt)|*.txt";

                if (ofd.ShowDialog() == true)
                {
                    TextRange doc = new TextRange(textEditor.Document.ContentStart, textEditor.Document.ContentEnd);
                    using (FileStream fs = new FileStream(ofd.FileName, FileMode.Open))
                    {
                        if (System.IO.Path.GetExtension(ofd.FileName).ToLower() == ".rtf")
                            doc.Load(fs, DataFormats.Rtf);

                        if (System.IO.Path.GetExtension(ofd.FileName).ToLower() == ".txt")
                            doc.Load(fs, DataFormats.Text);
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            await Task.Delay(0);
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "RichText Files (*.rtf)|*.rtf|Text Files (*.txt)|*.txt";
                if (sfd.ShowDialog() == true)
                {
                    TextRange doc = new TextRange(textEditor.Document.ContentStart, textEditor.Document.ContentEnd);
                    using (FileStream fs = File.Create(sfd.FileName))
                    {
                        if (System.IO.Path.GetExtension(sfd.FileName).ToLower() == ".rtf")
                            doc.Save(fs, DataFormats.Rtf);

                        if (System.IO.Path.GetExtension(sfd.FileName).ToLower() == ".txt")
                            doc.Save(fs, DataFormats.Text);
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void printBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PrintDialog pd = new PrintDialog();

                if ((pd.ShowDialog() == true))
                {
                    pd.PrintVisual(textEditor as Visual, "Print Visual");
                    pd.PrintDocument((((IDocumentPaginatorSource)textEditor.Document).DocumentPaginator), "Print Document");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (fontCombobox.SelectedItem != null && textEditor!= null)
            {
                textEditor.Selection.ApplyPropertyValue(RichTextBox.FontFamilyProperty, fontCombobox.SelectedItem);
                textEditor.Focus();
            }
        }

        private void ComboBox_TextChanged(object sender, RoutedEventArgs e)
        {
            double size;
            
            if (textEditor != null)
                if (Double.TryParse(fontSizeBox.Text, out size))
                {

                    textEditor.Selection.ApplyPropertyValue(Inline.FontSizeProperty, size);
                    textEditor.Focus();
                }
        }

        private void textEditor_SelectionChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                object temp = textEditor.Selection.GetPropertyValue(Inline.FontWeightProperty);
                btnBold.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontWeights.Bold));
                temp = textEditor.Selection.GetPropertyValue(Inline.FontStyleProperty);
                btnItalic.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontStyles.Italic));
                temp = textEditor.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
                btnUnderline.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextDecorations.Underline));

                temp = textEditor.Selection.GetPropertyValue(Inline.FontFamilyProperty);
                fontCombobox.SelectedItem = temp;
                temp = textEditor.Selection.GetPropertyValue(Inline.FontSizeProperty);
                fontSizeBox.Text = temp.ToString();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
    }
}

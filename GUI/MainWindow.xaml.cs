using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;
using Microsoft.Win32;
using Small_lang;
using Path = System.IO.Path;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    #nullable enable
    public partial class MainWindow : Window
    {
        private Compiler _compiler = new Compiler();
        private StreamWriter? _inputWriter;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Reset()
        {
            _compiler.Clear();
            _inputWriter?.Dispose();
            _inputWriter = null;
            Input.Text = "";
            Exec.Text = "";
            Input.IsEnabled = false;
        }
        private void OpenFileDialog(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() ?? false)
            {
                Source.Text = File.ReadAllText(dialog.FileName);
            }
        }
        
        private void SaveFileDialog(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            if (dialog.ShowDialog() ?? false)
            {
                File.WriteAllText(dialog.FileName, Source.Text);
            }
        }

        private void Compile2PCode(object sender, RoutedEventArgs e)
        {
            this.Reset();
            _compiler.PreProcess(Source.Text);
            PCode.Text = _compiler.Compile();
        }
        
        private Task<string> InnerRunAll(string path)
        {
            return Task.Run(() =>
            {
                using var p = new Process
                {
                    StartInfo = new ProcessStartInfo("node", $"D:/VSWorkspace/Small-lang/PMachine/PMachine.js {path}")
                };
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardInput = true;
                p.Start();
                _inputWriter = p.StandardInput;
                p.WaitForExit();
                File.Delete(path);
                return p.StandardOutput.ReadToEnd();
            });
        }
        
        private async void RunAll(object sender, RoutedEventArgs e)
        {
            var tmp = Path.GetTempFileName();
            await File.WriteAllTextAsync(tmp, PCode.Text);
            Input.IsEnabled = true;
            Exec.Text = await InnerRunAll(tmp);
        }
        
        private void Input_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (Input.Text == string.Empty || Input.Text.Last() == '\n')
            {
                return;
            }
            _inputWriter?.WriteLine(Input.Text.Split('\n', StringSplitOptions.RemoveEmptyEntries).Last());
            // Console.Out.WriteLine(Input.Text.Split('\n').Last());
            //_inputWriter?.WriteLine("20\n30");
        }

        private async void Ttest(object sender, RoutedEventArgs e)
        {
            await Task.Delay(2000);
            Heap.Text = "!!!";
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
    public partial class MainWindow : Window
    {
        private Compiler _compiler = new Compiler();
        public MainWindow()
        {
            InitializeComponent();
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
            _compiler.PreProcess(Source.Text);
            PCode.Text = _compiler.Compile();
        }
        
        private void RunAll(object sender, RoutedEventArgs e)
        {
            var tmp = Path.GetTempFileName();
            File.WriteAllText(tmp, PCode.Text);
            using (var p = new Process
            {
                StartInfo = new ProcessStartInfo("node", $"D:/VSWorkspace/Small-lang/PMachine/PMachine.js {tmp}")
            })
            {
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardInput = true;
                p.Start();
                p.StandardInput.WriteLine("30\n20");
                p.WaitForExit();
                Exec.Text = p.StandardOutput.ReadToEnd();
                File.Delete(tmp);
            }
        }
    }
}
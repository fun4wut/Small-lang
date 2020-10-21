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
using System.Windows.Input;
using System.Windows.Threading;
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
        private bool _firstStep = true;
        private Process? _cli = null;
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void Reset()
        {
            _compiler.Clear();
            _inputWriter?.Close();
            _inputWriter?.Dispose();
            _cli?.Dispose();
            _inputWriter = null;
            _firstStep = true;
            Input.Text = "";
            PCode.Text = "";
            Exec.Text = "";
            Error.Text = "";
            Stack.Text = "";
            Input.IsReadOnly = true;
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
            try
            {
                _compiler.PreProcess(Source.Text);
                PCode.Text = _compiler.Compile();
            }
            catch (Exception exception)
            {
                Error.Text = exception.Message;
            }
        }
        
        private async Task RunAsync(
            string path,
            DataReceivedEventHandler stdOutCallback,
            bool showHeap = false
        )
        {
            _cli = new Process
            {
                // StartInfo = new ProcessStartInfo(
                //     "node", 
                //     $"D:/VSWorkspace/Small-lang/PMachine/PMachine.js {path} {(showHeap ? "-s 1" : "")}")
                StartInfo = new ProcessStartInfo(
                    @"D:\VSWorkspace\Small-lang\PMachine\bin\Debug\netcoreapp3.1\PMachine.exe", 
                    $" {path} {(showHeap ? "-v" : "")}")
            };
            _cli.StartInfo.UseShellExecute = false;
            _cli.StartInfo.RedirectStandardOutput = true;
            _cli.StartInfo.RedirectStandardInput = true;
            _cli.StartInfo.RedirectStandardError = true;
            _cli.StartInfo.CreateNoWindow = true;
            _cli.OutputDataReceived += stdOutCallback;
            _cli.ErrorDataReceived += (_, e) => Dispatcher.Invoke(() => Error.Text += e.Data);
            await Task.Run(() =>
            {
                //p.ErrorDataReceived += errorCallback;
                _cli.Start();
                _cli.BeginOutputReadLine();
                _cli.BeginErrorReadLine();
                _inputWriter = _cli.StandardInput;
                _cli.WaitForExit();
                File.Delete(path);
            });
            _cli.Dispose();
        }
        
        private async void RunAll(object sender, RoutedEventArgs e)
        {
            var tmp = Path.GetTempFileName();
            await File.WriteAllTextAsync(tmp, PCode.Text);
            Input.IsReadOnly = false;
            
            await RunAsync(
                tmp,
                (_, e) =>
                {
                    Dispatcher.Invoke(() => Exec.Text += e.Data + "\n", DispatcherPriority.Render);
                }
            );
        }
        
        private void Input_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (Input.Text == string.Empty || Input.Text.Last() != '\n')
            {
                return;
            }
            _inputWriter?.WriteLine(Input.Text.Split('\n', StringSplitOptions.RemoveEmptyEntries).Last());
            // Console.Out.WriteLine(Input.Text.Split('\n').Last());
            //_inputWriter?.WriteLine("20\n30");
        }

        private async void RunByStep(object sender, RoutedEventArgs e)
        {
            if (_firstStep)
            {
                var tmp = Path.GetTempFileName();
                await File.WriteAllTextAsync(tmp, PCode.Text);
                Input.IsReadOnly = false;
                var buffer = new StringBuilder();
                var stepOutput = "";
                var curIns = "";
                var pc = 0;
                _firstStep = false;
                await RunAsync(
                    tmp,
                    (_, e) =>
                    {
                        // Console.Out.WriteLine(e.Data);
                        if (e.Data?.StartsWith("**") ?? false)
                        {
                            var output = stepOutput;
                            var ins = curIns;
                            Dispatcher.Invoke(() =>
                            {
                                Stack.Text = buffer.ToString();
                                Exec.Text += output;
                                PCode.Focus();
                                //Console.Out.WriteLine(pc);
                                if (pc == -99) return;
                                var idx = PCode.GetCharacterIndexFromLineIndex(pc);
                                PCode.Select(idx, ins.Length);
                            }, DispatcherPriority.Render);
                            buffer.Clear();
                            stepOutput = "";
                            curIns = "";
                        }
                        else if (e.Data?.StartsWith("print") ?? false)
                        {
                            stepOutput = e.Data + "\n";
                        }
                        else if (e.Data?.StartsWith("Press") ?? false)
                        {
                            // do nothing
                        }
                        else
                        {
                            if (e.Data?.StartsWith("-->") ?? false)
                            {
                                curIns = e.Data.Substring(9); // catch ins
                            }

                            if (e.Data?.StartsWith("PC =") ?? false)
                            {
                                Console.Out.WriteLine(e.Data.Substring(5));
                                pc = int.Parse(e.Data.Substring(5)); // catch PC
                            }
                            buffer.AppendLine(e.Data);
                        }
                    },
                    true
                );
            }
            await (_inputWriter?.WriteLineAsync("c") ?? Task.FromResult(1));
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            _cli?.Dispose();
            Application.Current.Shutdown();
        }

        private void CommandAlwaysTrue(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_OnReset(object sender, ExecutedRoutedEventArgs e)
        {
            this.Reset();
            Source.Text = "";
        }
    }

}
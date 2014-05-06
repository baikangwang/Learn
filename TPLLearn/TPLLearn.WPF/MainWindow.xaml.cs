using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TPLLearn.WPF
{
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Threading;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CancellationTokenSource tokenSource;//=new CancellationTokenSource();
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            tokenSource=new CancellationTokenSource();
            
            txtblk1.Text = string.Empty;
            lbl1.Content = "Milliseconds: ";

            Stopwatch watcher = Stopwatch.StartNew();
            List<Task> tasks = new List<Task>();

            var ui = TaskScheduler.FromCurrentSynchronizationContext();

            for (int i = 2; i < 20; i++)
            {
                int j = i;
                var compute = Task.Factory.StartNew(() => SumRootN(j), tokenSource.Token);
                tasks.Add(compute);

                compute.ContinueWith(
                    new Action<Task<double>>(
                        task =>
                            { txtblk1.Text += string.Format("root {0} : {1}{2}", j, task.Result, Environment.NewLine); }),
                            CancellationToken.None,
                            TaskContinuationOptions.OnlyOnRanToCompletion, 
                    ui);

                compute.ContinueWith(
                    task =>
                        {
                            {
                                txtblk1.Text += string.Format("root {0} : {1}{2}", j, "Cancelled", Environment.NewLine);
                            }
                        },
                    CancellationToken.None,
                    TaskContinuationOptions.OnlyOnCanceled,
                    ui);
            }

            Task.Factory.ContinueWhenAll(
                tasks.ToArray(),
                ts => { lbl1.Content += watcher.Elapsed.ToString(); },
                CancellationToken.None,
                TaskContinuationOptions.None,
                ui);
        }

        public double SumRootN(int root)
        {
            double result = 0;
            for (int i = 1; i < 10000000; i++)
            {
                tokenSource.Token.ThrowIfCancellationRequested();
                result += Math.Exp(Math.Log(i) / root);
            }

            return result;
        }

        private void bt2_Click(object sender, RoutedEventArgs e)
        {
            tokenSource.Cancel();
        }
    }
}

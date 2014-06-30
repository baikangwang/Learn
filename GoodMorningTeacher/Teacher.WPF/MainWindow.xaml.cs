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

namespace Teacher.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : NavigationWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.Source = new Uri("http://www.teacher.com.cn");
            this.Navigating += (sender, args) => MessageBox.Show(string.Format("{0} -> Content is {1}","Navigating",args.Content==null?"null":args.Content.GetType().ToString()));
            this.Navigated += (sender, args) => MessageBox.Show(string.Format("{0} -> Content is {1}", "Navigated", args.Content == null ? "null" : args.Content.GetType().ToString()));
            this.NavigationProgress += (sender, args) => MessageBox.Show(string.Format("{0} -> Navigator is {1}, total {2}, read {3}", "NavigationProgress", args.Navigator == null ? "null" : args.Navigator.ToString(),args.MaxBytes,args.BytesRead));
            this.NavigationFailed += (sender, args) => MessageBox.Show(string.Format("{0} -> Exceptioin is {1}", "NavigationFailed", args.Exception == null ? "null" : args.Exception.ToString()));
            this.NavigationStopped += (sender, args) => MessageBox.Show(string.Format("{0} -> Content is {1}", "NavigationStopped", args.Content == null ? "null" : args.Content.GetType().ToString()));
            this.LoadCompleted += (sender, args) => MessageBox.Show(string.Format("{0} -> Content is {1}", "LoadCompleted", args.Content == null ? "null" : args.Content.GetType().ToString()));
            this.FragmentNavigation += (sender, args) => MessageBox.Show(string.Format("{0} -> Fragment is {1}", "FragmentNavigation", string.IsNullOrEmpty( args.Fragment) ? "null" : args.Fragment));
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace BankSystem.View
{
    /// <summary>
    /// Логика взаимодействия для MyWindow.xaml
    /// </summary>
    public partial class MyWindow : Window 
    {
        public MyWindow(string text, double showtime)
        {
            InitializeComponent();
            Window_Loaded(showtime);
            System.Media.SystemSounds.Asterisk.Play();
            message.Text = text;
            
        }


        private void Window_Loaded(double showtime)
        {
            System.Timers.Timer t = new System.Timers.Timer();
            t.Interval = showtime;
            t.Elapsed += new ElapsedEventHandler(t_Elapsed);
            t.Start();
        }

        void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.Close();
            }), null);
        }
    }
}

using Chess.controller;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Chess.view
{
    /// <summary>
    /// StartWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StartWindow : Window
    {
        

        public StartWindow()
        {
            InitializeComponent();
        }

        private void AiGameBtn_Clicked(object sender, RoutedEventArgs e)
        {
            var aiGame = new AiGame();
            aiGame.Show();
            Close();
        }

        private void HumanGameBtn_Clicked(object sender, RoutedEventArgs e)
        {
            var window = new GameHall();
            window.Show();
            Close();
        }
    }
}

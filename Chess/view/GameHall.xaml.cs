using Chess.controller;
using Chess.network;
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
    /// GameHall.xaml 的交互逻辑
    /// </summary>
    public partial class GameHall : Window
    {

        public GameHall()
        {
            InitializeComponent();
            this.Resources["vm"].CastTo<GameHallVM>().Window = this;
        }

        private  void Exit(object sender, EventArgs e)
        {
            Task.Run(async () =>
                await this.Resources["vm"].CastTo<GameHallVM>().OnHallClosed()
            ).Wait();
        }

        //private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
          //  this.Resources["vm"].CastTo<GameHallVM>().SelectedRoom = roomGridView.SelectedItem;
        //}
    }
}

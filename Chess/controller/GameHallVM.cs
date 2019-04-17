using Chess.network;
using Chess.view;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Chess.controller
{
    class GameHallVM : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private List<Client> clients;

        private List<Room> rooms;

        private volatile bool exit = false;

        private Room selectedRoom = null;

        private bool startGame = false;

        private LocalSocket socket = new LocalSocket();

        private Task updateTask;

        public GameHall Window { set; get; }

        public GameHallVM()
        {
            socket.ReceivedOk += OnReceiveOk;
            socket.AnotherJoinRoom += OnAnotherJoinedRoom;
            TimingUpdate();
        }

        public async Task Exit()
        {
            exit = true;
            await updateTask;
            if (!startGame)
            {
                await socket.Exit();
            }
            else
            {
                socket.ReceivedOk -= OnReceiveOk;
                socket.AnotherJoinRoom -= OnAnotherJoinedRoom;
            }
        }

        public Task OnHallClosed()
        {
            return Task.Run(async () =>
            {
                Console.WriteLine("开始释放资源");
                await Exit();
                Console.WriteLine("成功释放资源");
            });
        }

        private void TimingUpdate()
        {
            updateTask = Task.Run(() =>
            {
                while (!exit)
                {
                    Thread.Sleep(1000);
                    OnlineClients = (from pair in socket.OnlineClients
                                     select pair.Value).ToList();
                    OnlineRooms = (from pair in socket.OnlineRooms
                                   select pair.Value).ToList();
                }
            });
        }

        public Room SelectedRoom
        {
            set
            {
                selectedRoom = value;
                OnPropertyChanged("SelectedRoom");
            }
            get
            {
                return selectedRoom;
            }

        }

        public List<Client> OnlineClients
        {
            private set
            {
                clients = value;
                OnPropertyChanged("OnlineClients");
            }
            get
            {
                return clients;
            }
        }

        public List<Room> OnlineRooms
        {
            private set
            {
                rooms = value;
                OnPropertyChanged("OnlineRooms");
            }
            get
            {
                return rooms;
            }
        }

        public ICommand BuildRoom
        {
            get
            {
                return new DelegateCommand<object>((p) =>
                {
                    socket.BuildRoom();
                });
            }
        }

        public ICommand JoinRoom
        {
            get
            {
                return new DelegateCommand<object>((p) =>
                {
                    if (SelectedRoom != null)
                    {
                        Console.WriteLine("发送加入请求");
                        if (socket.IsWaiterPlayer)
                        {
                            MessageBox.Show("正在等待其他人加入房间");
                        }
                        else
                        {
                            socket.JoinRoom(selectedRoom.Name);
                        }
                    }
                });
            }
        }

        public void OnReceiveOk() => Window.Dispatcher.Invoke(() =>
        {
            startGame = true;
            new HumanGame(socket, ChessFlag.BLACK).Show();
            Window.Close();
            Console.WriteLine("接收到OK");
        });

        public void OnAnotherJoinedRoom() => Window.Dispatcher.Invoke(() =>
        {
               startGame = true;
               new HumanGame(socket, ChessFlag.RED).Show();
               Window.Close();
               Console.WriteLine("接受加入房间");
       });



        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

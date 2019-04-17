using Chess.chess;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chess.network
{
    public class Client
    {
        public string Name { set; get; }
        public IPEndPoint IPEnd { set; get; }
        public int Time { set; get; }
    }

    public class Room
    {
        public string Name { set; get; }
        public IPEndPoint IPEnd { set; get; }
        public int Time { set; get; }
    }

    struct UdpState
    {
        public UdpClient u;
        public IPEndPoint e;
    }

    class LocalSocket
    {

        private static readonly int port = 6001;

        private static int counter = 0;

        public event Action<Client> ClientJoined;

        public event Action ReceivedOk;

        public event Action<Move> ReceivedMove;

        public event Action AnotherJoinRoom;

        private static IPEndPoint broadIPEnd = new IPEndPoint(IPAddress.Broadcast, port);

        public ConcurrentDictionary<string, Client> OnlineClients { private set; get; } = new ConcurrentDictionary<string, Client>();

        public ConcurrentDictionary<string, Room> OnlineRooms { private set; get; } = new ConcurrentDictionary<string, Room>();

        private ConcurrentQueue<Message> queueMsgs = new ConcurrentQueue<Message>();

        private UdpClient udpClient;

        private volatile bool exit = false;

        private volatile bool waiterPlayer = false;

        public bool IsWaiterPlayer => waiterPlayer;

        private Task listenTask;

        private Task checkTask;

        private volatile IPEndPoint sessionClient;

        private Dictionary<Message.MessageType, Action<Message, IPEndPoint>> processActions;

        public LocalSocket()
        {
            Console.WriteLine($"初始化LocalSocket! counter={counter++}");
            udpClient = new UdpClient(port);
            udpClient.Client.Blocking = false;
            processActions = new Dictionary<Message.MessageType, Action<Message, IPEndPoint>>
            {
                {Message.MessageType.CLIENT_DETECT, ProcessClientDetect },
                {Message.MessageType.CLIENT_INFO, ProcessClientInfo },
                {Message.MessageType.ROOM_DETECT, ProcessRoomDetect },
                {Message.MessageType.ROOM_INFO, ProcessRoomInfo },
                {Message.MessageType.JOIN_ROOM, ProcessJoinRoom },
                {Message.MessageType.OK, ProcessOK },
                {Message.MessageType.MOVE_INFO, ProcessMove},
            };
            StartListen();
            ChectHeart();
        }

        public void BuildRoom()
        {
            waiterPlayer = true;
        }

        public bool JoinRoom(string name)
        {
            if (OnlineRooms.TryGetValue(name, out var room))
            {
                Console.WriteLine("发送JoinRoom");
                sessionClient = room.IPEnd;
                SendMsg(Message.JOIN_ROOM, sessionClient);
            }
            return false;
        }

        public void SendMove(Move move)
        {
            if (sessionClient != null)
            {
                Console.WriteLine("Send Move");
                var msg = new Message(Message.MessageType.MOVE_INFO, move);
                SendMsg(msg, sessionClient);
            }

        }

        public void SendOk()
        {
            if (sessionClient != null)
            {
                SendMsg(Message.OK, sessionClient);
            }
        }

        public async Task Exit()
        {
            exit = true;
            await listenTask;
            await checkTask;
            udpClient.Close();
            Console.WriteLine("udp Close!");
        }

        void StartListen()
        {
            listenTask = Task.Run(() =>
            {
                while (!exit)
                {
                    var remoteEP = new IPEndPoint(IPAddress.Any, port);
                    while (!exit && udpClient.Available == 0) ;
                    if (!exit)
                    {
                        var data = udpClient.Receive(ref remoteEP);
                        var msg = Message.Deserialize(data);
                        ProcessMsg(msg, remoteEP);
                    }
                }
            });
        }

        void ChectHeart()
        {
            checkTask = Task.Run(async () =>
            {
                while (!exit)
                {
                    foreach (var client in OnlineClients.Values)
                    {
                        client.Time++;
                    }
                    foreach (var room in OnlineRooms.Values)
                    {
                        room.Time++;
                    }
                    DetectClients();
                    DetectRooms();
                    await Task.Delay(1000);
                    var rmClientKeys = (from pair in OnlineClients where pair.Value.Time > 1 select pair.Key).ToList();
                    rmClientKeys.Select(key => OnlineClients.TryRemove(key, out var v));
                    var rmRoomKeys = (from pair in OnlineRooms where pair.Value.Time > 1 select pair.Key).ToList();
                    rmRoomKeys.Select(key => OnlineRooms.TryRemove(key, out var v));
                }
            });
        }


        void ProcessMsg(Message msg, IPEndPoint iPEnd)
        {
            processActions[msg.Type](msg, iPEnd);
        }

        void ProcessOK(Message msg, IPEndPoint iPEnd)
        {
            if (iPEnd != null && iPEnd.Equals(sessionClient))
            {
                ReceivedOk?.Invoke();
            }
        }

        void ProcessMove(Message msg, IPEndPoint iPEnd)
        {
            if (iPEnd != null && iPEnd.Equals(sessionClient))
            {
                ReceivedMove?.Invoke((Move)msg.Data);
            }
        }

        void ProcessJoinRoom(Message msg, IPEndPoint iPEnd)
        {
            if (waiterPlayer)
            {
                Console.WriteLine("收到JoinRoom");
                sessionClient = iPEnd;
                SendMsg(Message.OK, iPEnd);
                AnotherJoinRoom?.Invoke();
                waiterPlayer = false;
            }
        }

        void ProcessClientDetect(Message msg, IPEndPoint iPEnd)
        {
            string name = Environment.UserName;
            var reply = new Message(Message.MessageType.CLIENT_INFO, name);
            SendMsg(reply, iPEnd);
        }

        void ProcessClientInfo(Message msg, IPEndPoint iPEnd)
        {
            string name = (string)msg.Data;
            if (OnlineClients.ContainsKey(name))
            {
                OnlineClients[name].Time = 0;
            }
            else
            {
                Client client = new Client { Name = name, IPEnd = iPEnd, Time = 0 };
                OnlineClients[name] = client;
            }
        }

        void ProcessRoomDetect(Message msg, IPEndPoint iPEnd)
        {
            if (waiterPlayer)
            {
                var reply = new Message(Message.MessageType.ROOM_INFO, Environment.UserName);
                SendMsg(reply, iPEnd);
            }
        }

        void ProcessRoomInfo(Message msg, IPEndPoint iPEnd)
        {
            var name = (string)msg.Data;
            if (OnlineRooms.ContainsKey(name))
            {
                OnlineRooms[name].Time = 0;
            }
            else
            {
                var room = new Room() { Name = name, IPEnd = iPEnd, Time = 0 };
                OnlineRooms[name] = room;
            }
        }

        void SendMsg(Message msg, IPEndPoint iPEnd)
        {
            var sendBuf = msg.Serialize();
            udpClient.SendAsync(sendBuf, sendBuf.Length, iPEnd);
        }

        void DetectClients()
        {
            SendMsg(Message.CLIENT_DETECT_MSG, broadIPEnd);
        }

        void DetectRooms()
        {
            SendMsg(Message.ROOM_DETECT_MSG, broadIPEnd);
        }
    }

    [Serializable]
    class Message
    {

        public static readonly Message CLIENT_DETECT_MSG = new Message(MessageType.CLIENT_DETECT, null);
        public static readonly Message ROOM_DETECT_MSG = new Message(MessageType.ROOM_DETECT, null);
        public static readonly Message OK = new Message(MessageType.OK, null);
        public static readonly Message JOIN_ROOM = new Message(MessageType.JOIN_ROOM, null);


        public enum MessageType
        {
            CLIENT_DETECT,
            CLIENT_INFO,
            ROOM_DETECT,
            ROOM_INFO,
            MOVE_INFO,
            OK,
            JOIN_ROOM,
        }

        public MessageType Type { private set; get; }

        public object Data { private set; get; }

        public Message(MessageType t, object data)
        {
            Type = t;
            this.Data = data;
        }

        public byte[] Serialize()
        {
            var mem = new MemoryStream();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(mem, this);
            return mem.GetBuffer();
        }

        public static Message Deserialize(byte[] bytes)
        {
            var mem = new MemoryStream(bytes);
            var binaryFormatter = new BinaryFormatter();
            var obj = (Message)binaryFormatter.Deserialize(mem);
            return obj;
        }
    }


}

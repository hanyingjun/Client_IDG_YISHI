﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace IDG
{
    public interface IGameManager
    {
        void Init(FSClient client);
        int InitLayer { get; }
    }

    /// <summary>
    /// 【帧同步客户端】负责与【帧同步服务器】连接
    /// </summary>
    public class FSClient
    {
        /// <summary>
        /// 帧同步时间间隔
        /// </summary>
        public readonly static Fixed deltaTime = new Fixed(0.1f);

        /// <summary>
        /// 服务器连接
        /// </summary>
        public Connection ServerCon
        {
            get
            {
                lock (_serverCon)
                {
                    return _serverCon;
                }
            }
        }

        /// <summary>
        /// 消息队列
        /// </summary>
        public Queue<ProtocolBase> MessageList
        {
            get
            {
                lock (_messageList)
                {
                    return _messageList;
                }
            }
        }
        private Connection _serverCon;

        public InputCenter inputCenter;
        public NetObjectManager objectManager;
        public CoroutineManager coroutine;
        public NetData localPlayer;
        public ShapPhysics physics;
        public object unityClient;
        public IDG.Random random;
        public List<IGameManager> gameManagers;
        public T GetManager<T>() where T : class, IGameManager
        {
            foreach (var manager in gameManagers)
            {
                if (manager is T)
                {
                    return manager as T;
                }
            }
            return null;
        }
        protected int maxUserCount;
        protected IGameManager[] managers;
        private Queue<ProtocolBase> _messageList = new Queue<ProtocolBase>();
        /// <summary>
        /// 连接服务器函数
        /// </summary>
        /// <param name="serverIP">服务器IP地址</param>
        /// <param name="serverPort">服务器端口</param>
        /// <param name="maxUserCount">最大玩家数</param>
        public void Connect(string serverIP, int serverPort, int maxUserCount, params IGameManager[] managers)
        {
            _serverCon = new Connection();
            ServerCon.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ServerCon.socket.NoDelay = true;
            this.maxUserCount = maxUserCount;
            this.managers = managers;
            SkillManager.Init();
            AiManager.Init();
            WeaponManager.Init();
            ServerCon.socket.BeginConnect(serverIP, serverPort, ConnectCallback, ServerCon);
        }

        protected void ConnectCallback(IAsyncResult ar)
        {
            ServerCon.socket.BeginReceive(ServerCon.readBuff, 0, ServerCon.BuffRemain, SocketFlags.None, ReceiveCallBack, ServerCon);
            inputCenter = new InputCenter();
            inputCenter.Init(this, maxUserCount);
            objectManager = new NetObjectManager(this);
            physics = new ShapPhysics();
            physics.Init();
            random = new IDG.Random(20190220);
            gameManagers = new List<IGameManager>();
            coroutine = new CoroutineManager();
            gameManagers.AddRange(managers);
            gameManagers.Add(new TeamManager());
            gameManagers.Sort((a, b) => { if (a.InitLayer > b.InitLayer) { return 1; } else { return -1; } });
        }

        /// <summary>
        /// 数据接受回调函数
        /// </summary>
        protected void ReceiveCallBack(IAsyncResult ar)
        {
            Connection con = (Connection)ar.AsyncState;
            // 获取收到的字节数
            int read = con.socket.EndReceive(ar);
            if (read > 0)
            {
                con.length += read;
                ProcessData(con);
                con.socket.BeginReceive(con.readBuff, con.length, con.BuffRemain, SocketFlags.None, ReceiveCallBack, con);
            }
            else
            {
                // 远程主机已断开，断开socket 断开socket
                _isConnect = false;
                m_nPing = 999;
                con.socket.Close();
                Debug.LogError("远程主机断开连接");
            }
        }

        /// <summary>
        /// 解析字节数据
        /// </summary>
        /// <param name="connection">要解析数据的连接</param>
        private void ProcessData(Connection connection)
        {
            if (connection.length < sizeof(Int32))
            {
                Debug.Log("获取不到信息大小重新接包解析：" + connection.length.ToString());
                return;
            }
            Array.Copy(connection.readBuff, connection.lenBytes, sizeof(Int32));
            connection.msgLength = BitConverter.ToInt32(connection.lenBytes, 0);

            if (connection.length < connection.msgLength + sizeof(Int32))
            {
                Debug.Log("信息大小不匹配重新接包解析：" + connection.length + ":" + (connection.msgLength + 4).ToString());
                return;
            }
            //Debug.LogWarning("接收信息大小：" + connection.msgLength.ToString());

            ProtocolBase message = new ByteProtocol();
            message.InitMessage(connection.ReceiveBytes);
            MessageList.Enqueue(message);
            //    Debug.Log("ProcessDataOver");
            //Send(connection, str);
            int count = connection.length - connection.msgLength - sizeof(Int32);
            Array.Copy(connection.readBuff, sizeof(Int32) + connection.msgLength, connection.readBuff, 0, count);
            connection.length = count;
            if (connection.length > 0)
            {
                ProcessData(connection);
            }
        }

        /// <summary>
        /// 发送字节
        /// </summary>
        /// <param name="bytes">发送内容</param>
        public void Send(byte[] bytes)
        {
            if (this._isConnect)
            {
                byte[] length = BitConverter.GetBytes(bytes.Length);
                byte[] send = new byte[4 + bytes.Length];
                Array.Copy(length, 0, send, 0, 4);
                Array.Copy(bytes, 0, send, 4, bytes.Length);
                Debug.Log("send: " + send.Length);
                try
                {
                    ServerCon.socket.BeginSend(send, 0, send.Length, SocketFlags.None, null, null);
                }
                catch (Exception e)
                {
                    this._isConnect = false;
                    UnityEngine.Debug.LogError(e);
                    ServerCon.socket.Shutdown(SocketShutdown.Both);
                    ServerCon.socket.Close();
                }
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            if (inputCenter != null)
                inputCenter.Stop();
        }
        /// <summary>
        /// 解析消息并进行消息分发
        /// </summary>
        /// <param name="protocol">要解析的消息</param>
        public void ParseMessage(ProtocolBase protocol, int deep = 0)
        {
            var t = (MessageType)protocol.getByte();
            switch (t)
            {
                case MessageType.Init:
                    _isConnect = true;
                    _time = UnityEngine.Time.realtimeSinceStartup;
                    ServerCon.clientId = protocol.getByte();
                    Debug.Log("clientID:" + ServerCon.clientId);
                    break;
                case MessageType.Frame:
                    inputCenter.ReceiveStep(protocol);
                    break;
                case MessageType.RandomSeed:
                    random = new IDG.Random((ushort)protocol.getInt32());
                    foreach (var m in gameManagers)
                    {
                        m.Init(this);
                    }
                    break;
                case MessageType.Ping:
                    TimeSpan ts = DateTime.Now - m_dtLastPingTime;
                    m_nPing = (int)(ts.TotalMilliseconds);
                    m_dtLastPingTime = DateTime.Now;
                    m_nPingMsgNum--;
                    break;
                case MessageType.BattleEnd:
                    break;
                case MessageType.end:
                    break;
                default:
                    Debug.LogError("消息解析错误 未解析类型" + t);
                    return;
            }

            if (t != MessageType.end && deep < 5)
            {
                ParseMessage(protocol, deep + 1);
            }
            else
            {
            }

            if (protocol.Length > 0)
            {
                Debug.LogError("剩余未解析" + protocol.Length);
            }
        }

        private void SendPingMsg()
        {
            m_dtLastPingTime = DateTime.Now;
            ProtocolBase protocol = new ByteProtocol();
            protocol.push((byte)MessageType.Ping);
            protocol.push((byte)ServerCon.clientId);
            this.Send(protocol.GetByteStream());
        }

        protected DateTime m_dtLastPingTime = DateTime.Now;
        private int m_nPing = 999;
        public int Ping
        {
            get
            {
                return m_nPing;
            }
        }

        private int m_nPingMsgNum = 0;
        private bool _isConnect = false;
        private float _time = 0.0f;
        public void OnUpdate()
        {
            if (_isConnect)
            {
                if (UnityEngine.Time.realtimeSinceStartup - _time > 5)
                {
                    _time = UnityEngine.Time.realtimeSinceStartup;
                    ++m_nPingMsgNum;
                    SendPingMsg();
                    if (m_nPingMsgNum >= 3)
                    {
                        // 长时间未响应，断开服务器
                    }
                }
            }
        }
    }

    public enum MessageType : byte
    {
        Init = 11,
        Frame = 12,
        ClientReady = 13,
        RandomSeed = 14,
        BattleEnd = 15,
        Ping = 16,
        end = 200,
    }
}
using DEngine.Common;
using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LOLClient
{
    public class Worker
    {
        private Thread workerThread;

        private volatile int _workIndex;
        private volatile int _frameCount;

        public bool IsAlive { get { return workerThread.IsAlive; } }

        public Worker(int index)
        {
            _workIndex = index;
            workerThread = new Thread(DoWork);
        }

        public void Start()
        {
            workerThread.Start();
        }

        public void Stop()
        {
            _frameCount = -1;
            workerThread.Join();
        }

        void DoWork()
        {
            PhotonController controller = new PhotonController();
            controller.Connected += Controller_Connected;
            controller.Disconnected += Controller_Disconnected;
            controller.ResponseReceived += Controller_ResponseReceived;

            controller.Connect("192.168.1.10:4530", "HeroWorld");
            //controller.Connect("ngocvaio:4530", "HeroWorld");

            while (_frameCount >= 0)
            {
                Thread.Sleep(50);
                controller.OnUpdate();
            }

            controller.Disconnect();
        }

        void Controller_Connected(object sender, EventArgs e)
        {
            PhotonController controller = (PhotonController)sender;
            Console.WriteLine("Controller_Connected on thread {0}", _workIndex);

            controller.SendOperation((byte)OperationCode.ZonesList);
        }

        void Controller_Disconnected(object sender, PhotonEventArgs e)
        {
            PhotonController controller = (PhotonController)sender;
            Console.WriteLine("Controller_Disconnected on thread {0}", _workIndex);
        }

        void Controller_ResponseReceived(object sender, PhotonEventArgs e)
        {
            PhotonController controller = (PhotonController)sender;

            OperationResponse response = e.Response;
            if (response == null || response.ReturnCode != (short)ErrorCode.Success)
                return;

            switch ((OperationCode)response.OperationCode)
            {
                case OperationCode.SignIn:
                    _frameCount = 10;
                    controller.SendOperation((byte)OperationCode.ZonesList);
                    break;

                case OperationCode.ZonesList:
                    {
                        if (_frameCount < 10)
                        {
                            Dictionary<string, string> zoneList = (Dictionary<string, string>)response[(byte)ParameterCode.ZoneList];
                            int zoneIdx = _workIndex % zoneList.Count;
                            Guid zoneId = new Guid(zoneList.ElementAt(zoneIdx).Key);

                            Console.WriteLine("SendSignIn on thread {0}", _workIndex);
                            SendSignIn(controller, zoneId, string.Format("Bot{0:D3}", _workIndex + 1), "123");
                        }
                        else
                        {
                            Console.WriteLine("SendZonesList on thread {0}", _workIndex);
                            controller.SendOperation((byte)OperationCode.ZonesList);
                            _frameCount++;
                        }
                    }
                    break;
            }
        }

        private void SendSignIn(PhotonController controller, Guid zoneId, string userName, string password)
        {
            OperationRequest request = new OperationRequest() { OperationCode = (byte)OperationCode.SignIn };
            request.Parameters = new Dictionary<byte, object>();
            request.Parameters[(byte)ParameterCode.ZoneId] = zoneId.ToByteArray();
            request.Parameters[(byte)ParameterCode.UserName] = userName;
            request.Parameters[(byte)ParameterCode.Password] = password;
            controller.SendOperation(request);
        }
    }
}

using ExitGames.Concurrency.Fibers;
using ExitGames.Logging;
using ExitGames.Logging.Log4Net;
using log4net;
using log4net.Config;
using Photon.SocketServer;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using LogManager = ExitGames.Logging.LogManager;

namespace DEngine.PhotonFX.Common
{
    public abstract class PhotonApplication : ApplicationBase
    {
        #region Fields

        protected static ILogger Log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Properties

        protected virtual string MasterIP { get { return "127.0.0.1"; } }
        protected virtual int MasterPort { get { return 4520; } }
        protected virtual int ZoneMaxCCU { get { return 500; } }

        protected PoolFiber ExcutionFiber { get; private set; }
        protected IPhotonHandler DefaultHandler { get; private set; }
        protected Dictionary<byte, IPhotonHandler> RequestHandlers { get; private set; }
        protected Dictionary<byte, IPhotonHandler> ResponseHandlers { get; private set; }
        protected Dictionary<byte, IPhotonHandler> EventHandlers { get; private set; }

        #endregion

        public PhotonApplication()
        {
            RequestHandlers = new Dictionary<byte, IPhotonHandler>();
            ResponseHandlers = new Dictionary<byte, IPhotonHandler>();
            EventHandlers = new Dictionary<byte, IPhotonHandler>();

            ExcutionFiber = new PoolFiber();
            ExcutionFiber.Start();

            ExcutionFiber.ScheduleOnInterval(Update, 1000, 60000);

            DefaultHandler = CreateDefaultHandler();
        }

        protected abstract IPhotonHandler CreateDefaultHandler();

        public virtual void HandleRequest(OperationRequest request, PeerBase peer)
        {
            IPhotonHandler handler;

            if (RequestHandlers.TryGetValue(request.OperationCode, out handler))
                ExcutionFiber.Enqueue(() => { handler.HandleRequest(request, peer); });
            else
                ExcutionFiber.Enqueue(() => { DefaultHandler.HandleRequest(request, peer); });
        }

        public virtual void HandleResponse(OperationResponse response, PeerBase peer)
        {
            IPhotonHandler handler;

            if (ResponseHandlers.TryGetValue(response.OperationCode, out handler))
                ExcutionFiber.Enqueue(() => { handler.HandleResponse(response, peer); });
            else
                ExcutionFiber.Enqueue(() => { DefaultHandler.HandleResponse(response, peer); });
        }

        public virtual void HandleEvent(IEventData eventData, PeerBase peer)
        {
            IPhotonHandler handler;

            if (EventHandlers.TryGetValue(eventData.Code, out handler))
                ExcutionFiber.Enqueue(() => { handler.HandleEvent(eventData, peer); });
            else
                ExcutionFiber.Enqueue(() => { DefaultHandler.HandleEvent(eventData, peer); });
        }

        protected void RegisterHandler(IPhotonHandler handler)
        {
            if ((handler.Type & HandlerType.Request) == HandlerType.Request)
            {
                if (RequestHandlers.ContainsKey((byte)handler.RequestCode))
                    Log.WarnFormat("RequestHandler {0} was already registered.", handler.RequestCode);
                else
                    RequestHandlers.Add((byte)handler.RequestCode, handler);
            }

            if ((handler.Type & HandlerType.Response) == HandlerType.Response)
            {
                if (ResponseHandlers.ContainsKey((byte)handler.ResponseCode))
                    Log.WarnFormat("ResponseHandler {0} was already registered.", handler.ResponseCode);
                else
                    ResponseHandlers.Add((byte)handler.ResponseCode, handler);
            }

            if ((handler.Type & HandlerType.Event) == HandlerType.Event)
            {
                if (EventHandlers.ContainsKey((byte)handler.EventCode))
                    Log.WarnFormat("EventHandler {0} was already registered.", handler.EventCode);
                else
                    EventHandlers.Add((byte)handler.EventCode, handler);
            }
        }

        #region Overrides of ApplicationBase

        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            return null;
        }

        protected override void Setup()
        {
            LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
            GlobalContext.Properties["LogFileName"] = ApplicationName;
            XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(BinaryPath, "log4net.config")));
        }

        protected override void TearDown()
        {
            ExcutionFiber.Dispose();
        }

        protected virtual void Update()
        {
        }

        #endregion
    }
}

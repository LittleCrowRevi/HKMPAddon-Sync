using Hkmp.Api.Client;
using System.Reflection;

namespace HKMPAddon.HKMP
{
    /// <summary>
    /// The client add-on class.
    /// </summary>
    internal class HkmpAddonClientAddon : ClientAddon
    {
        protected override string Name => Assembly.GetExecutingAssembly().GetName().Name;

        protected override string Version => HkmpAddon.Instance.GetVersion();

        public override bool NeedsNetwork => true;

        /// <summary>
        /// The global instance of the client add-on.
        /// </summary>
        public static HkmpAddonClientAddon Instance { get; private set; }

        /// <summary>
        /// Holds a reference to the client API passed into Initialize.
        /// </summary>
        private IClientApi _clientApi;

        public override void Initialize(IClientApi clientApi)
        {
            Instance = this;
            _clientApi = clientApi;

            // Sends packets from this client to the server.
            var sender = clientApi.NetClient.GetNetworkSender<FromClientToServerPackets>(Instance);

            // Receives and handles packets from the server to this client.
            var receiver = clientApi.NetClient.GetNetworkReceiver<FromServerToClientPackets>(Instance, clientPacket =>
            {
                return clientPacket switch
                {
                    FromServerToClientPackets.SendMessage => new MessageFromServerToClientData(),
                    _ => null
                };
            });

            receiver.RegisterPacketHandler<MessageFromServerToClientData>
            (
                FromServerToClientPackets.SendMessage,
                packetData =>
                {
                    HkmpAddon.Instance.Log("[Client] Message from server: " + packetData.Message);
                }
            );

            // Register an instance of the client command class.
            clientApi.CommandManager.RegisterCommand(new HKMPAddonCommand());

            clientApi.ClientManager.ConnectEvent += () => 
            {
                HkmpAddon.Instance.Log("[Client] You have connected to the server.");
            };

            clientApi.ClientManager.DisconnectEvent += () =>
            {
                HkmpAddon.Instance.Log("[Client] You have disconnected from the server.");
            };

            clientApi.ClientManager.PlayerConnectEvent += OnPlayerConnect;

            clientApi.ClientManager.PlayerDisconnectEvent += OnPlayerDisconnect;

            clientApi.ClientManager.PlayerEnterSceneEvent += OnPlayerEnterScene;

            clientApi.ClientManager.PlayerLeaveSceneEvent += OnPlayerLeaveScene;
        }

        /// <summary>
        /// Sends a message from the client to the server.
        /// </summary>
        public void SendMessage(string message)
        {
            var sender = _clientApi.NetClient.GetNetworkSender<FromClientToServerPackets>(Instance);
            sender.SendSingleData(FromClientToServerPackets.SendMessage, new MessageFromClientToServerData
            {
                Message = message,
            });
        }

        private void OnPlayerConnect(IClientPlayer player)
        {
            HkmpAddon.Instance.Log($"[Client] Player {player.Username} has connected to the server.");
        }

        private void OnPlayerDisconnect(IClientPlayer player)
        {
            HkmpAddon.Instance.Log($"[Client] Player {player.Username} has disconnected from the server.");
        }

        private void OnPlayerEnterScene(IClientPlayer player)
        {
            HkmpAddon.Instance.Log($"[Client] Player {player.Username} has entered a scene.");
        }

        private void OnPlayerLeaveScene(IClientPlayer player)
        { 
            HkmpAddon.Instance.Log($"[Client] Player {player.Username} has left a scene.");
        }
    }
}
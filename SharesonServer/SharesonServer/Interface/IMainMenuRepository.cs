
namespace SharesonServer.Interface
{
    public interface IMainMenuRepository
    {
        bool RunServer();
        bool StopServer();
        int ConnectedUsers();
        bool RepeatCountConnectedClients_Task { get; set; }
    }
}

using Mirror;
using System.Threading.Tasks;

public class MyNetworkManager : NetworkManager
{
    private static bool isServerLoadedServerFlag = false;
    private static bool isServerLoadedClientFlag = false;

    public override async void OnStartServer()
    {
        await SpellFactory.InitializeSpellsAsync();
        isServerLoadedServerFlag = true;
    }

    public override async void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        while (!isServerLoadedServerFlag)
        {
            await Task.Delay(100);
        }

        TargetNotifyServerLoaded(conn);
    }

    public override async void OnStartClient()
    {
        await WaitServerLoadedAsync();
        await SpellFactory.InitializeSpellsAsync();
    }

    private static async Task WaitServerLoadedAsync()
    {
        while (!isServerLoadedClientFlag)
        {
            await Task.Delay(100);
        }
    }

    [TargetRpc]
    private void TargetNotifyServerLoaded(NetworkConnection target)
    {
        isServerLoadedClientFlag = true;
    }
}
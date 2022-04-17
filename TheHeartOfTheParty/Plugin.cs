using Dalamud.Data;
using Dalamud.Game.ClientState;
using Dalamud.IoC;
using Dalamud.Plugin;

namespace TheHeartOfTheParty;

public class Plugin : IDalamudPlugin {
    public string Name => "The Heart of the Party";

    [PluginService]
    internal DalamudPluginInterface Interface { get; init; }

    [PluginService]
    internal ClientState ClientState { get; init; }

    [PluginService]
    internal DataManager DataManager { get; init; }

    internal GameFunctions GameFunctions { get; }

    private PluginUi Ui { get; }

    public Plugin() {
        this.GameFunctions = new GameFunctions(this);
        this.Ui = new PluginUi(this);
    }

    public void Dispose() {
        this.Ui.Dispose();
    }
}

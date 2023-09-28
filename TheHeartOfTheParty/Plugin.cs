using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace TheHeartOfTheParty;

public class Plugin : IDalamudPlugin {
    internal static string Name => "The Heart of the Party";

    [PluginService]
    internal static IGameInteropProvider GameInteropProvider { get; private set; }

    [PluginService]
    internal DalamudPluginInterface Interface { get; init; }

    [PluginService]
    internal IClientState ClientState { get; init; }

    [PluginService]
    internal ICommandManager CommandManager { get; init; }

    [PluginService]
    internal IDataManager DataManager { get; init; }

    internal Configuration Config { get; }
    internal GameFunctions Functions { get; }
    internal PluginUi Ui { get; }
    private Commands Commands { get; }

    public Plugin() {
        this.Config = this.Interface!.GetPluginConfig() as Configuration ?? new Configuration();

        this.Functions = new GameFunctions();
        this.Ui = new PluginUi(this);
        this.Commands = new Commands(this);
    }

    public void Dispose() {
        this.Commands.Dispose();
        this.Ui.Dispose();
    }

    internal void SaveConfig() {
        this.Interface.SavePluginConfig(this.Config);
    }
}

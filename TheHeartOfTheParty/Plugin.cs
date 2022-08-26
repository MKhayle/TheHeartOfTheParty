using Dalamud.Data;
using Dalamud.Game.ClientState;
using Dalamud.Game.Command;
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
    internal CommandManager CommandManager { get; init; }

    [PluginService]
    internal DataManager DataManager { get; init; }

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

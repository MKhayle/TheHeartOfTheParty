using Dalamud.Game.Command;

namespace TheHeartOfTheParty;

internal class Commands : IDisposable {
    private static readonly string[] CommandNames = {
        "/thotp",
        "/titles",
    };

    private Plugin Plugin { get; }

    internal Commands(Plugin plugin) {
        this.Plugin = plugin;

        foreach (var name in CommandNames) {
            this.Plugin.CommandManager.AddHandler(name, new CommandInfo(this.OnCommand) {
                HelpMessage = $"Toggles {Plugin.Name}",
            });
        }
    }

    public void Dispose() {
        foreach (var name in CommandNames) {
            this.Plugin.CommandManager.RemoveHandler(name);
        }
    }

    private void OnCommand(string command, string arguments) {
        this.Plugin.Ui.Toggle();
    }
}

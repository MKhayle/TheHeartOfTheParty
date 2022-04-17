using Dalamud.Configuration;

namespace TheHeartOfTheParty; 

internal class Configuration : IPluginConfiguration {
    public int Version { get; set; } = 1;

    public bool OnlyUnlocked = true;
    public SortOrder SortOrder = SortOrder.Default;
}

internal enum SortOrder {
    Default,
    Alphabetical,
}

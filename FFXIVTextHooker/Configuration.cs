using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

namespace FFXIVTextHooker;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public bool HookText { get; set; } = true;
    public bool HookBattleText { get; set; } = true;
    public bool HookChat { get; set; } = true;

    #region HOOK_CHAT_VARS
    public bool HookChatNone { get; set; } = true;
    public bool HookChatDebug { get; set; } = true;
    public bool HookChatUrgent { get; set; } = true;
    public bool HookChatNotice { get; set; } = true;
    public bool HookChatSay { get; set; } = true;
    public bool HookChatShout { get; set; } = true;
    public bool HookChatTellOutgoing { get; set; } = true;
    public bool HookChatTellIncoming { get; set; } = true;
    public bool HookChatParty { get; set; } = true;
    public bool HookChatAlliance { get; set; } = true;
    public bool HookChatLs1 { get; set; } = true;
    public bool HookChatLs2 { get; set; } = true;
    public bool HookChatLs3 { get; set; } = true;
    public bool HookChatLs4 { get; set; } = true;
    public bool HookChatLs5 { get; set; } = true;
    public bool HookChatLs6 { get; set; } = true;
    public bool HookChatLs7 { get; set; } = true;
    public bool HookChatLs8 { get; set; } = true;
    public bool HookChatFreeCompany { get; set; } = true;
    public bool HookChatNoviceNetwork { get; set; } = true;
    public bool HookChatCustomEmote { get; set; } = true;
    public bool HookChatStandardEmote { get; set; } = true;
    public bool HookChatYell { get; set; } = true;
    public bool HookChatCrossParty { get; set; } = true;
    public bool HookChatPvPTeam { get; set; } = true;
    public bool HookChatCrossLinkShell1 { get; set; } = true;
    public bool HookChatEcho { get; set; } = true;
    public bool HookChatSystemError { get; set; } = true;
    public bool HookChatSystemMessage { get; set; } = true;
    public bool HookChatGatheringSystemMessage { get; set; } = true;
    public bool HookChatErrorMessage { get; set; } = true;
    public bool HookChatNPCDialogue { get; set; } = true;
    public bool HookChatNPCDialogueAnnouncements { get; set; } = true;
    public bool HookChatRetainerSale { get; set; } = true;
    public bool HookChatCrossLinkShell2 { get; set; } = true;
    public bool HookChatCrossLinkShell3 { get; set; } = true;
    public bool HookChatCrossLinkShell4 { get; set; } = true;
    public bool HookChatCrossLinkShell5 { get; set; } = true;
    public bool HookChatCrossLinkShell6 { get; set; } = true;
    public bool HookChatCrossLinkShell7 { get; set; } = true;
    public bool HookChatCrossLinkShell8 { get; set; } = true;
    public bool HookChatFallback { get; set; } = false;
    #endregion

    public bool HookTooltips { get; set; } = true;
    public Hotkey.Enum ModifierKey { get; set; } = Hotkey.Enum.VkShift;
    public Hotkey.Enum PrimaryKey { get; set; } = Hotkey.Enum.VkT;
    public bool HookActionTooltips { get; set; } = true;
    public bool HookItemTooltips { get; set; } = true; 
    // the below exist just to make saving less cumbersome
    [NonSerialized]
    private DalamudPluginInterface? PluginInterface;

    public void Initialize(DalamudPluginInterface pluginInterface)
    {
        PluginInterface = pluginInterface;
    }

    public void Save()
    {
        PluginInterface!.SavePluginConfig(this);
    }
}

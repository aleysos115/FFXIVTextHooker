using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVTextHooker.Windows;
using XivCommon;
using XivCommon.Functions;
using Lumina.Data.Parsing;
using Dalamud.Logging;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using System.Windows.Forms;
using FFXIVClientStructs.FFXIV.Client.System.Input;
using System;
using XivCommon.Functions.Tooltips;
using Dalamud.Game.Gui;

namespace FFXIVTextHooker;

public sealed class Plugin : IDalamudPlugin
{
    private const string CommandName = "/th";

    private IFramework Framework { get; init; }
    private IKeyState KeyState { get; init; }
    private DalamudPluginInterface PluginInterface { get; init; }
    private ICommandManager CommandManager { get; init; }
    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("SamplePlugin");
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }

    [PluginService] public static IChatGui Chat { get; private set; } = null!;

    private static XivCommonBase Common { get; set; }

    private ActionTooltip hoveredAction = null;
    private ItemTooltip hoveredItem = null;

    public Plugin(
        [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
        [RequiredVersion("1.0")] ICommandManager commandManager,
        [RequiredVersion("1.0")] ITextureProvider textureProvider,
        [RequiredVersion("1.0")] IKeyState keyState,
        [RequiredVersion("1.0")] IFramework framework)
    {
        PluginInterface = pluginInterface;
        CommandManager = commandManager;
        KeyState = keyState;
        Framework = framework;

        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Configuration.Initialize(PluginInterface);

        Common = new XivCommonBase(PluginInterface, Hooks.Talk | Hooks.BattleTalk | Hooks.Tooltips);
        
        // you might normally want to embed resources and load them from the manifest stream
        var file = new FileInfo(Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "goat.png"));

        // ITextureProvider takes care of the image caching and dispose
        var goatImage = textureProvider.GetTextureFromFile(file);

        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this, goatImage);

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "A useful message to display in /xlhelp"
        });

        PluginInterface.UiBuilder.Draw += DrawUI;

        // This adds a button to the plugin installer entry of this plugin which allows
        // to toggle the display status of the configuration ui
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;

        // Adds another button that is doing the same but for the main ui of the plugin
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;

        Common.Functions.Talk.OnTalk += this.GetTalk;
        Common.Functions.BattleTalk.OnBattleTalk += this.GetBattleTalk;
        Common.Functions.Tooltips.OnItemTooltip += this.GetItemTooltip;
        Common.Functions.Tooltips.OnActionTooltip += this.GetActionTooltip;

        Chat.ChatMessage += this.GetChat;

        Framework.Update += this.OnFrameworkTick;
    }

    private void OnFrameworkTick(IFramework framework)
    {
        if(IsKeyBindPressed())
        {
            if(hoveredItem != null) CopyToClipboard("Item Tooltip", hoveredItem[ItemTooltipString.Name].TextValue);
            if(hoveredAction != null) CopyToClipboard("Action Tooltip", hoveredAction[ActionTooltipString.Name].TextValue);
        }
    }

    private void CopyToClipboard(string origin, string text)
    {
        MainWindow.AddTextEntry(origin, DateTime.Now.ToString(), text);
        System.Windows.Forms.Clipboard.SetText(text);
    }

    private void GetTalk(ref SeString name, ref SeString text, ref TalkStyle style)
    {
        if (Configuration.HookText)
        {
            string combinedText = string.Format("{0}: {1}", name.TextValue, text.TextValue);
            CopyToClipboard("Talk", combinedText);
        }
    }

    private void GetBattleTalk(ref SeString sender, ref SeString message, ref BattleTalkOptions options, ref bool isHandled)
    {
        if (Configuration.HookBattleText)
        {
            string combinedText = string.Format("{0}: {1}", sender.TextValue, message.TextValue);
            CopyToClipboard("BattleTalk", combinedText);
        }
    }

    private void GetChat(XivChatType type, uint id, ref SeString sender, ref SeString message, ref bool handled)
    {
        if(Configuration.HookChat)
        {
            string text = string.Format("{0}: {1}", sender.TextValue, message.TextValue);
            #region CHAT_TYPES
            switch (type)
            {
                case XivChatType.None:
                    if (Configuration.HookChatNone) CopyToClipboard("Chat/None", text);
                    break;
                case XivChatType.Debug:
                    if (Configuration.HookChatDebug) CopyToClipboard("Chat/Debug", text);
                    break;
                case XivChatType.Urgent:
                    if (Configuration.HookChatUrgent) CopyToClipboard("Chat/Urgent", text);
                    break;
                case XivChatType.Notice:
                    if (Configuration.HookChatNotice) CopyToClipboard("Chat/Notice", text);
                    break;
                case XivChatType.Say:
                    if (Configuration.HookChatSay) CopyToClipboard("Chat/Say", text);
                    break;
                case XivChatType.Shout:
                    if (Configuration.HookChatShout) CopyToClipboard("Chat/Shout", text);
                    break;
                case XivChatType.TellOutgoing:
                    if (Configuration.HookChatTellOutgoing) CopyToClipboard("Chat/TellOutgoing", text);
                    break;
                case XivChatType.TellIncoming:
                    if (Configuration.HookChatTellIncoming) CopyToClipboard("Chat/TellIncoming", text);
                    break;
                case XivChatType.Party:
                    if (Configuration.HookChatParty) CopyToClipboard("Chat/Party", text);
                    break;
                case XivChatType.Alliance:
                    if (Configuration.HookChatAlliance) CopyToClipboard("Chat/Alliance", text);
                    break;
                case XivChatType.Ls1:
                    if (Configuration.HookChatLs1) CopyToClipboard("Chat/Ls1", text);
                    break;
                case XivChatType.Ls2:
                    if (Configuration.HookChatLs2) CopyToClipboard("Chat/Ls2", text);
                    break;
                case XivChatType.Ls3:
                    if (Configuration.HookChatLs3) CopyToClipboard("Chat/Ls3", text);
                    break;
                case XivChatType.Ls4:
                    if (Configuration.HookChatLs4) CopyToClipboard("Chat/Ls4", text);
                    break;
                case XivChatType.Ls5:
                    if (Configuration.HookChatLs5) CopyToClipboard("Chat/Ls5", text);
                    break;
                case XivChatType.Ls6:
                    if (Configuration.HookChatLs6) CopyToClipboard("Chat/Ls6", text);
                    break;
                case XivChatType.Ls7:
                    if (Configuration.HookChatLs7) CopyToClipboard("Chat/Ls7", text);
                    break;
                case XivChatType.Ls8:
                    if (Configuration.HookChatLs8) CopyToClipboard("Chat/Ls8", text);
                    break;
                case XivChatType.FreeCompany:
                    if (Configuration.HookChatFreeCompany) CopyToClipboard("Chat/FreeCompany", text);
                    break;
                case XivChatType.NoviceNetwork:
                    if (Configuration.HookChatNoviceNetwork) CopyToClipboard("Chat/NoviceNetwork", text);
                    break;
                case XivChatType.CustomEmote:
                    if (Configuration.HookChatCustomEmote) CopyToClipboard("Chat/CustomEmote", text);
                    break;
                case XivChatType.StandardEmote:
                    if (Configuration.HookChatStandardEmote) CopyToClipboard("Chat/StandardEmote", text);
                    break;
                case XivChatType.Yell:
                    if (Configuration.HookChatYell) CopyToClipboard("Chat/Yell", text);
                    break;
                case XivChatType.CrossParty:
                    if (Configuration.HookChatCrossParty) CopyToClipboard("Chat/CrossParty", text);
                    break;
                case XivChatType.PvPTeam:
                    if (Configuration.HookChatPvPTeam) CopyToClipboard("Chat/PvPTeam", text);
                    break;
                case XivChatType.CrossLinkShell1:
                    if (Configuration.HookChatCrossLinkShell1) CopyToClipboard("Chat/CrossLinkShell1", text);
                    break;
                case XivChatType.Echo:
                    if (Configuration.HookChatEcho) CopyToClipboard("Chat/Echo", text);
                    break;
                case XivChatType.SystemError:
                    if (Configuration.HookChatSystemError) CopyToClipboard("Chat/SystemError", text);
                    break;
                case XivChatType.SystemMessage:
                    if (Configuration.HookChatSystemMessage) CopyToClipboard("Chat/SystemMessage", text);
                    break;
                case XivChatType.GatheringSystemMessage:
                    if (Configuration.HookChatGatheringSystemMessage) CopyToClipboard("Chat/GatheringSystemMessage", text);
                    break;
                case XivChatType.ErrorMessage:
                    if (Configuration.HookChatErrorMessage) CopyToClipboard("Chat/ErrorMessage", text);
                    break;
                case XivChatType.NPCDialogue:
                    if (Configuration.HookChatNPCDialogue) CopyToClipboard("Chat/NPCDialogue", text);
                    break;
                case XivChatType.NPCDialogueAnnouncements:
                    if (Configuration.HookChatNPCDialogueAnnouncements) CopyToClipboard("Chat/NPCDialogueAnnouncements", text);
                    break;
                case XivChatType.RetainerSale:
                    if (Configuration.HookChatRetainerSale) CopyToClipboard("Chat/RetainerSale", text);
                    break;
                case XivChatType.CrossLinkShell2:
                    if (Configuration.HookChatCrossLinkShell2) CopyToClipboard("Chat/CrossLinkShell2", text);
                    break;
                case XivChatType.CrossLinkShell3:
                    if (Configuration.HookChatCrossLinkShell3) CopyToClipboard("Chat/CrossLinkShell3", text);
                    break;
                case XivChatType.CrossLinkShell4:
                    if (Configuration.HookChatCrossLinkShell4) CopyToClipboard("Chat/CrossLinkShell4", text);
                    break;
                case XivChatType.CrossLinkShell5:
                    if (Configuration.HookChatCrossLinkShell5) CopyToClipboard("Chat/CrossLinkShell5", text);
                    break;
                case XivChatType.CrossLinkShell6:
                    if (Configuration.HookChatCrossLinkShell6) CopyToClipboard("Chat/CrossLinkShell6", text);
                    break;
                case XivChatType.CrossLinkShell7:
                    if (Configuration.HookChatCrossLinkShell7) CopyToClipboard("Chat/CrossLinkShell7", text);
                    break;
                case XivChatType.CrossLinkShell8:
                    if (Configuration.HookChatCrossLinkShell8) CopyToClipboard("Chat/CrossLinkShell8", text);
                    break;
                default:
                    if (Configuration.HookChatFallback) CopyToClipboard("Chat/Fallback", text);
                    break;
            }
            #endregion
        }
    }

    //Note: GetRawValue ~ 3 = KeyPressed, 1 = KeyDown, 4 = KeyUp
    public bool IsKeyBindPressed()
    {
        //if (!this.Configuration.KeybindEnabled) return true;
        if (this.Configuration.PrimaryKey == Hotkey.Enum.VkNone)
        {
            return KeyState.GetRawValue((byte)this.Configuration.ModifierKey) == 3;
        }

        if (this.Configuration.ModifierKey == Hotkey.Enum.VkNone)
        {
            return KeyState.GetRawValue((byte)this.Configuration.PrimaryKey) == 3;
        }

        return KeyState[(byte)this.Configuration.ModifierKey] && KeyState.GetRawValue((byte)this.Configuration.PrimaryKey) == 3;
    }

    private void GetItemTooltip(ItemTooltip itemTooltip, ulong itemId)
    {
        hoveredItem = itemTooltip;
        hoveredAction = null;
        //CopyToClipboard("Item Tooltip", itemTooltip[ItemTooltipString.Name].TextValue);
    }

    private void GetActionTooltip(ActionTooltip actionTooltip, HoveredAction action)
    {
        hoveredAction = actionTooltip;
        hoveredItem = null;
        //CopyToClipboard("Action Tooltip", actionTooltip[ActionTooltipString.Name].TextValue);
    }

    public void Dispose()
    {
        Framework.Update -= this.OnFrameworkTick;
        Chat.ChatMessage -= this.GetChat;
        Common.Functions.Talk.OnTalk -= this.GetTalk;
        Common.Functions.BattleTalk.OnBattleTalk -= this.GetBattleTalk;
        Common.Functions.Tooltips.OnItemTooltip -= this.GetItemTooltip;
        Common.Functions.Tooltips.OnActionTooltip -= this.GetActionTooltip;
        Common.Functions.Dispose();

        Common?.Dispose();

        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();

        CommandManager.RemoveHandler(CommandName);
    }

    private void OnCommand(string command, string args)
    {
        // in response to the slash command, just toggle the display status of our main ui
        ToggleMainUI();
    }

    private void DrawUI() => WindowSystem.Draw();

    public void ToggleConfigUI() => ConfigWindow.Toggle();
    public void ToggleMainUI() => MainWindow.Toggle();
}

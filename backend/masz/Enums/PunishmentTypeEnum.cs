using DSharpPlus.SlashCommands;

namespace masz.Enums
{
    public enum PunishmentType
    {
        [ChoiceName("Warn")]
        None,
        [ChoiceName("Mute")]
        Mute,
        [ChoiceName("Kick")]
        Kick,
        [ChoiceName("Ban")]
        Ban
    }
}
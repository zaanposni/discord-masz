using Discord;
using Discord.Interactions;
using MASZ.Enums;

namespace MASZ.Models;

public class PunishModalKey
{
    public static readonly string Reason = "reason";
    public static readonly string Duration = "duration";
    public static readonly string Description = "description";
    public static readonly string DmNotification = "dm-notification";
    public static readonly string PublicNotification = "public-notification";
}


public class PunishModal
{
    public static Modal Create(PunishmentType punishmentType, string title, ulong userId, bool showDuration)
    {
        var modalBuilder = new ModalBuilder(title, $"punish:{punishmentType.ToString().ToLower()}:{userId}");

        modalBuilder.AddTextInput("Reason", PunishModalKey.Reason, placeholder: "Reason", maxLength: 250);
        if (showDuration) modalBuilder.AddTextInput("Duration", PunishModalKey.Duration, placeholder: "Duration", value: "2w", required: false);
        modalBuilder.AddTextInput("Description", PunishModalKey.Description, style: TextInputStyle.Paragraph, placeholder: "No description provided", required: false, maxLength: 1000);
        modalBuilder.AddTextInput("Send DM Notification", PunishModalKey.DmNotification, value: "true", required: false);
        modalBuilder.AddTextInput("Send Public Notification", PunishModalKey.PublicNotification, value: "false", required: false);

        return modalBuilder.Build();
    }

    [ModalTextInput("reason")]
    public string Reason { get; set; }

    [ModalTextInput("duration")]
    public string Duration { get; set; }

    [ModalTextInput("description")]
    public string Description { get; set; }

    [ModalTextInput("dm-notification")]
    public string DmNotification { get; set; }

    [ModalTextInput("public-notification")]
    public string PublicNotification { get; set; }
}

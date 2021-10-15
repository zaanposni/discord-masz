using System;
using masz.Extensions;
using masz.Enums;

namespace masz.Translations
{
    public class Translation
    {
        public Language preferredLanguage { get; set; }
        private Translation(Language preferredLanguage = Language.en)
        {
            this.preferredLanguage = preferredLanguage;
        }
        public static Translation Ctx(Language preferredLanguage = Language.en) {
            return new Translation(preferredLanguage);
        }
        public string Features() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Features";
                case Language.de:
                    return "Features";
                case Language.at:
                    return "Features";
            }
            return "Features";
        }
        public string Automoderation() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Automoderation";
                case Language.de:
                    return "Automoderation";
                case Language.at:
                    return "Automodaration";
            }
            return "Automoderation";
        }
        public string Action() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Action";
                case Language.de:
                    return "Aktion";
                case Language.at:
                    return "Aktio";
            }
            return "Action";
        }
        public string NotFound() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Not found.";
                case Language.de:
                    return "Nicht gefunden.";
                case Language.at:
                    return "Ned gfundn.";
            }
            return "Not found.";
        }
        public string Author() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Author";
                case Language.de:
                    return "Autor";
                case Language.at:
                    return "Autoa";
            }
            return "Author";
        }
        public string MessageContent() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Message content";
                case Language.de:
                    return "Nachrichteninhalt";
                case Language.at:
                    return "Nochrichtninhoit";
            }
            return "Message content";
        }
        public string ViewDetailsOn(string url) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"View details on: {url}";
                case Language.de:
                    return $"Details anzeigen auf: {url}";
                case Language.at:
                    return $"Details ozeign auf: {url}";
            }
            return $"View details on: {url}";
        }
        public string Attachments() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Attachments";
                case Language.de:
                    return "Anhänge";
                case Language.at:
                    return "Ohäng";
            }
            return "Attachments";
        }
        public string AndXMore(int count) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"and {count} more...";
                case Language.de:
                    return $"und {count} weitere...";
                case Language.at:
                    return $"und {count} weitare...";
            }
            return $"and {count} more...";
        }
        public string Channel() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Channel";
                case Language.de:
                    return "Kanal";
                case Language.at:
                    return "Kanoi";
            }
            return "Channel";
        }
        public string SomethingWentWrong() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Something went wrong.";
                case Language.de:
                    return "Etwas ist schief gelaufen.";
                case Language.at:
                    return "Etwos hot ned funktioniat.";
            }
            return "Something went wrong.";
        }
        public string Code() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Code";
                case Language.de:
                    return "Code";
                case Language.at:
                    return "Code";
            }
            return "Code";
        }
        public string LanguageWord() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Language";
                case Language.de:
                    return "Sprache";
                case Language.at:
                    return "Sproch";
            }
            return "Language";
        }
        public string Timestamps() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Timestamps";
                case Language.de:
                    return "Zeitstempel";
                case Language.at:
                    return "Zeitstempl";
            }
            return "Timestamps";
        }
        public string Support() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Support";
                case Language.de:
                    return "Support";
                case Language.at:
                    return "Supoat";
            }
            return "Support";
        }
        public string Punishment() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Punishment";
                case Language.de:
                    return "Bestrafung";
                case Language.at:
                    return "Bestrofung";
            }
            return "Punishment";
        }
        public string Until() {
            switch (preferredLanguage) {
                case Language.en:
                    return "until";
                case Language.de:
                    return "bis";
                case Language.at:
                    return "bis";
            }
            return "until";
        }
        public string PunishmentUntil() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Punished until";
                case Language.de:
                    return "Bestrafung bis";
                case Language.at:
                    return "Bestroft bis";
            }
            return "Punished until";
        }
        public string Description() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Description";
                case Language.de:
                    return "Beschreibung";
                case Language.at:
                    return "Beschreibung";
            }
            return "Description";
        }
        public string Labels() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Labels";
                case Language.de:
                    return "Labels";
                case Language.at:
                    return "Labl";
            }
            return "Labels";
        }
        public string Filename() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Filename";
                case Language.de:
                    return "Dateiname";
                case Language.at:
                    return "Dateinom";
            }
            return "Filename";
        }
        public string Message() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Message";
                case Language.de:
                    return "Nachricht";
                case Language.at:
                    return "Nochricht";
            }
            return "Message";
        }
        public string UserNote() {
            switch (preferredLanguage) {
                case Language.en:
                    return "UserNote";
                case Language.de:
                    return "Benutzernotiz";
                case Language.at:
                    return "Benutzanotiz";
            }
            return "UserNote";
        }
        public string UserNotes() {
            switch (preferredLanguage) {
                case Language.en:
                    return "UserNotes";
                case Language.de:
                    return "Benutzernotizen";
                case Language.at:
                    return "Benutzanotizn";
            }
            return "UserNotes";
        }
        public string Cases() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Cases";
                case Language.de:
                    return "Vorfälle";
                case Language.at:
                    return "Vorfälle";
            }
            return "Cases";
        }
        public string ActivePunishments() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Active punishments";
                case Language.de:
                    return "Aktive Bestrafungen";
                case Language.at:
                    return "Aktive Bestrofungen";
            }
            return "Active punishments";
        }
        public string UserMap() {
            switch (preferredLanguage) {
                case Language.en:
                    return "UserMap";
                case Language.de:
                    return "Benutzerbeziehung";
                case Language.at:
                    return "Benutzabeziehung";
            }
            return "UserMap";
        }
        public string UserMaps() {
            switch (preferredLanguage) {
                case Language.en:
                    return "UserMaps";
                case Language.de:
                    return "Benutzerbeziehungen";
                case Language.at:
                    return "Benutzabeziehungen";
            }
            return "UserMaps";
        }
        public string UserMapBetween(masz.Models.UserMapping userMap) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"UserMap between {userMap.UserA} and {userMap.UserB}.";
                case Language.de:
                    return $"Benutzerbeziehung zwischen {userMap.UserA} und {userMap.UserB}.";
                case Language.at:
                    return $"Benutzabeziehung zwischa {userMap.UserA} und {userMap.UserB}.";
            }
            return $"UserMap between {userMap.UserA} and {userMap.UserB}.";
        }
        public string Type() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Type";
                case Language.de:
                    return "Typ";
                case Language.at:
                    return "Typ";
            }
            return "Type";
        }
        public string Joined() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Joined";
                case Language.de:
                    return "Beigetreten";
                case Language.at:
                    return "Beigetretn";
            }
            return "Joined";
        }
        public string Registered() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Registered";
                case Language.de:
                    return "Registriert";
                case Language.at:
                    return "Registriat";
            }
            return "Registered";
        }
        public string NotificationModcaseCreatePublic(masz.Models.ModCase modCase) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been created.";
                case Language.de:
                    return $"Ein **Vorfall** für <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) wurde erstellt.";
                case Language.at:
                    return $"A **Vorfoi** fia <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) is erstöt woan.";
            }
            return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been created.";
        }
        public string NotificationModcaseCreateInternal(masz.Models.ModCase modCase, DSharpPlus.Entities.DiscordUser moderator) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been created by <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).";
                case Language.de:
                    return $"Ein **Vorfall** für <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) wurde von <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}) erstellt.";
                case Language.at:
                    return $"A **Vorfoi** fia <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) woad fo <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}) erstöt.";
            }
            return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been created by <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).";
        }
        public string NotificationModcaseUpdatePublic(masz.Models.ModCase modCase) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been updated.";
                case Language.de:
                    return $"Ein **Vorfall** für <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) wurde aktualisiert.";
                case Language.at:
                    return $"A **Vorfoi** fia <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) is aktualisiert woan.";
            }
            return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been updated.";
        }
        public string NotificationModcaseUpdateInternal(masz.Models.ModCase modCase, DSharpPlus.Entities.DiscordUser moderator) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been updated by <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).";
                case Language.de:
                    return $"Ein **Vorfall** für <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) wurde von <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}) aktualisiert.";
                case Language.at:
                    return $"A **Vorfoi** fia <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) is fo <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}) aktualisiert woan.";
            }
            return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been updated by <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).";
        }
        public string NotificationModcaseDeletePublic(masz.Models.ModCase modCase) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been deleted.";
                case Language.de:
                    return $"Ein **Vorfall** für <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) wurde gelöscht.";
                case Language.at:
                    return $"A **Vorfoi** fia <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) is glescht woan";
            }
            return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been deleted.";
        }
        public string NotificationModcaseDeleteInternal(masz.Models.ModCase modCase, DSharpPlus.Entities.DiscordUser moderator) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been deleted by <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).";
                case Language.de:
                    return $"Ein **Vorfall** für <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) wurde von <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}) gelöscht.";
                case Language.at:
                    return $"A **Vorfoi** fia <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) is vo <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}) glescht woan.";
            }
            return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been deleted by <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).";
        }
        public string NotificationModcaseCommentsShortCreate() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Comment created";
                case Language.de:
                    return "Kommentar erstellt";
                case Language.at:
                    return "Kommentoa erstöt";
            }
            return "Comment created";
        }
        public string NotificationModcaseCommentsShortUpdate() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Comment updated";
                case Language.de:
                    return "Kommentar aktualisiert";
                case Language.at:
                    return "kommentoa aktualisiert";
            }
            return "Comment updated";
        }
        public string NotificationModcaseCommentsShortDelete() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Comment deleted";
                case Language.de:
                    return "Kommentar gelöscht";
                case Language.at:
                    return "kommentoa glescht";
            }
            return "Comment deleted";
        }
        public string NotificationModcaseCommentsCreate(DSharpPlus.Entities.DiscordUser actor) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"A **comment** has been created by <@{actor.Id}>.";
                case Language.de:
                    return $"Ein **Kommentar** wurde von <@{actor.Id}> erstellt.";
                case Language.at:
                    return $"A **Kommentoa** wuad vo <@{actor.Id}> erstöt.";
            }
            return $"A **comment** has been created by <@{actor.Id}>.";
        }
        public string NotificationModcaseCommentsUpdate(DSharpPlus.Entities.DiscordUser actor) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"A **comment** has been updated by <@{actor.Id}>.";
                case Language.de:
                    return $"Ein **Kommentar** wurde von <@{actor.Id}> aktualisiert.";
                case Language.at:
                    return $"A **Kommentoa** is vo <@{actor.Id}> aktualisiert woan.";
            }
            return $"A **comment** has been updated by <@{actor.Id}>.";
        }
        public string NotificationModcaseCommentsDelete(DSharpPlus.Entities.DiscordUser actor) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"A **comment** has been deleted by <@{actor.Id}>.";
                case Language.de:
                    return $"Ein **Kommentar** wurde von <@{actor.Id}> gelöscht.";
                case Language.at:
                    return $"A **Kommentoa** wuad vo <@{actor.Id}> glescht.";
            }
            return $"A **comment** has been deleted by <@{actor.Id}>.";
        }
        public string NotificationModcaseFileCreate(DSharpPlus.Entities.DiscordUser actor) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"A **file** has been uploaded by <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).";
                case Language.de:
                    return $"Eine **Datei** wurde von <@{actor.Id}> ({actor.Username}#{actor.Discriminator}) hochgeladen.";
                case Language.at:
                    return $"A **Datei** woad vo <@{actor.Id}> ({actor.Username}#{actor.Discriminator}) hochglodn.";
            }
            return $"A **file** has been uploaded by <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).";
        }
        public string NotificationModcaseFileDelete(DSharpPlus.Entities.DiscordUser actor) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"A **file** has been deleted by <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).";
                case Language.de:
                    return $"Eine **Datei** wurde von <@{actor.Id}> ({actor.Username}#{actor.Discriminator}) gelöscht.";
                case Language.at:
                    return $"A **Datei** is vo <@{actor.Id}> ({actor.Username}#{actor.Discriminator}) glescht woan.";
            }
            return $"A **file** has been deleted by <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).";
        }
        public string NotificationModcaseFileUpdate(DSharpPlus.Entities.DiscordUser actor) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"A **file** has been updated by <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).";
                case Language.de:
                    return $"Eine **Datei** wurde von <@{actor.Id}> ({actor.Username}#{actor.Discriminator}) aktualisiert.";
                case Language.at:
                    return $"A **Datei** is vo <@{actor.Id}> ({actor.Username}#{actor.Discriminator}) aktualisiert woan.";
            }
            return $"A **file** has been updated by <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).";
        }
        public string NotificationModcaseDMWarn(masz.Models.ModCase modCase, DSharpPlus.Entities.DiscordGuild guild, string serviceBaseUrl) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"The moderators of guild `{guild.Name}` have warned you.\nFor more information or rehabilitation visit: {serviceBaseUrl}";
                case Language.de:
                    return $"Die Moderatoren von `{guild.Name}` haben dich verwarnt.\nFür weitere Informationen besuche: {serviceBaseUrl}";
                case Language.at:
                    return $"Die Moderatoan vo `{guild.Name}` hom di verwoarnt.\nFia weitere Infos schau bei {serviceBaseUrl} noch.";
            }
            return $"The moderators of guild `{guild.Name}` have warned you.\nFor more information or rehabilitation visit: {serviceBaseUrl}";
        }
        public string NotificationModcaseDMMuteTemp(masz.Models.ModCase modCase, DSharpPlus.Entities.DiscordGuild guild, string serviceBaseUrl) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"The moderators of guild `{guild.Name}` have temporarily muted you until {modCase.PunishedUntil.Value.ToDiscordTS()}.\nFor more information or rehabilitation visit: {serviceBaseUrl}";
                case Language.de:
                    return $"Die Moderatoren von `{guild.Name}` haben dich temporär stummgeschalten bis {modCase.PunishedUntil.Value.ToDiscordTS()}.\nFür weitere Informationen besuche: {serviceBaseUrl}";
                case Language.at:
                    return $"Die Moderatoan vo `{guild.Name}` hom di bis am {modCase.PunishedUntil.Value.ToDiscordTS()} stummgschoit.\nFia weitere Infos schau bei {serviceBaseUrl} noch";
            }
            return $"The moderators of guild `{guild.Name}` have temporarily muted you until {modCase.PunishedUntil.Value.ToDiscordTS()}.\nFor more information or rehabilitation visit: {serviceBaseUrl}";
        }
        public string NotificationModcaseDMMutePerm(masz.Models.ModCase modCase, DSharpPlus.Entities.DiscordGuild guild, string serviceBaseUrl) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"The moderators of guild `{guild.Name}` have muted you.\nFor more information or rehabilitation visit: {serviceBaseUrl}";
                case Language.de:
                    return $"Die Moderatoren von `{guild.Name}` haben dich stummgeschalten.\nFür weitere Informationen besuche: {serviceBaseUrl}";
                case Language.at:
                    return $"Die Moderatoan vo `{guild.Name}` hom di stummgschoit.\nFia weitere Infos schau bei {serviceBaseUrl} noch.";
            }
            return $"The moderators of guild `{guild.Name}` have muted you.\nFor more information or rehabilitation visit: {serviceBaseUrl}";
        }
        public string NotificationModcaseDMBanTemp(masz.Models.ModCase modCase, DSharpPlus.Entities.DiscordGuild guild, string serviceBaseUrl) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"The moderators of guild `{guild.Name}` have temporarily banned you until {modCase.PunishedUntil.Value.ToDiscordTS()}.\nFor more information or rehabilitation visit: {serviceBaseUrl}";
                case Language.de:
                    return $"Die Moderatoren von `{guild.Name}` haben dich temporär gebannt bis {modCase.PunishedUntil.Value.ToDiscordTS()}.\nFür weitere Informationen besuche: {serviceBaseUrl}";
                case Language.at:
                    return $"Die Moderatoan vo `{guild.Name}` hom di bis am {modCase.PunishedUntil.Value.ToDiscordTS()} vom Serva ausgsperrt.\nFia weitere Infos schau bei {serviceBaseUrl} noch.";
            }
            return $"The moderators of guild `{guild.Name}` have temporarily banned you until {modCase.PunishedUntil.Value.ToDiscordTS()}.\nFor more information or rehabilitation visit: {serviceBaseUrl}";
        }
        public string NotificationModcaseDMBanPerm(masz.Models.ModCase modCase, DSharpPlus.Entities.DiscordGuild guild, string serviceBaseUrl) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"The moderators of guild `{guild.Name}` have banned you.\nFor more information or rehabilitation visit: {serviceBaseUrl}";
                case Language.de:
                    return $"Die Moderatoren von `{guild.Name}` haben dich gebannt.\nFür weitere Informationen besuche: {serviceBaseUrl}";
                case Language.at:
                    return $"Die Moderatoan vo `{guild.Name}` hom di vom Serva ausgsperrt.\nFia weitere Infos schau bei {serviceBaseUrl} noch";
            }
            return $"The moderators of guild `{guild.Name}` have banned you.\nFor more information or rehabilitation visit: {serviceBaseUrl}";
        }
        public string NotificationModcaseDMKick(masz.Models.ModCase modCase, DSharpPlus.Entities.DiscordGuild guild, string serviceBaseUrl) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"The moderators of guild `{guild.Name}` have kicked you.\nFor more information or rehabilitation visit: {serviceBaseUrl}";
                case Language.de:
                    return $"Die Moderatoren von `{guild.Name}` haben dich kickt.\nFür weitere Informationen besuche: {serviceBaseUrl}";
                case Language.at:
                    return $"Die Moderatoan vo `{guild.Name}` hom di rausgschmissn.\nFia weitere Infos schau bei {serviceBaseUrl} noch.";
            }
            return $"The moderators of guild `{guild.Name}` have kicked you.\nFor more information or rehabilitation visit: {serviceBaseUrl}";
        }
        public string NotificationFilesCreate() {
            switch (preferredLanguage) {
                case Language.en:
                    return "File uploaded";
                case Language.de:
                    return "Datei hochgeladen";
                case Language.at:
                    return "Datei hochglodn";
            }
            return "File uploaded";
        }
        public string NotificationFilesDelete() {
            switch (preferredLanguage) {
                case Language.en:
                    return "File deleted";
                case Language.de:
                    return "Datei gelöscht";
                case Language.at:
                    return "Datei glescht";
            }
            return "File deleted";
        }
        public string NotificationFilesUpdate() {
            switch (preferredLanguage) {
                case Language.en:
                    return "File updated";
                case Language.de:
                    return "Datei aktualisiert";
                case Language.at:
                    return "Datei aktualisiert";
            }
            return "File updated";
        }
        public string NotificationRegisterWelcomeToMASZ() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Welcome to MASZ!";
                case Language.de:
                    return "Willkommen bei MASZ!";
                case Language.at:
                    return "Servus bei MASZ!";
            }
            return "Welcome to MASZ!";
        }
        public string NotificationRegisterDescriptionThanks() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Thanks for registering your guild.\nIn the following you will learn some useful tips for setting up and using **MASZ**.";
                case Language.de:
                    return "Vielen Dank für deine Registrierung.\nIm Folgenden wirst du einige nützliche Tipps zum Einrichten und Verwenden von **MASZ** erhalten.";
                case Language.at:
                    return "Donksche fia dei Registrierung.\nDu siachst glei ei poar nützliche Tipps zum Eirichtn und Vawendn vo **MASZ**.";
            }
            return "Thanks for registering your guild.\nIn the following you will learn some useful tips for setting up and using **MASZ**.";
        }
        public string NotificationRegisterUseFeaturesCommand() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Use the `/features` command to test if your current guild setup supports all features of **MASZ**.";
                case Language.de:
                    return "Benutze den `/features` Befehl um zu sehen welche Features von **MASZ** dein aktuelles Setup unterstützt.";
                case Language.at:
                    return "Nutz den `/features` Beföhl um nochzumschauen wöchane Features dei aktuelles **MASZ**  Setup untastützn tuad.";
            }
            return "Use the `/features` command to test if your current guild setup supports all features of **MASZ**.";
        }
        public string NotificationRegisterDefaultLanguageUsed(string language) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"MASZ will use `{language}` as default language for this guild whenever possible.";
                case Language.de:
                    return $"MASZ wird `{language}` als Standard-Sprache für diese Gilde verwenden, wenn möglich.";
                case Language.at:
                    return $"Dei MASZ wiad `{language}` ois Standard-Sproch fia die Güde nehma, wenns geht.";
            }
            return $"MASZ will use `{language}` as default language for this guild whenever possible.";
        }
        public string NotificationRegisterConfusingTimestamps() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Timezones can be confusing.\nMASZ uses a Discord feature to display timestamps in the local timezone of your computer/phone.";
                case Language.de:
                    return "Zeitzonen können kompliziert sein.\nMASZ benutzt ein Discord-Feature um Zeitstempel in der lokalen Zeitzone deines Computers/Handys anzuzeigen.";
                case Language.at:
                    return "De Zeitzonen kennan a weng schwer san.\nMASZ nutzt a Discord-Feature um Zeitstempl in da lokalen Zeitzon vo deim PC/Handy ozumzeign.";
            }
            return "Timezones can be confusing.\nMASZ uses a Discord feature to display timestamps in the local timezone of your computer/phone.";
        }
        public string NotificationRegisterSupport() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Please refer to the [MASZ Support Server](https://discord.gg/5zjpzw6h3S) for further questions.";
                case Language.de:
                    return "Bitte wende dich an den [MASZ Support Server](https://discord.gg/5zjpzw6h3S) für weitere Fragen.";
                case Language.at:
                    return "Bitte wend di on den [MASZ Support Server](https://discord.gg/5zjpzw6h3S) fia weitare Frogn.";
            }
            return "Please refer to the [MASZ Support Server](https://discord.gg/5zjpzw6h3S) for further questions.";
        }
        public string NotificationAutomoderationInternal(DSharpPlus.Entities.DiscordUser user) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"{user.Mention} triggered automoderation.";
                case Language.de:
                    return $"{user.Mention} hat die Automoderation ausgelöst.";
                case Language.at:
                    return $"{user.Mention} hot de Automodaration ausglest.";
            }
            return $"{user.Mention} triggered automoderation.";
        }
        public string NotificationAutomoderationCase(DSharpPlus.Entities.DiscordUser user) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"{user.Username}#{user.Discriminator} triggered automoderation.";
                case Language.de:
                    return $"{user.Username}#{user.Discriminator} hat die Automoderation ausgelöst.";
                case Language.at:
                    return $"{user.Username}#{user.Discriminator} hot de Automodaration ausglest.";
            }
            return $"{user.Username}#{user.Discriminator} triggered automoderation.";
        }
        public string NotificationAutomoderationDM(DSharpPlus.Entities.DiscordUser user, DSharpPlus.Entities.DiscordChannel channel, string reason, string action) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"Hi {user.Mention},\n\nYou triggered automoderation in {channel.Mention}.\nReason: {reason}\nAction: {action}";
                case Language.de:
                    return $"Hallo {user.Mention},\n\nDu hast die Automoderation in {channel.Mention} ausgelöst.\nGrund: {reason}\nAktion: {action}";
                case Language.at:
                    return $"Servus {user.Mention},\n\nDu host de Automodaration in {channel.Mention} ausglest. Grund: {reason}\nAktion: {action}";
            }
            return $"Hi {user.Mention},\n\nYou triggered automoderation in {channel.Mention}.\nReason: {reason}\nAction: {action}";
        }
        public string NotificationAutomoderationChannel(DSharpPlus.Entities.DiscordUser user, string reason) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"{user.Mention} you triggered automoderation. Reason: {reason}. Your message has been deleted.";
                case Language.de:
                    return $"{user.Mention} du hast die Automoderation ausgelöst. Grund: {reason}. Dein Nachricht wurde gelöscht.";
                case Language.at:
                    return $"{user.Mention} du host de Automodaration ausglest. Grund: {reason}. Dei Nochricht wuad glescht.";
            }
            return $"{user.Mention} you triggered automoderation. Reason: {reason}. Your message has been deleted.";
        }
        public string NotificationAutoWhoisJoinWith(DSharpPlus.Entities.DiscordUser user, DateTime registered, string invite) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"{user.Mention} (registered {registered.ToDiscordTS()}) joined with invite `{invite}`.";
                case Language.de:
                    return $"{user.Mention} (registriert {registered.ToDiscordTS()}) ist mit dem Invite `{invite}` beigetreten.";
            }
            return $"{user.Mention} (registered {registered.ToDiscordTS()}) joined with invite `{invite}`.";
        }
        public string NotificationAutoWhoisJoinWithAndFrom(DSharpPlus.Entities.DiscordUser user, ulong by, DateTime createdAt, DateTime registered, string invite) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"{user.Mention} (registered {registered.ToDiscordTS()}) joined with invite `{invite}` (created {createdAt.ToDiscordTS()}) by <@{by}>.";
                case Language.de:
                    return $"{user.Mention} (registriert {registered.ToDiscordTS()}) ist mit dem Invite `{invite}` von <@{by}> (am {createdAt.ToDiscordTS()}) beigetreten.";
            }
            return $"{user.Mention} (registered {registered.ToDiscordTS()}) joined with invite `{invite}` (created {createdAt.ToDiscordTS()}) by <@{by}>.";
        }
        public string CmdOnlyTextChannel() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Only text channels are allowed.";
                case Language.de:
                    return "Nur Textkanäle sind erlaubt.";
                case Language.at:
                    return "Nua Textkanö san guat.";
            }
            return "Only text channels are allowed.";
        }
        public string CmdCannotViewOrDeleteInChannel() {
            switch (preferredLanguage) {
                case Language.en:
                    return "I'm not allowed to view or delete messages in this channel!";
                case Language.de:
                    return "Ich darf keine Nachrichten in diesem Kanal sehen oder löschen!";
                case Language.at:
                    return "I derf kane Nochrichtn in dem Kanoi sehn oda leschn!";
            }
            return "I'm not allowed to view or delete messages in this channel!";
        }
        public string CmdCannotFindChannel() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Cannot find channel.";
                case Language.de:
                    return "Kanal konnte nicht gefunden werden.";
                case Language.at:
                    return "Kanoi konnt ned gfundn wan.";
            }
            return "Cannot find channel.";
        }
        public string CmdNoWebhookConfigured() {
            switch (preferredLanguage) {
                case Language.en:
                    return "This guild has no webhook for internal notifications configured.";
                case Language.de:
                    return "Dieser Server hat keinen internen Webhook für Benachrichtigungen konfiguriert.";
                case Language.at:
                    return "Da Serva hot kan internan Webhook fia Benochrichtigungen konfiguriat.";
            }
            return "This guild has no webhook for internal notifications configured.";
        }
        public string CmdCleanup(int count, DSharpPlus.Entities.DiscordChannel channel) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"Deleted {count} messages in {channel.Mention}.";
                case Language.de:
                    return $"{count} Nachrichten in {channel.Mention} gelöscht.";
                case Language.at:
                    return $"{count} Nochrichtn in {channel.Mention} glescht.";
            }
            return $"Deleted {count} messages in {channel.Mention}.";
        }
        public string CmdFeaturesKickPermissionGranted() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Kick permission granted.";
                case Language.de:
                    return "Kick-Berechtigung erteilt.";
                case Language.at:
                    return "Kick-Berechtigung erteit.";
            }
            return "Kick permission granted.";
        }
        public string CmdFeaturesKickPermissionNotGranted() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Kick permission not granted.";
                case Language.de:
                    return "Kick-Berechtigung nicht erteilt.";
                case Language.at:
                    return "Kick-Berechtigung ned erteit.";
            }
            return "Kick permission not granted.";
        }
        public string CmdFeaturesBanPermissionGranted() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Ban permission granted.";
                case Language.de:
                    return "Ban-Berechtigung erteilt.";
                case Language.at:
                    return "Ban-Berechtigung erteit.";
            }
            return "Ban permission granted.";
        }
        public string CmdFeaturesBanPermissionNotGranted() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Ban permission not granted.";
                case Language.de:
                    return "Ban-Berechtigung nicht erteilt.";
                case Language.at:
                    return "Ban-Berechtigung ned erteit.";
            }
            return "Ban permission not granted.";
        }
        public string CmdFeaturesManageRolePermissionGranted() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Manage role permission granted.";
                case Language.de:
                    return "Manage-Rolle-Berechtigung erteilt.";
                case Language.at:
                    return "Manage-Rolle-Berechtigung ereit.";
            }
            return "Manage role permission granted.";
        }
        public string CmdFeaturesManageRolePermissionNotGranted() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Manage role permission not granted.";
                case Language.de:
                    return "Manage-Rolle-Berechtigung nicht erteilt.";
                case Language.at:
                    return "Manage-Rolle-Berechtigung ned erteit.";
            }
            return "Manage role permission not granted.";
        }
        public string CmdFeaturesMutedRoleDefined() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Muted role defined.";
                case Language.de:
                    return "Stummrolle definiert.";
                case Language.at:
                    return "Stummroi definiat.";
            }
            return "Muted role defined.";
        }
        public string CmdFeaturesMutedRoleDefinedButTooHigh() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Muted role defined but too high in role hierarchy.";
                case Language.de:
                    return "Stummrolle definiert, aber zu hoch in der Rollenhierarchie.";
                case Language.at:
                    return "Stummroi definiat, oba zu hoch in da Roinhierarchie.";
            }
            return "Muted role defined but too high in role hierarchy.";
        }
        public string CmdFeaturesMutedRoleDefinedButInvalid() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Muted role defined but invalid.";
                case Language.de:
                    return "Stummrolle definiert, aber ungültig.";
                case Language.at:
                    return "Stummroi definiat, oba ned gütig.";
            }
            return "Muted role defined but invalid.";
        }
        public string CmdFeaturesMutedRoleUndefined() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Muted role undefined.";
                case Language.de:
                    return "Stummrolle nicht definiert.";
                case Language.at:
                    return "Stummroi ned definiat.";
            }
            return "Muted role undefined.";
        }
        public string CmdFeaturesPunishmentExecution() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Punishment execution";
                case Language.de:
                    return "Bestrafungsverwaltung";
                case Language.at:
                    return "Bestrofungsverwoitung";
            }
            return "Punishment execution";
        }
        public string CmdFeaturesPunishmentExecutionDescription() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Let MASZ handle punishments (e.g. tempbans, mutes, etc.).";
                case Language.de:
                    return "Lass MASZ die Bestrafungen verwalten (z.B. temporäre Banns, Stummschaltungen, etc.).";
                case Language.at:
                    return "Loss MASZ de Bestrofungen verwoitn (z.B. temporäre Banns, Stummschoitungen, etc.).";
            }
            return "Let MASZ handle punishments (e.g. tempbans, mutes, etc.).";
        }
        public string CmdFeaturesUnbanRequests() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Unban requests";
                case Language.de:
                    return "Entbannungs-Anfragen";
                case Language.at:
                    return "Entbannungs-Ofrogn";
            }
            return "Unban requests";
        }
        public string CmdFeaturesUnbanRequestsDescriptionGranted() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Allows banned members to see their cases and comment on it for unban requests.";
                case Language.de:
                    return "Erlaubt Gebannten MASZ aufzurufen, sich ihre Fälle anzusehen und diese sie zu kommentieren.";
                case Language.at:
                    return "Erlaubt ausgsperrtn MASZ aufzuruafa, sich ernane Fälle ozumschaun und de zum kommentian.";
            }
            return "Allows banned members to see their cases and comment on it for unban requests.";
        }
        public string CmdFeaturesUnbanRequestsDescriptionNotGranted() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Allows banned members to see their cases and comment on it for unban requests.\nGrant this bot the ban permission to use this feature.";
                case Language.de:
                    return "Erlaubt Gebannten MASZ aufzurufen, sich ihre Fälle anzusehen und diese sie zu kommentieren.\nErteile diesem Bot die Ban-Berechtigung, um diese Funktion zu nutzen.";
                case Language.at:
                    return "Erlaubt ausgsperrtn MASZ aufzurufa, sich ernane Fälle ozumschaun und de zum kommentian. \nErteil dem Bot die Ban-Berechtigung, um de Funktion nutza zu kenna.";
            }
            return "Allows banned members to see their cases and comment on it for unban requests.\nGrant this bot the ban permission to use this feature.";
        }
        public string CmdFeaturesReportCommand() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Report command";
                case Language.de:
                    return "Melde-Befehl";
                case Language.at:
                    return "Möde-Befehl";
            }
            return "Report command";
        }
        public string CmdFeaturesReportCommandDescriptionGranted() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Allows members to report messages.";
                case Language.de:
                    return "Erlaubt Mitgliedern, Nachrichten zu melden.";
                case Language.at:
                    return "Erlaubt Mitglieda, Nochrichtn zu mödn.";
            }
            return "Allows members to report messages.";
        }
        public string CmdFeaturesReportCommandDescriptionNotGranted() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Allows members to report messages.\nDefine a internal staff webhook to use this feature.";
                case Language.de:
                    return "Erlaubt Mitgliedern, Nachrichten zu melden.\nDefiniere einen internen Webhook, um diese Funktion zu nutzen.";
                case Language.at:
                    return "Erlaub Mitglieda, Nochrichtn zum mödn. \nDefinia an internen Webook, um de Funktion nutzn zum kennan.";
            }
            return "Allows members to report messages.\nDefine a internal staff webhook to use this feature.";
        }
        public string CmdFeaturesInviteTracking() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Invite tracking";
                case Language.de:
                    return "Einladungsverfolgung";
                case Language.at:
                    return "Eiladungsverfoigung";
            }
            return "Invite tracking";
        }
        public string CmdFeaturesInviteTrackingDescriptionGranted() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Allows MASZ to track the invites new members are using.";
                case Language.de:
                    return "Erlaubt MASZ, die Einladungen neuer Mitglieder zu verfolgen.";
                case Language.at:
                    return "Erlaubt MASZ, de Eiladungen vo neichn Mitglieda zu verfoign.";
            }
            return "Allows MASZ to track the invites new members are using.";
        }
        public string CmdFeaturesInviteTrackingDescriptionNotGranted() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Allows MASZ to track the invites new members are using.\nGrant this bot the manage guild permission to use this feature.";
                case Language.de:
                    return "Erlaubt MASZ, die Einladungen neuer Mitglieder zu verfolgen.\nErteile diesem Bot die Verwalten-Gilden-Berechtigung, um diese Funktion zu nutzen.";
                case Language.at:
                    return "Erlaubt MASZ, de Eiladungen vo neichn Mitglieda zu verfoign.\nErteil dem Bot die Verwoitn-Gilden-Berechtigung, um de Funktion nutzn zu kenna.";
            }
            return "Allows MASZ to track the invites new members are using.\nGrant this bot the manage guild permission to use this feature.";
        }
        public string CmdFeaturesSupportAllFeatures() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Your bot on this guild is configured correctly. All features of MASZ can be used.";
                case Language.de:
                    return "Dein Bot auf diesem Server ist richtig konfiguriert. Alle Funktionen von MASZ können genutzt werden.";
                case Language.at:
                    return "Dei Bot auf dem Serva is richtig konfiguriat. Olle Funktionen vo MASZ kennen gnutzt wean.";
            }
            return "Your bot on this guild is configured correctly. All features of MASZ can be used.";
        }
        public string CmdFeaturesMissingFeatures() {
            switch (preferredLanguage) {
                case Language.en:
                    return "There are features of MASZ that you cannot use right now.";
                case Language.de:
                    return "Es gibt Funktionen von MASZ, die du jetzt nicht nutzen kannst.";
                case Language.at:
                    return "Es gibt Funktionen vo MASZ, die du jetzt ned nutzn konnst.";
            }
            return "There are features of MASZ that you cannot use right now.";
        }
        public string CmdInvite() {
            switch (preferredLanguage) {
                case Language.en:
                    return "You will have to host your own instance of MASZ on your server or pc.\nCheckout https://github.com/zaanposni/discord-masz#hosting";
                case Language.de:
                    return "Du musst deine eigene Instanz von MASZ auf deinem Server oder PC hosten.\nSchau dir https://github.com/zaanposni/discord-masz#hosting an";
                case Language.at:
                    return "Du muast dei eignane Inszanz vo MASZ auf deim Serva oda PC hosn.\nSchau da https://github.com/zaanposni/discord-masz#hosting o";
            }
            return "You will have to host your own instance of MASZ on your server or pc.\nCheckout https://github.com/zaanposni/discord-masz#hosting";
        }
        public string CmdPunish(int caseId, string caseLink) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"Case `#{caseId}` created: {caseLink}";
                case Language.de:
                    return $"Fall `#{caseId}` erstellt: {caseLink}";
                case Language.at:
                    return $"Foi `#{caseId}` erstöt: {caseLink}";
            }
            return $"Case `#{caseId}` created: {caseLink}";
        }
        public string CmdRegister(string url) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"A siteadmin can register a guild at: {url}";
                case Language.de:
                    return $"Ein Siteadmin kann eine Gilde registrieren unter: {url}";
                case Language.at:
                    return $"A Seitnadmin ko a Güde unta {url} registrian.";
            }
            return $"A siteadmin can register a guild at: {url}";
        }
        public string CmdReportFailed() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Failed to send internal notification to moderators for report command.";
                case Language.de:
                    return "Interner Benachrichtigungsversand an Moderatoren für Meldebefehl fehlgeschlagen.";
                case Language.at:
                    return "Interna Benochrichtigungsvasond on de Modaratoan fian Mödebefehl fehlgschlogn.";
            }
            return "Failed to send internal notification to moderators for report command.";
        }
        public string CmdReportSent() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Report sent.";
                case Language.de:
                    return "Meldung gesendet.";
                case Language.at:
                    return "Mödung gsendt.";
            }
            return "Report sent.";
        }
        public string CmdReportContent(DSharpPlus.Entities.DiscordUser user, DSharpPlus.Entities.DiscordMessage message) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"{user.Mention} reported a message from {message.Author.Mention} in {message.Channel.Mention}.\n{message.JumpLink}";
                case Language.de:
                    return $"{user.Mention} meldete eine Nachricht von {message.Author.Mention} in {message.Channel.Mention}.\n{message.JumpLink}";
                case Language.at:
                    return $"{user.Mention} mödet a Nochricht vo {message.Author.Mention} in {message.Channel.Mention}.\n{message.JumpLink}";
            }
            return $"{user.Mention} reported a message from {message.Author.Mention} in {message.Channel.Mention}.\n{message.JumpLink}";
        }
        public string CmdSayFailed() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Failed to send message";
                case Language.de:
                    return "Senden der Nachricht fehlgeschlagen";
                case Language.at:
                    return "Sendn vo da Nachricht fehlgschlogn.";
            }
            return "Failed to send message";
        }
        public string CmdSaySent() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Message sent.";
                case Language.de:
                    return "Nachricht gesendet.";
                case Language.at:
                    return "Nochricht gsendet.";
            }
            return "Message sent.";
        }
        public string CmdTrackInviteNotFromThisGuild() {
            switch (preferredLanguage) {
                case Language.en:
                    return "The invite is not from this guild.";
                case Language.de:
                    return "Die Einladung ist nicht von dieser Gilde.";
                case Language.at:
                    return "Die Eiladung is ned vo dera Güde.";
            }
            return "The invite is not from this guild.";
        }
        public string CmdTrackCannotFindInvite() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Could not find invite in database or in this guild.";
                case Language.de:
                    return "Konnte die Einladung nicht in der Datenbank oder in dieser Gilde finden.";
                case Language.at:
                    return "Konnt de Eiladung ned in da Datnbank vo da Güde findn.";
            }
            return "Could not find invite in database or in this guild.";
        }
        public string CmdTrackFailedToFetchInvite() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Failed to fetch invite.";
                case Language.de:
                    return "Konnte die Einladung nicht abrufen.";
                case Language.at:
                    return "Konnt die Eiladung ned orufn.";
            }
            return "Failed to fetch invite.";
        }
        public string CmdTrackCreatedAt(string inviteCode, DateTime createdAt) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"`{inviteCode}` created at {createdAt.ToDiscordTS()}.";
                case Language.de:
                    return $"`{inviteCode}` erstellt am {createdAt.ToDiscordTS()}.";
                case Language.at:
                    return $"`{inviteCode}` erstöt vo {createdAt.ToDiscordTS()}.";
            }
            return $"`{inviteCode}` created at {createdAt.ToDiscordTS()}.";
        }
        public string CmdTrackCreatedBy(string inviteCode, DSharpPlus.Entities.DiscordUser createdBy) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"`{inviteCode}` created by {createdBy.Mention}.";
                case Language.de:
                    return $"`{inviteCode}` erstellt von {createdBy.Mention}.";
                case Language.at:
                    return $"`{inviteCode}` erstöt vo {createdBy.Mention}.";
            }
            return $"`{inviteCode}` created by {createdBy.Mention}.";
        }
        public string CmdTrackCreatedByAt(string inviteCode, DSharpPlus.Entities.DiscordUser createdBy, DateTime createdAt) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"`{inviteCode}` created by {createdBy.Mention} at {createdAt.ToDiscordTS()}.";
                case Language.de:
                    return $"`{inviteCode}` erstellt von {createdBy.Mention} am {createdAt.ToDiscordTS()}.";
                case Language.at:
                    return $"`{inviteCode}` erstöt vo {createdBy.Mention} om {createdAt.ToDiscordTS()}.";
            }
            return $"`{inviteCode}` created by {createdBy.Mention} at {createdAt.ToDiscordTS()}.";
        }
        public string CmdTrackNotTrackedYet() {
            switch (preferredLanguage) {
                case Language.en:
                    return "This invite has not been tracked by MASZ yet.";
                case Language.de:
                    return "Diese Einladung wurde noch nicht von MASZ gespeichert.";
                case Language.at:
                    return "Die Eiladung wuad no ned vo MASZ gspeichat.";
            }
            return "This invite has not been tracked by MASZ yet.";
        }
        public string CmdTrackUsedBy(int count) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"Used by [{count}]";
                case Language.de:
                    return $"Benutzt von [{count}]";
                case Language.at:
                    return $"Benutzt vo [{count}]";
            }
            return $"Used by [{count}]";
        }
        public string CmdViewInvalidGuildId() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Please specify a valid guildid.";
                case Language.de:
                    return "Bitte gib eine gültige Gilden-ID an.";
                case Language.at:
                    return "Bitte gib a gütige Güdn-ID o.";
            }
            return "Please specify a valid guildid.";
        }
        public string CmdViewNotAllowedToView() {
            switch (preferredLanguage) {
                case Language.en:
                    return "You are not allowed to view this case.";
                case Language.de:
                    return "Du darfst diesen Fall nicht ansehen.";
                case Language.at:
                    return "Du derfst da den Foi ned oschaun.";
            }
            return "You are not allowed to view this case.";
        }
        public string CmdWhoisUsedInvite(string inviteCode) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"Used invite `{inviteCode}`.";
                case Language.de:
                    return $"Benutzte Einladung `{inviteCode}`.";
                case Language.at:
                    return $"Benutze Eilodung `{inviteCode}`.";
            }
            return $"Used invite `{inviteCode}`.";
        }
        public string CmdWhoisInviteBy(ulong user) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"By <@{user}>.";
                case Language.de:
                    return $"Von <@{user}>.";
                case Language.at:
                    return $"Vo <@{user}>";
            }
            return $"By <@{user}>.";
        }
        public string CmdWhoisNoCases() {
            switch (preferredLanguage) {
                case Language.en:
                    return "There are no cases for this user.";
                case Language.de:
                    return "Es gibt keine Fälle für diesen Benutzer.";
                case Language.at:
                    return "Es gibt kane Fälle fia diesn Benutza.";
            }
            return "There are no cases for this user.";
        }
        public string Enum(masz.Enums.PunishmentType enumValue) {
            switch (enumValue) {
                case masz.Enums.PunishmentType.Mute:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Mute";
                        case Language.de:
                            return "Stummschaltung";
                        case Language.at:
                            return "Stummschoitung";
                        default:
                            return "Mute";
                    }
                case masz.Enums.PunishmentType.Ban:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Ban";
                        case Language.de:
                            return "Sperrung";
                        case Language.at:
                            return "Rauswuaf";
                        default:
                            return "Ban";
                    }
                case masz.Enums.PunishmentType.Kick:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Kick";
                        case Language.de:
                            return "Kick";
                        case Language.at:
                            return "Tritt";
                        default:
                            return "Kick";
                    }
                case masz.Enums.PunishmentType.None:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Warn";
                        case Language.de:
                            return "Verwarnung";
                        case Language.at:
                            return "Verwoarnt";
                        default:
                            return "Warn";
                    }
            }
            return "Unknown";
        }
        public string Enum(masz.Enums.ViewPermission enumValue) {
            switch (enumValue) {
                case masz.Enums.ViewPermission.Self:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Self";
                        case Language.de:
                            return "Privat";
                        case Language.at:
                            return "Privot";
                        default:
                            return "Self";
                    }
                case masz.Enums.ViewPermission.Guild:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Guild";
                        case Language.de:
                            return "Gilde";
                        case Language.at:
                            return "Güde";
                        default:
                            return "Guild";
                    }
                case masz.Enums.ViewPermission.Global:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Global";
                        case Language.de:
                            return "Global";
                        case Language.at:
                            return "Globoi";
                        default:
                            return "Global";
                    }
            }
            return "Unknown";
        }
        public string Enum(masz.Enums.AutoModerationAction enumValue) {
            switch (enumValue) {
                case masz.Enums.AutoModerationAction.None:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "No action";
                        case Language.de:
                            return "Keine Aktion";
                        case Language.at:
                            return "Nix tuan";
                        default:
                            return "No action";
                    }
                case masz.Enums.AutoModerationAction.ContentDeleted:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Content deleted";
                        case Language.de:
                            return "Nachricht gelöscht";
                        case Language.at:
                            return "Nochricht glescht";
                        default:
                            return "Content deleted";
                    }
                case masz.Enums.AutoModerationAction.CaseCreated:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Case created";
                        case Language.de:
                            return "Vorfall erstellt";
                        case Language.at:
                            return "Vorfoi erstöt";
                        default:
                            return "Case created";
                    }
                case masz.Enums.AutoModerationAction.ContentDeletedAndCaseCreated:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Content deleted and case created";
                        case Language.de:
                            return "Nachricht gelöscht und Vorfall erstellt";
                        case Language.at:
                            return "Nochricht glescht und Vorfoi erstöt";
                        default:
                            return "Content deleted and case created";
                    }
            }
            return "Unknown";
        }
        public string Enum(masz.Enums.AutoModerationType enumValue) {
            switch (enumValue) {
                case masz.Enums.AutoModerationType.InvitePosted:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Invite posted";
                        case Language.de:
                            return "Einladung gesendet";
                        case Language.at:
                            return "Eiladung gsendet";
                        default:
                            return "Invite posted";
                    }
                case masz.Enums.AutoModerationType.TooManyEmotes:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Too many emotes used";
                        case Language.de:
                            return "Zu viele Emojis verwendet";
                        case Language.at:
                            return "Zu vü Emojis san vawendt woan";
                        default:
                            return "Too many emotes used";
                    }
                case masz.Enums.AutoModerationType.TooManyMentions:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Too many users mentioned";
                        case Language.de:
                            return "Zu viele Benutzer erwähnt";
                        case Language.at:
                            return "Zu vü Nutza san erwähnt woan";
                        default:
                            return "Too many users mentioned";
                    }
                case masz.Enums.AutoModerationType.TooManyAttachments:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Too many attachments used";
                        case Language.de:
                            return "Zu viele Anhänge verwendet";
                        case Language.at:
                            return "Zu vü Ohäng san verwendt woan";
                        default:
                            return "Too many attachments used";
                    }
                case masz.Enums.AutoModerationType.TooManyEmbeds:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Too many embeds used";
                        case Language.de:
                            return "Zu viele Einbettungen verwendet";
                        case Language.at:
                            return "Zu vü Eibettungen san vawendt woan";
                        default:
                            return "Too many embeds used";
                    }
                case masz.Enums.AutoModerationType.TooManyAutoModerations:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Too many auto-moderations";
                        case Language.de:
                            return "Zu viele automatische Moderationen";
                        case Language.at:
                            return "Zu vü automatische Modarationen";
                        default:
                            return "Too many auto-moderations";
                    }
                case masz.Enums.AutoModerationType.CustomWordFilter:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Custom wordfilter triggered";
                        case Language.de:
                            return "Benutzerdefinierter Wortfilter ausgelöst";
                        case Language.at:
                            return "Eigena Wortfüta is ausglest woan";
                        default:
                            return "Custom wordfilter triggered";
                    }
                case masz.Enums.AutoModerationType.TooManyMessages:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Too many messages";
                        case Language.de:
                            return "Zu viele Nachrichten";
                        case Language.at:
                            return "Zu vü Nochrichtn";
                        default:
                            return "Too many messages";
                    }
            }
            return "Unknown";
        }
        public string Enum(masz.Enums.APIError enumValue) {
            switch (enumValue) {
                case masz.Enums.APIError.Unknown:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Unknown error";
                        case Language.de:
                            return "Unbekannter Fehler";
                        case Language.at:
                            return "Unbekonnta Föhla";
                        default:
                            return "Unknown error";
                    }
                case masz.Enums.APIError.InvalidDiscordUser:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Invalid discord user";
                        case Language.de:
                            return "Ungültiger Discordbenutzer";
                        case Language.at:
                            return "Ungütiga Discordbenutza";
                        default:
                            return "Invalid discord user";
                    }
                case masz.Enums.APIError.ProtectedModCaseSuspect:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "User is protected";
                        case Language.de:
                            return "Benutzer ist geschützt";
                        case Language.at:
                            return "Benutza is gschützt";
                        default:
                            return "User is protected";
                    }
                case masz.Enums.APIError.ProtectedModCaseSuspectIsBot:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "User is protected. He is a bot.";
                        case Language.de:
                            return "Benutzer ist geschützt. Er ist ein Bot.";
                        case Language.at:
                            return "Benutza is gschützt, es is a Bot.";
                        default:
                            return "User is protected. He is a bot.";
                    }
                case masz.Enums.APIError.ProtectedModCaseSuspectIsSiteAdmin:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "User is protected. He is a site admin.";
                        case Language.de:
                            return "Benutzer ist geschützt. Er ist ein Seitenadministrator.";
                        case Language.at:
                            return "Benutza is gschützt, er is a Seitenadministraotr.";
                        default:
                            return "User is protected. He is a site admin.";
                    }
                case masz.Enums.APIError.ProtectedModCaseSuspectIsTeam:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "User is protected. He is a team member.";
                        case Language.de:
                            return "Benutzer ist geschützt. Er ist ein Teammitglied.";
                        case Language.at:
                            return "Benutza is gschützt, er is a Teammitglied.";
                        default:
                            return "User is protected. He is a team member.";
                    }
                case masz.Enums.APIError.ResourceNotFound:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Resource not found";
                        case Language.de:
                            return "Ressource nicht gefunden";
                        case Language.at:
                            return "Ressource ned gfundn.";
                        default:
                            return "Resource not found";
                    }
                case masz.Enums.APIError.InvalidIdentity:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Invalid identity";
                        case Language.de:
                            return "Ungültige Identität";
                        case Language.at:
                            return "Ungültige Identität";
                        default:
                            return "Invalid identity";
                    }
                case masz.Enums.APIError.GuildUnregistered:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Guild is not registered";
                        case Language.de:
                            return "Gilde ist nicht registriert";
                        case Language.at:
                            return "Güde is ned registriat";
                        default:
                            return "Guild is not registered";
                    }
                case masz.Enums.APIError.Unauthorized:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Unauthorized";
                        case Language.de:
                            return "Nicht berechtigt";
                        case Language.at:
                            return "Ned berechtigt";
                        default:
                            return "Unauthorized";
                    }
                case masz.Enums.APIError.GuildUndefinedMutedRoles:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Guild has no roles for mute punishment defined.";
                        case Language.de:
                            return "Gilde hat keine Rollen für Stummschaltungen definiert.";
                        case Language.at:
                            return "Güde hot kane Roin fia de Stummschoitung definiat.";
                        default:
                            return "Guild has no roles for mute punishment defined.";
                    }
                case masz.Enums.APIError.ModCaseIsMarkedToBeDeleted:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Modcase is marked to be deleted";
                        case Language.de:
                            return "Modcase ist zum Löschen markiert";
                        case Language.at:
                            return "Modcase is zum Löscha markiat";
                        default:
                            return "Modcase is marked to be deleted";
                    }
                case masz.Enums.APIError.ModCaseIsNotMarkedToBeDeleted:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Modcase is not marked to be deleted";
                        case Language.de:
                            return "Modcase ist nicht zum Löschen markiert";
                        case Language.at:
                            return "Modcase is ned zum Lösche markiat";
                        default:
                            return "Modcase is not marked to be deleted";
                    }
                case masz.Enums.APIError.GuildAlreadyRegistered:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Guild is already registered";
                        case Language.de:
                            return "Gilde ist bereits registriert";
                        case Language.at:
                            return "Güde is bereits registriat";
                        default:
                            return "Guild is already registered";
                    }
                case masz.Enums.APIError.NotAllowedInDemoMode:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "This action is not allowed in demo mode";
                        case Language.de:
                            return "Diese Aktion ist in der Demo-Version nicht erlaubt";
                        case Language.at:
                            return "De Aktion is in da Demo-Version ned erlaubt";
                        default:
                            return "This action is not allowed in demo mode";
                    }
                case masz.Enums.APIError.RoleNotFound:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Role not found";
                        case Language.de:
                            return "Rolle nicht gefunden";
                        case Language.at:
                            return "Rolle ned gfundn";
                        default:
                            return "Role not found";
                    }
                case masz.Enums.APIError.TokenCannotManageThisResource:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Tokens cannot manage this resource";
                        case Language.de:
                            return "Tokens können diese Ressource nicht verwalten";
                        case Language.at:
                            return "Tokns kennan de Ressourcen ned vawoitn";
                        default:
                            return "Tokens cannot manage this resource";
                    }
                case masz.Enums.APIError.TokenAlreadyRegistered:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Token is already registered";
                        case Language.de:
                            return "Token ist bereits registriert";
                        case Language.at:
                            return "Tokn is bereits registriat";
                        default:
                            return "Token is already registered";
                    }
                case masz.Enums.APIError.CannotBeSameUser:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Both users are the same.";
                        case Language.de:
                            return "Beide Benutzer sind gleich.";
                        case Language.at:
                            return "Beide Benutza san gleich.";
                        default:
                            return "Both users are the same.";
                    }
                case masz.Enums.APIError.ResourceAlreadyExists:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Resource already exists";
                        case Language.de:
                            return "Ressource existiert bereits";
                        case Language.at:
                            return "De Ressource gibts bereits";
                        default:
                            return "Resource already exists";
                    }
                case masz.Enums.APIError.ModCaseDoesNotAllowComments:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Comments are locked for this modcase";
                        case Language.de:
                            return "Kommentare sind für diesen Vorfall gesperrt";
                        case Language.at:
                            return "Kommentare san fia den Vorfoi gsperrt";
                        default:
                            return "Comments are locked for this modcase";
                    }
                case masz.Enums.APIError.LastCommentAlreadyFromSuspect:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "The last comment was already from the suspect.";
                        case Language.de:
                            return "Der letzte Kommentar war schon von dem Beschuldigten.";
                        case Language.at:
                            return "Da letzte Kommentar woa scho vom Beschuldigten.";
                        default:
                            return "The last comment was already from the suspect.";
                    }
                case masz.Enums.APIError.InvalidAutomoderationAction:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Invalid automoderation action";
                        case Language.de:
                            return "Ungültige automoderationsaktion";
                        case Language.at:
                            return "Ned gütige automodarationsaktion";
                        default:
                            return "Invalid automoderation action";
                    }
                case masz.Enums.APIError.InvalidAutomoderationType:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Invalid automoderation type";
                        case Language.de:
                            return "Ungültiger automoderationstyp";
                        case Language.at:
                            return "Ungütiga automodarationstyp";
                        default:
                            return "Invalid automoderation type";
                    }
                case masz.Enums.APIError.TooManyTemplates:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "User has reached the max limit of templates";
                        case Language.de:
                            return "Benutzer hat die maximale Anzahl an Templates erreicht";
                        case Language.at:
                            return "Benutza hod de maximale Onzoi vo de Templates erreicht";
                        default:
                            return "User has reached the max limit of templates";
                    }
                case masz.Enums.APIError.InvalidFilePath:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Invalid file path";
                        case Language.de:
                            return "Ungültiger Dateipfad";
                        case Language.at:
                            return "Ungütiga Dateipfad";
                        default:
                            return "Invalid file path";
                    }
                case masz.Enums.APIError.NoGuildsRegistered:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "There are no guilds registered";
                        case Language.de:
                            return "Es sind keine Gilden registriert";
                        case Language.at:
                            return "Es san kane Güdn registriat";
                        default:
                            return "There are no guilds registered";
                    }
                case masz.Enums.APIError.OnlyUsableInAGuild:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "This action is only usable in a guild";
                        case Language.de:
                            return "Diese Aktion ist nur in einer Gilde nutzbar";
                        case Language.at:
                            return "De Aktion is nua in ana Güdn nutzboa";
                        default:
                            return "This action is only usable in a guild";
                    }
            }
            return "Unknown";
        }
        public string Enum(masz.Enums.CaseCreationType enumValue) {
            switch (enumValue) {
                case masz.Enums.CaseCreationType.Default:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Default";
                        case Language.de:
                            return "Default";
                        case Language.at:
                            return "Default";
                        default:
                            return "Default";
                    }
                case masz.Enums.CaseCreationType.AutoModeration:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Case is automoderated.";
                        case Language.de:
                            return "Automoderiert.";
                        case Language.at:
                            return "Automodariat.";
                        default:
                            return "Case is automoderated.";
                    }
                case masz.Enums.CaseCreationType.Imported:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Case is imported.";
                        case Language.de:
                            return "Importiert.";
                        case Language.at:
                            return "Importiat";
                        default:
                            return "Case is imported.";
                    }
                case masz.Enums.CaseCreationType.ByCommand:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Case created by command.";
                        case Language.de:
                            return "Durch Befehl erstellt.";
                        case Language.at:
                            return "Durch an Beföh erstöt.";
                        default:
                            return "Case created by command.";
                    }
            }
            return "Unknown";
        }
        public string Enum(masz.Enums.Language enumValue) {
            switch (enumValue) {
                case masz.Enums.Language.en:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "English";
                        case Language.de:
                            return "Englisch";
                        case Language.at:
                            return "Englisch";
                        default:
                            return "English";
                    }
                case masz.Enums.Language.de:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "German";
                        case Language.de:
                            return "Deutsch";
                        case Language.at:
                            return "Piefchinesisch";
                        default:
                            return "German";
                    }
                case masz.Enums.Language.fr:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "French";
                        case Language.de:
                            return "Französisch";
                        case Language.at:
                            return "Franzesisch";
                        default:
                            return "French";
                    }
                case masz.Enums.Language.es:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Spanish";
                        case Language.de:
                            return "Spanisch";
                        case Language.at:
                            return "Spanisch";
                        default:
                            return "Spanish";
                    }
                case masz.Enums.Language.it:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Italian";
                        case Language.de:
                            return "Italienisch";
                        case Language.at:
                            return "Italienisch";
                        default:
                            return "Italian";
                    }
                case masz.Enums.Language.at:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Austrian";
                        case Language.de:
                            return "Österreich";
                        case Language.at:
                            return "Esterreichisch";
                        default:
                            return "Austrian";
                    }
            }
            return "Unknown";
        }

    }
}

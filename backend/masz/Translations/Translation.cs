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
            }
            return $"Hi {user.Mention},\n\nYou triggered automoderation in {channel.Mention}.\nReason: {reason}\nAction: {action}";
        }
        public string NotificationAutomoderationChannel(DSharpPlus.Entities.DiscordUser user, string reason) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"{user.Mention} you triggered automoderation. Reason: {reason}. Your message has been deleted.";
                case Language.de:
                    return $"{user.Mention} du hast die Automoderation ausgelöst. Grund: {reason}. Dein Nachricht wurde gelöscht.";
            }
            return $"{user.Mention} you triggered automoderation. Reason: {reason}. Your message has been deleted.";
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
                            return "None";
                        case Language.de:
                            return "Keine Aktion";
                        case Language.at:
                            return "Nix tuan";
                        default:
                            return "None";
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

    }
}

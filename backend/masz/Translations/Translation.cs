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
        public string Punishment() {
            switch (this.preferredLanguage) {
                case Language.en:
                    return "Punishment";
                case Language.de:
                    return "Bestrafung";
            }
            return "Punishment";
        }
        public string PunishmentUntil(string timezone) {
            switch (this.preferredLanguage) {
                case Language.en:
                    return $"Punished until ({timezone})";
                case Language.de:
                    return $"Bestrafung bis ({timezone})";
            }
            return $"Punished until ({timezone})";
        }
        public string Description() {
            switch (this.preferredLanguage) {
                case Language.en:
                    return "Description";
                case Language.de:
                    return "Beschreibung";
            }
            return "Description";
        }
        public string Labels() {
            switch (this.preferredLanguage) {
                case Language.en:
                    return "Labels";
                case Language.de:
                    return "Labels";
            }
            return "Labels";
        }
        public string Filename() {
            switch (this.preferredLanguage) {
                case Language.en:
                    return "Filename";
                case Language.de:
                    return "Dateiname";
            }
            return "Filename";
        }
        public string Message() {
            switch (this.preferredLanguage) {
                case Language.en:
                    return "Message";
                case Language.de:
                    return "Nachricht";
            }
            return "Message";
        }
        public string EnumsPunishmentMute() {
            switch (this.preferredLanguage) {
                case Language.en:
                    return "Mute";
                case Language.de:
                    return "Stummschaltung";
            }
            return "Mute";
        }
        public string EnumsPunishmentBan() {
            switch (this.preferredLanguage) {
                case Language.en:
                    return "Ban";
                case Language.de:
                    return "Sperrung";
            }
            return "Ban";
        }
        public string EnumsPunishmentKick() {
            switch (this.preferredLanguage) {
                case Language.en:
                    return "Kick";
                case Language.de:
                    return "Kick";
            }
            return "Kick";
        }
        public string EnumsPunishmentWarn() {
            switch (this.preferredLanguage) {
                case Language.en:
                    return "Warn";
                case Language.de:
                    return "Verwarnung";
            }
            return "Warn";
        }
        public string EnumsViewPermissionSelf() {
            switch (this.preferredLanguage) {
                case Language.en:
                    return "Self";
                case Language.de:
                    return "Privat";
            }
            return "Self";
        }
        public string EnumsViewPermissionGuild() {
            switch (this.preferredLanguage) {
                case Language.en:
                    return "Guild";
                case Language.de:
                    return "Gilde";
            }
            return "Guild";
        }
        public string EnumsViewPermissionGlobal() {
            switch (this.preferredLanguage) {
                case Language.en:
                    return "Global";
                case Language.de:
                    return "Global";
            }
            return "Global";
        }
        public string EnumsAutoModActionsNone() {
            switch (this.preferredLanguage) {
                case Language.en:
                    return "None";
                case Language.de:
                    return "Keine Aktion";
            }
            return "None";
        }
        public string EnumsAutoModActionsContentDeleted() {
            switch (this.preferredLanguage) {
                case Language.en:
                    return "Content deleted";
                case Language.de:
                    return "Nachricht gelöscht";
            }
            return "Content deleted";
        }
        public string EnumsAutoModActionsCaseCreated() {
            switch (this.preferredLanguage) {
                case Language.en:
                    return "Case created";
                case Language.de:
                    return "Vorfall erstellt";
            }
            return "Case created";
        }
        public string EnumsAutoModActionsContentDeletedAndCaseCreated() {
            switch (this.preferredLanguage) {
                case Language.en:
                    return "Content deleted and case created";
                case Language.de:
                    return "Nachricht gelöscht und Vorfall erstellt";
            }
            return "Content deleted and case created";
        }
        public string NotificationModcaseCreatePublic(masz.Models.ModCase modCase) {
            switch (this.preferredLanguage) {
                case Language.en:
                    return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been created.";
                case Language.de:
                    return $"Ein **Vorfall** für <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) wurde erstellt.";
            }
            return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been created.";
        }
        public string NotificationModcaseCreateInternal(masz.Models.ModCase modCase, masz.Dtos.DiscordAPIResponses.User moderator) {
            switch (this.preferredLanguage) {
                case Language.en:
                    return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been created by <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).";
                case Language.de:
                    return $"Ein **Vorfall** für <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) wurde von <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}) erstellt.";
            }
            return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been created by <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).";
        }
        public string NotificationModcaseUpdatePublic(masz.Models.ModCase modCase) {
            switch (this.preferredLanguage) {
                case Language.en:
                    return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been updated.";
                case Language.de:
                    return $"Ein **Vorfall** für <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) wurde aktualisiert.";
            }
            return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been updated.";
        }
        public string NotificationModcaseUpdateInternal(masz.Models.ModCase modCase, masz.Dtos.DiscordAPIResponses.User moderator) {
            switch (this.preferredLanguage) {
                case Language.en:
                    return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been updated by <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).";
                case Language.de:
                    return $"Ein **Vorfall** für <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) wurde von <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}) aktualisiert.";
            }
            return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been updated by <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).";
        }
        public string NotificationModcaseDeletePublic(masz.Models.ModCase modCase) {
            switch (this.preferredLanguage) {
                case Language.en:
                    return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been deleted.";
                case Language.de:
                    return $"Ein **Vorfall** für <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) wurde gelöscht.";
            }
            return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been deleted.";
        }
        public string NotificationModcaseDeleteInternal(masz.Models.ModCase modCase, masz.Dtos.DiscordAPIResponses.User moderator) {
            switch (this.preferredLanguage) {
                case Language.en:
                    return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been deleted by <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).";
                case Language.de:
                    return $"Ein **Vorfall** für <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) wurde von <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}) gelöscht.";
            }
            return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been deleted by <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).";
        }
        public string NotificationModcaseCommentsShortCreate() {
            switch (this.preferredLanguage) {
                case Language.en:
                    return "Comment created";
                case Language.de:
                    return "Kommentar erstellt";
            }
            return "Comment created";
        }
        public string NotificationModcaseCommentsShortUpdate() {
            switch (this.preferredLanguage) {
                case Language.en:
                    return "Comment updated";
                case Language.de:
                    return "Kommentar aktualisiert";
            }
            return "Comment updated";
        }
        public string NotificationModcaseCommentsShortDelete() {
            switch (this.preferredLanguage) {
                case Language.en:
                    return "Comment deleted";
                case Language.de:
                    return "Kommentar gelöscht";
            }
            return "Comment deleted";
        }
        public string NotificationModcaseCommentsCreate(masz.Dtos.DiscordAPIResponses.User actor) {
            switch (this.preferredLanguage) {
                case Language.en:
                    return $"A **comment** has been created by <@{actor.Id}>.";
                case Language.de:
                    return $"Ein **Kommentar** wurde von <@{actor.Id}> erstellt.";
            }
            return $"A **comment** has been created by <@{actor.Id}>.";
        }
        public string NotificationModcaseCommentsUpdate(masz.Dtos.DiscordAPIResponses.User actor) {
            switch (this.preferredLanguage) {
                case Language.en:
                    return $"A **comment** has been updated by <@{actor.Id}>.";
                case Language.de:
                    return $"Ein **Kommentar** wurde von <@{actor.Id}> aktualisiert.";
            }
            return $"A **comment** has been updated by <@{actor.Id}>.";
        }
        public string NotificationModcaseCommentsDelete(masz.Dtos.DiscordAPIResponses.User actor) {
            switch (this.preferredLanguage) {
                case Language.en:
                    return $"A **comment** has been deleted by <@{actor.Id}>.";
                case Language.de:
                    return $"Ein **Kommentar** wurde von <@{actor.Id}> gelöscht.";
            }
            return $"A **comment** has been deleted by <@{actor.Id}>.";
        }
        public string NotificationModcaseFileCreate(masz.Dtos.DiscordAPIResponses.User actor) {
            switch (this.preferredLanguage) {
                case Language.en:
                    return $"A **file** has been uploaded by <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).";
                case Language.de:
                    return $"Eine **Datei** wurde von <@{actor.Id}> ({actor.Username}#{actor.Discriminator}) hochgeladen.";
            }
            return $"A **file** has been uploaded by <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).";
        }
        public string NotificationModcaseFileDelete(masz.Dtos.DiscordAPIResponses.User actor) {
            switch (this.preferredLanguage) {
                case Language.en:
                    return $"A **file** has been deleted by <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).";
                case Language.de:
                    return $"Eine **Datei** wurde von <@{actor.Id}> ({actor.Username}#{actor.Discriminator}) gelöscht.";
            }
            return $"A **file** has been deleted by <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).";
        }
        public string NotificationModcaseFileUpdate(masz.Dtos.DiscordAPIResponses.User actor) {
            switch (this.preferredLanguage) {
                case Language.en:
                    return $"A **file** has been updated by <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).";
                case Language.de:
                    return $"Eine **Datei** wurde von <@{actor.Id}> ({actor.Username}#{actor.Discriminator}) aktualisiert.";
            }
            return $"A **file** has been updated by <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).";
        }
        public string NotificationModcaseDMWarn(masz.Models.ModCase modCase, masz.Dtos.DiscordAPIResponses.Guild guild, string botPrefix, string serviceBaseUrl) {
            switch (this.preferredLanguage) {
                case Language.en:
                    return $"The moderators of guild `{guild.Name}` have warned you.\nUse `{botPrefix}viewg {modCase.GuildId} {modCase.CaseId}` to view more details about this case.\nFor more information or rehabilitation visit: {serviceBaseUrl}";
                case Language.de:
                    return $"Die Moderatoren von `{guild.Name}` haben dich verwarnt.\nBenutze `{botPrefix}viewg {modCase.GuildId} {modCase.CaseId}` um mehr Details über diesen Vorfall zu sehen.\nFür weitere Informationen besuche: {serviceBaseUrl}";
            }
            return $"The moderators of guild `{guild.Name}` have warned you.\nUse `{botPrefix}viewg {modCase.GuildId} {modCase.CaseId}` to view more details about this case.\nFor more information or rehabilitation visit: {serviceBaseUrl}";
        }
        public string NotificationModcaseDMMuteTemp(masz.Models.ModCase modCase, masz.Dtos.DiscordAPIResponses.Guild guild, string botPrefix, string serviceBaseUrl, string timezone) {
            switch (this.preferredLanguage) {
                case Language.en:
                    return $"The moderators of guild `{guild.Name}` have temporarily muted you until `{modCase.PunishedUntil.Value.ToString("dd.MMMM.yyyy HH:mm:ss")} ({timezone})`.\nUse `{botPrefix}viewg {modCase.GuildId} {modCase.CaseId}` to view more details about this case.\nFor more information or rehabilitation visit: {serviceBaseUrl}";
                case Language.de:
                    return $"Die Moderatoren von `{guild.Name}` haben dich temporär stummgeschalten bis `{modCase.PunishedUntil.Value.ToString("dd.MMMM.yyyy HH:mm:ss")} ({timezone})`.\nBenutze `{botPrefix}viewg {modCase.GuildId} {modCase.CaseId}` um mehr Details über diesen Vorfall zu sehen.\nFür weitere Informationen besuche: {serviceBaseUrl}";
            }
            return $"The moderators of guild `{guild.Name}` have temporarily muted you until `{modCase.PunishedUntil.Value.ToString("dd.MMMM.yyyy HH:mm:ss")} ({timezone})`.\nUse `{botPrefix}viewg {modCase.GuildId} {modCase.CaseId}` to view more details about this case.\nFor more information or rehabilitation visit: {serviceBaseUrl}";
        }
        public string NotificationModcaseDMMutePerm(masz.Models.ModCase modCase, masz.Dtos.DiscordAPIResponses.Guild guild, string botPrefix, string serviceBaseUrl) {
            switch (this.preferredLanguage) {
                case Language.en:
                    return $"The moderators of guild `{guild.Name}` have muted you.\nUse `{botPrefix}viewg {modCase.GuildId} {modCase.CaseId}` to view more details about this case.\nFor more information or rehabilitation visit: {serviceBaseUrl}";
                case Language.de:
                    return $"Die Moderatoren von `{guild.Name}` haben dich stummgeschalten.\nBenutze `{botPrefix}viewg {modCase.GuildId} {modCase.CaseId}` um mehr Details über diesen Vorfall zu sehen.\nFür weitere Informationen besuche: {serviceBaseUrl}";
            }
            return $"The moderators of guild `{guild.Name}` have muted you.\nUse `{botPrefix}viewg {modCase.GuildId} {modCase.CaseId}` to view more details about this case.\nFor more information or rehabilitation visit: {serviceBaseUrl}";
        }
        public string NotificationModcaseDMBanTemp(masz.Models.ModCase modCase, masz.Dtos.DiscordAPIResponses.Guild guild, string botPrefix, string serviceBaseUrl, string timezone) {
            switch (this.preferredLanguage) {
                case Language.en:
                    return $"The moderators of guild `{guild.Name}` have temporarily banned you until `{modCase.PunishedUntil.Value.ToString("dd.MMMM.yyyy HH:mm:ss")} ({timezone})`.\nUse `{botPrefix}viewg {modCase.GuildId} {modCase.CaseId}` to view more details about this case.\nFor more information or rehabilitation visit: {serviceBaseUrl}";
                case Language.de:
                    return $"Die Moderatoren von `{guild.Name}` haben dich temporär gebannt bis `{modCase.PunishedUntil.Value.ToString("dd.MMMM.yyyy HH:mm:ss")} ({timezone})`.\nBenutze `{botPrefix}viewg {modCase.GuildId} {modCase.CaseId}` um mehr Details über diesen Vorfall zu sehen.\nFür weitere Informationen besuche: {serviceBaseUrl}";
            }
            return $"The moderators of guild `{guild.Name}` have temporarily banned you until `{modCase.PunishedUntil.Value.ToString("dd.MMMM.yyyy HH:mm:ss")} ({timezone})`.\nUse `{botPrefix}viewg {modCase.GuildId} {modCase.CaseId}` to view more details about this case.\nFor more information or rehabilitation visit: {serviceBaseUrl}";
        }
        public string NotificationModcaseDMBanPerm(masz.Models.ModCase modCase, masz.Dtos.DiscordAPIResponses.Guild guild, string botPrefix, string serviceBaseUrl) {
            switch (this.preferredLanguage) {
                case Language.en:
                    return $"The moderators of guild `{guild.Name}` have banned you.\nUse `{botPrefix}viewg {modCase.GuildId} {modCase.CaseId}` to view more details about this case.\nFor more information or rehabilitation visit: {serviceBaseUrl}";
                case Language.de:
                    return $"Die Moderatoren von `{guild.Name}` haben dich gebannt.\nBenutze `{botPrefix}viewg {modCase.GuildId} {modCase.CaseId}` um mehr Details über diesen Vorfall zu sehen.\nFür weitere Informationen besuche: {serviceBaseUrl}";
            }
            return $"The moderators of guild `{guild.Name}` have banned you.\nUse `{botPrefix}viewg {modCase.GuildId} {modCase.CaseId}` to view more details about this case.\nFor more information or rehabilitation visit: {serviceBaseUrl}";
        }
        public string NotificationModcaseDMKick(masz.Models.ModCase modCase, masz.Dtos.DiscordAPIResponses.Guild guild, string botPrefix, string serviceBaseUrl) {
            switch (this.preferredLanguage) {
                case Language.en:
                    return $"The moderators of guild `{guild.Name}` have kicked you.\nUse `{botPrefix}viewg {modCase.GuildId} {modCase.CaseId}` to view more details about this case.\nFor more information or rehabilitation visit: {serviceBaseUrl}";
                case Language.de:
                    return $"Die Moderatoren von `{guild.Name}` haben dich kickt.\nBenutze `{botPrefix}viewg {modCase.GuildId} {modCase.CaseId}` um mehr Details über diesen Vorfall zu sehen.\nFür weitere Informationen besuche: {serviceBaseUrl}";
            }
            return $"The moderators of guild `{guild.Name}` have kicked you.\nUse `{botPrefix}viewg {modCase.GuildId} {modCase.CaseId}` to view more details about this case.\nFor more information or rehabilitation visit: {serviceBaseUrl}";
        }
        public string NotificationFilesCreate() {
            switch (this.preferredLanguage) {
                case Language.en:
                    return "File uploaded";
                case Language.de:
                    return "Datei hochgeladen";
            }
            return "File uploaded";
        }
        public string NotificationFilesDelete() {
            switch (this.preferredLanguage) {
                case Language.en:
                    return "File deleted";
                case Language.de:
                    return "Datei gelöscht";
            }
            return "File deleted";
        }
        public string NotificationFilesUpdate() {
            switch (this.preferredLanguage) {
                case Language.en:
                    return "File updated";
                case Language.de:
                    return "Datei aktualisiert";
            }
            return "File updated";
        }

    }
}

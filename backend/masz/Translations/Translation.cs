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
		public string Description() {
			switch (this.preferredLanguage) {
				case Language.en:
					return "Description";
				case Language.de:
					return "Beschreibung";
			}
			return "Description";
		}
		public string NotificationModcaseCreatePublic(masz.Models.ModCase modCase) {
			switch (this.preferredLanguage) {
				case Language.en:
					return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been created.";
			}
			return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been created.";
		}
		public string NotificationModcaseCreateInternal(masz.Models.ModCase modCase, masz.Dtos.DiscordAPIResponses.User moderator) {
			switch (this.preferredLanguage) {
				case Language.en:
					return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been created by <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).";
			}
			return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been created by <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).";
		}

    }
}

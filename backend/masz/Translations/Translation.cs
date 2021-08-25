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

        public string Welcome(string name) {
            switch (this.preferredLanguage) {
                case Language.en:
                    return $"Welcome {name}";
                case Language.de:
                    return $"Willkommen {name}";
                case Language.fr:
                    return $"Bienvenue {name}";
                case Language.es:
                    return $"Bienvenido {name}";
                case Language.it:
                    return $"Benvenuto {name}";
            }
            return $"Welcome {name}";
        }
        public string Zen(string name) {
            switch (this.preferredLanguage) {
                case Language.en:
                    return $"{name} Anything added dilutes everything else.";
                case Language.de:
                    return $"{name} Alles hängt von allen anderen ab.";
                case Language.fr:
                    return $"{name} Tout est ajouté à tout.";
                case Language.es:
                    return $"{name} Cualquier cosa añadida desaparece de todo.";
            }
            return $"{name} Anything added dilutes everything else.";
        }
    }
}
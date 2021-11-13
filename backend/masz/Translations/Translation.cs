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
                case Language.fr:
                    return "Caractéristiques";
                case Language.es:
                    return "Características";
                case Language.ru:
                    return "Функции";
                case Language.it:
                    return "Caratteristiche";
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
                case Language.fr:
                    return "Automodération";
                case Language.es:
                    return "Automoderación";
                case Language.ru:
                    return "Автомобильная промышленность";
                case Language.it:
                    return "Automoderazione";
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
                case Language.fr:
                    return "action";
                case Language.es:
                    return "Acción";
                case Language.ru:
                    return "Действие";
                case Language.it:
                    return "Azione";
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
                case Language.fr:
                    return "Pas trouvé.";
                case Language.es:
                    return "Extraviado.";
                case Language.ru:
                    return "Не найден.";
                case Language.it:
                    return "Non trovato.";
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
                case Language.fr:
                    return "Auteur";
                case Language.es:
                    return "Autor";
                case Language.ru:
                    return "Автор";
                case Language.it:
                    return "Autore";
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
                case Language.fr:
                    return "Contenu du message";
                case Language.es:
                    return "Contenido del mensaje";
                case Language.ru:
                    return "Содержание сообщения";
                case Language.it:
                    return "Contenuto del messaggio";
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
                case Language.fr:
                    return $"Voir les détails sur : {url}";
                case Language.es:
                    return $"Ver detalles en: {url}";
                case Language.ru:
                    return $"Подробнее о: {url}";
                case Language.it:
                    return $"Visualizza dettagli su: {url}";
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
                case Language.fr:
                    return "Pièces jointes";
                case Language.es:
                    return "Archivos adjuntos";
                case Language.ru:
                    return "Вложения";
                case Language.it:
                    return "Allegati";
            }
            return "Attachments";
        }
        public string Attachment() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Attachment";
                case Language.de:
                    return "Anhang";
                case Language.at:
                    return "Ohang";
                case Language.fr:
                    return "Attachement";
                case Language.es:
                    return "Adjunto";
                case Language.ru:
                    return "Вложение";
                case Language.it:
                    return "allegato";
            }
            return "Attachment";
        }
        public string AndXMore(int count) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"and {count} more...";
                case Language.de:
                    return $"und {count} weitere...";
                case Language.at:
                    return $"und {count} weitare...";
                case Language.fr:
                    return $"et {count} plus...";
                case Language.es:
                    return $"y {count} más ...";
                case Language.ru:
                    return $"и еще {count} ...";
                case Language.it:
                    return $"e {count} altro...";
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
                case Language.fr:
                    return "Canaliser";
                case Language.es:
                    return "Canal";
                case Language.ru:
                    return "Канал";
                case Language.it:
                    return "Canale";
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
                case Language.fr:
                    return "Quelque chose s'est mal passé.";
                case Language.es:
                    return "Algo salió mal.";
                case Language.ru:
                    return "Что-то пошло не так.";
                case Language.it:
                    return "Qualcosa è andato storto.";
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
                case Language.fr:
                    return "Code";
                case Language.es:
                    return "Código";
                case Language.ru:
                    return "Код";
                case Language.it:
                    return "Codice";
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
                case Language.fr:
                    return "Langue";
                case Language.es:
                    return "Idioma";
                case Language.ru:
                    return "Язык";
                case Language.it:
                    return "Lingua";
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
                case Language.fr:
                    return "Horodatage";
                case Language.es:
                    return "Marcas de tiempo";
                case Language.ru:
                    return "Отметки времени";
                case Language.it:
                    return "Timestamp";
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
                case Language.fr:
                    return "Soutien";
                case Language.es:
                    return "Apoyo";
                case Language.ru:
                    return "Служба поддержки";
                case Language.it:
                    return "Supporto";
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
                case Language.fr:
                    return "Châtiment";
                case Language.es:
                    return "Castigo";
                case Language.ru:
                    return "Наказание";
                case Language.it:
                    return "Punizione";
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
                case Language.fr:
                    return "jusqu'à";
                case Language.es:
                    return "Hasta que";
                case Language.ru:
                    return "до";
                case Language.it:
                    return "fino a";
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
                case Language.fr:
                    return "Puni jusqu'à";
                case Language.es:
                    return "Castigado hasta";
                case Language.ru:
                    return "Наказан до";
                case Language.it:
                    return "Punito fino a";
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
                case Language.fr:
                    return "La description";
                case Language.es:
                    return "Descripción";
                case Language.ru:
                    return "Описание";
                case Language.it:
                    return "Descrizione";
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
                case Language.fr:
                    return "Étiquettes";
                case Language.es:
                    return "Etiquetas";
                case Language.ru:
                    return "Этикетки";
                case Language.it:
                    return "etichette";
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
                case Language.fr:
                    return "Nom de fichier";
                case Language.es:
                    return "Nombre del archivo";
                case Language.ru:
                    return "Имя файла";
                case Language.it:
                    return "Nome del file";
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
                case Language.fr:
                    return "Un message";
                case Language.es:
                    return "Mensaje";
                case Language.ru:
                    return "Сообщение";
                case Language.it:
                    return "Messaggio";
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
                case Language.fr:
                    return "Note de l'utilisateur";
                case Language.es:
                    return "UserNote";
                case Language.ru:
                    return "UserNote";
                case Language.it:
                    return "Nota utente";
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
                case Language.fr:
                    return "Notes de l'utilisateur";
                case Language.es:
                    return "Notas de usuario";
                case Language.ru:
                    return "UserNotes";
                case Language.it:
                    return "Note utente";
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
                case Language.fr:
                    return "Cas";
                case Language.es:
                    return "Casos";
                case Language.ru:
                    return "Случаи";
                case Language.it:
                    return "casi";
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
                case Language.fr:
                    return "Punitions actives";
                case Language.es:
                    return "Castigos activos";
                case Language.ru:
                    return "Активные наказания";
                case Language.it:
                    return "punizioni attive";
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
                case Language.fr:
                    return "UserMap";
                case Language.es:
                    return "UserMap";
                case Language.ru:
                    return "UserMap";
                case Language.it:
                    return "Mappa utente";
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
                case Language.fr:
                    return "UserMaps";
                case Language.es:
                    return "UserMaps";
                case Language.ru:
                    return "UserMaps";
                case Language.it:
                    return "Mappe utente";
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
                case Language.fr:
                    return $"UserMap entre {userMap.UserA} et {userMap.UserB}.";
                case Language.es:
                    return $"UserMap entre {userMap.UserA} y {userMap.UserB}.";
                case Language.ru:
                    return $"UserMap между {userMap.UserA} и {userMap.UserB}.";
                case Language.it:
                    return $"UserMap tra {userMap.UserA} e {userMap.UserB}.";
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
                case Language.fr:
                    return "Taper";
                case Language.es:
                    return "Escribe";
                case Language.ru:
                    return "Тип";
                case Language.it:
                    return "Tipo";
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
                case Language.fr:
                    return "Inscrit";
                case Language.es:
                    return "Unido";
                case Language.ru:
                    return "Присоединился";
                case Language.it:
                    return "Partecipato";
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
                case Language.fr:
                    return "Inscrit";
                case Language.es:
                    return "Registrado";
                case Language.ru:
                    return "Зарегистрировано";
                case Language.it:
                    return "Registrato";
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
                case Language.fr:
                    return $"Un **Modcase** pour <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) a été créé.";
                case Language.es:
                    return $"Se ha creado un **Modcase** para <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}).";
                case Language.ru:
                    return $"**Modcase** для <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) был создан.";
                case Language.it:
                    return $"È stato creato un **Modcase** per <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}).";
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
                case Language.fr:
                    return $"Un **Modcase** pour <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) a été créé par <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).";
                case Language.es:
                    return $"Un **Modcase** para <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) ha sido creado por <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).";
                case Language.ru:
                    return $"**Modcase** для <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) был создан <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).";
                case Language.it:
                    return $"Un **Modcase** per <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) è stato creato da <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).";
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
                case Language.fr:
                    return $"Un **Modcase** pour <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) a été mis à jour.";
                case Language.es:
                    return $"Se ha actualizado **Modcase** para <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}).";
                case Language.ru:
                    return $"**Modcase** для <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) был обновлен.";
                case Language.it:
                    return $"È stato aggiornato un **Modcase** per <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}).";
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
                case Language.fr:
                    return $"Un **Modcase** pour <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) a été mis à jour par <@{moderator.Id}> ({moderator.Username}#{moderator. Discriminator}).";
                case Language.es:
                    return $"Un **Modcase** para <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) ha sido actualizado por <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).";
                case Language.ru:
                    return $"**Modcase** для <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) был обновлен <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).";
                case Language.it:
                    return $"Un **Modcase** per <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) è stato aggiornato da <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).";
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
                case Language.fr:
                    return $"Un **Modcase** pour <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) a été supprimé.";
                case Language.es:
                    return $"Se ha eliminado un **Modcase** para <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}).";
                case Language.ru:
                    return $"**Modcase** для <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) был удален.";
                case Language.it:
                    return $"Un **Modcase** per <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) è stato eliminato.";
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
                case Language.fr:
                    return $"Un **Modcase** pour <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) a été supprimé par <@{moderator.Id}> ({moderator.Username}#{moderator. Discriminator}).";
                case Language.es:
                    return $"Un **Modcase** para <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) ha sido eliminado por <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).";
                case Language.ru:
                    return $"**Modcase** для <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) был удален <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).";
                case Language.it:
                    return $"Un **Modcase** per <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) è stato eliminato da <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).";
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
                case Language.fr:
                    return "Commentaire créé";
                case Language.es:
                    return "Comentario creado";
                case Language.ru:
                    return "Комментарий создан";
                case Language.it:
                    return "Commento creato";
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
                case Language.fr:
                    return "Commentaire mis à jour";
                case Language.es:
                    return "Comentario actualizado";
                case Language.ru:
                    return "Комментарий обновлен";
                case Language.it:
                    return "Commento aggiornato";
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
                case Language.fr:
                    return "Commentaire supprimé";
                case Language.es:
                    return "Comentario borrado";
                case Language.ru:
                    return "Комментарий удален";
                case Language.it:
                    return "Commento cancellato";
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
                case Language.fr:
                    return $"Un **commentaire** a été créé par <@{actor.Id}>.";
                case Language.es:
                    return $"<@{actor.Id}> ha creado un **comentario**.";
                case Language.ru:
                    return $"**комментарий** был создан <@{actor.Id}>.";
                case Language.it:
                    return $"Un **commento** è stato creato da <@{actor.Id}>.";
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
                case Language.fr:
                    return $"Un **commentaire** a été mis à jour par <@{actor.Id}>.";
                case Language.es:
                    return $"<@{actor.Id}> ha actualizado un **comentario **.";
                case Language.ru:
                    return $"**комментарий ** был обновлен пользователем <@{actor.Id}>.";
                case Language.it:
                    return $"Un **commento** è stato aggiornato da <@{actor.Id}>.";
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
                case Language.fr:
                    return $"Un **commentaire** a été supprimé par <@{actor.Id}>.";
                case Language.es:
                    return $"<@{actor.Id}> ha eliminado un **comentario **.";
                case Language.ru:
                    return $"**комментарий** был удален <@{actor.Id}>.";
                case Language.it:
                    return $"Un **commento** è stato eliminato da <@{actor.Id}>.";
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
                case Language.fr:
                    return $"Un **fichier** a été téléchargé par <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).";
                case Language.es:
                    return $"<@{actor.Id}> ({actor.Username}#{actor.Discriminator} ha subido un **archivo**).";
                case Language.ru:
                    return $"**файл** был загружен пользователем <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).";
                case Language.it:
                    return $"Un **file** è stato caricato da <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).";
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
                case Language.fr:
                    return $"Un **fichier** a été supprimé par <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).";
                case Language.es:
                    return $"<@{actor.Id}> ({actor.Username}#{actor.Discriminator}) ha eliminado un **archivo**.";
                case Language.ru:
                    return $"**файл** был удален <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).";
                case Language.it:
                    return $"Un **file** è stato eliminato da <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).";
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
                case Language.fr:
                    return $"Un **fichier** a été mis à jour par <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).";
                case Language.es:
                    return $"<@{actor.Id}> ({actor.Username}#{actor.Discriminator}) ha actualizado un **archivo**.";
                case Language.ru:
                    return $"**файл** был обновлен <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).";
                case Language.it:
                    return $"Un **file** è stato aggiornato da <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).";
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
                case Language.fr:
                    return $"Les modérateurs de la guilde `{guild.Name}` vous ont prévenu.\nPour plus d'informations ou pour une réhabilitation, visitez : {serviceBaseUrl}";
                case Language.es:
                    return $"Los moderadores del gremio `{guild.Name}` te han advertido.\nPara obtener más información o rehabilitación, visite: {serviceBaseUrl}";
                case Language.ru:
                    return $"Модераторы гильдии `{guild.Name}` вас предупредили.\nДля получения дополнительной информации или реабилитации посетите: {serviceBaseUrl}";
                case Language.it:
                    return $"I moderatori della gilda `{guild.Name}` ti hanno avvisato.\nPer maggiori informazioni o visita riabilitativa: {serviceBaseUrl}";
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
                case Language.fr:
                    return $"Les modérateurs de la guilde `{guild.Name}` vous ont temporairement mis en sourdine jusqu'à {modCase.PunishedUntil.Value.ToDiscordTS()}.\nPour plus d'informations ou pour une réhabilitation, visitez : {serviceBaseUrl}";
                case Language.es:
                    return $"Los moderadores del gremio `{guild.Name}` te han silenciado temporalmente hasta {modCase.PunishedUntil.Value.ToDiscordTS ()}.\nPara obtener más información o rehabilitación, visite: {serviceBaseUrl}";
                case Language.ru:
                    return $"Модераторы гильдии `{guild.Name}` временно отключили ваш звук до {modCase.PunishedUntil.Value.ToDiscordTS ()}.\nДля получения дополнительной информации или реабилитации посетите: {serviceBaseUrl}";
                case Language.it:
                    return $"I moderatori della gilda `{guild.Name}` ti hanno temporaneamente disattivato l'audio fino a {modCase.PunishedUntil.Value.ToDiscordTS()}.\nPer maggiori informazioni o visita riabilitativa: {serviceBaseUrl}";
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
                case Language.fr:
                    return $"Les modérateurs de la guilde `{guild.Name}` vous ont mis en sourdine.\nPour plus d'informations ou pour une réhabilitation, visitez : {serviceBaseUrl}";
                case Language.es:
                    return $"Los moderadores del gremio `{guild.Name}` te han silenciado.\nPara obtener más información o rehabilitación, visite: {serviceBaseUrl}";
                case Language.ru:
                    return $"Модераторы гильдии `{guild.Name}` отключили вас.\nДля получения дополнительной информации или реабилитации посетите: {serviceBaseUrl}";
                case Language.it:
                    return $"I moderatori della gilda `{guild.Name}` ti hanno disattivato.\nPer maggiori informazioni o visita riabilitativa: {serviceBaseUrl}";
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
                case Language.fr:
                    return $"Les modérateurs de la guilde `{guild.Name}` vous ont temporairement banni jusqu'à {modCase.PunishedUntil.Value.ToDiscordTS()}.\nPour plus d'informations ou pour une réhabilitation, visitez : {serviceBaseUrl}";
                case Language.es:
                    return $"Los moderadores del gremio `{guild.Name}` te han baneado temporalmente hasta el {modCase.PunishedUntil.Value.ToDiscordTS ()}.\nPara obtener más información o rehabilitación, visite: {serviceBaseUrl}";
                case Language.ru:
                    return $"Модераторы гильдии `{guild.Name}` временно заблокировали вас до {modCase.PunishedUntil.Value.ToDiscordTS ()}.\nДля получения дополнительной информации или реабилитации посетите: {serviceBaseUrl}";
                case Language.it:
                    return $"I moderatori della gilda `{guild.Name}` ti hanno temporaneamente bannato fino al {modCase.PunishedUntil.Value.ToDiscordTS()}.\nPer maggiori informazioni o visita riabilitativa: {serviceBaseUrl}";
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
                case Language.fr:
                    return $"Les modérateurs de la guilde `{guild.Name}` vous ont banni.\nPour plus d'informations ou pour une réhabilitation, visitez : {serviceBaseUrl}";
                case Language.es:
                    return $"Los moderadores del gremio `{guild.Name}` te han prohibido.\nPara obtener más información o rehabilitación, visite: {serviceBaseUrl}";
                case Language.ru:
                    return $"Модераторы гильдии `{guild.Name}` забанили вас.\nДля получения дополнительной информации или реабилитации посетите: {serviceBaseUrl}";
                case Language.it:
                    return $"I moderatori della gilda `{guild.Name}` ti hanno bannato.\nPer maggiori informazioni o visita riabilitativa: {serviceBaseUrl}";
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
                case Language.fr:
                    return $"Les modérateurs de la guilde `{guild.Name}` vous ont viré.\nPour plus d'informations ou pour une réhabilitation, visitez : {serviceBaseUrl}";
                case Language.es:
                    return $"Los moderadores del gremio `{guild.Name}` te han pateado.\nPara obtener más información o rehabilitación, visite: {serviceBaseUrl}";
                case Language.ru:
                    return $"Модераторы гильдии `{guild.Name}` выгнали вас.\nДля получения дополнительной информации или реабилитации посетите: {serviceBaseUrl}";
                case Language.it:
                    return $"I moderatori della gilda `{guild.Name}` ti hanno espulso.\nPer maggiori informazioni o visita riabilitativa: {serviceBaseUrl}";
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
                case Language.fr:
                    return "Fichier téléchargé";
                case Language.es:
                    return "Archivo subido";
                case Language.ru:
                    return "Файл загружен";
                case Language.it:
                    return "File caricato";
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
                case Language.fr:
                    return "Fichier supprimé";
                case Language.es:
                    return "Archivo eliminado";
                case Language.ru:
                    return "Файл удален";
                case Language.it:
                    return "File cancellato";
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
                case Language.fr:
                    return "Fichier mis à jour";
                case Language.es:
                    return "Archivo actualizado";
                case Language.ru:
                    return "Файл обновлен";
                case Language.it:
                    return "File aggiornato";
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
                case Language.fr:
                    return "Bienvenue à MASZ !";
                case Language.es:
                    return "¡Bienvenido a MASZ!";
                case Language.ru:
                    return "Добро пожаловать в МАСЗ!";
                case Language.it:
                    return "Benvenuto in MASZ!";
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
                case Language.fr:
                    return "Merci d'avoir enregistré votre guilde.\nDans ce qui suit, vous apprendrez quelques conseils utiles pour configurer et utiliser **MASZ**.";
                case Language.es:
                    return "Gracias por registrar tu gremio.\nA continuación, aprenderá algunos consejos útiles para configurar y usar **MASZ**.";
                case Language.ru:
                    return "Спасибо за регистрацию вашей гильдии.\nДалее вы получите несколько полезных советов по настройке и использованию **MASZ**.";
                case Language.it:
                    return "Grazie per aver registrato la tua gilda.\nDi seguito imparerai alcuni suggerimenti utili per impostare e utilizzare **MASZ**.";
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
                case Language.fr:
                    return "Utilisez la commande `/features` pour tester si votre configuration de guilde actuelle prend en charge toutes les fonctionnalités de **MASZ**.";
                case Language.es:
                    return "Usa el comando `/ features` para probar si la configuración de tu gremio actual es compatible con todas las características de **MASZ**.";
                case Language.ru:
                    return "Используйте команду `/ features`, чтобы проверить, поддерживает ли ваша текущая настройка гильдии все функции **MASZ**.";
                case Language.it:
                    return "Usa il comando `/features` per verificare se l'attuale configurazione della gilda supporta tutte le funzionalità di **MASZ**.";
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
                case Language.fr:
                    return $"MASZ utilisera `{language}` comme langue par défaut pour cette guilde dans la mesure du possible.";
                case Language.es:
                    return $"MASZ usará `{language}` como idioma predeterminado para este gremio siempre que sea posible.";
                case Language.ru:
                    return $"MASZ будет использовать `{language}` как язык по умолчанию для этой гильдии, когда это возможно.";
                case Language.it:
                    return $"MASZ utilizzerà `{language}` come lingua predefinita per questa gilda ogni volta che sarà possibile.";
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
                case Language.fr:
                    return "Les fuseaux horaires peuvent être déroutants.\nMASZ utilise une fonction Discord pour afficher les horodatages dans le fuseau horaire local de votre ordinateur/téléphone.";
                case Language.es:
                    return "Las zonas horarias pueden resultar confusas.\nMASZ usa una función de Discord para mostrar marcas de tiempo en la zona horaria local de su computadora / teléfono.";
                case Language.ru:
                    return "Часовые пояса могут сбивать с толку.\nMASZ использует функцию Discord для отображения меток времени в местном часовом поясе вашего компьютера / телефона.";
                case Language.it:
                    return "I fusi orari possono creare confusione.\nMASZ utilizza una funzione Discord per visualizzare i timestamp nel fuso orario locale del tuo computer/telefono.";
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
                case Language.fr:
                    return "Veuillez vous référer au [serveur de support MASZ] (https://discord.gg/5zjpzw6h3S) pour d'autres questions.";
                case Language.es:
                    return "Consulte el [servidor de soporte MASZ] (https://discord.gg/5zjpzw6h3S) si tiene más preguntas.";
                case Language.ru:
                    return "Дополнительные вопросы можно найти на [сервере поддержки MASZ] (https://discord.gg/5zjpzw6h3S).";
                case Language.it:
                    return "Fare riferimento al [Server di supporto MASZ] (https://discord.gg/5zjpzw6h3S) per ulteriori domande.";
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
                case Language.fr:
                    return $"{user.Mention} a déclenché l'automodération.";
                case Language.es:
                    return $"{user.Mention} activó la automoderación.";
                case Language.ru:
                    return $"{user.Mention} запустил автомодерацию.";
                case Language.it:
                    return $"{user.Mention} ha attivato la moderazione automatica.";
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
                case Language.fr:
                    return $"{user.Username}#{user.Discriminator} a déclenché la modération automatique.";
                case Language.es:
                    return $"{user.Username}#{user.Discriminator} desencadenó la automoderación.";
                case Language.ru:
                    return $"{user.Username}#{user.Discriminator} запускает автомодерацию.";
                case Language.it:
                    return $"{user.Username}#{user.Discriminator} ha attivato la moderazione automatica.";
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
                case Language.fr:
                    return $"Salut {user.Mention},\n\nVous avez déclenché l'automodération dans {channel.Mention}.\nRaison : {reason}\nAction : {action}";
                case Language.es:
                    return $"Hola, {user.Mention}:\n\nActivó la automoderación en {channel.Mention}.\nMotivo: {reason}\nAcción: {action}";
                case Language.ru:
                    return $"Привет, {user.Mention}!\n\nВы активировали автомодерацию в {channel.Mention}.\nПричина: {reason}\nДействие: {action}";
                case Language.it:
                    return $"Ciao {user.Mention},\n\nHai attivato la moderazione automatica in {channel.Mention}.\nMotivo: {reason}\nAzione: {action}";
            }
            return $"Hi {user.Mention},\n\nYou triggered automoderation in {channel.Mention}.\nReason: {reason}\nAction: {action}";
        }
        public string NotificationAutomoderationChannel(DSharpPlus.Entities.DiscordUser user, string reason) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"{user.Mention} you triggered automoderation. Reason: {reason}. Your message has been deleted.";
                case Language.de:
                    return $"{user.Mention} du hast die Automoderation ausgelöst. Grund: {reason}. Deine Nachricht wurde gelöscht.";
                case Language.at:
                    return $"{user.Mention} du host de Automodaration ausglest. Grund: {reason}. Dei Nochricht wuad glescht.";
                case Language.fr:
                    return $"{user.Mention} vous avez déclenché l'automodération. Raison : {reason}. Votre message a été supprimé.";
                case Language.es:
                    return $"{user.Mention} has activado la automoderación. Razón: {reason}. Su mensaje ha sido eliminado.";
                case Language.ru:
                    return $"{user.Mention} вы запустили автомодерацию. Причина: {reason}. Ваше сообщение было удалено.";
                case Language.it:
                    return $"{user.Mention} hai attivato la moderazione automatica. reason: {reason}. Il tuo messaggio è stato cancellato.";
            }
            return $"{user.Mention} you triggered automoderation. Reason: {reason}. Your message has been deleted.";
        }
        public string NotificationAutoWhoisJoinWith(DSharpPlus.Entities.DiscordUser user, DateTime registered, string invite) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"{user.Mention} (registered {registered.ToDiscordTS()}) joined with invite `{invite}`.";
                case Language.de:
                    return $"{user.Mention} (registriert {registered.ToDiscordTS()}) ist mit dem Invite `{invite}` beigetreten.";
                case Language.at:
                    return $"{user.Mention} (registriat {registered.ToDiscordTS()}) is mit da Eiladung `{invite}` beigetretn.";
                case Language.fr:
                    return $"{user.Mention} (enregistré {registered.ToDiscordTS()}) rejoint avec l'invitation `{invite}`.";
                case Language.es:
                    return $"{user.Mention} (registrado {registered.ToDiscordTS ()}) se unió con la invitación `{invite}`.";
                case Language.ru:
                    return $"{user.Mention} (зарегистрированный {registered.ToDiscordTS ()}) присоединился с приглашением `{invite}`.";
                case Language.it:
                    return $"{user.Mention} (registrato {registered.ToDiscordTS()}) si è unito con l'invito `{invite}`.";
            }
            return $"{user.Mention} (registered {registered.ToDiscordTS()}) joined with invite `{invite}`.";
        }
        public string NotificationAutoWhoisJoinWithAndFrom(DSharpPlus.Entities.DiscordUser user, ulong by, DateTime createdAt, DateTime registered, string invite) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"{user.Mention} (registered {registered.ToDiscordTS()}) joined with invite `{invite}` (created {createdAt.ToDiscordTS()}) by <@{by}>.";
                case Language.de:
                    return $"{user.Mention} (registriert {registered.ToDiscordTS()}) ist mit dem Invite `{invite}` von <@{by}> (am {createdAt.ToDiscordTS()}) beigetreten.";
                case Language.at:
                    return $"{user.Mention} (registriert {registered.ToDiscordTS()}) is mit da Eiladung `{invite}` vo <@{by}> (am {createdAt.ToDiscordTS()}) beigetretn.";
                case Language.fr:
                    return $"{user.Mention} (enregistré {registered.ToDiscordTS()}) rejoint avec invite `{invite}` (créé {createdAt.ToDiscordTS()}) par <@{by}>.";
                case Language.es:
                    return $"{user.Mention} (registrado {registered.ToDiscordTS()}) se unió con la invitación `{invite}` (creado {createdAt.ToDiscordTS()}) por <@{by}>.";
                case Language.ru:
                    return $"{user.Mention} (зарегистрированный {registered.ToDiscordTS()}) присоединился с помощью приглашения `{invite}` (created {createdAt.ToDiscordTS()}) пользователем <@{by}>.";
                case Language.it:
                    return $"{user.Mention} (registrato {registered.ToDiscordTS()}) si è unito all'invito `{invite}` (creato {createdAt.ToDiscordTS()}) da <@{by}>.";
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
                case Language.fr:
                    return "Seuls les canaux de texte sont autorisés.";
                case Language.es:
                    return "Solo se permiten canales de texto.";
                case Language.ru:
                    return "Разрешены только текстовые каналы.";
                case Language.it:
                    return "Sono consentiti solo canali di testo.";
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
                case Language.fr:
                    return "Je ne suis pas autorisé à afficher ou supprimer les messages de cette chaîne !";
                case Language.es:
                    return "¡No puedo ver ni borrar mensajes en este canal!";
                case Language.ru:
                    return "Мне не разрешено просматривать или удалять сообщения на этом канале!";
                case Language.it:
                    return "Non sono autorizzato a visualizzare o eliminare i messaggi in questo canale!";
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
                case Language.fr:
                    return "Impossible de trouver la chaîne.";
                case Language.es:
                    return "No se puede encontrar el canal.";
                case Language.ru:
                    return "Не могу найти канал.";
                case Language.it:
                    return "Impossibile trovare il canale.";
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
                case Language.fr:
                    return "Cette guilde n'a pas configuré de webhook pour les notifications internes.";
                case Language.es:
                    return "Este gremio no tiene configurado ningún webhook para notificaciones internas.";
                case Language.ru:
                    return "У этой гильдии нет настроенного веб-перехватчика для внутренних уведомлений.";
                case Language.it:
                    return "Questa gilda non ha webhook per le notifiche interne configurate.";
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
                case Language.fr:
                    return $"{count} messages supprimés dans {channel.Mention}.";
                case Language.es:
                    return $"Se eliminaron {count} mensajes en {channel.Mention}.";
                case Language.ru:
                    return $"Удалено {count} сообщений в {channel.Mention}.";
                case Language.it:
                    return $"Eliminati {count} messaggi in {channel.Mention}.";
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
                case Language.fr:
                    return "Autorisation de kick accordée.";
                case Language.es:
                    return "Permiso de patada concedido.";
                case Language.ru:
                    return "Разрешение на удар предоставлено.";
                case Language.it:
                    return "Autorizzazione calci concessa.";
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
                case Language.fr:
                    return "L'autorisation de kick n'est pas accordée.";
                case Language.es:
                    return "Permiso de patada no concedido.";
                case Language.ru:
                    return "Разрешение на удар не предоставлено.";
                case Language.it:
                    return "Autorizzazione calcio non concessa.";
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
                case Language.fr:
                    return "Autorisation d'interdiction accordée.";
                case Language.es:
                    return "Prohibición concedida.";
                case Language.ru:
                    return "Получено разрешение на запрет.";
                case Language.it:
                    return "Autorizzazione al divieto concessa.";
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
                case Language.fr:
                    return "Autorisation d'interdiction non accordée.";
                case Language.es:
                    return "Prohibir permiso no concedido.";
                case Language.ru:
                    return "Разрешение на запрет не предоставлено.";
                case Language.it:
                    return "Autorizzazione al divieto non concessa.";
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
                case Language.fr:
                    return "Gérer l'autorisation de rôle accordée.";
                case Language.es:
                    return "Administrar el permiso de función otorgado.";
                case Language.ru:
                    return "Разрешение на управление ролью предоставлено.";
                case Language.it:
                    return "Gestire l'autorizzazione del ruolo concessa.";
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
                case Language.fr:
                    return "L'autorisation de gestion du rôle n'est pas accordée.";
                case Language.es:
                    return "Administrar el permiso de función no concedido.";
                case Language.ru:
                    return "Не предоставлено разрешение на управление ролью.";
                case Language.it:
                    return "Autorizzazione di gestione del ruolo non concessa.";
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
                case Language.fr:
                    return "Rôle muet défini.";
                case Language.es:
                    return "Función silenciada definida.";
                case Language.ru:
                    return "Определена приглушенная роль.";
                case Language.it:
                    return "Ruolo disattivato definito.";
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
                case Language.fr:
                    return "Rôle en sourdine défini mais trop élevé dans la hiérarchie des rôles.";
                case Language.es:
                    return "Función silenciada definida pero demasiado alta en la jerarquía de funciones.";
                case Language.ru:
                    return "Определена приглушенная роль, но иерархия ролей слишком высока.";
                case Language.it:
                    return "Ruolo disattivato definito ma troppo alto nella gerarchia dei ruoli.";
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
                case Language.fr:
                    return "Rôle muet défini mais invalide.";
                case Language.es:
                    return "Función silenciada definida pero no válida.";
                case Language.ru:
                    return "Роль без звука определена, но недействительна.";
                case Language.it:
                    return "Ruolo disattivato definito ma non valido.";
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
                case Language.fr:
                    return "Rôle en sourdine non défini.";
                case Language.es:
                    return "Rol silenciado indefinido.";
                case Language.ru:
                    return "Отключенная роль не определена.";
                case Language.it:
                    return "Ruolo disattivato non definito.";
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
                case Language.fr:
                    return "Exécution de la peine";
                case Language.es:
                    return "Ejecución del castigo";
                case Language.ru:
                    return "Казнь";
                case Language.it:
                    return "Esecuzione della punizione";
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
                case Language.fr:
                    return "Laissez MASZ gérer les punitions (par exemple, tempbans, muets, etc.).";
                case Language.es:
                    return "Deje que MASZ maneje los castigos (por ejemplo, tempbans, mudos, etc.).";
                case Language.ru:
                    return "Позвольте MASZ заниматься наказаниями (например, временным запретом, отключением звука и т. Д.).";
                case Language.it:
                    return "Lascia che MASZ gestisca le punizioni (ad esempio tempban, mute, ecc.).";
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
                case Language.fr:
                    return "Annuler l'interdiction des demandes";
                case Language.es:
                    return "Solicitudes de anulación de la prohibición";
                case Language.ru:
                    return "Запросы на разблокировку";
                case Language.it:
                    return "Riattiva richieste";
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
                case Language.fr:
                    return "Permet aux membres bannis de voir leurs cas et de les commenter pour les demandes de déban.";
                case Language.es:
                    return "Permite a los miembros prohibidos ver sus casos y comentarlos para las solicitudes de deshabilitación.";
                case Language.ru:
                    return "Позволяет заблокированным участникам просматривать свои дела и комментировать их для запросов на разблокировку.";
                case Language.it:
                    return "Consente ai membri bannati di vedere i loro casi e commentarli per le richieste di sban.";
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
                case Language.fr:
                    return "Permet aux membres bannis de voir leurs cas et de les commenter pour les demandes de déban.\nAccordez à ce bot l'autorisation d'interdire l'utilisation de cette fonctionnalité.";
                case Language.es:
                    return "Permite a los miembros prohibidos ver sus casos y comentarlos para las solicitudes de deshabilitación.\nOtorga a este bot el permiso de prohibición para usar esta función.";
                case Language.ru:
                    return "Позволяет заблокированным участникам просматривать свои дела и комментировать их для запросов на разблокировку.\nПредоставьте этому боту разрешение на использование этой функции.";
                case Language.it:
                    return "Consente ai membri bannati di vedere i loro casi e commentarli per le richieste di sban.\nConcedi a questo bot il permesso di ban per utilizzare questa funzione.";
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
                case Language.fr:
                    return "Commande de rapport";
                case Language.es:
                    return "Comando de informe";
                case Language.ru:
                    return "Команда отчета";
                case Language.it:
                    return "Comando di rapporto";
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
                case Language.fr:
                    return "Permet aux membres de signaler des messages.";
                case Language.es:
                    return "Permite a los miembros informar mensajes.";
                case Language.ru:
                    return "Позволяет участникам сообщать о сообщениях.";
                case Language.it:
                    return "Consente ai membri di segnalare i messaggi.";
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
                case Language.fr:
                    return "Permet aux membres de signaler des messages.\nDéfinissez un webhook interne pour le personnel pour utiliser cette fonctionnalité.";
                case Language.es:
                    return "Permite a los miembros informar mensajes.\nDefina un webhook de personal interno para utilizar esta función.";
                case Language.ru:
                    return "Позволяет участникам сообщать о сообщениях.\nОпределите внутренний веб-перехватчик персонала, чтобы использовать эту функцию.";
                case Language.it:
                    return "Consente ai membri di segnalare i messaggi.\nDefinire un webhook personale interno per utilizzare questa funzione.";
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
                case Language.fr:
                    return "Suivi des invitations";
                case Language.es:
                    return "Seguimiento de invitaciones";
                case Language.ru:
                    return "Отслеживание приглашений";
                case Language.it:
                    return "Invita il monitoraggio";
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
                case Language.fr:
                    return "Permet MASZ de suivre les nouveaux membres invite utilisent.";
                case Language.es:
                    return "Permite a MASZ realizar un seguimiento de las invitaciones que están utilizando los nuevos miembros.";
                case Language.ru:
                    return "Позволяет MASZ отслеживать приглашения, которые используют новые участники.";
                case Language.it:
                    return "Consente a MASZ di tenere traccia degli inviti utilizzati dai nuovi membri.";
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
                case Language.fr:
                    return "Permet à MASZ de suivre les invitations que les nouveaux membres utilisent.\nAccordez à ce bot l'autorisation de gestion de guilde pour utiliser cette fonctionnalité.";
                case Language.es:
                    return "Permite a MASZ realizar un seguimiento de las invitaciones que están utilizando los nuevos miembros.\nOtorga a este bot el permiso de gestión del gremio para usar esta función.";
                case Language.ru:
                    return "Позволяет MASZ отслеживать приглашения, которые используют новые участники.\nПредоставьте этому боту разрешение на управление гильдией на использование этой функции.";
                case Language.it:
                    return "Consente a MASZ di tenere traccia degli inviti utilizzati dai nuovi membri.\nConcedi a questo bot il permesso di gestione della gilda per utilizzare questa funzione.";
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
                case Language.fr:
                    return "Votre bot sur cette guilde est correctement configuré. Toutes les fonctionnalités de MASZ peuvent être utilisées.";
                case Language.es:
                    return "Tu bot en este gremio está configurado correctamente. Se pueden utilizar todas las funciones de MASZ.";
                case Language.ru:
                    return "Ваш бот в этой гильдии настроен правильно. Можно использовать все возможности MASZ.";
                case Language.it:
                    return "Il tuo bot in questa gilda è configurato correttamente. Tutte le funzionalità di MASZ possono essere utilizzate.";
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
                case Language.fr:
                    return "Il y a des fonctionnalités de MASZ que vous ne pouvez pas utiliser pour le moment.";
                case Language.es:
                    return "Hay funciones de MASZ que no puede utilizar en este momento.";
                case Language.ru:
                    return "Есть функции MASZ, которые вы не можете использовать прямо сейчас.";
                case Language.it:
                    return "Ci sono funzionalità di MASZ che non puoi usare in questo momento.";
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
                case Language.fr:
                    return "Vous devrez héberger votre propre instance de MASZ sur votre serveur ou votre PC.\nCommander https://github.com/zaanposni/discord-masz#hosting";
                case Language.es:
                    return "Tendrá que alojar su propia instancia de MASZ en su servidor o PC.\nPagar https://github.com/zaanposni/discord-masz#hosting";
                case Language.ru:
                    return "Вам нужно будет разместить свой собственный экземпляр MASZ на вашем сервере или компьютере.\nОформить заказ https://github.com/zaanposni/discord-masz#hosting";
                case Language.it:
                    return "Dovrai ospitare la tua istanza di MASZ sul tuo server o PC.\nAcquista https://github.com/zaanposni/discord-masz#hosting";
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
                case Language.fr:
                    return $"Cas `#{caseId}` créé : {caseLink}";
                case Language.es:
                    return $"Caso `# {caseId}` creado: {caseLink}";
                case Language.ru:
                    return $"Обращение `# {caseId}` создано: {caseLink}";
                case Language.it:
                    return $"Caso `#{caseId}` creato: {caseLink}";
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
                case Language.fr:
                    return $"Un administrateur de site peut enregistrer une guilde à l'adresse : {url}";
                case Language.es:
                    return $"Un administrador de sitio puede registrar un gremio en: {url}";
                case Language.ru:
                    return $"Администратор сайта может зарегистрировать гильдию по адресу: {url}";
                case Language.it:
                    return $"Un amministratore del sito può registrare una gilda su: {url}";
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
                case Language.fr:
                    return "Échec de l'envoi de la notification interne aux modérateurs pour la commande de rapport.";
                case Language.es:
                    return "No se pudo enviar una notificación interna a los moderadores para el comando de informe.";
                case Language.ru:
                    return "Не удалось отправить внутреннее уведомление модераторам для команды отчета.";
                case Language.it:
                    return "Impossibile inviare una notifica interna ai moderatori per il comando di segnalazione.";
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
                case Language.fr:
                    return "Rapport envoyé.";
                case Language.es:
                    return "Reporte enviado.";
                case Language.ru:
                    return "Отчет отправлен.";
                case Language.it:
                    return "Rapporto inviato.";
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
                case Language.fr:
                    return $"{user.Mention} a signalé un message de {message.Author.Mention} dans {message.Channel.Mention}.\n{message.JumpLink}";
                case Language.es:
                    return $"{user.Mention} informó un mensaje de {message.Author.Mention} en {message.Channel.Mention}.\n{message.JumpLink}";
                case Language.ru:
                    return $"{user.Mention} сообщил о сообщении от {message.Author.Mention} в {message.Channel.Mention}.\n{message.JumpLink}";
                case Language.it:
                    return $"{user.Mention} ha segnalato un messaggio da {message.Author.Mention} in {message.Channel.Mention}.\n{message.JumpLink}";
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
                case Language.fr:
                    return "Échec de l'envoi du message";
                case Language.es:
                    return "No se pudo enviar el mensaje";
                case Language.ru:
                    return "Не удалось отправить сообщение";
                case Language.it:
                    return "Impossibile inviare il messaggio";
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
                case Language.fr:
                    return "Message envoyé.";
                case Language.es:
                    return "Mensaje enviado.";
                case Language.ru:
                    return "Сообщение отправлено.";
                case Language.it:
                    return "Messaggio inviato.";
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
                case Language.fr:
                    return "L'invitation n'est pas de cette guilde.";
                case Language.es:
                    return "La invitación no es de este gremio.";
                case Language.ru:
                    return "Приглашение не из этой гильдии.";
                case Language.it:
                    return "L'invito non è di questa gilda.";
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
                case Language.fr:
                    return "Impossible de trouver l'invitation dans la base de données ou dans cette guilde.";
                case Language.es:
                    return "No se pudo encontrar la invitación en la base de datos o en este gremio.";
                case Language.ru:
                    return "Не удалось найти инвайт в базе данных или в этой гильдии.";
                case Language.it:
                    return "Impossibile trovare l'invito nel database o in questa gilda.";
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
                case Language.fr:
                    return "Échec de la récupération de l'invitation.";
                case Language.es:
                    return "No se pudo recuperar la invitación.";
                case Language.ru:
                    return "Не удалось получить приглашение.";
                case Language.it:
                    return "Impossibile recuperare l'invito.";
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
                case Language.fr:
                    return $"`{inviteCode}` créé à {createdAt.ToDiscordTS()}.";
                case Language.es:
                    return $"`{inviteCode}` creado en {createdAt.ToDiscordTS ()}.";
                case Language.ru:
                    return $"`{inviteCode}` создан в {createdAt.ToDiscordTS ()}.";
                case Language.it:
                    return $"`{inviteCode}` creato su {createdAt.ToDiscordTS()}.";
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
                case Language.fr:
                    return $"`{inviteCode}` créé par {createdBy.Mention}.";
                case Language.es:
                    return $"`{inviteCode}` creado por {createdBy.Mention}.";
                case Language.ru:
                    return $"`{inviteCode}` создан {createdBy.Mention}.";
                case Language.it:
                    return $"`{inviteCode}` creato da {createdBy.Mention}.";
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
                case Language.fr:
                    return $"`{inviteCode}` créé par {createdBy.Mention} à {createdAt.ToDiscordTS()}.";
                case Language.es:
                    return $"`{inviteCode}` creado por {createdBy.Mention} en {createdAt.ToDiscordTS ()}.";
                case Language.ru:
                    return $"`{inviteCode}` создан {createdBy.Mention} в {createdAt.ToDiscordTS ()}.";
                case Language.it:
                    return $"`{inviteCode}` creato da {createdBy.Mention} su {createdAt.ToDiscordTS()}.";
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
                case Language.fr:
                    return "Cette invitation n'a pas encore été suivie par MASZ.";
                case Language.es:
                    return "MASZ aún no ha realizado el seguimiento de esta invitación.";
                case Language.ru:
                    return "Это приглашение еще не отслежено MASZ.";
                case Language.it:
                    return "Questo invito non è stato ancora monitorato da MASZ.";
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
                case Language.fr:
                    return $"Utilisé par [{count}]";
                case Language.es:
                    return $"Usado por [{count}]";
                case Language.ru:
                    return $"Используется [{count}]";
                case Language.it:
                    return $"Utilizzato da [{count}]";
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
                case Language.fr:
                    return "Veuillez spécifier un identifiant de guilde valide.";
                case Language.es:
                    return "Por favor, especifique un guildid válido.";
                case Language.ru:
                    return "Укажите действующего гильдида.";
                case Language.it:
                    return "Si prega di specificare un ID gilda valido.";
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
                case Language.fr:
                    return "Vous n'êtes pas autorisé à voir ce cas.";
                case Language.es:
                    return "No se le permite ver este caso.";
                case Language.ru:
                    return "Вам не разрешено просматривать это дело.";
                case Language.it:
                    return "Non sei autorizzato a visualizzare questo caso.";
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
                case Language.fr:
                    return $"Invitation utilisée `{inviteCode}`.";
                case Language.es:
                    return $"Invitación usada `{inviteCode}`.";
                case Language.ru:
                    return $"Использовал инвайт `{inviteCode}`.";
                case Language.it:
                    return $"Invito usato `{inviteCode}`.";
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
                case Language.fr:
                    return $"Par <@{user}>.";
                case Language.es:
                    return $"Por <@{user}>.";
                case Language.ru:
                    return $"Автор <@{user}>.";
                case Language.it:
                    return $"Da <@{user}>.";
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
                case Language.fr:
                    return "Il n'y a pas de cas pour cet utilisateur.";
                case Language.es:
                    return "No hay casos para este usuario.";
                case Language.ru:
                    return "Для этого пользователя нет случаев.";
                case Language.it:
                    return "Non ci sono casi per questo utente.";
            }
            return "There are no cases for this user.";
        }
        public string GuildAuditLogChannel() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Channel";
                case Language.de:
                    return "Kanal";
                case Language.at:
                    return "Kanal";
                case Language.fr:
                    return "Canaliser";
                case Language.es:
                    return "Canal";
                case Language.ru:
                    return "Канал";
                case Language.it:
                    return "Canale";
            }
            return "Channel";
        }
        public string GuildAuditLogChannelId() {
            switch (preferredLanguage) {
                case Language.en:
                    return "ChannelId";
                case Language.de:
                    return "KanalId";
                case Language.at:
                    return "KanalId";
                case Language.fr:
                    return "Identifiant de la chaine";
                case Language.es:
                    return "Canal ID";
                case Language.ru:
                    return "ChannelId";
                case Language.it:
                    return "Canale ID";
            }
            return "ChannelId";
        }
        public string GuildAuditLogID() {
            switch (preferredLanguage) {
                case Language.en:
                    return "ID";
                case Language.de:
                    return "ID";
                case Language.at:
                    return "ID";
                case Language.fr:
                    return "identifiant";
                case Language.es:
                    return "IDENTIFICACIÓN";
                case Language.ru:
                    return "Я БЫ";
                case Language.it:
                    return "ID";
            }
            return "ID";
        }
        public string GuildAuditLogUserID() {
            switch (preferredLanguage) {
                case Language.en:
                    return "UserId";
                case Language.de:
                    return "NutzerId";
                case Language.at:
                    return "NutzaId";
                case Language.fr:
                    return "Identifiant d'utilisateur";
                case Language.es:
                    return "UserId";
                case Language.ru:
                    return "ID пользователя";
                case Language.it:
                    return "ID utente";
            }
            return "UserId";
        }
        public string GuildAuditLogUser() {
            switch (preferredLanguage) {
                case Language.en:
                    return "User";
                case Language.de:
                    return "Nutzer";
                case Language.at:
                    return "Nutza";
                case Language.fr:
                    return "Utilisateur";
                case Language.es:
                    return "Usuario";
                case Language.ru:
                    return "Пользователь";
                case Language.it:
                    return "Utente";
            }
            return "User";
        }
        public string GuildAuditLogAuthor() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Author";
                case Language.de:
                    return "Autor";
                case Language.at:
                    return "Autor";
                case Language.fr:
                    return "Auteur";
                case Language.es:
                    return "Autor";
                case Language.ru:
                    return "Автор";
                case Language.it:
                    return "Autore";
            }
            return "Author";
        }
        public string GuildAuditLogCreated() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Created";
                case Language.de:
                    return "Erstellt";
                case Language.at:
                    return "Erstöt";
                case Language.fr:
                    return "Créé";
                case Language.es:
                    return "Creado";
                case Language.ru:
                    return "Созданный";
                case Language.it:
                    return "Creato";
            }
            return "Created";
        }
        public string GuildAuditLogCouldNotFetch() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Could not fetch.";
                case Language.de:
                    return "Information konnte nicht abgerufen werden.";
                case Language.at:
                    return "Info konnt ned orufn werdn.";
                case Language.fr:
                    return "Impossible de récupérer.";
                case Language.es:
                    return "No se pudo recuperar.";
                case Language.ru:
                    return "Не удалось получить.";
                case Language.it:
                    return "Impossibile recuperare.";
            }
            return "Could not fetch.";
        }
        public string GuildAuditLogNotFoundInCache() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Information not cached.";
                case Language.de:
                    return "Information nicht im Cache.";
                case Language.at:
                    return "Info ned im Cache.";
                case Language.fr:
                    return "Informations non mises en cache.";
                case Language.es:
                    return "Información no almacenada en caché.";
                case Language.ru:
                    return "Информация не кешируется.";
                case Language.it:
                    return "Informazioni non memorizzate nella cache.";
            }
            return "Information not cached.";
        }
        public string GuildAuditLogMessageSentTitle() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Message sent";
                case Language.de:
                    return "Nachricht gesendet";
                case Language.at:
                    return "Nochricht gsendet";
                case Language.fr:
                    return "Message envoyé";
                case Language.es:
                    return "Mensaje enviado";
                case Language.ru:
                    return "Сообщение отправлено";
                case Language.it:
                    return "Messaggio inviato";
            }
            return "Message sent";
        }
        public string GuildAuditLogMessageSentContent() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Content";
                case Language.de:
                    return "Inhalt";
                case Language.at:
                    return "Inhalt";
                case Language.fr:
                    return "Teneur";
                case Language.es:
                    return "Contenido";
                case Language.ru:
                    return "Содержание";
                case Language.it:
                    return "Contenuto";
            }
            return "Content";
        }
        public string GuildAuditLogMessageUpdatedTitle() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Message edited";
                case Language.de:
                    return "Nachricht aktualisiert";
                case Language.at:
                    return "Nochricht aktualisiert";
                case Language.fr:
                    return "Message modifié";
                case Language.es:
                    return "Mensaje editado";
                case Language.ru:
                    return "Сообщение отредактировано";
                case Language.it:
                    return "Messaggio modificato";
            }
            return "Message edited";
        }
        public string GuildAuditLogMessageUpdatedContentBefore() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Before";
                case Language.de:
                    return "Zuvor";
                case Language.at:
                    return "Davoa";
                case Language.fr:
                    return "Avant";
                case Language.es:
                    return "Antes";
                case Language.ru:
                    return "До";
                case Language.it:
                    return "Prima";
            }
            return "Before";
        }
        public string GuildAuditLogMessageUpdatedContentNew() {
            switch (preferredLanguage) {
                case Language.en:
                    return "New";
                case Language.de:
                    return "Neu";
                case Language.at:
                    return "Neich";
                case Language.fr:
                    return "Nouveau";
                case Language.es:
                    return "Nuevo";
                case Language.ru:
                    return "Новый";
                case Language.it:
                    return "Nuovo";
            }
            return "New";
        }
        public string GuildAuditLogMessageDeletedTitle() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Message deleted";
                case Language.de:
                    return "Nachricht gelöscht";
                case Language.at:
                    return "Nochricht gelöscht";
                case Language.fr:
                    return "Message supprimé";
                case Language.es:
                    return "Mensaje borrado";
                case Language.ru:
                    return "Сообщение удалено";
                case Language.it:
                    return "Messaggio cancellato";
            }
            return "Message deleted";
        }
        public string GuildAuditLogMessageDeletedContent() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Content";
                case Language.de:
                    return "Inhalt";
                case Language.at:
                    return "Inhalt";
                case Language.fr:
                    return "Teneur";
                case Language.es:
                    return "Contenido";
                case Language.ru:
                    return "Содержание";
                case Language.it:
                    return "Contenuto";
            }
            return "Content";
        }
        public string GuildAuditLogBanAddedTitle() {
            switch (preferredLanguage) {
                case Language.en:
                    return "User banned";
                case Language.de:
                    return "Nutzer gebannt";
                case Language.at:
                    return "Nutza ausgsperrt";
                case Language.fr:
                    return "Utilisateur banni";
                case Language.es:
                    return "Usuario baneado";
                case Language.ru:
                    return "Пользователь забанен";
                case Language.it:
                    return "Utente bannato";
            }
            return "User banned";
        }
        public string GuildAuditLogBanRemovedTitle() {
            switch (preferredLanguage) {
                case Language.en:
                    return "User unbanned";
                case Language.de:
                    return "Nutzer entbannt";
                case Language.at:
                    return "Nutza nimma ausgsperrt";
                case Language.fr:
                    return "Utilisateur non banni";
                case Language.es:
                    return "Usuario no prohibido";
                case Language.ru:
                    return "Пользователь разблокирован";
                case Language.it:
                    return "Utente non bannato";
            }
            return "User unbanned";
        }
        public string GuildAuditLogInviteCreatedTitle() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Invite created";
                case Language.de:
                    return "Einladung erstellt";
                case Language.at:
                    return "Eiladung erstöt";
                case Language.fr:
                    return "Invitation créée";
                case Language.es:
                    return "Invitación creada";
                case Language.ru:
                    return "Приглашение создано";
                case Language.it:
                    return "Invito creato";
            }
            return "Invite created";
        }
        public string GuildAuditLogInviteCreatedURL() {
            switch (preferredLanguage) {
                case Language.en:
                    return "URL";
                case Language.de:
                    return "URL";
                case Language.at:
                    return "URL";
                case Language.fr:
                    return "URL";
                case Language.es:
                    return "URL";
                case Language.ru:
                    return "URL";
                case Language.it:
                    return "URL";
            }
            return "URL";
        }
        public string GuildAuditLogInviteCreatedMaxUses() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Max uses";
                case Language.de:
                    return "Maximale Nutzungen";
                case Language.at:
                    return "Maximale Vawednungen";
                case Language.fr:
                    return "Utilisations maximales";
                case Language.es:
                    return "Usos máximos";
                case Language.ru:
                    return "Макс использует";
                case Language.it:
                    return "Usi massimi";
            }
            return "Max uses";
        }
        public string GuildAuditLogInviteCreatedExpiration() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Expiration date";
                case Language.de:
                    return "Ablaufdatum";
                case Language.at:
                    return "Oblaufdatum";
                case Language.fr:
                    return "Date d'expiration";
                case Language.es:
                    return "Fecha de caducidad";
                case Language.ru:
                    return "Срок хранения";
                case Language.it:
                    return "Data di scadenza";
            }
            return "Expiration date";
        }
        public string GuildAuditLogInviteCreatedTargetChannel() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Target channel";
                case Language.de:
                    return "Zielkanal";
                case Language.at:
                    return "Zielkanal";
                case Language.fr:
                    return "Canal cible";
                case Language.es:
                    return "Canal objetivo";
                case Language.ru:
                    return "Целевой канал";
                case Language.it:
                    return "Canale di destinazione";
            }
            return "Target channel";
        }
        public string GuildAuditLogInviteDeletedTitle() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Invite deleted";
                case Language.de:
                    return "Einladung gelöscht";
                case Language.at:
                    return "Eiladung glescht";
                case Language.fr:
                    return "Invitation supprimée";
                case Language.es:
                    return "Invitación eliminada";
                case Language.ru:
                    return "Приглашение удалено";
                case Language.it:
                    return "Invito cancellato";
            }
            return "Invite deleted";
        }
        public string GuildAuditLogMemberJoinedTitle() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Member joined";
                case Language.de:
                    return "Mitglied beigetreten";
                case Language.at:
                    return "Mitglied beitretn";
                case Language.fr:
                    return "Membre rejoint";
                case Language.es:
                    return "Miembro se unió";
                case Language.ru:
                    return "Участник присоединился";
                case Language.it:
                    return "Membro iscritto";
            }
            return "Member joined";
        }
        public string GuildAuditLogMemberJoinedRegistered() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Registered";
                case Language.de:
                    return "Registriert";
                case Language.at:
                    return "Registriat";
                case Language.fr:
                    return "Inscrit";
                case Language.es:
                    return "Registrado";
                case Language.ru:
                    return "Зарегистрировано";
                case Language.it:
                    return "Registrato";
            }
            return "Registered";
        }
        public string GuildAuditLogMemberRemovedTitle() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Member removed";
                case Language.de:
                    return "Mitglied entfernt";
                case Language.at:
                    return "Mitglied entfernt";
                case Language.fr:
                    return "Membre supprimé";
                case Language.es:
                    return "Miembro eliminado";
                case Language.ru:
                    return "Участник удален";
                case Language.it:
                    return "Membro rimosso";
            }
            return "Member removed";
        }
        public string GuildAuditLogThreadCreatedTitle() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Thread created";
                case Language.de:
                    return "Thread erstellt";
                case Language.at:
                    return "Thread erstöt";
                case Language.fr:
                    return "Fil créé";
                case Language.es:
                    return "Hilo creado";
                case Language.ru:
                    return "Тема создана";
                case Language.it:
                    return "Discussione creata";
            }
            return "Thread created";
        }
        public string GuildAuditLogThreadCreatedParent() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Parent";
                case Language.de:
                    return "Elternkanal";
                case Language.at:
                    return "Eltankanal";
                case Language.fr:
                    return "Parent";
                case Language.es:
                    return "Padre";
                case Language.ru:
                    return "Родитель";
                case Language.it:
                    return "Genitore";
            }
            return "Parent";
        }
        public string GuildAuditLogThreadCreatedCreator() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Creator";
                case Language.de:
                    return "Ersteller";
                case Language.at:
                    return "Erstölla";
                case Language.fr:
                    return "Créateur";
                case Language.es:
                    return "Creador";
                case Language.ru:
                    return "Создатель";
                case Language.it:
                    return "Creatore";
            }
            return "Creator";
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
                        case Language.fr:
                            return "Muet";
                        case Language.es:
                            return "Silencio";
                        case Language.ru:
                            return "Немой";
                        case Language.it:
                            return "Muto";
                        default:
                            return "Mute";
                    }
                case masz.Enums.PunishmentType.Ban:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Ban";
                        case Language.de:
                            return "Bann";
                        case Language.at:
                            return "Rauswuaf";
                        case Language.fr:
                            return "Interdire";
                        case Language.es:
                            return "Prohibición";
                        case Language.ru:
                            return "Запретить";
                        case Language.it:
                            return "Bandire";
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
                        case Language.fr:
                            return "Coup";
                        case Language.es:
                            return "Patear";
                        case Language.ru:
                            return "Пинать";
                        case Language.it:
                            return "Calcio";
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
                        case Language.fr:
                            return "Avertir";
                        case Language.es:
                            return "Advertir";
                        case Language.ru:
                            return "Предупреждать";
                        case Language.it:
                            return "Avvisare";
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
                        case Language.fr:
                            return "Soi";
                        case Language.es:
                            return "Uno mismo";
                        case Language.ru:
                            return "Себя";
                        case Language.it:
                            return "Se stesso";
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
                        case Language.fr:
                            return "Guilde";
                        case Language.es:
                            return "Gremio";
                        case Language.ru:
                            return "Гильдия";
                        case Language.it:
                            return "Gilda";
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
                        case Language.fr:
                            return "Global";
                        case Language.es:
                            return "Global";
                        case Language.ru:
                            return "Глобальный";
                        case Language.it:
                            return "Globale";
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
                        case Language.fr:
                            return "Pas d'action";
                        case Language.es:
                            return "Ninguna acción";
                        case Language.ru:
                            return "Бездействие";
                        case Language.it:
                            return "Nessuna azione";
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
                        case Language.fr:
                            return "Contenu supprimé";
                        case Language.es:
                            return "Contenido eliminado";
                        case Language.ru:
                            return "Контент удален";
                        case Language.it:
                            return "Contenuto eliminato";
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
                        case Language.fr:
                            return "Cas créé";
                        case Language.es:
                            return "Caso creado";
                        case Language.ru:
                            return "Дело создано";
                        case Language.it:
                            return "Caso creato";
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
                        case Language.fr:
                            return "Contenu supprimé et dossier créé";
                        case Language.es:
                            return "Contenido eliminado y caso creado";
                        case Language.ru:
                            return "Контент удален, а дело создано";
                        case Language.it:
                            return "Contenuto eliminato e caso creato";
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
                        case Language.fr:
                            return "Invitation publiée";
                        case Language.es:
                            return "Invitación publicada";
                        case Language.ru:
                            return "Приглашение опубликовано";
                        case Language.it:
                            return "Invito pubblicato";
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
                        case Language.fr:
                            return "Trop d'émoticônes utilisées";
                        case Language.es:
                            return "Demasiados emotes usados";
                        case Language.ru:
                            return "Использовано слишком много эмоций";
                        case Language.it:
                            return "Troppe emoticon usate";
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
                        case Language.fr:
                            return "Trop d'utilisateurs mentionnés";
                        case Language.es:
                            return "Demasiados usuarios mencionados";
                        case Language.ru:
                            return "Упомянуто слишком много пользователей";
                        case Language.it:
                            return "Troppi utenti citati";
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
                        case Language.fr:
                            return "Trop de pièces jointes utilisées";
                        case Language.es:
                            return "Se han utilizado demasiados archivos adjuntos";
                        case Language.ru:
                            return "Использовано слишком много вложений";
                        case Language.it:
                            return "Troppi allegati utilizzati";
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
                        case Language.fr:
                            return "Trop d'intégrations utilisées";
                        case Language.es:
                            return "Se han utilizado demasiados elementos incrustados";
                        case Language.ru:
                            return "Использовано слишком много закладных";
                        case Language.it:
                            return "Troppi incorporamenti utilizzati";
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
                        case Language.fr:
                            return "Trop de modérations automatiques";
                        case Language.es:
                            return "Demasiadas moderaciones automáticas";
                        case Language.ru:
                            return "Слишком много автоматических модераций";
                        case Language.it:
                            return "Troppe moderazioni automatiche";
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
                        case Language.fr:
                            return "Filtre de mots personnalisé déclenché";
                        case Language.es:
                            return "Filtro de palabras personalizado activado";
                        case Language.ru:
                            return "Пользовательский фильтр слов активирован";
                        case Language.it:
                            return "Filtro parole personalizzato attivato";
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
                        case Language.fr:
                            return "Trop de messages";
                        case Language.es:
                            return "Demasiados mensajes";
                        case Language.ru:
                            return "Слишком много сообщений";
                        case Language.it:
                            return "Troppi messaggi";
                        default:
                            return "Too many messages";
                    }
                case masz.Enums.AutoModerationType.TooManyDuplicatedCharacters:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Too many duplicated characters used";
                        case Language.de:
                            return "Zu viele wiederholende Buchstaben verwendet";
                        case Language.at:
                            return "Zu vü wiedaholende Buchstobn vawendet";
                        case Language.fr:
                            return "Trop de caractères dupliqués utilisés";
                        case Language.es:
                            return "Se han utilizado demasiados caracteres duplicados";
                        case Language.ru:
                            return "Использовано слишком много повторяющихся символов";
                        case Language.it:
                            return "Troppi caratteri duplicati utilizzati";
                        default:
                            return "Too many duplicated characters used";
                    }
                case masz.Enums.AutoModerationType.TooManyLinks:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Too many links used";
                        case Language.de:
                            return "Zu viele Links verwendet";
                        case Language.at:
                            return "Zu vü Links vawendet";
                        case Language.fr:
                            return "Trop de liens utilisés";
                        case Language.es:
                            return "Se han utilizado demasiados enlaces";
                        case Language.ru:
                            return "Использовано слишком много ссылок";
                        case Language.it:
                            return "Troppi link utilizzati";
                        default:
                            return "Too many links used";
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
                        case Language.fr:
                            return "Erreur inconnue";
                        case Language.es:
                            return "Error desconocido";
                        case Language.ru:
                            return "Неизвестная ошибка";
                        case Language.it:
                            return "Errore sconosciuto";
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
                        case Language.fr:
                            return "Utilisateur discord invalide";
                        case Language.es:
                            return "Usuario de discordia no válido";
                        case Language.ru:
                            return "Недействительный пользователь Discord";
                        case Language.it:
                            return "Utente discord non valido";
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
                        case Language.fr:
                            return "L'utilisateur est protégé";
                        case Language.es:
                            return "El usuario está protegido";
                        case Language.ru:
                            return "Пользователь защищен";
                        case Language.it:
                            return "L'utente è protetto";
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
                        case Language.fr:
                            return "L'utilisateur est protégé. C'est un robot.";
                        case Language.es:
                            return "El usuario está protegido. El es un bot.";
                        case Language.ru:
                            return "Пользователь защищен. Он бот.";
                        case Language.it:
                            return "L'utente è protetto. Lui è un bot.";
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
                        case Language.fr:
                            return "L'utilisateur est protégé. Il est administrateur du site.";
                        case Language.es:
                            return "El usuario está protegido. Es administrador de un sitio.";
                        case Language.ru:
                            return "Пользователь защищен. Он администратор сайта.";
                        case Language.it:
                            return "L'utente è protetto. È un amministratore del sito.";
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
                        case Language.fr:
                            return "L'utilisateur est protégé. Il est membre de l'équipe.";
                        case Language.es:
                            return "El usuario está protegido. Es un miembro del equipo.";
                        case Language.ru:
                            return "Пользователь защищен. Он член команды.";
                        case Language.it:
                            return "L'utente è protetto. È un membro della squadra.";
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
                        case Language.fr:
                            return "Ressource introuvable";
                        case Language.es:
                            return "Recurso no encontrado";
                        case Language.ru:
                            return "Ресурс не найден";
                        case Language.it:
                            return "Risorsa non trovata";
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
                        case Language.fr:
                            return "Identité invalide";
                        case Language.es:
                            return "Identidad inválida";
                        case Language.ru:
                            return "Неверная личность";
                        case Language.it:
                            return "Identità non valida";
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
                        case Language.fr:
                            return "La guilde n'est pas enregistrée";
                        case Language.es:
                            return "El gremio no está registrado";
                        case Language.ru:
                            return "Гильдия не зарегистрирована";
                        case Language.it:
                            return "La gilda non è registrata";
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
                        case Language.fr:
                            return "Non autorisé";
                        case Language.es:
                            return "No autorizado";
                        case Language.ru:
                            return "Неавторизованный";
                        case Language.it:
                            return "non autorizzato";
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
                        case Language.fr:
                            return "La guilde n'a pas de rôle défini pour la punition muette.";
                        case Language.es:
                            return "El gremio no tiene roles definidos para el castigo mudo.";
                        case Language.ru:
                            return "У гильдии нет определенных ролей для немого наказания.";
                        case Language.it:
                            return "La gilda non ha ruoli definiti per la punizione muta.";
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
                        case Language.fr:
                            return "Modcase est marqué pour être supprimé";
                        case Language.es:
                            return "Modcase está marcado para ser eliminado";
                        case Language.ru:
                            return "Modcase отмечен для удаления";
                        case Language.it:
                            return "Modcase è contrassegnato per essere eliminato";
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
                        case Language.fr:
                            return "Modcase n'est pas marqué pour être supprimé";
                        case Language.es:
                            return "Modcase no está marcado para ser eliminado";
                        case Language.ru:
                            return "Modcase не отмечен для удаления";
                        case Language.it:
                            return "Modcase non è contrassegnato per essere eliminato";
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
                        case Language.fr:
                            return "La guilde est déjà enregistrée";
                        case Language.es:
                            return "El gremio ya está registrado";
                        case Language.ru:
                            return "Гильдия уже зарегистрирована";
                        case Language.it:
                            return "La gilda è già registrata";
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
                        case Language.fr:
                            return "Cette action n'est pas autorisée en mode démo";
                        case Language.es:
                            return "Esta acción no está permitida en el modo de demostración.";
                        case Language.ru:
                            return "Это действие запрещено в демонстрационном режиме.";
                        case Language.it:
                            return "Questa azione non è consentita in modalità demo";
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
                        case Language.fr:
                            return "Rôle introuvable";
                        case Language.es:
                            return "Rol no encontrado";
                        case Language.ru:
                            return "Роль не найдена";
                        case Language.it:
                            return "Ruolo non trovato";
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
                        case Language.fr:
                            return "Les jetons ne peuvent pas gérer cette ressource";
                        case Language.es:
                            return "Los tokens no pueden administrar este recurso";
                        case Language.ru:
                            return "Лексемы не могут управлять этим ресурсом";
                        case Language.it:
                            return "I token non possono gestire questa risorsa";
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
                        case Language.fr:
                            return "Le jeton est déjà enregistré";
                        case Language.es:
                            return "El token ya está registrado";
                        case Language.ru:
                            return "Токен уже зарегистрирован";
                        case Language.it:
                            return "Il token è già registrato";
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
                        case Language.fr:
                            return "Les deux utilisateurs sont les mêmes.";
                        case Language.es:
                            return "Ambos usuarios son iguales.";
                        case Language.ru:
                            return "Оба пользователя одинаковые.";
                        case Language.it:
                            return "Entrambi gli utenti sono gli stessi.";
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
                        case Language.fr:
                            return "La ressource existe déjà";
                        case Language.es:
                            return "El recurso ya existe";
                        case Language.ru:
                            return "Ресурс уже существует";
                        case Language.it:
                            return "La risorsa esiste già";
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
                        case Language.fr:
                            return "Les commentaires sont verrouillés pour ce modcase";
                        case Language.es:
                            return "Los comentarios están bloqueados para este modcase";
                        case Language.ru:
                            return "Комментарии заблокированы для этого мода";
                        case Language.it:
                            return "I commenti sono bloccati per questo modcase";
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
                        case Language.fr:
                            return "Le dernier commentaire était déjà du suspect.";
                        case Language.es:
                            return "El último comentario ya era del sospechoso.";
                        case Language.ru:
                            return "Последний комментарий уже был от подозреваемого.";
                        case Language.it:
                            return "L'ultimo commento era già del sospettato.";
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
                        case Language.fr:
                            return "Action de modération automatique non valide";
                        case Language.es:
                            return "Acción de automoderación no válida";
                        case Language.ru:
                            return "Недопустимое действие автомодерации";
                        case Language.it:
                            return "Azione di moderazione automatica non valida";
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
                        case Language.fr:
                            return "Type d'automodération non valide";
                        case Language.es:
                            return "Tipo de automoderación no válido";
                        case Language.ru:
                            return "Неверный тип автомодерации.";
                        case Language.it:
                            return "Tipo di moderazione automatica non valido";
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
                        case Language.fr:
                            return "L'utilisateur a atteint la limite maximale de modèles";
                        case Language.es:
                            return "El usuario ha alcanzado el límite máximo de plantillas";
                        case Language.ru:
                            return "Пользователь достиг максимального предела шаблонов";
                        case Language.it:
                            return "L'utente ha raggiunto il limite massimo di modelli";
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
                        case Language.fr:
                            return "Chemin de fichier invalide";
                        case Language.es:
                            return "Ruta de archivo no válida";
                        case Language.ru:
                            return "Неверный путь к файлу";
                        case Language.it:
                            return "Percorso file non valido";
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
                        case Language.fr:
                            return "Il n'y a pas de guildes enregistrées";
                        case Language.es:
                            return "No hay gremios registrados";
                        case Language.ru:
                            return "Нет зарегистрированных гильдий";
                        case Language.it:
                            return "Non ci sono gilde registrate";
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
                        case Language.fr:
                            return "Cette action n'est utilisable que dans une guilde";
                        case Language.es:
                            return "Esta acción solo se puede usar en un gremio.";
                        case Language.ru:
                            return "Это действие доступно только в гильдии.";
                        case Language.it:
                            return "Questa azione è utilizzabile solo in una gilda";
                        default:
                            return "This action is only usable in a guild";
                    }
                case masz.Enums.APIError.InvalidAuditLogEvent:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Invalid auditlogevent type";
                        case Language.de:
                            return "Ungültiger Auditlogeventstyp";
                        case Language.at:
                            return "Ungütiga oduitlogeventstyp";
                        case Language.fr:
                            return "Type d'événement auditlog non valide";
                        case Language.es:
                            return "Tipo de evento de auditoría no válido";
                        case Language.ru:
                            return "Неверный тип auditlogevent";
                        case Language.it:
                            return "Tipo di evento auditlog non valido";
                        default:
                            return "Invalid auditlogevent type";
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
                        case Language.fr:
                            return "Défaut";
                        case Language.es:
                            return "Defecto";
                        case Language.ru:
                            return "Дефолт";
                        case Language.it:
                            return "Predefinito";
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
                        case Language.fr:
                            return "Le cas est automodéré.";
                        case Language.es:
                            return "El caso está autoderado.";
                        case Language.ru:
                            return "Корпус автоматический.";
                        case Language.it:
                            return "Il caso è moderato automaticamente.";
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
                        case Language.fr:
                            return "Le cas est importé.";
                        case Language.es:
                            return "El caso es importado.";
                        case Language.ru:
                            return "Корпус импортный.";
                        case Language.it:
                            return "Il caso è importato.";
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
                        case Language.fr:
                            return "Cas créé par commande.";
                        case Language.es:
                            return "Caso creado por comando.";
                        case Language.ru:
                            return "Дело создано командой.";
                        case Language.it:
                            return "Caso creato da comando.";
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
                        case Language.fr:
                            return "Anglais";
                        case Language.es:
                            return "inglés";
                        case Language.ru:
                            return "английский";
                        case Language.it:
                            return "inglese";
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
                        case Language.fr:
                            return "Allemand";
                        case Language.es:
                            return "alemán";
                        case Language.ru:
                            return "Немецкий";
                        case Language.it:
                            return "Tedesco";
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
                        case Language.fr:
                            return "français";
                        case Language.es:
                            return "francés";
                        case Language.ru:
                            return "французкий язык";
                        case Language.it:
                            return "francese";
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
                        case Language.fr:
                            return "Espagnol";
                        case Language.es:
                            return "Español";
                        case Language.ru:
                            return "испанский";
                        case Language.it:
                            return "spagnolo";
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
                        case Language.fr:
                            return "italien";
                        case Language.es:
                            return "italiano";
                        case Language.ru:
                            return "Итальянский";
                        case Language.it:
                            return "italiano";
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
                        case Language.fr:
                            return "autrichien";
                        case Language.es:
                            return "austriaco";
                        case Language.ru:
                            return "Австрийский";
                        case Language.it:
                            return "austriaco";
                        default:
                            return "Austrian";
                    }
                case masz.Enums.Language.ru:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Russian";
                        case Language.de:
                            return "Russisch";
                        case Language.at:
                            return "Rusisch";
                        case Language.fr:
                            return "Russe";
                        case Language.es:
                            return "Ruso";
                        case Language.ru:
                            return "Русский";
                        case Language.it:
                            return "Russo";
                        default:
                            return "Russian";
                    }
            }
            return "Unknown";
        }

    }
}

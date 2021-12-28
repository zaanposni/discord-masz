using Discord;
using MASZ.Extensions;
using MASZ.Enums;
using MASZ.Models;

namespace MASZ.Utils
{
    public class Translation
    {
        public Language PreferredLanguage { get; set; }
        private Translation(Language preferredLanguage = Language.en)
        {
            PreferredLanguage = preferredLanguage;
        }
        public static Translation Ctx(Language preferredLanguage = Language.en)
        {
            return new Translation(preferredLanguage);
        }
        public string Features()
        {
            return PreferredLanguage switch
            {
                Language.de => "Features",
                Language.at => "Features",
                Language.fr => "Caractéristiques",
                Language.es => "Características",
                Language.ru => "Функции",
                Language.it => "Caratteristiche",
                _ => "Features",
            };
        }
        public string Automoderation()
        {
            return PreferredLanguage switch
            {
                Language.de => "Automoderation",
                Language.at => "Automodaration",
                Language.fr => "Automodération",
                Language.es => "Automoderación",
                Language.ru => "Автомобильная промышленность",
                Language.it => "Automoderazione",
                _ => "Automoderation",
            };
        }
        public string Action()
        {
            return PreferredLanguage switch
            {
                Language.de => "Aktion",
                Language.at => "Aktio",
                Language.fr => "action",
                Language.es => "Acción",
                Language.ru => "Действие",
                Language.it => "Azione",
                _ => "Action",
            };
        }
        public string NotFound()
        {
            return PreferredLanguage switch
            {
                Language.de => "Nicht gefunden.",
                Language.at => "Ned gfundn.",
                Language.fr => "Pas trouvé.",
                Language.es => "Extraviado.",
                Language.ru => "Не найден.",
                Language.it => "Non trovato.",
                _ => "Not found.",
            };
        }
        public string Author()
        {
            return PreferredLanguage switch
            {
                Language.de => "Autor",
                Language.at => "Autoa",
                Language.fr => "Auteur",
                Language.es => "Autor",
                Language.ru => "Автор",
                Language.it => "Autore",
                _ => "Author",
            };
        }
        public string MessageContent()
        {
            return PreferredLanguage switch
            {
                Language.de => "Nachrichteninhalt",
                Language.at => "Nochrichtninhoit",
                Language.fr => "Contenu du message",
                Language.es => "Contenido del mensaje",
                Language.ru => "Содержание сообщения",
                Language.it => "Contenuto del messaggio",
                _ => "Message content",
            };
        }
        public string ViewDetailsOn(string url)
        {
            return PreferredLanguage switch
            {
                Language.de => $"Details anzeigen auf: {url}",
                Language.at => $"Details ozeign auf: {url}",
                Language.fr => $"Voir les détails sur : {url}",
                Language.es => $"Ver detalles en: {url}",
                Language.ru => $"Подробнее о: {url}",
                Language.it => $"Visualizza dettagli su: {url}",
                _ => $"View details on: {url}",
            };
        }
        public string Attachments()
        {
            return PreferredLanguage switch
            {
                Language.de => "Anhänge",
                Language.at => "Ohäng",
                Language.fr => "Pièces jointes",
                Language.es => "Archivos adjuntos",
                Language.ru => "Вложения",
                Language.it => "Allegati",
                _ => "Attachments",
            };
        }
        public string Attachment()
        {
            return PreferredLanguage switch
            {
                Language.de => "Anhang",
                Language.at => "Ohang",
                Language.fr => "Attachement",
                Language.es => "Adjunto",
                Language.ru => "Вложение",
                Language.it => "allegato",
                _ => "Attachment",
            };
        }
        public string AndXMore(int count)
        {
            return PreferredLanguage switch
            {
                Language.de => $"und {count} weitere...",
                Language.at => $"und {count} weitare...",
                Language.fr => $"et {count} plus...",
                Language.es => $"y {count} más ...",
                Language.ru => $"и еще {count} ...",
                Language.it => $"e {count} altro...",
                _ => $"and {count} more...",
            };
        }
        public string Channel()
        {
            return PreferredLanguage switch
            {
                Language.de => "Kanal",
                Language.at => "Kanoi",
                Language.fr => "Canaliser",
                Language.es => "Canal",
                Language.ru => "Канал",
                Language.it => "Canale",
                _ => "Channel",
            };
        }
        public string SomethingWentWrong()
        {
            return PreferredLanguage switch
            {
                Language.de => "Etwas ist schief gelaufen.",
                Language.at => "Etwos hot ned funktioniat.",
                Language.fr => "Quelque chose s'est mal passé.",
                Language.es => "Algo salió mal.",
                Language.ru => "Что-то пошло не так.",
                Language.it => "Qualcosa è andato storto.",
                _ => "Something went wrong.",
            };
        }
        public string Code()
        {
            return PreferredLanguage switch
            {
                Language.de => "Code",
                Language.at => "Code",
                Language.fr => "Code",
                Language.es => "Código",
                Language.ru => "Код",
                Language.it => "Codice",
                _ => "Code",
            };
        }
        public string LanguageWord()
        {
            return PreferredLanguage switch
            {
                Language.de => "Sprache",
                Language.at => "Sproch",
                Language.fr => "Langue",
                Language.es => "Idioma",
                Language.ru => "Язык",
                Language.it => "Lingua",
                _ => "Language",
            };
        }
        public string Timestamps()
        {
            return PreferredLanguage switch
            {
                Language.de => "Zeitstempel",
                Language.at => "Zeitstempl",
                Language.fr => "Horodatage",
                Language.es => "Marcas de tiempo",
                Language.ru => "Отметки времени",
                Language.it => "Timestamp",
                _ => "Timestamps",
            };
        }
        public string Support()
        {
            return PreferredLanguage switch
            {
                Language.de => "Support",
                Language.at => "Supoat",
                Language.fr => "Soutien",
                Language.es => "Apoyo",
                Language.ru => "Служба поддержки",
                Language.it => "Supporto",
                _ => "Support",
            };
        }
        public string Punishment()
        {
            return PreferredLanguage switch
            {
                Language.de => "Bestrafung",
                Language.at => "Bestrofung",
                Language.fr => "Châtiment",
                Language.es => "Castigo",
                Language.ru => "Наказание",
                Language.it => "Punizione",
                _ => "Punishment",
            };
        }
        public string Until()
        {
            return PreferredLanguage switch
            {
                Language.de => "bis",
                Language.at => "bis",
                Language.fr => "jusqu'à",
                Language.es => "Hasta que",
                Language.ru => "до",
                Language.it => "fino a",
                _ => "until",
            };
        }
        public string PunishmentUntil()
        {
            return PreferredLanguage switch
            {
                Language.de => "Bestrafung bis",
                Language.at => "Bestroft bis",
                Language.fr => "Puni jusqu'à",
                Language.es => "Castigado hasta",
                Language.ru => "Наказан до",
                Language.it => "Punito fino a",
                _ => "Punished until",
            };
        }
        public string Description()
        {
            return PreferredLanguage switch
            {
                Language.de => "Beschreibung",
                Language.at => "Beschreibung",
                Language.fr => "La description",
                Language.es => "Descripción",
                Language.ru => "Описание",
                Language.it => "Descrizione",
                _ => "Description",
            };
        }
        public string Labels()
        {
            return PreferredLanguage switch
            {
                Language.de => "Labels",
                Language.at => "Labl",
                Language.fr => "Étiquettes",
                Language.es => "Etiquetas",
                Language.ru => "Этикетки",
                Language.it => "etichette",
                _ => "Labels",
            };
        }
        public string Filename()
        {
            return PreferredLanguage switch
            {
                Language.de => "Dateiname",
                Language.at => "Dateinom",
                Language.fr => "Nom de fichier",
                Language.es => "Nombre del archivo",
                Language.ru => "Имя файла",
                Language.it => "Nome del file",
                _ => "Filename",
            };
        }
        public string Message()
        {
            return PreferredLanguage switch
            {
                Language.de => "Nachricht",
                Language.at => "Nochricht",
                Language.fr => "Un message",
                Language.es => "Mensaje",
                Language.ru => "Сообщение",
                Language.it => "Messaggio",
                _ => "Message",
            };
        }
        public string UserNote()
        {
            return PreferredLanguage switch
            {
                Language.de => "Benutzernotiz",
                Language.at => "Benutzanotiz",
                Language.fr => "Note de l'utilisateur",
                Language.es => "UserNote",
                Language.ru => "UserNote",
                Language.it => "Nota utente",
                _ => "UserNote",
            };
        }
        public string UserNotes()
        {
            return PreferredLanguage switch
            {
                Language.de => "Benutzernotizen",
                Language.at => "Benutzanotizn",
                Language.fr => "Notes de l'utilisateur",
                Language.es => "Notas de usuario",
                Language.ru => "UserNotes",
                Language.it => "Note utente",
                _ => "UserNotes",
            };
        }
        public string Cases()
        {
            return PreferredLanguage switch
            {
                Language.de => "Vorfälle",
                Language.at => "Vorfälle",
                Language.fr => "Cas",
                Language.es => "Casos",
                Language.ru => "Случаи",
                Language.it => "casi",
                _ => "Cases",
            };
        }
        public string MotD()
        {
            return PreferredLanguage switch
            {
                Language.de => "Nachricht des Tages",
                _ => "Message of the Day",
            };
        }
        public string ActivePunishments()
        {
            return PreferredLanguage switch
            {
                Language.de => "Aktive Bestrafungen",
                Language.at => "Aktive Bestrofungen",
                Language.fr => "Punitions actives",
                Language.es => "Castigos activos",
                Language.ru => "Активные наказания",
                Language.it => "punizioni attive",
                _ => "Active punishments",
            };
        }
        public string UserMap()
        {
            return PreferredLanguage switch
            {
                Language.de => "Benutzerbeziehung",
                Language.at => "Benutzabeziehung",
                Language.fr => "UserMap",
                Language.es => "UserMap",
                Language.ru => "UserMap",
                Language.it => "Mappa utente",
                _ => "UserMap",
            };
        }
        public string UserMaps()
        {
            return PreferredLanguage switch
            {
                Language.de => "Benutzerbeziehungen",
                Language.at => "Benutzabeziehungen",
                Language.fr => "UserMaps",
                Language.es => "UserMaps",
                Language.ru => "UserMaps",
                Language.it => "Mappe utente",
                _ => "UserMaps",
            };
        }
        public string UserMapBetween(MASZ.Models.UserMapping userMap)
        {
            return PreferredLanguage switch
            {
                Language.de => $"Benutzerbeziehung zwischen {userMap.UserA} und {userMap.UserB}.",
                Language.at => $"Benutzabeziehung zwischa {userMap.UserA} und {userMap.UserB}.",
                Language.fr => $"UserMap entre {userMap.UserA} et {userMap.UserB}.",
                Language.es => $"UserMap entre {userMap.UserA} y {userMap.UserB}.",
                Language.ru => $"UserMap между {userMap.UserA} и {userMap.UserB}.",
                Language.it => $"UserMap tra {userMap.UserA} e {userMap.UserB}.",
                _ => $"UserMap between {userMap.UserA} and {userMap.UserB}.",
            };
        }
        public string Imported()
        {
            return PreferredLanguage switch
            {
                Language.de => "Importiert",
                _ => "Imported",
            };
        }
        public string ImportedFromExistingBans()
        {
            return PreferredLanguage switch
            {
                Language.de => "Importiert aus bestehenden Sperren",
                _ => "Imported from existing bans",
            };
        }
        public string Type()
        {
            return PreferredLanguage switch
            {
                Language.de => "Typ",
                Language.at => "Typ",
                Language.fr => "Taper",
                Language.es => "Escribe",
                Language.ru => "Тип",
                Language.it => "Tipo",
                _ => "Type",
            };
        }
        public string Joined()
        {
            return PreferredLanguage switch
            {
                Language.de => "Beigetreten",
                Language.at => "Beigetretn",
                Language.fr => "Inscrit",
                Language.es => "Unido",
                Language.ru => "Присоединился",
                Language.it => "Partecipato",
                _ => "Joined",
            };
        }
        public string Registered()
        {
            return PreferredLanguage switch
            {
                Language.de => "Registriert",
                Language.at => "Registriat",
                Language.fr => "Inscrit",
                Language.es => "Registrado",
                Language.ru => "Зарегистрировано",
                Language.it => "Registrato",
                _ => "Registered",
            };
        }
        public string NotificationModcaseCreatePublic(ModCase modCase)
        {
            return PreferredLanguage switch
            {
                Language.de => $"Ein **Vorfall** für <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) wurde erstellt.",
                Language.at => $"A **Vorfoi** fia <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) is erstöt woan.",
                Language.fr => $"Un **Modcase** pour <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) a été créé.",
                Language.es => $"Se ha creado un **Modcase** para <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}).",
                Language.ru => $"**Modcase** для <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) был создан.",
                Language.it => $"È stato creato un **Modcase** per <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}).",
                _ => $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been created.",
            };
        }
        public string NotificationModcaseCreateInternal(ModCase modCase, IUser moderator)
        {
            return PreferredLanguage switch
            {
                Language.de => $"Ein **Vorfall** für <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) wurde von <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}) erstellt.",
                Language.at => $"A **Vorfoi** fia <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) woad fo <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}) erstöt.",
                Language.fr => $"Un **Modcase** pour <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) a été créé par <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).",
                Language.es => $"Un **Modcase** para <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) ha sido creado por <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).",
                Language.ru => $"**Modcase** для <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) был создан <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).",
                Language.it => $"Un **Modcase** per <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) è stato creato da <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).",
                _ => $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been created by <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).",
            };
        }
        public string NotificationModcaseUpdatePublic(ModCase modCase)
        {
            return PreferredLanguage switch
            {
                Language.de => $"Ein **Vorfall** für <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) wurde aktualisiert.",
                Language.at => $"A **Vorfoi** fia <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) is aktualisiert woan.",
                Language.fr => $"Un **Modcase** pour <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) a été mis à jour.",
                Language.es => $"Se ha actualizado **Modcase** para <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}).",
                Language.ru => $"**Modcase** для <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) был обновлен.",
                Language.it => $"È stato aggiornato un **Modcase** per <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}).",
                _ => $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been updated.",
            };
        }
        public string NotificationModcaseUpdateInternal(ModCase modCase, IUser moderator)
        {
            return PreferredLanguage switch
            {
                Language.de => $"Ein **Vorfall** für <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) wurde von <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}) aktualisiert.",
                Language.at => $"A **Vorfoi** fia <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) is fo <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}) aktualisiert woan.",
                Language.fr => $"Un **Modcase** pour <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) a été mis à jour par <@{moderator.Id}> ({moderator.Username}#{moderator. Discriminator}).",
                Language.es => $"Un **Modcase** para <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) ha sido actualizado por <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).",
                Language.ru => $"**Modcase** для <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) был обновлен <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).",
                Language.it => $"Un **Modcase** per <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) è stato aggiornato da <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).",
                _ => $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been updated by <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).",
            };
        }
        public string NotificationModcaseDeletePublic(ModCase modCase)
        {
            return PreferredLanguage switch
            {
                Language.de => $"Ein **Vorfall** für <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) wurde gelöscht.",
                Language.at => $"A **Vorfoi** fia <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) is glescht woan",
                Language.fr => $"Un **Modcase** pour <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) a été supprimé.",
                Language.es => $"Se ha eliminado un **Modcase** para <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}).",
                Language.ru => $"**Modcase** для <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) был удален.",
                Language.it => $"Un **Modcase** per <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) è stato eliminato.",
                _ => $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been deleted.",
            };
        }
        public string NotificationModcaseDeleteInternal(ModCase modCase, IUser moderator)
        {
            return PreferredLanguage switch
            {
                Language.de => $"Ein **Vorfall** für <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) wurde von <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}) gelöscht.",
                Language.at => $"A **Vorfoi** fia <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) is vo <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}) glescht woan.",
                Language.fr => $"Un **Modcase** pour <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) a été supprimé par <@{moderator.Id}> ({moderator.Username}#{moderator. Discriminator}).",
                Language.es => $"Un **Modcase** para <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) ha sido eliminado por <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).",
                Language.ru => $"**Modcase** для <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) был удален <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).",
                Language.it => $"Un **Modcase** per <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) è stato eliminato da <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).",
                _ => $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been deleted by <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).",
            };
        }
        public string NotificationModcaseCommentsShortCreate()
        {
            return PreferredLanguage switch
            {
                Language.de => "Kommentar erstellt",
                Language.at => "Kommentoa erstöt",
                Language.fr => "Commentaire créé",
                Language.es => "Comentario creado",
                Language.ru => "Комментарий создан",
                Language.it => "Commento creato",
                _ => "Comment created",
            };
        }
        public string NotificationModcaseCommentsShortUpdate()
        {
            return PreferredLanguage switch
            {
                Language.de => "Kommentar aktualisiert",
                Language.at => "kommentoa aktualisiert",
                Language.fr => "Commentaire mis à jour",
                Language.es => "Comentario actualizado",
                Language.ru => "Комментарий обновлен",
                Language.it => "Commento aggiornato",
                _ => "Comment updated",
            };
        }
        public string NotificationModcaseCommentsShortDelete()
        {
            return PreferredLanguage switch
            {
                Language.de => "Kommentar gelöscht",
                Language.at => "kommentoa glescht",
                Language.fr => "Commentaire supprimé",
                Language.es => "Comentario borrado",
                Language.ru => "Комментарий удален",
                Language.it => "Commento cancellato",
                _ => "Comment deleted",
            };
        }
        public string NotificationModcaseCommentsCreate(IUser actor)
        {
            return PreferredLanguage switch
            {
                Language.de => $"Ein **Kommentar** wurde von <@{actor.Id}> erstellt.",
                Language.at => $"A **Kommentoa** wuad vo <@{actor.Id}> erstöt.",
                Language.fr => $"Un **commentaire** a été créé par <@{actor.Id}>.",
                Language.es => $"<@{actor.Id}> ha creado un **comentario**.",
                Language.ru => $"**комментарий** был создан <@{actor.Id}>.",
                Language.it => $"Un **commento** è stato creato da <@{actor.Id}>.",
                _ => $"A **comment** has been created by <@{actor.Id}>.",
            };
        }
        public string NotificationModcaseCommentsUpdate(IUser actor)
        {
            return PreferredLanguage switch
            {
                Language.de => $"Ein **Kommentar** wurde von <@{actor.Id}> aktualisiert.",
                Language.at => $"A **Kommentoa** is vo <@{actor.Id}> aktualisiert woan.",
                Language.fr => $"Un **commentaire** a été mis à jour par <@{actor.Id}>.",
                Language.es => $"<@{actor.Id}> ha actualizado un **comentario **.",
                Language.ru => $"**комментарий ** был обновлен пользователем <@{actor.Id}>.",
                Language.it => $"Un **commento** è stato aggiornato da <@{actor.Id}>.",
                _ => $"A **comment** has been updated by <@{actor.Id}>.",
            };
        }
        public string NotificationModcaseCommentsDelete(IUser actor)
        {
            return PreferredLanguage switch
            {
                Language.de => $"Ein **Kommentar** wurde von <@{actor.Id}> gelöscht.",
                Language.at => $"A **Kommentoa** wuad vo <@{actor.Id}> glescht.",
                Language.fr => $"Un **commentaire** a été supprimé par <@{actor.Id}>.",
                Language.es => $"<@{actor.Id}> ha eliminado un **comentario **.",
                Language.ru => $"**комментарий** был удален <@{actor.Id}>.",
                Language.it => $"Un **commento** è stato eliminato da <@{actor.Id}>.",
                _ => $"A **comment** has been deleted by <@{actor.Id}>.",
            };
        }
        public string NotificationModcaseFileCreate(IUser actor)
        {
            return PreferredLanguage switch
            {
                Language.de => $"Eine **Datei** wurde von <@{actor.Id}> ({actor.Username}#{actor.Discriminator}) hochgeladen.",
                Language.at => $"A **Datei** woad vo <@{actor.Id}> ({actor.Username}#{actor.Discriminator}) hochglodn.",
                Language.fr => $"Un **fichier** a été téléchargé par <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).",
                Language.es => $"<@{actor.Id}> ({actor.Username}#{actor.Discriminator} ha subido un **archivo**).",
                Language.ru => $"**файл** был загружен пользователем <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).",
                Language.it => $"Un **file** è stato caricato da <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).",
                _ => $"A **file** has been uploaded by <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).",
            };
        }
        public string NotificationModcaseFileDelete(IUser actor)
        {
            return PreferredLanguage switch
            {
                Language.de => $"Eine **Datei** wurde von <@{actor.Id}> ({actor.Username}#{actor.Discriminator}) gelöscht.",
                Language.at => $"A **Datei** is vo <@{actor.Id}> ({actor.Username}#{actor.Discriminator}) glescht woan.",
                Language.fr => $"Un **fichier** a été supprimé par <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).",
                Language.es => $"<@{actor.Id}> ({actor.Username}#{actor.Discriminator}) ha eliminado un **archivo**.",
                Language.ru => $"**файл** был удален <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).",
                Language.it => $"Un **file** è stato eliminato da <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).",
                _ => $"A **file** has been deleted by <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).",
            };
        }
        public string NotificationModcaseFileUpdate(IUser actor)
        {
            return PreferredLanguage switch
            {
                Language.de => $"Eine **Datei** wurde von <@{actor.Id}> ({actor.Username}#{actor.Discriminator}) aktualisiert.",
                Language.at => $"A **Datei** is vo <@{actor.Id}> ({actor.Username}#{actor.Discriminator}) aktualisiert woan.",
                Language.fr => $"Un **fichier** a été mis à jour par <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).",
                Language.es => $"<@{actor.Id}> ({actor.Username}#{actor.Discriminator}) ha actualizado un **archivo**.",
                Language.ru => $"**файл** был обновлен <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).",
                Language.it => $"Un **file** è stato aggiornato da <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).",
                _ => $"A **file** has been updated by <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).",
            };
        }
        public string NotificationModcaseDMWarn(IGuild guild, string serviceBaseUrl)
        {
            return PreferredLanguage switch
            {
                Language.de => $"Die Moderatoren von `{guild.Name}` haben dich verwarnt.\nFür weitere Informationen besuche: {serviceBaseUrl}",
                Language.at => $"Die Moderatoan vo `{guild.Name}` hom di verwoarnt.\nFia weitere Infos schau bei {serviceBaseUrl} noch.",
                Language.fr => $"Les modérateurs de la guilde `{guild.Name}` vous ont prévenu.\nPour plus d'informations ou pour une réhabilitation, visitez : {serviceBaseUrl}",
                Language.es => $"Los moderadores del gremio `{guild.Name}` te han advertido.\nPara obtener más información o rehabilitación, visite: {serviceBaseUrl}",
                Language.ru => $"Модераторы гильдии `{guild.Name}` вас предупредили.\nДля получения дополнительной информации или реабилитации посетите: {serviceBaseUrl}",
                Language.it => $"I moderatori della gilda `{guild.Name}` ti hanno avvisato.\nPer maggiori informazioni o visita riabilitativa: {serviceBaseUrl}",
                _ => $"The moderators of guild `{guild.Name}` have warned you.\nFor more information or rehabilitation visit: {serviceBaseUrl}",
            };
        }
        public string NotificationModcaseDMMuteTemp(ModCase modCase, IGuild guild, string serviceBaseUrl)
        {
            return PreferredLanguage switch
            {
                Language.de => $"Die Moderatoren von `{guild.Name}` haben dich temporär stummgeschalten bis {modCase.PunishedUntil.Value.ToDiscordTS()}.\nFür weitere Informationen besuche: {serviceBaseUrl}",
                Language.at => $"Die Moderatoan vo `{guild.Name}` hom di bis am {modCase.PunishedUntil.Value.ToDiscordTS()} stummgschoit.\nFia weitere Infos schau bei {serviceBaseUrl} noch",
                Language.fr => $"Les modérateurs de la guilde `{guild.Name}` vous ont temporairement mis en sourdine jusqu'à {modCase.PunishedUntil.Value.ToDiscordTS()}.\nPour plus d'informations ou pour une réhabilitation, visitez : {serviceBaseUrl}",
                Language.es => $"Los moderadores del gremio `{guild.Name}` te han silenciado temporalmente hasta {modCase.PunishedUntil.Value.ToDiscordTS ()}.\nPara obtener más información o rehabilitación, visite: {serviceBaseUrl}",
                Language.ru => $"Модераторы гильдии `{guild.Name}` временно отключили ваш звук до {modCase.PunishedUntil.Value.ToDiscordTS ()}.\nДля получения дополнительной информации или реабилитации посетите: {serviceBaseUrl}",
                Language.it => $"I moderatori della gilda `{guild.Name}` ti hanno temporaneamente disattivato l'audio fino a {modCase.PunishedUntil.Value.ToDiscordTS()}.\nPer maggiori informazioni o visita riabilitativa: {serviceBaseUrl}",
                _ => $"The moderators of guild `{guild.Name}` have temporarily muted you until {modCase.PunishedUntil.Value.ToDiscordTS()}.\nFor more information or rehabilitation visit: {serviceBaseUrl}",
            };
        }
        public string NotificationModcaseDMMutePerm(IGuild guild, string serviceBaseUrl)
        {
            return PreferredLanguage switch
            {
                Language.de => $"Die Moderatoren von `{guild.Name}` haben dich stummgeschalten.\nFür weitere Informationen besuche: {serviceBaseUrl}",
                Language.at => $"Die Moderatoan vo `{guild.Name}` hom di stummgschoit.\nFia weitere Infos schau bei {serviceBaseUrl} noch.",
                Language.fr => $"Les modérateurs de la guilde `{guild.Name}` vous ont mis en sourdine.\nPour plus d'informations ou pour une réhabilitation, visitez : {serviceBaseUrl}",
                Language.es => $"Los moderadores del gremio `{guild.Name}` te han silenciado.\nPara obtener más información o rehabilitación, visite: {serviceBaseUrl}",
                Language.ru => $"Модераторы гильдии `{guild.Name}` отключили вас.\nДля получения дополнительной информации или реабилитации посетите: {serviceBaseUrl}",
                Language.it => $"I moderatori della gilda `{guild.Name}` ti hanno disattivato.\nPer maggiori informazioni o visita riabilitativa: {serviceBaseUrl}",
                _ => $"The moderators of guild `{guild.Name}` have muted you.\nFor more information or rehabilitation visit: {serviceBaseUrl}",
            };
        }
        public string NotificationModcaseDMBanTemp(ModCase modCase, IGuild guild, string serviceBaseUrl)
        {
            return PreferredLanguage switch
            {
                Language.de => $"Die Moderatoren von `{guild.Name}` haben dich temporär gebannt bis {modCase.PunishedUntil.Value.ToDiscordTS()}.\nFür weitere Informationen besuche: {serviceBaseUrl}",
                Language.at => $"Die Moderatoan vo `{guild.Name}` hom di bis am {modCase.PunishedUntil.Value.ToDiscordTS()} vom Serva ausgsperrt.\nFia weitere Infos schau bei {serviceBaseUrl} noch.",
                Language.fr => $"Les modérateurs de la guilde `{guild.Name}` vous ont temporairement banni jusqu'à {modCase.PunishedUntil.Value.ToDiscordTS()}.\nPour plus d'informations ou pour une réhabilitation, visitez : {serviceBaseUrl}",
                Language.es => $"Los moderadores del gremio `{guild.Name}` te han baneado temporalmente hasta el {modCase.PunishedUntil.Value.ToDiscordTS ()}.\nPara obtener más información o rehabilitación, visite: {serviceBaseUrl}",
                Language.ru => $"Модераторы гильдии `{guild.Name}` временно заблокировали вас до {modCase.PunishedUntil.Value.ToDiscordTS ()}.\nДля получения дополнительной информации или реабилитации посетите: {serviceBaseUrl}",
                Language.it => $"I moderatori della gilda `{guild.Name}` ti hanno temporaneamente bannato fino al {modCase.PunishedUntil.Value.ToDiscordTS()}.\nPer maggiori informazioni o visita riabilitativa: {serviceBaseUrl}",
                _ => $"The moderators of guild `{guild.Name}` have temporarily banned you until {modCase.PunishedUntil.Value.ToDiscordTS()}.\nFor more information or rehabilitation visit: {serviceBaseUrl}",
            };
        }
        public string NotificationModcaseDMBanPerm(IGuild guild, string serviceBaseUrl)
        {
            return PreferredLanguage switch
            {
                Language.de => $"Die Moderatoren von `{guild.Name}` haben dich gebannt.\nFür weitere Informationen besuche: {serviceBaseUrl}",
                Language.at => $"Die Moderatoan vo `{guild.Name}` hom di vom Serva ausgsperrt.\nFia weitere Infos schau bei {serviceBaseUrl} noch",
                Language.fr => $"Les modérateurs de la guilde `{guild.Name}` vous ont banni.\nPour plus d'informations ou pour une réhabilitation, visitez : {serviceBaseUrl}",
                Language.es => $"Los moderadores del gremio `{guild.Name}` te han prohibido.\nPara obtener más información o rehabilitación, visite: {serviceBaseUrl}",
                Language.ru => $"Модераторы гильдии `{guild.Name}` забанили вас.\nДля получения дополнительной информации или реабилитации посетите: {serviceBaseUrl}",
                Language.it => $"I moderatori della gilda `{guild.Name}` ti hanno bannato.\nPer maggiori informazioni o visita riabilitativa: {serviceBaseUrl}",
                _ => $"The moderators of guild `{guild.Name}` have banned you.\nFor more information or rehabilitation visit: {serviceBaseUrl}",
            };
        }
        public string NotificationModcaseDMKick(IGuild guild, string serviceBaseUrl)
        {
            return PreferredLanguage switch
            {
                Language.de => $"Die Moderatoren von `{guild.Name}` haben dich kickt.\nFür weitere Informationen besuche: {serviceBaseUrl}",
                Language.at => $"Die Moderatoan vo `{guild.Name}` hom di rausgschmissn.\nFia weitere Infos schau bei {serviceBaseUrl} noch.",
                Language.fr => $"Les modérateurs de la guilde `{guild.Name}` vous ont viré.\nPour plus d'informations ou pour une réhabilitation, visitez : {serviceBaseUrl}",
                Language.es => $"Los moderadores del gremio `{guild.Name}` te han pateado.\nPara obtener más información o rehabilitación, visite: {serviceBaseUrl}",
                Language.ru => $"Модераторы гильдии `{guild.Name}` выгнали вас.\nДля получения дополнительной информации или реабилитации посетите: {serviceBaseUrl}",
                Language.it => $"I moderatori della gilda `{guild.Name}` ti hanno espulso.\nPer maggiori informazioni o visita riabilitativa: {serviceBaseUrl}",
                _ => $"The moderators of guild `{guild.Name}` have kicked you.\nFor more information or rehabilitation visit: {serviceBaseUrl}",
            };
        }
        public string NotificationFilesCreate()
        {
            return PreferredLanguage switch
            {
                Language.de => "Datei hochgeladen",
                Language.at => "Datei hochglodn",
                Language.fr => "Fichier téléchargé",
                Language.es => "Archivo subido",
                Language.ru => "Файл загружен",
                Language.it => "File caricato",
                _ => "File uploaded",
            };
        }
        public string NotificationFilesDelete()
        {
            return PreferredLanguage switch
            {
                Language.de => "Datei gelöscht",
                Language.at => "Datei glescht",
                Language.fr => "Fichier supprimé",
                Language.es => "Archivo eliminado",
                Language.ru => "Файл удален",
                Language.it => "File cancellato",
                _ => "File deleted",
            };
        }
        public string NotificationFilesUpdate()
        {
            return PreferredLanguage switch
            {
                Language.de => "Datei aktualisiert",
                Language.at => "Datei aktualisiert",
                Language.fr => "Fichier mis à jour",
                Language.es => "Archivo actualizado",
                Language.ru => "Файл обновлен",
                Language.it => "File aggiornato",
                _ => "File updated",
            };
        }
        public string NotificationRegisterWelcomeToMASZ()
        {
            return PreferredLanguage switch
            {
                Language.de => "Willkommen bei MASZ!",
                Language.at => "Servus bei MASZ!",
                Language.fr => "Bienvenue à MASZ !",
                Language.es => "¡Bienvenido a MASZ!",
                Language.ru => "Добро пожаловать в МАСЗ!",
                Language.it => "Benvenuto in MASZ!",
                _ => "Welcome to MASZ!",
            };
        }
        public string NotificationRegisterDescriptionThanks()
        {
            return PreferredLanguage switch
            {
                Language.de => "Vielen Dank für deine Registrierung.\nIm Folgenden wirst du einige nützliche Tipps zum Einrichten und Verwenden von **MASZ** erhalten.",
                Language.at => "Donksche fia dei Registrierung.\nDu siachst glei ei poar nützliche Tipps zum Eirichtn und Vawendn vo **MASZ**.",
                Language.fr => "Merci d'avoir enregistré votre guilde.\nDans ce qui suit, vous apprendrez quelques conseils utiles pour configurer et utiliser **MASZ**.",
                Language.es => "Gracias por registrar tu gremio.\nA continuación, aprenderá algunos consejos útiles para configurar y usar **MASZ**.",
                Language.ru => "Спасибо за регистрацию вашей гильдии.\nДалее вы получите несколько полезных советов по настройке и использованию **MASZ**.",
                Language.it => "Grazie per aver registrato la tua gilda.\nDi seguito imparerai alcuni suggerimenti utili per impostare e utilizzare **MASZ**.",
                _ => "Thanks for registering your guild.\nIn the following you will learn some useful tips for setting up and using **MASZ**.",
            };
        }
        public string NotificationRegisterUseFeaturesCommand()
        {
            return PreferredLanguage switch
            {
                Language.de => "Benutze den `/features` Befehl um zu sehen welche Features von **MASZ** dein aktuelles Setup unterstützt.",
                Language.at => "Nutz den `/features` Beföhl um nochzumschauen wöchane Features dei aktuelles **MASZ**  Setup untastützn tuad.",
                Language.fr => "Utilisez la commande `/features` pour tester si votre configuration de guilde actuelle prend en charge toutes les fonctionnalités de **MASZ**.",
                Language.es => "Usa el comando `/ features` para probar si la configuración de tu gremio actual es compatible con todas las características de **MASZ**.",
                Language.ru => "Используйте команду `/ features`, чтобы проверить, поддерживает ли ваша текущая настройка гильдии все функции **MASZ**.",
                Language.it => "Usa il comando `/features` per verificare se l'attuale configurazione della gilda supporta tutte le funzionalità di **MASZ**.",
                _ => "Use the `/features` command to test if your current guild setup supports all features of **MASZ**.",
            };
        }
        public string NotificationRegisterDefaultLanguageUsed(string language)
        {
            return PreferredLanguage switch
            {
                Language.de => $"MASZ wird `{language}` als Standard-Sprache für diese Gilde verwenden, wenn möglich.",
                Language.at => $"Dei MASZ wiad `{language}` ois Standard-Sproch fia die Güde nehma, wenns geht.",
                Language.fr => $"MASZ utilisera `{language}` comme langue par défaut pour cette guilde dans la mesure du possible.",
                Language.es => $"MASZ usará `{language}` como idioma predeterminado para este gremio siempre que sea posible.",
                Language.ru => $"MASZ будет использовать `{language}` как язык по умолчанию для этой гильдии, когда это возможно.",
                Language.it => $"MASZ utilizzerà `{language}` come lingua predefinita per questa gilda ogni volta che sarà possibile.",
                _ => $"MASZ will use `{language}` as default language for this guild whenever possible.",
            };
        }
        public string NotificationRegisterConfusingTimestamps()
        {
            return PreferredLanguage switch
            {
                Language.de => "Zeitzonen können kompliziert sein.\nMASZ benutzt ein Discord-Feature um Zeitstempel in der lokalen Zeitzone deines Computers/Handys anzuzeigen.",
                Language.at => "De Zeitzonen kennan a weng schwer san.\nMASZ nutzt a Discord-Feature um Zeitstempl in da lokalen Zeitzon vo deim PC/Handy ozumzeign.",
                Language.fr => "Les fuseaux horaires peuvent être déroutants.\nMASZ utilise une fonction Discord pour afficher les horodatages dans le fuseau horaire local de votre ordinateur/téléphone.",
                Language.es => "Las zonas horarias pueden resultar confusas.\nMASZ usa una función de Discord para mostrar marcas de tiempo en la zona horaria local de su computadora / teléfono.",
                Language.ru => "Часовые пояса могут сбивать с толку.\nMASZ использует функцию Discord для отображения меток времени в местном часовом поясе вашего компьютера / телефона.",
                Language.it => "I fusi orari possono creare confusione.\nMASZ utilizza una funzione Discord per visualizzare i timestamp nel fuso orario locale del tuo computer/telefono.",
                _ => "Timezones can be confusing.\nMASZ uses a Discord feature to display timestamps in the local timezone of your computer/phone.",
            };
        }
        public string NotificationRegisterSupport()
        {
            return PreferredLanguage switch
            {
                Language.de => "Bitte wende dich an den [MASZ Support Server](https://discord.gg/5zjpzw6h3S) für weitere Fragen.",
                Language.at => "Bitte wend di on den [MASZ Support Server](https://discord.gg/5zjpzw6h3S) fia weitare Frogn.",
                Language.fr => "Veuillez vous référer au [serveur de support MASZ] (https://discord.gg/5zjpzw6h3S) pour d'autres questions.",
                Language.es => "Consulte el [servidor de soporte MASZ] (https://discord.gg/5zjpzw6h3S) si tiene más preguntas.",
                Language.ru => "Дополнительные вопросы можно найти на [сервере поддержки MASZ] (https://discord.gg/5zjpzw6h3S).",
                Language.it => "Fare riferimento al [Server di supporto MASZ] (https://discord.gg/5zjpzw6h3S) per ulteriori domande.",
                _ => "Please refer to the [MASZ Support Server](https://discord.gg/5zjpzw6h3S) for further questions.",
            };
        }
        public string NotificationAutomoderationInternal(IUser user)
        {
            return PreferredLanguage switch
            {
                Language.de => $"{user.Mention} hat die Automoderation ausgelöst.",
                Language.at => $"{user.Mention} hot de Automodaration ausglest.",
                Language.fr => $"{user.Mention} a déclenché l'automodération.",
                Language.es => $"{user.Mention} activó la automoderación.",
                Language.ru => $"{user.Mention} запустил автомодерацию.",
                Language.it => $"{user.Mention} ha attivato la moderazione automatica.",
                _ => $"{user.Mention} triggered automoderation.",
            };
        }
        public string NotificationAutomoderationCase(IUser user)
        {
            return PreferredLanguage switch
            {
                Language.de => $"{user.Username}#{user.Discriminator} hat die Automoderation ausgelöst.",
                Language.at => $"{user.Username}#{user.Discriminator} hot de Automodaration ausglest.",
                Language.fr => $"{user.Username}#{user.Discriminator} a déclenché la modération automatique.",
                Language.es => $"{user.Username}#{user.Discriminator} desencadenó la automoderación.",
                Language.ru => $"{user.Username}#{user.Discriminator} запускает автомодерацию.",
                Language.it => $"{user.Username}#{user.Discriminator} ha attivato la moderazione automatica.",
                _ => $"{user.Username}#{user.Discriminator} triggered automoderation.",
            };
        }
        public string NotificationAutomoderationDM(IUser user, IMentionable channel, string reason, string action)
        {
            return PreferredLanguage switch
            {
                Language.de => $"Hallo {user.Mention},\n\nDu hast die Automoderation in {channel.Mention} ausgelöst.\nGrund: {reason}\nAktion: {action}",
                Language.at => $"Servus {user.Mention},\n\nDu host de Automodaration in {channel.Mention} ausglest. Grund: {reason}\nAktion: {action}",
                Language.fr => $"Salut {user.Mention},\n\nVous avez déclenché l'automodération dans {channel.Mention}.\nRaison : {reason}\nAction : {action}",
                Language.es => $"Hola, {user.Mention}:\n\nActivó la automoderación en {channel.Mention}.\nMotivo: {reason}\nAcción: {action}",
                Language.ru => $"Привет, {user.Mention}!\n\nВы активировали автомодерацию в {channel.Mention}.\nПричина: {reason}\nДействие: {action}",
                Language.it => $"Ciao {user.Mention},\n\nHai attivato la moderazione automatica in {channel.Mention}.\nMotivo: {reason}\nAzione: {action}",
                _ => $"Hi {user.Mention},\n\nYou triggered automoderation in {channel.Mention}.\nReason: {reason}\nAction: {action}",
            };
        }
        public string NotificationAutomoderationChannel(IUser user, string reason)
        {
            return PreferredLanguage switch
            {
                Language.de => $"{user.Mention} du hast die Automoderation ausgelöst. Grund: {reason}. Deine Nachricht wurde gelöscht.",
                Language.at => $"{user.Mention} du host de Automodaration ausglest. Grund: {reason}. Dei Nochricht wuad glescht.",
                Language.fr => $"{user.Mention} vous avez déclenché l'automodération. Raison : {reason}. Votre message a été supprimé.",
                Language.es => $"{user.Mention} has activado la automoderación. Razón: {reason}. Su mensaje ha sido eliminado.",
                Language.ru => $"{user.Mention} вы запустили автомодерацию. Причина: {reason}. Ваше сообщение было удалено.",
                Language.it => $"{user.Mention} hai attivato la moderazione automatica. reason: {reason}. Il tuo messaggio è stato cancellato.",
                _ => $"{user.Mention} you triggered automoderation. Reason: {reason}. Your message has been deleted.",
            };
        }
        public string NotificationMotdInternalCreate(IUser actor)
        {
            return PreferredLanguage switch
            {
                Language.de => $"Neue MotD wurde von {actor.Mention} erstellt.",
                _ => $"New MotD has been created by {actor.Mention}.",
            };
        }
        public string NotificationMotdInternalEdited(IUser actor)
        {
            return PreferredLanguage switch
            {
                Language.de => $"MotD wurde von {actor.Mention} bearbeitet.",
                _ => $"MotD has been edited by {actor.Mention}.",
            };
        }
        public string NotificationMotdShow()
        {
            return PreferredLanguage switch
            {
                Language.de => "Anzeigen",
                _ => "Show",
            };
        }
        public string NotificationAutomoderationConfigInternalCreate(string eventType, IUser actor)
        {
            return PreferredLanguage switch
            {
                Language.de => $"Automodkonfiguration für {eventType} von {actor.Mention} erstellt.",
                _ => $"Automodconfig created for {eventType} by {actor.Mention}.",
            };
        }
        public string NotificationAutomoderationConfigInternalUpdate(string eventType, IUser actor)
        {
            return PreferredLanguage switch
            {
                Language.de => $"Automodkonfiguration für {eventType} von {actor.Mention} bearbeitet.",
                _ => $"Automodconfig updated for {eventType} by {actor.Mention}.",
            };
        }
        public string NotificationAutomoderationConfigInternalDelete(string eventType, IUser actor)
        {
            return PreferredLanguage switch
            {
                Language.de => $"Automodkonfiguration für {eventType} von {actor.Mention} gelöscht.",
                _ => $"Automodconfig deleted for {eventType} by {actor.Mention}.",
            };
        }
        public string NotificationAutomoderationConfigLimit()
        {
            return PreferredLanguage switch
            {
                Language.de => "Limit",
                _ => "Limit",
            };
        }
        public string NotificationAutomoderationConfigTimeLimit()
        {
            return PreferredLanguage switch
            {
                Language.de => "Zeitlimit",
                _ => "Time limit",
            };
        }
        public string NotificationAutomoderationConfigIgnoredRoles()
        {
            return PreferredLanguage switch
            {
                Language.de => "Ignorierte Rollen",
                _ => "Ignored roles",
            };
        }
        public string NotificationAutomoderationConfigIgnoredChannels()
        {
            return PreferredLanguage switch
            {
                Language.de => "Ignorierte Kanäle",
                _ => "Ignored channels",
            };
        }
        public string NotificationAutomoderationConfigDuration()
        {
            return PreferredLanguage switch
            {
                Language.de => "Dauer",
                _ => "Duration",
            };
        }
        public string NotificationAutomoderationConfigDeleteMessage()
        {
            return PreferredLanguage switch
            {
                Language.de => "Nachricht löschen",
                _ => "Delete message",
            };
        }
        public string NotificationAutomoderationConfigSendPublic()
        {
            return PreferredLanguage switch
            {
                Language.de => "Sende öffentliche Nachricht",
                _ => "Send public notification",
            };
        }
        public string NotificationAutomoderationConfigSendDM()
        {
            return PreferredLanguage switch
            {
                Language.de => "Sende DM Nachricht",
                _ => "Send DM notification",
            };
        }
        public string NotificationAutoWhoisJoinWith(IUser user, DateTime registered, string invite)
        {
            return PreferredLanguage switch
            {
                Language.de => $"{user.Mention} (registriert {registered.ToDiscordTS()}) ist mit dem Invite `{invite}` beigetreten.",
                Language.at => $"{user.Mention} (registriat {registered.ToDiscordTS()}) is mit da Eiladung `{invite}` beigetretn.",
                Language.fr => $"{user.Mention} (enregistré {registered.ToDiscordTS()}) rejoint avec l'invitation `{invite}`.",
                Language.es => $"{user.Mention} (registrado {registered.ToDiscordTS ()}) se unió con la invitación `{invite}`.",
                Language.ru => $"{user.Mention} (зарегистрированный {registered.ToDiscordTS ()}) присоединился с приглашением `{invite}`.",
                Language.it => $"{user.Mention} (registrato {registered.ToDiscordTS()}) si è unito con l'invito `{invite}`.",
                _ => $"{user.Mention} (registered {registered.ToDiscordTS()}) joined with invite `{invite}`.",
            };
        }
        public string NotificationAutoWhoisJoinWithAndFrom(IUser user, ulong by, DateTime createdAt, DateTime registered, string invite)
        {
            return PreferredLanguage switch
            {
                Language.de => $"{user.Mention} (registriert {registered.ToDiscordTS()}) ist mit dem Invite `{invite}` von <@{by}> (am {createdAt.ToDiscordTS()}) beigetreten.",
                Language.at => $"{user.Mention} (registriert {registered.ToDiscordTS()}) is mit da Eiladung `{invite}` vo <@{by}> (am {createdAt.ToDiscordTS()}) beigetretn.",
                Language.fr => $"{user.Mention} (enregistré {registered.ToDiscordTS()}) rejoint avec invite `{invite}` (créé {createdAt.ToDiscordTS()}) par <@{by}>.",
                Language.es => $"{user.Mention} (registrado {registered.ToDiscordTS()}) se unió con la invitación `{invite}` (creado {createdAt.ToDiscordTS()}) por <@{by}>.",
                Language.ru => $"{user.Mention} (зарегистрированный {registered.ToDiscordTS()}) присоединился с помощью приглашения `{invite}` (created {createdAt.ToDiscordTS()}) пользователем <@{by}>.",
                Language.it => $"{user.Mention} (registrato {registered.ToDiscordTS()}) si è unito all'invito `{invite}` (creato {createdAt.ToDiscordTS()}) da <@{by}>.",
                _ => $"{user.Mention} (registered {registered.ToDiscordTS()}) joined with invite `{invite}` (created {createdAt.ToDiscordTS()}) by <@{by}>.",
            };
        }
        public string NotificationDiscordAuditLogPunishmentsExecute(int caseId, ulong modId, string reason)
        {
            return PreferredLanguage switch
            {
                Language.de => $"Bestrafung für Vorfall #{caseId} durch Moderator {modId} ausgeführt: \"{reason}\"",
                _ => $"Punishment for ModCase #{caseId} by moderator {modId} executed: \"{reason}\"",
            };
        }
        public string NotificationDiscordAuditLogPunishmentsUndone(int caseId, string reason)
        {
            return PreferredLanguage switch
            {
                Language.de => $"Bestrafung für Vorfall #{caseId} rückgängig gemacht: \"{reason}\"",
                _ => $"Punishment for ModCase #{caseId} undone: \"{reason}\"",
            };
        }
        public string CmdOnlyTextChannel()
        {
            return PreferredLanguage switch
            {
                Language.de => "Nur Textkanäle sind erlaubt.",
                Language.at => "Nua Textkanö san guat.",
                Language.fr => "Seuls les canaux de texte sont autorisés.",
                Language.es => "Solo se permiten canales de texto.",
                Language.ru => "Разрешены только текстовые каналы.",
                Language.it => "Sono consentiti solo canali di testo.",
                _ => "Only text channels are allowed.",
            };
        }
        public string CmdCannotViewOrDeleteInChannel()
        {
            return PreferredLanguage switch
            {
                Language.de => "Ich darf keine Nachrichten in diesem Kanal sehen oder löschen!",
                Language.at => "I derf kane Nochrichtn in dem Kanoi sehn oda leschn!",
                Language.fr => "Je ne suis pas autorisé à afficher ou supprimer les messages de cette chaîne !",
                Language.es => "¡No puedo ver ni borrar mensajes en este canal!",
                Language.ru => "Мне не разрешено просматривать или удалять сообщения на этом канале!",
                Language.it => "Non sono autorizzato a visualizzare o eliminare i messaggi in questo canale!",
                _ => "I'm not allowed to view or delete messages in this channel!",
            };
        }
        public string CmdGetAvatarURL()
        {
            return PreferredLanguage switch
            {
                Language.de => "Avatar URL",
                Language.at => "Avadar URL",
                Language.fr => "l'URL d'avatar",
                Language.es => "URL de avatar",
                Language.ru => "URL аватара",
                Language.it => "l'URL dell'avatar",
                _ => "Avatar URL",
            };
        }
        public string CmdUserID()
        {
            return PreferredLanguage switch
            {
                Language.de => "NutzerId",
                Language.at => "NutzaId",
                Language.fr => "Identifiant d'utilisateur",
                Language.es => "User ID",
                Language.ru => "ID пользователя",
                Language.it => "ID utente",
                _ => "User ID",
            };
        }
        public string CmdCannotFindChannel()
        {
            return PreferredLanguage switch
            {
                Language.de => "Kanal konnte nicht gefunden werden.",
                Language.at => "Kanoi konnt ned gfundn wan.",
                Language.fr => "Impossible de trouver la chaîne.",
                Language.es => "No se puede encontrar el canal.",
                Language.ru => "Не могу найти канал.",
                Language.it => "Impossibile trovare il canale.",
                _ => "Cannot find channel.",
            };
        }
        public string CmdNoWebhookConfigured()
        {
            return PreferredLanguage switch
            {
                Language.de => "Dieser Server hat keinen internen Webhook für Benachrichtigungen konfiguriert.",
                Language.at => "Da Serva hot kan internan Webhook fia Benochrichtigungen konfiguriat.",
                Language.fr => "Cette guilde n'a pas configuré de webhook pour les notifications internes.",
                Language.es => "Este gremio no tiene configurado ningún webhook para notificaciones internas.",
                Language.ru => "У этой гильдии нет настроенного веб-перехватчика для внутренних уведомлений.",
                Language.it => "Questa gilda non ha webhook per le notifiche interne configurate.",
                _ => "This guild has no webhook for internal notifications configured.",
            };
        }
        public string CmdCleanup(int count, IMentionable channel)
        {
            return PreferredLanguage switch
            {
                Language.de => $"{count} Nachrichten in {channel.Mention} gelöscht.",
                Language.at => $"{count} Nochrichtn in {channel.Mention} glescht.",
                Language.fr => $"{count} messages supprimés dans {channel.Mention}.",
                Language.es => $"Se eliminaron {count} mensajes en {channel.Mention}.",
                Language.ru => $"Удалено {count} сообщений в {channel.Mention}.",
                Language.it => $"Eliminati {count} messaggi in {channel.Mention}.",
                _ => $"Deleted {count} messages in {channel.Mention}.",
            };
        }
        public string CmdFeaturesKickPermissionGranted()
        {
            return PreferredLanguage switch
            {
                Language.de => "Kick-Berechtigung erteilt.",
                Language.at => "Kick-Berechtigung erteit.",
                Language.fr => "Autorisation de kick accordée.",
                Language.es => "Permiso de patada concedido.",
                Language.ru => "Разрешение на удар предоставлено.",
                Language.it => "Autorizzazione calci concessa.",
                _ => "Kick permission granted.",
            };
        }
        public string CmdFeaturesKickPermissionNotGranted()
        {
            return PreferredLanguage switch
            {
                Language.de => "Kick-Berechtigung nicht erteilt.",
                Language.at => "Kick-Berechtigung ned erteit.",
                Language.fr => "L'autorisation de kick n'est pas accordée.",
                Language.es => "Permiso de patada no concedido.",
                Language.ru => "Разрешение на удар не предоставлено.",
                Language.it => "Autorizzazione calcio non concessa.",
                _ => "Kick permission not granted.",
            };
        }
        public string CmdFeaturesBanPermissionGranted()
        {
            return PreferredLanguage switch
            {
                Language.de => "Ban-Berechtigung erteilt.",
                Language.at => "Ban-Berechtigung erteit.",
                Language.fr => "Autorisation d'interdiction accordée.",
                Language.es => "Prohibición concedida.",
                Language.ru => "Получено разрешение на запрет.",
                Language.it => "Autorizzazione al divieto concessa.",
                _ => "Ban permission granted.",
            };
        }
        public string CmdFeaturesBanPermissionNotGranted()
        {
            return PreferredLanguage switch
            {
                Language.de => "Ban-Berechtigung nicht erteilt.",
                Language.at => "Ban-Berechtigung ned erteit.",
                Language.fr => "Autorisation d'interdiction non accordée.",
                Language.es => "Prohibir permiso no concedido.",
                Language.ru => "Разрешение на запрет не предоставлено.",
                Language.it => "Autorizzazione al divieto non concessa.",
                _ => "Ban permission not granted.",
            };
        }
        public string CmdFeaturesManageRolePermissionGranted()
        {
            return PreferredLanguage switch
            {
                Language.de => "Manage-Rolle-Berechtigung erteilt.",
                Language.at => "Manage-Rolle-Berechtigung ereit.",
                Language.fr => "Gérer l'autorisation de rôle accordée.",
                Language.es => "Administrar el permiso de función otorgado.",
                Language.ru => "Разрешение на управление ролью предоставлено.",
                Language.it => "Gestire l'autorizzazione del ruolo concessa.",
                _ => "Manage role permission granted.",
            };
        }
        public string CmdFeaturesManageRolePermissionNotGranted()
        {
            return PreferredLanguage switch
            {
                Language.de => "Manage-Rolle-Berechtigung nicht erteilt.",
                Language.at => "Manage-Rolle-Berechtigung ned erteit.",
                Language.fr => "L'autorisation de gestion du rôle n'est pas accordée.",
                Language.es => "Administrar el permiso de función no concedido.",
                Language.ru => "Не предоставлено разрешение на управление ролью.",
                Language.it => "Autorizzazione di gestione del ruolo non concessa.",
                _ => "Manage role permission not granted.",
            };
        }
        public string CmdFeaturesMutedRoleDefined()
        {
            return PreferredLanguage switch
            {
                Language.de => "Stummrolle definiert.",
                Language.at => "Stummroi definiat.",
                Language.fr => "Rôle muet défini.",
                Language.es => "Función silenciada definida.",
                Language.ru => "Определена приглушенная роль.",
                Language.it => "Ruolo disattivato definito.",
                _ => "Muted role defined.",
            };
        }
        public string CmdFeaturesMutedRoleDefinedButTooHigh()
        {
            return PreferredLanguage switch
            {
                Language.de => "Stummrolle definiert, aber zu hoch in der Rollenhierarchie.",
                Language.at => "Stummroi definiat, oba zu hoch in da Roinhierarchie.",
                Language.fr => "Rôle en sourdine défini mais trop élevé dans la hiérarchie des rôles.",
                Language.es => "Función silenciada definida pero demasiado alta en la jerarquía de funciones.",
                Language.ru => "Определена приглушенная роль, но иерархия ролей слишком высока.",
                Language.it => "Ruolo disattivato definito ma troppo alto nella gerarchia dei ruoli.",
                _ => "Muted role defined but too high in role hierarchy.",
            };
        }
        public string CmdFeaturesMutedRoleDefinedButInvalid()
        {
            return PreferredLanguage switch
            {
                Language.de => "Stummrolle definiert, aber ungültig.",
                Language.at => "Stummroi definiat, oba ned gütig.",
                Language.fr => "Rôle muet défini mais invalide.",
                Language.es => "Función silenciada definida pero no válida.",
                Language.ru => "Роль без звука определена, но недействительна.",
                Language.it => "Ruolo disattivato definito ma non valido.",
                _ => "Muted role defined but invalid.",
            };
        }
        public string CmdFeaturesMutedRoleUndefined()
        {
            return PreferredLanguage switch
            {
                Language.de => "Stummrolle nicht definiert.",
                Language.at => "Stummroi ned definiat.",
                Language.fr => "Rôle en sourdine non défini.",
                Language.es => "Rol silenciado indefinido.",
                Language.ru => "Отключенная роль не определена.",
                Language.it => "Ruolo disattivato non definito.",
                _ => "Muted role undefined.",
            };
        }
        public string CmdFeaturesPunishmentExecution()
        {
            return PreferredLanguage switch
            {
                Language.de => "Bestrafungsverwaltung",
                Language.at => "Bestrofungsverwoitung",
                Language.fr => "Exécution de la peine",
                Language.es => "Ejecución del castigo",
                Language.ru => "Казнь",
                Language.it => "Esecuzione della punizione",
                _ => "Punishment execution",
            };
        }
        public string CmdFeaturesPunishmentExecutionDescription()
        {
            return PreferredLanguage switch
            {
                Language.de => "Lass MASZ die Bestrafungen verwalten (z.B. temporäre Banns, Stummschaltungen, etc.).",
                Language.at => "Loss MASZ de Bestrofungen verwoitn (z.B. temporäre Banns, Stummschoitungen, etc.).",
                Language.fr => "Laissez MASZ gérer les punitions (par exemple, tempbans, muets, etc.).",
                Language.es => "Deje que MASZ maneje los castigos (por ejemplo, tempbans, mudos, etc.).",
                Language.ru => "Позвольте MASZ заниматься наказаниями (например, временным запретом, отключением звука и т. Д.).",
                Language.it => "Lascia che MASZ gestisca le punizioni (ad esempio tempban, mute, ecc.).",
                _ => "Let MASZ handle punishments (e.g. tempbans, mutes, etc.).",
            };
        }
        public string CmdFeaturesUnbanRequests()
        {
            return PreferredLanguage switch
            {
                Language.de => "Entbannungs-Anfragen",
                Language.at => "Entbannungs-Ofrogn",
                Language.fr => "Annuler l'interdiction des demandes",
                Language.es => "Solicitudes de anulación de la prohibición",
                Language.ru => "Запросы на разблокировку",
                Language.it => "Riattiva richieste",
                _ => "Unban requests",
            };
        }
        public string CmdFeaturesUnbanRequestsDescriptionGranted()
        {
            return PreferredLanguage switch
            {
                Language.de => "Erlaubt Gebannten MASZ aufzurufen, sich ihre Fälle anzusehen und diese sie zu kommentieren.",
                Language.at => "Erlaubt ausgsperrtn MASZ aufzuruafa, sich ernane Fälle ozumschaun und de zum kommentian.",
                Language.fr => "Permet aux membres bannis de voir leurs cas et de les commenter pour les demandes de déban.",
                Language.es => "Permite a los miembros prohibidos ver sus casos y comentarlos para las solicitudes de deshabilitación.",
                Language.ru => "Позволяет заблокированным участникам просматривать свои дела и комментировать их для запросов на разблокировку.",
                Language.it => "Consente ai membri bannati di vedere i loro casi e commentarli per le richieste di sban.",
                _ => "Allows banned members to see their cases and comment on it for unban requests.",
            };
        }
        public string CmdFeaturesUnbanRequestsDescriptionNotGranted()
        {
            return PreferredLanguage switch
            {
                Language.de => "Erlaubt Gebannten MASZ aufzurufen, sich ihre Fälle anzusehen und diese sie zu kommentieren.\nErteile diesem Bot die Ban-Berechtigung, um diese Funktion zu nutzen.",
                Language.at => "Erlaubt ausgsperrtn MASZ aufzurufa, sich ernane Fälle ozumschaun und de zum kommentian. \nErteil dem Bot die Ban-Berechtigung, um de Funktion nutza zu kenna.",
                Language.fr => "Permet aux membres bannis de voir leurs cas et de les commenter pour les demandes de déban.\nAccordez à ce bot l'autorisation d'interdire l'utilisation de cette fonctionnalité.",
                Language.es => "Permite a los miembros prohibidos ver sus casos y comentarlos para las solicitudes de deshabilitación.\nOtorga a este bot el permiso de prohibición para usar esta función.",
                Language.ru => "Позволяет заблокированным участникам просматривать свои дела и комментировать их для запросов на разблокировку.\nПредоставьте этому боту разрешение на использование этой функции.",
                Language.it => "Consente ai membri bannati di vedere i loro casi e commentarli per le richieste di sban.\nConcedi a questo bot il permesso di ban per utilizzare questa funzione.",
                _ => "Allows banned members to see their cases and comment on it for unban requests.\nGrant this bot the ban permission to use this feature.",
            };
        }
        public string CmdFeaturesReportCommand()
        {
            return PreferredLanguage switch
            {
                Language.de => "Melde-Befehl",
                Language.at => "Möde-Befehl",
                Language.fr => "Commande de rapport",
                Language.es => "Comando de informe",
                Language.ru => "Команда отчета",
                Language.it => "Comando di rapporto",
                _ => "Report command",
            };
        }
        public string CmdFeaturesReportCommandDescriptionGranted()
        {
            return PreferredLanguage switch
            {
                Language.de => "Erlaubt Mitgliedern, Nachrichten zu melden.",
                Language.at => "Erlaubt Mitglieda, Nochrichtn zu mödn.",
                Language.fr => "Permet aux membres de signaler des messages.",
                Language.es => "Permite a los miembros informar mensajes.",
                Language.ru => "Позволяет участникам сообщать о сообщениях.",
                Language.it => "Consente ai membri di segnalare i messaggi.",
                _ => "Allows members to report messages.",
            };
        }
        public string CmdFeaturesReportCommandDescriptionNotGranted()
        {
            return PreferredLanguage switch
            {
                Language.de => "Erlaubt Mitgliedern, Nachrichten zu melden.\nDefiniere einen internen Webhook, um diese Funktion zu nutzen.",
                Language.at => "Erlaub Mitglieda, Nochrichtn zum mödn. \nDefinia an internen Webook, um de Funktion nutzn zum kennan.",
                Language.fr => "Permet aux membres de signaler des messages.\nDéfinissez un webhook interne pour le personnel pour utiliser cette fonctionnalité.",
                Language.es => "Permite a los miembros informar mensajes.\nDefina un webhook de personal interno para utilizar esta función.",
                Language.ru => "Позволяет участникам сообщать о сообщениях.\nОпределите внутренний веб-перехватчик персонала, чтобы использовать эту функцию.",
                Language.it => "Consente ai membri di segnalare i messaggi.\nDefinire un webhook personale interno per utilizzare questa funzione.",
                _ => "Allows members to report messages.\nDefine a internal staff webhook to use this feature.",
            };
        }
        public string CmdFeaturesInviteTracking()
        {
            return PreferredLanguage switch
            {
                Language.de => "Einladungsverfolgung",
                Language.at => "Eiladungsverfoigung",
                Language.fr => "Suivi des invitations",
                Language.es => "Seguimiento de invitaciones",
                Language.ru => "Отслеживание приглашений",
                Language.it => "Invita il monitoraggio",
                _ => "Invite tracking",
            };
        }
        public string CmdFeaturesInviteTrackingDescriptionGranted()
        {
            return PreferredLanguage switch
            {
                Language.de => "Erlaubt MASZ, die Einladungen neuer Mitglieder zu verfolgen.",
                Language.at => "Erlaubt MASZ, de Eiladungen vo neichn Mitglieda zu verfoign.",
                Language.fr => "Permet MASZ de suivre les nouveaux membres invite utilisent.",
                Language.es => "Permite a MASZ realizar un seguimiento de las invitaciones que están utilizando los nuevos miembros.",
                Language.ru => "Позволяет MASZ отслеживать приглашения, которые используют новые участники.",
                Language.it => "Consente a MASZ di tenere traccia degli inviti utilizzati dai nuovi membri.",
                _ => "Allows MASZ to track the invites new members are using.",
            };
        }
        public string CmdFeaturesInviteTrackingDescriptionNotGranted()
        {
            return PreferredLanguage switch
            {
                Language.de => "Erlaubt MASZ, die Einladungen neuer Mitglieder zu verfolgen.\nErteile diesem Bot die Verwalten-Gilden-Berechtigung, um diese Funktion zu nutzen.",
                Language.at => "Erlaubt MASZ, de Eiladungen vo neichn Mitglieda zu verfoign.\nErteil dem Bot die Verwoitn-Gilden-Berechtigung, um de Funktion nutzn zu kenna.",
                Language.fr => "Permet à MASZ de suivre les invitations que les nouveaux membres utilisent.\nAccordez à ce bot l'autorisation de gestion de guilde pour utiliser cette fonctionnalité.",
                Language.es => "Permite a MASZ realizar un seguimiento de las invitaciones que están utilizando los nuevos miembros.\nOtorga a este bot el permiso de gestión del gremio para usar esta función.",
                Language.ru => "Позволяет MASZ отслеживать приглашения, которые используют новые участники.\nПредоставьте этому боту разрешение на управление гильдией на использование этой функции.",
                Language.it => "Consente a MASZ di tenere traccia degli inviti utilizzati dai nuovi membri.\nConcedi a questo bot il permesso di gestione della gilda per utilizzare questa funzione.",
                _ => "Allows MASZ to track the invites new members are using.\nGrant this bot the manage guild permission to use this feature.",
            };
        }
        public string CmdFeaturesSupportAllFeatures()
        {
            return PreferredLanguage switch
            {
                Language.de => "Dein Bot auf diesem Server ist richtig konfiguriert.",
                Language.at => "Dei Bot auf dem Serva is richtig konfiguriat.",
                Language.fr => "Votre bot sur cette guilde est correctement configuré.",
                Language.es => "Tu bot en este gremio está configurado correctamente.",
                Language.ru => "Ваш бот в этой гильдии настроен правильно.",
                Language.it => "Il tuo bot in questa gilda è configurato correttamente.",
                _ => "Your bot on this guild is configured correctly.",
            };
        }
        public string CmdFeaturesSupportAllFeaturesDesc()
        {
            return PreferredLanguage switch
            {
                Language.de => "Alle Funktionen von MASZ können genutzt werden.",
                Language.at => "Olle Funktionen vo MASZ kennen gnutzt wean.",
                Language.fr => "Toutes les fonctionnalités de MASZ peuvent être utilisées.",
                Language.es => "Se pueden utilizar todas las funciones de MASZ.",
                Language.ru => "Можно использовать все возможности MASZ.",
                Language.it => "Tutte le funzionalità di MASZ possono essere utilizzate.",
                _ => "All features of MASZ can be used.",
            };
        }
        public string CmdFeaturesMissingFeatures()
        {
            return PreferredLanguage switch
            {
                Language.de => "Es gibt Funktionen von MASZ, die du jetzt nicht nutzen kannst.",
                Language.at => "Es gibt Funktionen vo MASZ, die du jetzt ned nutzn konnst.",
                Language.fr => "Il y a des fonctionnalités de MASZ que vous ne pouvez pas utiliser pour le moment.",
                Language.es => "Hay funciones de MASZ que no puede utilizar en este momento.",
                Language.ru => "Есть функции MASZ, которые вы не можете использовать прямо сейчас.",
                Language.it => "Ci sono funzionalità di MASZ che non puoi usare in questo momento.",
                _ => "There are features of MASZ that you cannot use right now.",
            };
        }
        public string CmdInvite()
        {
            return PreferredLanguage switch
            {
                Language.de => "Du musst deine eigene Instanz von MASZ auf deinem Server oder PC hosten.\nSchau dir https://github.com/zaanposni/discord-masz#hosting an",
                Language.at => "Du muast dei eignane Inszanz vo MASZ auf deim Serva oda PC hosn.\nSchau da https://github.com/zaanposni/discord-masz#hosting o",
                Language.fr => "Vous devrez héberger votre propre instance de MASZ sur votre serveur ou votre PC.\nCommander https://github.com/zaanposni/discord-masz#hosting",
                Language.es => "Tendrá que alojar su propia instancia de MASZ en su servidor o PC.\nPagar https://github.com/zaanposni/discord-masz#hosting",
                Language.ru => "Вам нужно будет разместить свой собственный экземпляр MASZ на вашем сервере или компьютере.\nОформить заказ https://github.com/zaanposni/discord-masz#hosting",
                Language.it => "Dovrai ospitare la tua istanza di MASZ sul tuo server o PC.\nAcquista https://github.com/zaanposni/discord-masz#hosting",
                _ => "You will have to host your own instance of MASZ on your server or pc.\nCheck out https://github.com/zaanposni/discord-masz#hosting",
            };
        }
        public string CmdPunish(int caseId, string caseLink)
        {
            return PreferredLanguage switch
            {
                Language.de => $"Fall `#{caseId}` erstellt: {caseLink}",
                Language.at => $"Foi `#{caseId}` erstöt: {caseLink}",
                Language.fr => $"Cas `#{caseId}` créé : {caseLink}",
                Language.es => $"Caso `# {caseId}` creado: {caseLink}",
                Language.ru => $"Обращение `# {caseId}` создано: {caseLink}",
                Language.it => $"Caso `#{caseId}` creato: {caseLink}",
                _ => $"Case `#{caseId}` created: {caseLink}",
            };
        }
        public string CmdRegister(string url)
        {
            return PreferredLanguage switch
            {
                Language.de => $"Ein Siteadmin kann eine Gilde registrieren unter: {url}",
                Language.at => $"A Seitnadmin ko a Güde unta {url} registrian.",
                Language.fr => $"Un administrateur de site peut enregistrer une guilde à l'adresse : {url}",
                Language.es => $"Un administrador de sitio puede registrar un gremio en: {url}",
                Language.ru => $"Администратор сайта может зарегистрировать гильдию по адресу: {url}",
                Language.it => $"Un amministratore del sito può registrare una gilda su: {url}",
                _ => $"A siteadmin can register a guild at: {url}",
            };
        }
        public string CmdReportFailed()
        {
            return PreferredLanguage switch
            {
                Language.de => "Interner Benachrichtigungsversand an Moderatoren für Meldebefehl fehlgeschlagen.",
                Language.at => "Interna Benochrichtigungsvasond on de Modaratoan fian Mödebefehl fehlgschlogn.",
                Language.fr => "Échec de l'envoi de la notification interne aux modérateurs pour la commande de rapport.",
                Language.es => "No se pudo enviar una notificación interna a los moderadores para el comando de informe.",
                Language.ru => "Не удалось отправить внутреннее уведомление модераторам для команды отчета.",
                Language.it => "Impossibile inviare una notifica interna ai moderatori per il comando di segnalazione.",
                _ => "Failed to send internal notification to moderators for report command.",
            };
        }
        public string CmdReportSent()
        {
            return PreferredLanguage switch
            {
                Language.de => "Meldung gesendet.",
                Language.at => "Mödung gsendt.",
                Language.fr => "Rapport envoyé.",
                Language.es => "Reporte enviado.",
                Language.ru => "Отчет отправлен.",
                Language.it => "Rapporto inviato.",
                _ => "Report sent.",
            };
        }
        public string CmdReportContent(IUser user, IMessage message, IMentionable channel)
        {
            return PreferredLanguage switch
            {
                Language.de => $"{user.Mention} meldete eine Nachricht von {message.Author.Mention} in {channel.Mention}.\n{message.GetJumpUrl()}",
                Language.at => $"{user.Mention} mödet a Nochricht vo {message.Author.Mention} in {channel.Mention}.\n{message.GetJumpUrl()}",
                Language.fr => $"{user.Mention} a signalé un message de {message.Author.Mention} dans {channel.Mention}.\n{message.GetJumpUrl()}",
                Language.es => $"{user.Mention} informó un mensaje de {message.Author.Mention} en {channel.Mention}.\n{message.GetJumpUrl()}",
                Language.ru => $"{user.Mention} сообщил о сообщении от {message.Author.Mention} в {channel.Mention}.\n{message.GetJumpUrl()}",
                Language.it => $"{user.Mention} ha segnalato un messaggio da {message.Author.Mention} in {channel.Mention}.\n{message.GetJumpUrl()}",
                _ => $"{user.Mention} reported a message from {message.Author.Mention} in {channel.Mention}.\n{message.GetJumpUrl()}",
            };
        }
        public string CmdSayFailed()
        {
            return PreferredLanguage switch
            {
                Language.de => "Senden der Nachricht fehlgeschlagen",
                Language.at => "Sendn vo da Nachricht fehlgschlogn.",
                Language.fr => "Échec de l'envoi du message",
                Language.es => "No se pudo enviar el mensaje",
                Language.ru => "Не удалось отправить сообщение",
                Language.it => "Impossibile inviare il messaggio",
                _ => "Failed to send message",
            };
        }
        public string CmdSaySent()
        {
            return PreferredLanguage switch
            {
                Language.de => "Nachricht gesendet.",
                Language.at => "Nochricht gsendet.",
                Language.fr => "Message envoyé.",
                Language.es => "Mensaje enviado.",
                Language.ru => "Сообщение отправлено.",
                Language.it => "Messaggio inviato.",
                _ => "Message sent.",
            };
        }
        public string CmdTrackInviteNotFromThisGuild()
        {
            return PreferredLanguage switch
            {
                Language.de => "Die Einladung ist nicht von dieser Gilde.",
                Language.at => "Die Eiladung is ned vo dera Güde.",
                Language.fr => "L'invitation n'est pas de cette guilde.",
                Language.es => "La invitación no es de este gremio.",
                Language.ru => "Приглашение не из этой гильдии.",
                Language.it => "L'invito non è di questa gilda.",
                _ => "The invite is not from this guild.",
            };
        }
        public string CmdTrackCannotFindInvite()
        {
            return PreferredLanguage switch
            {
                Language.de => "Konnte die Einladung nicht in der Datenbank oder in dieser Gilde finden.",
                Language.at => "Konnt de Eiladung ned in da Datnbank vo da Güde findn.",
                Language.fr => "Impossible de trouver l'invitation dans la base de données ou dans cette guilde.",
                Language.es => "No se pudo encontrar la invitación en la base de datos o en este gremio.",
                Language.ru => "Не удалось найти инвайт в базе данных или в этой гильдии.",
                Language.it => "Impossibile trovare l'invito nel database o in questa gilda.",
                _ => "Could not find invite in database or in this guild.",
            };
        }
        public string CmdTrackFailedToFetchInvite()
        {
            return PreferredLanguage switch
            {
                Language.de => "Konnte die Einladung nicht abrufen.",
                Language.at => "Konnt die Eiladung ned orufn.",
                Language.fr => "Échec de la récupération de l'invitation.",
                Language.es => "No se pudo recuperar la invitación.",
                Language.ru => "Не удалось получить приглашение.",
                Language.it => "Impossibile recuperare l'invito.",
                _ => "Failed to fetch invite.",
            };
        }
        public string CmdTrackCreatedAt(string inviteCode, DateTime createdAt)
        {
            return PreferredLanguage switch
            {
                Language.de => $"`{inviteCode}` erstellt am {createdAt.ToDiscordTS()}.",
                Language.at => $"`{inviteCode}` erstöt vo {createdAt.ToDiscordTS()}.",
                Language.fr => $"`{inviteCode}` créé à {createdAt.ToDiscordTS()}.",
                Language.es => $"`{inviteCode}` creado en {createdAt.ToDiscordTS ()}.",
                Language.ru => $"`{inviteCode}` создан в {createdAt.ToDiscordTS ()}.",
                Language.it => $"`{inviteCode}` creato su {createdAt.ToDiscordTS()}.",
                _ => $"`{inviteCode}` created at {createdAt.ToDiscordTS()}.",
            };
        }
        public string CmdTrackCreatedBy(string inviteCode, IUser createdBy)
        {
            return PreferredLanguage switch
            {
                Language.de => $"`{inviteCode}` erstellt von {createdBy.Mention}.",
                Language.at => $"`{inviteCode}` erstöt vo {createdBy.Mention}.",
                Language.fr => $"`{inviteCode}` créé par {createdBy.Mention}.",
                Language.es => $"`{inviteCode}` creado por {createdBy.Mention}.",
                Language.ru => $"`{inviteCode}` создан {createdBy.Mention}.",
                Language.it => $"`{inviteCode}` creato da {createdBy.Mention}.",
                _ => $"`{inviteCode}` created by {createdBy.Mention}.",
            };
        }
        public string CmdTrackCreatedByAt(string inviteCode, IUser createdBy, DateTime createdAt)
        {
            return PreferredLanguage switch
            {
                Language.de => $"`{inviteCode}` erstellt von {createdBy.Mention} am {createdAt.ToDiscordTS()}.",
                Language.at => $"`{inviteCode}` erstöt vo {createdBy.Mention} om {createdAt.ToDiscordTS()}.",
                Language.fr => $"`{inviteCode}` créé par {createdBy.Mention} à {createdAt.ToDiscordTS()}.",
                Language.es => $"`{inviteCode}` creado por {createdBy.Mention} en {createdAt.ToDiscordTS ()}.",
                Language.ru => $"`{inviteCode}` создан {createdBy.Mention} в {createdAt.ToDiscordTS ()}.",
                Language.it => $"`{inviteCode}` creato da {createdBy.Mention} su {createdAt.ToDiscordTS()}.",
                _ => $"`{inviteCode}` created by {createdBy.Mention} at {createdAt.ToDiscordTS()}.",
            };
        }
        public string CmdTrackNotTrackedYet()
        {
            return PreferredLanguage switch
            {
                Language.de => "Diese Einladung wurde noch nicht von MASZ gespeichert.",
                Language.at => "Die Eiladung wuad no ned vo MASZ gspeichat.",
                Language.fr => "Cette invitation n'a pas encore été suivie par MASZ.",
                Language.es => "MASZ aún no ha realizado el seguimiento de esta invitación.",
                Language.ru => "Это приглашение еще не отслежено MASZ.",
                Language.it => "Questo invito non è stato ancora monitorato da MASZ.",
                _ => "This invite has not been tracked by MASZ yet.",
            };
        }
        public string CmdTrackUsedBy(int count)
        {
            return PreferredLanguage switch
            {
                Language.de => $"Benutzt von [{count}]",
                Language.at => $"Benutzt vo [{count}]",
                Language.fr => $"Utilisé par [{count}]",
                Language.es => $"Usado por [{count}]",
                Language.ru => $"Используется [{count}]",
                Language.it => $"Utilizzato da [{count}]",
                _ => $"Used by [{count}]",
            };
        }
        public string CmdViewInvalidGuildId()
        {
            return PreferredLanguage switch
            {
                Language.de => "Bitte gib eine gültige Gilden-ID an.",
                Language.at => "Bitte gib a gütige Güdn-ID o.",
                Language.fr => "Veuillez spécifier un identifiant de guilde valide.",
                Language.es => "Por favor, especifique un guildid válido.",
                Language.ru => "Укажите действующего гильдида.",
                Language.it => "Si prega di specificare un ID gilda valido.",
                _ => "Please specify a valid guildid.",
            };
        }
        public string CmdViewNotAllowedToView()
        {
            return PreferredLanguage switch
            {
                Language.de => "Du darfst diesen Fall nicht ansehen.",
                Language.at => "Du derfst da den Foi ned oschaun.",
                Language.fr => "Vous n'êtes pas autorisé à voir ce cas.",
                Language.es => "No se le permite ver este caso.",
                Language.ru => "Вам не разрешено просматривать это дело.",
                Language.it => "Non sei autorizzato a visualizzare questo caso.",
                _ => "You are not allowed to view this case.",
            };
        }
        public string CmdWhoisUsedInvite(string inviteCode)
        {
            return PreferredLanguage switch
            {
                Language.de => $"Benutzte Einladung `{inviteCode}`.",
                Language.at => $"Benutze Eilodung `{inviteCode}`.",
                Language.fr => $"Invitation utilisée `{inviteCode}`.",
                Language.es => $"Invitación usada `{inviteCode}`.",
                Language.ru => $"Использовал инвайт `{inviteCode}`.",
                Language.it => $"Invito usato `{inviteCode}`.",
                _ => $"Used invite `{inviteCode}`.",
            };
        }
        public string CmdWhoisInviteBy(ulong user)
        {
            return PreferredLanguage switch
            {
                Language.de => $"Von <@{user}>.",
                Language.at => $"Vo <@{user}>",
                Language.fr => $"Par <@{user}>.",
                Language.es => $"Por <@{user}>.",
                Language.ru => $"Автор <@{user}>.",
                Language.it => $"Da <@{user}>.",
                _ => $"By <@{user}>.",
            };
        }
        public string CmdWhoisNoCases()
        {
            return PreferredLanguage switch
            {
                Language.de => "Es gibt keine Fälle für diesen Benutzer.",
                Language.at => "Es gibt kane Fälle fia diesn Benutza.",
                Language.fr => "Il n'y a pas de cas pour cet utilisateur.",
                Language.es => "No hay casos para este usuario.",
                Language.ru => "Для этого пользователя нет случаев.",
                Language.it => "Non ci sono casi per questo utente.",
                _ => "There are no cases for this user.",
            };
        }
        public string CmdUndoResultTitle()
        {
            return PreferredLanguage switch
            {
                Language.de => "Ergebnis",
                _ => "Result",
            };
        }
        public string CmdUndoResultWaiting()
        {
            return PreferredLanguage switch
            {
                Language.de => "Warte auf Bestätigung.",
                _ => "Waiting for approval.",
            };
        }
        public string CmdUndoResultTimedout()
        {
            return PreferredLanguage switch
            {
                Language.de => "Zeitüberschreitung",
                _ => "Timed out",
            };
        }
        public string CmdUndoResultCanceled()
        {
            return PreferredLanguage switch
            {
                Language.de => "Abgebrochen",
                _ => "Canceled",
            };
        }
        public string CmdUndoPublicNotificationTitle()
        {
            return PreferredLanguage switch
            {
                Language.de => "Öffentliche Benachrichtigung",
                _ => "Public notification",
            };
        }
        public string CmdUndoPublicNotificationDescription()
        {
            return PreferredLanguage switch
            {
                Language.de => "Soll eine öffentliche Benachrichtigung gesendet werden?",
                _ => "Send a public notification?",
            };
        }
        public string CmdUndoButtonsCancel()
        {
            return PreferredLanguage switch
            {
                Language.de => "Abbrechen",
                _ => "Cancel",
            };
        }
        public string CmdUndoButtonsPublicNotification()
        {
            return PreferredLanguage switch
            {
                Language.de => "Öffentliche Benachrichtigung",
                _ => "Public notification",
            };
        }
        public string CmdUndoButtonsNoPublicNotification()
        {
            return PreferredLanguage switch
            {
                Language.de => "Keine öffentliche Benachrichtigung",
                _ => "No public notification",
            };
        }
        public string CmdUndoCreatedAt()
        {
            return PreferredLanguage switch
            {
                Language.de => "Erstellt am.",
                _ => "Created at.",
            };
        }
        public string CmdUndoNoCases()
        {
            return PreferredLanguage switch
            {
                Language.de => "Keine aktiven Mod-Fälle wurden gefunden.",
                _ => "No active modcases have been found.",
            };
        }
        public string CmdUndoUnmuteFoundXCases(int caseCount)
        {
            return PreferredLanguage switch
            {
                Language.de => $"Es wurden `{caseCount}` aktive Fälle gefunden. Möchtest du alle deaktivieren oder löschen, um den Benutzer entsperren zu lassen?",
                _ => $"Found `{caseCount}` active cases. Do you want to deactivate or delete all of them to unmute the user?",
            };
        }
        public string CmdUndoUnmuteResultDeleted()
        {
            return PreferredLanguage switch
            {
                Language.de => "Sperrungen gelöscht",
                _ => "Mutes deleted",
            };
        }
        public string CmdUndoUnmuteResultDeactivated()
        {
            return PreferredLanguage switch
            {
                Language.de => "Sperrungen deaktiviert",
                _ => "Mutes deactivated",
            };
        }
        public string CmdUndoUnmuteButtonsDelete()
        {
            return PreferredLanguage switch
            {
                Language.de => "Sperrungen löschen",
                _ => "Delete Mutes",
            };
        }
        public string CmdUndoUnmuteButtonsDeactivate()
        {
            return PreferredLanguage switch
            {
                Language.de => "Sperrungen deaktivieren",
                _ => "Deativate Mutes",
            };
        }
        public string CmdUndoUnbanFoundXCases(int caseCount)
        {
            return PreferredLanguage switch
            {
                Language.de => $"Es wurden `{caseCount}` aktive Fälle gefunden. Möchtest du alle deaktivieren oder löschen, um den Benutzer entsperren zu lassen?",
                _ => $"Found `{caseCount}` active cases. Do you want to deactivate or delete all of them to unban the user?",
            };
        }
        public string CmdUndoUnbanResultDeleted()
        {
            return PreferredLanguage switch
            {
                Language.de => "Sperrungen gelöscht",
                _ => "Bans deleted",
            };
        }
        public string CmdUndoUnbanResultDeactivated()
        {
            return PreferredLanguage switch
            {
                Language.de => "Sperrungen deaktiviert",
                _ => "Bans deactivated",
            };
        }
        public string CmdUndoUnbanButtonsDelete()
        {
            return PreferredLanguage switch
            {
                Language.de => "Sperrungen löschen",
                _ => "Delete Bans",
            };
        }
        public string CmdUndoUnbanButtonsDeactivate()
        {
            return PreferredLanguage switch
            {
                Language.de => "Sperrungen deaktivieren",
                _ => "Deativate Bans",
            };
        }
        public string GuildAuditLogChannel()
        {
            return PreferredLanguage switch
            {
                Language.de => "Kanal",
                Language.at => "Kanal",
                Language.fr => "Canaliser",
                Language.es => "Canal",
                Language.ru => "Канал",
                Language.it => "Canale",
                _ => "Channel",
            };
        }
        public string GuildAuditLogChannelId()
        {
            return PreferredLanguage switch
            {
                Language.de => "KanalId",
                Language.at => "KanalId",
                Language.fr => "Identifiant de la chaine",
                Language.es => "Canal ID",
                Language.ru => "ChannelId",
                Language.it => "Canale ID",
                _ => "ChannelId",
            };
        }
        public string GuildAuditLogID()
        {
            return PreferredLanguage switch
            {
                Language.de => "ID",
                Language.at => "ID",
                Language.fr => "identifiant",
                Language.es => "IDENTIFICACIÓN",
                Language.ru => "Я БЫ",
                Language.it => "ID",
                _ => "ID",
            };
        }
        public string GuildAuditLogUserID()
        {
            return PreferredLanguage switch
            {
                Language.de => "NutzerId",
                Language.at => "NutzaId",
                Language.fr => "Identifiant d'utilisateur",
                Language.es => "User ID",
                Language.ru => "ID пользователя",
                Language.it => "ID utente",
                _ => "User ID",
            };
        }
        public string GuildAuditLogUser()
        {
            return PreferredLanguage switch
            {
                Language.de => "Nutzer",
                Language.at => "Nutza",
                Language.fr => "Utilisateur",
                Language.es => "Usuario",
                Language.ru => "Пользователь",
                Language.it => "Utente",
                _ => "User",
            };
        }
        public string GuildAuditLogAuthor()
        {
            return PreferredLanguage switch
            {
                Language.de => "Autor",
                Language.at => "Autor",
                Language.fr => "Auteur",
                Language.es => "Autor",
                Language.ru => "Автор",
                Language.it => "Autore",
                _ => "Author",
            };
        }
        public string GuildAuditLogCreated()
        {
            return PreferredLanguage switch
            {
                Language.de => "Erstellt",
                Language.at => "Erstöt",
                Language.fr => "Créé",
                Language.es => "Creado",
                Language.ru => "Созданный",
                Language.it => "Creato",
                _ => "Created",
            };
        }
        public string GuildAuditLogCouldNotFetch()
        {
            return PreferredLanguage switch
            {
                Language.de => "Information konnte nicht abgerufen werden.",
                Language.at => "Info konnt ned orufn werdn.",
                Language.fr => "Impossible de récupérer.",
                Language.es => "No se pudo recuperar.",
                Language.ru => "Не удалось получить.",
                Language.it => "Impossibile recuperare.",
                _ => "Could not fetch.",
            };
        }
        public string GuildAuditLogNotFoundInCache()
        {
            return PreferredLanguage switch
            {
                Language.de => "Information nicht im Cache.",
                Language.at => "Info ned im Cache.",
                Language.fr => "Informations non mises en cache.",
                Language.es => "Información no almacenada en caché.",
                Language.ru => "Информация не кешируется.",
                Language.it => "Informazioni non memorizzate nella cache.",
                _ => "Information not cached.",
            };
        }
        public string GuildAuditLogMessageSentTitle()
        {
            return PreferredLanguage switch
            {
                Language.de => "Nachricht gesendet",
                Language.at => "Nochricht gsendet",
                Language.fr => "Message envoyé",
                Language.es => "Mensaje enviado",
                Language.ru => "Сообщение отправлено",
                Language.it => "Messaggio inviato",
                _ => "Message sent",
            };
        }
        public string GuildAuditLogMessageSentContent()
        {
            return PreferredLanguage switch
            {
                Language.de => "Inhalt",
                Language.at => "Inhalt",
                Language.fr => "Teneur",
                Language.es => "Contenido",
                Language.ru => "Содержание",
                Language.it => "Contenuto",
                _ => "Content",
            };
        }
        public string GuildAuditLogMessageUpdatedTitle()
        {
            return PreferredLanguage switch
            {
                Language.de => "Nachricht aktualisiert",
                Language.at => "Nochricht aktualisiert",
                Language.fr => "Message modifié",
                Language.es => "Mensaje editado",
                Language.ru => "Сообщение отредактировано",
                Language.it => "Messaggio modificato",
                _ => "Message edited",
            };
        }
        public string GuildAuditLogMessageUpdatedContentBefore()
        {
            return PreferredLanguage switch
            {
                Language.de => "Zuvor",
                Language.at => "Davoa",
                Language.fr => "Avant",
                Language.es => "Antes",
                Language.ru => "До",
                Language.it => "Prima",
                _ => "Before",
            };
        }
        public string GuildAuditLogMessageUpdatedContentNew()
        {
            return PreferredLanguage switch
            {
                Language.de => "Neu",
                Language.at => "Neich",
                Language.fr => "Nouveau",
                Language.es => "Nuevo",
                Language.ru => "Новый",
                Language.it => "Nuovo",
                _ => "New",
            };
        }
        public string GuildAuditLogMessageDeletedTitle()
        {
            return PreferredLanguage switch
            {
                Language.de => "Nachricht gelöscht",
                Language.at => "Nochricht gelöscht",
                Language.fr => "Message supprimé",
                Language.es => "Mensaje borrado",
                Language.ru => "Сообщение удалено",
                Language.it => "Messaggio cancellato",
                _ => "Message deleted",
            };
        }
        public string GuildAuditLogMessageDeletedContent()
        {
            return PreferredLanguage switch
            {
                Language.de => "Inhalt",
                Language.at => "Inhalt",
                Language.fr => "Teneur",
                Language.es => "Contenido",
                Language.ru => "Содержание",
                Language.it => "Contenuto",
                _ => "Content",
            };
        }
        public string GuildAuditLogBanAddedTitle()
        {
            return PreferredLanguage switch
            {
                Language.de => "Nutzer gebannt",
                Language.at => "Nutza ausgsperrt",
                Language.fr => "Utilisateur banni",
                Language.es => "Usuario baneado",
                Language.ru => "Пользователь забанен",
                Language.it => "Utente bannato",
                _ => "User banned",
            };
        }
        public string GuildAuditLogBanRemovedTitle()
        {
            return PreferredLanguage switch
            {
                Language.de => "Nutzer entbannt",
                Language.at => "Nutza nimma ausgsperrt",
                Language.fr => "Utilisateur non banni",
                Language.es => "Usuario no prohibido",
                Language.ru => "Пользователь разблокирован",
                Language.it => "Utente non bannato",
                _ => "User unbanned",
            };
        }
        public string GuildAuditLogInviteCreatedTitle()
        {
            return PreferredLanguage switch
            {
                Language.de => "Einladung erstellt",
                Language.at => "Eiladung erstöt",
                Language.fr => "Invitation créée",
                Language.es => "Invitación creada",
                Language.ru => "Приглашение создано",
                Language.it => "Invito creato",
                _ => "Invite created",
            };
        }
        public string GuildAuditLogInviteCreatedURL()
        {
            return PreferredLanguage switch
            {
                Language.de => "URL",
                Language.at => "URL",
                Language.fr => "URL",
                Language.es => "URL",
                Language.ru => "URL",
                Language.it => "URL",
                _ => "URL",
            };
        }
        public string GuildAuditLogInviteCreatedMaxUses()
        {
            return PreferredLanguage switch
            {
                Language.de => "Maximale Nutzungen",
                Language.at => "Maximale Vawednungen",
                Language.fr => "Utilisations maximales",
                Language.es => "Usos máximos",
                Language.ru => "Макс использует",
                Language.it => "Usi massimi",
                _ => "Max uses",
            };
        }
        public string GuildAuditLogInviteCreatedExpiration()
        {
            return PreferredLanguage switch
            {
                Language.de => "Ablaufdatum",
                Language.at => "Oblaufdatum",
                Language.fr => "Date d'expiration",
                Language.es => "Fecha de caducidad",
                Language.ru => "Срок хранения",
                Language.it => "Data di scadenza",
                _ => "Expiration date",
            };
        }
        public string GuildAuditLogInviteCreatedTargetChannel()
        {
            return PreferredLanguage switch
            {
                Language.de => "Zielkanal",
                Language.at => "Zielkanal",
                Language.fr => "Canal cible",
                Language.es => "Canal objetivo",
                Language.ru => "Целевой канал",
                Language.it => "Canale di destinazione",
                _ => "Target channel",
            };
        }
        public string GuildAuditLogInviteDeletedTitle()
        {
            return PreferredLanguage switch
            {
                Language.de => "Einladung gelöscht",
                Language.at => "Eiladung glescht",
                Language.fr => "Invitation supprimée",
                Language.es => "Invitación eliminada",
                Language.ru => "Приглашение удалено",
                Language.it => "Invito cancellato",
                _ => "Invite deleted",
            };
        }
        public string GuildAuditLogMemberJoinedTitle()
        {
            return PreferredLanguage switch
            {
                Language.de => "Mitglied beigetreten",
                Language.at => "Mitglied beitretn",
                Language.fr => "Membre rejoint",
                Language.es => "Miembro se unió",
                Language.ru => "Участник присоединился",
                Language.it => "Membro iscritto",
                _ => "Member joined",
            };
        }
        public string GuildAuditLogMemberJoinedRegistered()
        {
            return PreferredLanguage switch
            {
                Language.de => "Registriert",
                Language.at => "Registriat",
                Language.fr => "Inscrit",
                Language.es => "Registrado",
                Language.ru => "Зарегистрировано",
                Language.it => "Registrato",
                _ => "Registered",
            };
        }
        public string GuildAuditLogMemberRemovedTitle()
        {
            return PreferredLanguage switch
            {
                Language.de => "Mitglied entfernt",
                Language.at => "Mitglied entfernt",
                Language.fr => "Membre supprimé",
                Language.es => "Miembro eliminado",
                Language.ru => "Участник удален",
                Language.it => "Membro rimosso",
                _ => "Member removed",
            };
        }
        public string GuildAuditLogThreadCreatedTitle()
        {
            return PreferredLanguage switch
            {
                Language.de => "Thread erstellt",
                Language.at => "Thread erstöt",
                Language.fr => "Fil créé",
                Language.es => "Hilo creado",
                Language.ru => "Тема создана",
                Language.it => "Discussione creata",
                _ => "Thread created",
            };
        }
        public string GuildAuditLogThreadCreatedParent()
        {
            return PreferredLanguage switch
            {
                Language.de => "Elternkanal",
                Language.at => "Eltankanal",
                Language.fr => "Parent",
                Language.es => "Padre",
                Language.ru => "Родитель",
                Language.it => "Genitore",
                _ => "Parent",
            };
        }
        public string GuildAuditLogThreadCreatedCreator()
        {
            return PreferredLanguage switch
            {
                Language.de => "Ersteller",
                Language.at => "Erstölla",
                Language.fr => "Créateur",
                Language.es => "Creador",
                Language.ru => "Создатель",
                Language.it => "Creatore",
                _ => "Creator",
            };
        }
        public string Enum(PunishmentType enumValue)
        {
            return enumValue switch
            {
                PunishmentType.Mute => PreferredLanguage switch
                {
                    Language.de => "Stummschaltung",
                    Language.at => "Stummschoitung",
                    Language.fr => "Muet",
                    Language.es => "Silencio",
                    Language.ru => "Немой",
                    Language.it => "Muto",
                    _ => "Mute",
                },
                PunishmentType.Ban => PreferredLanguage switch
                {
                    Language.de => "Bann",
                    Language.at => "Rauswuaf",
                    Language.fr => "Interdire",
                    Language.es => "Prohibición",
                    Language.ru => "Запретить",
                    Language.it => "Bandire",
                    _ => "Ban",
                },
                PunishmentType.Kick => PreferredLanguage switch
                {
                    Language.de => "Kick",
                    Language.at => "Tritt",
                    Language.fr => "Coup",
                    Language.es => "Patear",
                    Language.ru => "Пинать",
                    Language.it => "Calcio",
                    _ => "Kick",
                },
                PunishmentType.Warn => PreferredLanguage switch
                {
                    Language.de => "Verwarnung",
                    Language.at => "Verwoarnt",
                    Language.fr => "Avertir",
                    Language.es => "Advertir",
                    Language.ru => "Предупреждать",
                    Language.it => "Avvisare",
                    _ => "Warn",
                },
                _ => "Unknown",
            };
        }
        public string Enum(ViewPermission enumValue)
        {
            return enumValue switch
            {
                ViewPermission.Self => PreferredLanguage switch
                {
                    Language.de => "Privat",
                    Language.at => "Privot",
                    Language.fr => "Soi",
                    Language.es => "Uno mismo",
                    Language.ru => "Себя",
                    Language.it => "Se stesso",
                    _ => "Self",
                },
                ViewPermission.Guild => PreferredLanguage switch
                {
                    Language.de => "Gilde",
                    Language.at => "Güde",
                    Language.fr => "Guilde",
                    Language.es => "Gremio",
                    Language.ru => "Гильдия",
                    Language.it => "Gilda",
                    _ => "Guild",
                },
                ViewPermission.Global => PreferredLanguage switch
                {
                    Language.de => "Global",
                    Language.at => "Globoi",
                    Language.fr => "Global",
                    Language.es => "Global",
                    Language.ru => "Глобальный",
                    Language.it => "Globale",
                    _ => "Global",
                },
                _ => "Unknown",
            };
        }
        public string Enum(AutoModerationAction enumValue)
        {
            return enumValue switch
            {
                AutoModerationAction.None => PreferredLanguage switch
                {
                    Language.de => "Keine Aktion",
                    Language.at => "Nix tuan",
                    Language.fr => "Pas d'action",
                    Language.es => "Ninguna acción",
                    Language.ru => "Бездействие",
                    Language.it => "Nessuna azione",
                    _ => "No action",
                },
                AutoModerationAction.ContentDeleted => PreferredLanguage switch
                {
                    Language.de => "Nachricht gelöscht",
                    Language.at => "Nochricht glescht",
                    Language.fr => "Contenu supprimé",
                    Language.es => "Contenido eliminado",
                    Language.ru => "Контент удален",
                    Language.it => "Contenuto eliminato",
                    _ => "Content deleted",
                },
                AutoModerationAction.CaseCreated => PreferredLanguage switch
                {
                    Language.de => "Vorfall erstellt",
                    Language.at => "Vorfoi erstöt",
                    Language.fr => "Cas créé",
                    Language.es => "Caso creado",
                    Language.ru => "Дело создано",
                    Language.it => "Caso creato",
                    _ => "Case created",
                },
                AutoModerationAction.ContentDeletedAndCaseCreated => PreferredLanguage switch
                {
                    Language.de => "Nachricht gelöscht und Vorfall erstellt",
                    Language.at => "Nochricht glescht und Vorfoi erstöt",
                    Language.fr => "Contenu supprimé et dossier créé",
                    Language.es => "Contenido eliminado y caso creado",
                    Language.ru => "Контент удален, а дело создано",
                    Language.it => "Contenuto eliminato e caso creato",
                    _ => "Content deleted and case created",
                },
                _ => "Unknown",
            };
        }
        public string Enum(AutoModerationType enumValue)
        {
            return enumValue switch
            {
                AutoModerationType.InvitePosted => PreferredLanguage switch
                {
                    Language.de => "Einladung gesendet",
                    Language.at => "Eiladung gsendet",
                    Language.fr => "Invitation publiée",
                    Language.es => "Invitación publicada",
                    Language.ru => "Приглашение опубликовано",
                    Language.it => "Invito pubblicato",
                    _ => "Invite posted",
                },
                AutoModerationType.TooManyEmotes => PreferredLanguage switch
                {
                    Language.de => "Zu viele Emojis verwendet",
                    Language.at => "Zu vü Emojis san vawendt woan",
                    Language.fr => "Trop d'émoticônes utilisées",
                    Language.es => "Demasiados emotes usados",
                    Language.ru => "Использовано слишком много эмоций",
                    Language.it => "Troppe emoticon usate",
                    _ => "Too many emotes used",
                },
                AutoModerationType.TooManyMentions => PreferredLanguage switch
                {
                    Language.de => "Zu viele Benutzer erwähnt",
                    Language.at => "Zu vü Nutza san erwähnt woan",
                    Language.fr => "Trop d'utilisateurs mentionnés",
                    Language.es => "Demasiados usuarios mencionados",
                    Language.ru => "Упомянуто слишком много пользователей",
                    Language.it => "Troppi utenti citati",
                    _ => "Too many users mentioned",
                },
                AutoModerationType.TooManyAttachments => PreferredLanguage switch
                {
                    Language.de => "Zu viele Anhänge verwendet",
                    Language.at => "Zu vü Ohäng san verwendt woan",
                    Language.fr => "Trop de pièces jointes utilisées",
                    Language.es => "Se han utilizado demasiados archivos adjuntos",
                    Language.ru => "Использовано слишком много вложений",
                    Language.it => "Troppi allegati utilizzati",
                    _ => "Too many attachments used",
                },
                AutoModerationType.TooManyEmbeds => PreferredLanguage switch
                {
                    Language.de => "Zu viele Einbettungen verwendet",
                    Language.at => "Zu vü Eibettungen san vawendt woan",
                    Language.fr => "Trop d'intégrations utilisées",
                    Language.es => "Se han utilizado demasiados elementos incrustados",
                    Language.ru => "Использовано слишком много закладных",
                    Language.it => "Troppi incorporamenti utilizzati",
                    _ => "Too many embeds used",
                },
                AutoModerationType.TooManyAutoModerations => PreferredLanguage switch
                {
                    Language.de => "Zu viele automatische Moderationen",
                    Language.at => "Zu vü automatische Modarationen",
                    Language.fr => "Trop de modérations automatiques",
                    Language.es => "Demasiadas moderaciones automáticas",
                    Language.ru => "Слишком много автоматических модераций",
                    Language.it => "Troppe moderazioni automatiche",
                    _ => "Too many auto-moderations",
                },
                AutoModerationType.CustomWordFilter => PreferredLanguage switch
                {
                    Language.de => "Benutzerdefinierter Wortfilter ausgelöst",
                    Language.at => "Eigena Wortfüta is ausglest woan",
                    Language.fr => "Filtre de mots personnalisé déclenché",
                    Language.es => "Filtro de palabras personalizado activado",
                    Language.ru => "Пользовательский фильтр слов активирован",
                    Language.it => "Filtro parole personalizzato attivato",
                    _ => "Custom wordfilter triggered",
                },
                AutoModerationType.TooManyMessages => PreferredLanguage switch
                {
                    Language.de => "Zu viele Nachrichten",
                    Language.at => "Zu vü Nochrichtn",
                    Language.fr => "Trop de messages",
                    Language.es => "Demasiados mensajes",
                    Language.ru => "Слишком много сообщений",
                    Language.it => "Troppi messaggi",
                    _ => "Too many messages",
                },
                AutoModerationType.TooManyDuplicatedCharacters => PreferredLanguage switch
                {
                    Language.de => "Zu viele wiederholende Buchstaben verwendet",
                    Language.at => "Zu vü wiedaholende Buchstobn vawendet",
                    Language.fr => "Trop de caractères dupliqués utilisés",
                    Language.es => "Se han utilizado demasiados caracteres duplicados",
                    Language.ru => "Использовано слишком много повторяющихся символов",
                    Language.it => "Troppi caratteri duplicati utilizzati",
                    _ => "Too many duplicated characters used",
                },
                AutoModerationType.TooManyLinks => PreferredLanguage switch
                {
                    Language.de => "Zu viele Links verwendet",
                    Language.at => "Zu vü Links vawendet",
                    Language.fr => "Trop de liens utilisés",
                    Language.es => "Se han utilizado demasiados enlaces",
                    Language.ru => "Использовано слишком много ссылок",
                    Language.it => "Troppi link utilizzati",
                    _ => "Too many links used",
                },
                _ => "Unknown",
            };
        }
        public string Enum(APIError enumValue)
        {
            return enumValue switch
            {
                APIError.Unknown => PreferredLanguage switch
                {
                    Language.de => "Unbekannter Fehler",
                    Language.at => "Unbekonnta Föhla",
                    Language.fr => "Erreur inconnue",
                    Language.es => "Error desconocido",
                    Language.ru => "Неизвестная ошибка",
                    Language.it => "Errore sconosciuto",
                    _ => "Unknown error",
                },
                APIError.InvalidDiscordUser => PreferredLanguage switch
                {
                    Language.de => "Ungültiger Discordbenutzer",
                    Language.at => "Ungütiga Discordbenutza",
                    Language.fr => "Utilisateur discord invalide",
                    Language.es => "Usuario de discordia no válido",
                    Language.ru => "Недействительный пользователь Discord",
                    Language.it => "Utente discord non valido",
                    _ => "Invalid discord user",
                },
                APIError.ProtectedModCaseSuspect => PreferredLanguage switch
                {
                    Language.de => "Benutzer ist geschützt",
                    Language.at => "Benutza is gschützt",
                    Language.fr => "L'utilisateur est protégé",
                    Language.es => "El usuario está protegido",
                    Language.ru => "Пользователь защищен",
                    Language.it => "L'utente è protetto",
                    _ => "User is protected",
                },
                APIError.ProtectedModCaseSuspectIsBot => PreferredLanguage switch
                {
                    Language.de => "Benutzer ist geschützt. Er ist ein Bot.",
                    Language.at => "Benutza is gschützt, es is a Bot.",
                    Language.fr => "L'utilisateur est protégé. C'est un robot.",
                    Language.es => "El usuario está protegido. El es un bot.",
                    Language.ru => "Пользователь защищен. Он бот.",
                    Language.it => "L'utente è protetto. Lui è un bot.",
                    _ => "User is protected. He is a bot.",
                },
                APIError.ProtectedModCaseSuspectIsSiteAdmin => PreferredLanguage switch
                {
                    Language.de => "Benutzer ist geschützt. Er ist ein Seitenadministrator.",
                    Language.at => "Benutza is gschützt, er is a Seitenadministraotr.",
                    Language.fr => "L'utilisateur est protégé. Il est administrateur du site.",
                    Language.es => "El usuario está protegido. Es administrador de un sitio.",
                    Language.ru => "Пользователь защищен. Он администратор сайта.",
                    Language.it => "L'utente è protetto. È un amministratore del sito.",
                    _ => "User is protected. He is a site admin.",
                },
                APIError.ProtectedModCaseSuspectIsTeam => PreferredLanguage switch
                {
                    Language.de => "Benutzer ist geschützt. Er ist ein Teammitglied.",
                    Language.at => "Benutza is gschützt, er is a Teammitglied.",
                    Language.fr => "L'utilisateur est protégé. Il est membre de l'équipe.",
                    Language.es => "El usuario está protegido. Es un miembro del equipo.",
                    Language.ru => "Пользователь защищен. Он член команды.",
                    Language.it => "L'utente è protetto. È un membro della squadra.",
                    _ => "User is protected. He is a team member.",
                },
                APIError.ResourceNotFound => PreferredLanguage switch
                {
                    Language.de => "Ressource nicht gefunden",
                    Language.at => "Ressource ned gfundn.",
                    Language.fr => "Ressource introuvable",
                    Language.es => "Recurso no encontrado",
                    Language.ru => "Ресурс не найден",
                    Language.it => "Risorsa non trovata",
                    _ => "Resource not found",
                },
                APIError.InvalidIdentity => PreferredLanguage switch
                {
                    Language.de => "Ungültige Identität",
                    Language.at => "Ungültige Identität",
                    Language.fr => "Identité invalide",
                    Language.es => "Identidad inválida",
                    Language.ru => "Неверная личность",
                    Language.it => "Identità non valida",
                    _ => "Invalid identity",
                },
                APIError.GuildUnregistered => PreferredLanguage switch
                {
                    Language.de => "Gilde ist nicht registriert",
                    Language.at => "Güde is ned registriat",
                    Language.fr => "La guilde n'est pas enregistrée",
                    Language.es => "El gremio no está registrado",
                    Language.ru => "Гильдия не зарегистрирована",
                    Language.it => "La gilda non è registrata",
                    _ => "Guild is not registered",
                },
                APIError.Unauthorized => PreferredLanguage switch
                {
                    Language.de => "Nicht berechtigt",
                    Language.at => "Ned berechtigt",
                    Language.fr => "Non autorisé",
                    Language.es => "No autorizado",
                    Language.ru => "Неавторизованный",
                    Language.it => "non autorizzato",
                    _ => "Unauthorized",
                },
                APIError.GuildUndefinedMutedRoles => PreferredLanguage switch
                {
                    Language.de => "Gilde hat keine Rollen für Stummschaltungen definiert.",
                    Language.at => "Güde hot kane Roin fia de Stummschoitung definiat.",
                    Language.fr => "La guilde n'a pas de rôle défini pour la punition muette.",
                    Language.es => "El gremio no tiene roles definidos para el castigo mudo.",
                    Language.ru => "У гильдии нет определенных ролей для немого наказания.",
                    Language.it => "La gilda non ha ruoli definiti per la punizione muta.",
                    _ => "Guild has no roles for mute punishment defined.",
                },
                APIError.ModCaseIsMarkedToBeDeleted => PreferredLanguage switch
                {
                    Language.de => "Modcase ist zum Löschen markiert",
                    Language.at => "Modcase is zum Löscha markiat",
                    Language.fr => "Modcase est marqué pour être supprimé",
                    Language.es => "Modcase está marcado para ser eliminado",
                    Language.ru => "Modcase отмечен для удаления",
                    Language.it => "Modcase è contrassegnato per essere eliminato",
                    _ => "Modcase is marked to be deleted",
                },
                APIError.ModCaseIsNotMarkedToBeDeleted => PreferredLanguage switch
                {
                    Language.de => "Modcase ist nicht zum Löschen markiert",
                    Language.at => "Modcase is ned zum Lösche markiat",
                    Language.fr => "Modcase n'est pas marqué pour être supprimé",
                    Language.es => "Modcase no está marcado para ser eliminado",
                    Language.ru => "Modcase не отмечен для удаления",
                    Language.it => "Modcase non è contrassegnato per essere eliminato",
                    _ => "Modcase is not marked to be deleted",
                },
                APIError.GuildAlreadyRegistered => PreferredLanguage switch
                {
                    Language.de => "Gilde ist bereits registriert",
                    Language.at => "Güde is bereits registriat",
                    Language.fr => "La guilde est déjà enregistrée",
                    Language.es => "El gremio ya está registrado",
                    Language.ru => "Гильдия уже зарегистрирована",
                    Language.it => "La gilda è già registrata",
                    _ => "Guild is already registered",
                },
                APIError.NotAllowedInDemoMode => PreferredLanguage switch
                {
                    Language.de => "Diese Aktion ist in der Demo-Version nicht erlaubt",
                    Language.at => "De Aktion is in da Demo-Version ned erlaubt",
                    Language.fr => "Cette action n'est pas autorisée en mode démo",
                    Language.es => "Esta acción no está permitida en el modo de demostración.",
                    Language.ru => "Это действие запрещено в демонстрационном режиме.",
                    Language.it => "Questa azione non è consentita in modalità demo",
                    _ => "This action is not allowed in demo mode",
                },
                APIError.RoleNotFound => PreferredLanguage switch
                {
                    Language.de => "Rolle nicht gefunden",
                    Language.at => "Rolle ned gfundn",
                    Language.fr => "Rôle introuvable",
                    Language.es => "Rol no encontrado",
                    Language.ru => "Роль не найдена",
                    Language.it => "Ruolo non trovato",
                    _ => "Role not found",
                },
                APIError.TokenCannotManageThisResource => PreferredLanguage switch
                {
                    Language.de => "Tokens können diese Ressource nicht verwalten",
                    Language.at => "Tokns kennan de Ressourcen ned vawoitn",
                    Language.fr => "Les jetons ne peuvent pas gérer cette ressource",
                    Language.es => "Los tokens no pueden administrar este recurso",
                    Language.ru => "Лексемы не могут управлять этим ресурсом",
                    Language.it => "I token non possono gestire questa risorsa",
                    _ => "Tokens cannot manage this resource",
                },
                APIError.TokenAlreadyRegistered => PreferredLanguage switch
                {
                    Language.de => "Token ist bereits registriert",
                    Language.at => "Tokn is bereits registriat",
                    Language.fr => "Le jeton est déjà enregistré",
                    Language.es => "El token ya está registrado",
                    Language.ru => "Токен уже зарегистрирован",
                    Language.it => "Il token è già registrato",
                    _ => "Token is already registered",
                },
                APIError.CannotBeSameUser => PreferredLanguage switch
                {
                    Language.de => "Beide Benutzer sind gleich.",
                    Language.at => "Beide Benutza san gleich.",
                    Language.fr => "Les deux utilisateurs sont les mêmes.",
                    Language.es => "Ambos usuarios son iguales.",
                    Language.ru => "Оба пользователя одинаковые.",
                    Language.it => "Entrambi gli utenti sono gli stessi.",
                    _ => "Both users are the same.",
                },
                APIError.ResourceAlreadyExists => PreferredLanguage switch
                {
                    Language.de => "Ressource existiert bereits",
                    Language.at => "De Ressource gibts bereits",
                    Language.fr => "La ressource existe déjà",
                    Language.es => "El recurso ya existe",
                    Language.ru => "Ресурс уже существует",
                    Language.it => "La risorsa esiste già",
                    _ => "Resource already exists",
                },
                APIError.ModCaseDoesNotAllowComments => PreferredLanguage switch
                {
                    Language.de => "Kommentare sind für diesen Vorfall gesperrt",
                    Language.at => "Kommentare san fia den Vorfoi gsperrt",
                    Language.fr => "Les commentaires sont verrouillés pour ce modcase",
                    Language.es => "Los comentarios están bloqueados para este modcase",
                    Language.ru => "Комментарии заблокированы для этого мода",
                    Language.it => "I commenti sono bloccati per questo modcase",
                    _ => "Comments are locked for this modcase",
                },
                APIError.LastCommentAlreadyFromSuspect => PreferredLanguage switch
                {
                    Language.de => "Der letzte Kommentar war schon von dem Beschuldigten.",
                    Language.at => "Da letzte Kommentar woa scho vom Beschuldigten.",
                    Language.fr => "Le dernier commentaire était déjà du suspect.",
                    Language.es => "El último comentario ya era del sospechoso.",
                    Language.ru => "Последний комментарий уже был от подозреваемого.",
                    Language.it => "L'ultimo commento era già del sospettato.",
                    _ => "The last comment was already from the suspect.",
                },
                APIError.InvalidAutomoderationAction => PreferredLanguage switch
                {
                    Language.de => "Ungültige automoderationsaktion",
                    Language.at => "Ned gütige automodarationsaktion",
                    Language.fr => "Action de modération automatique non valide",
                    Language.es => "Acción de automoderación no válida",
                    Language.ru => "Недопустимое действие автомодерации",
                    Language.it => "Azione di moderazione automatica non valida",
                    _ => "Invalid automoderation action",
                },
                APIError.InvalidAutomoderationType => PreferredLanguage switch
                {
                    Language.de => "Ungültiger automoderationstyp",
                    Language.at => "Ungütiga automodarationstyp",
                    Language.fr => "Type d'automodération non valide",
                    Language.es => "Tipo de automoderación no válido",
                    Language.ru => "Неверный тип автомодерации.",
                    Language.it => "Tipo di moderazione automatica non valido",
                    _ => "Invalid automoderation type",
                },
                APIError.TooManyTemplates => PreferredLanguage switch
                {
                    Language.de => "Benutzer hat die maximale Anzahl an Templates erreicht",
                    Language.at => "Benutza hod de maximale Onzoi vo de Templates erreicht",
                    Language.fr => "L'utilisateur a atteint la limite maximale de modèles",
                    Language.es => "El usuario ha alcanzado el límite máximo de plantillas",
                    Language.ru => "Пользователь достиг максимального предела шаблонов",
                    Language.it => "L'utente ha raggiunto il limite massimo di modelli",
                    _ => "User has reached the max limit of templates",
                },
                APIError.InvalidFilePath => PreferredLanguage switch
                {
                    Language.de => "Ungültiger Dateipfad",
                    Language.at => "Ungütiga Dateipfad",
                    Language.fr => "Chemin de fichier invalide",
                    Language.es => "Ruta de archivo no válida",
                    Language.ru => "Неверный путь к файлу",
                    Language.it => "Percorso file non valido",
                    _ => "Invalid file path",
                },
                APIError.NoGuildsRegistered => PreferredLanguage switch
                {
                    Language.de => "Es sind keine Gilden registriert",
                    Language.at => "Es san kane Güdn registriat",
                    Language.fr => "Il n'y a pas de guildes enregistrées",
                    Language.es => "No hay gremios registrados",
                    Language.ru => "Нет зарегистрированных гильдий",
                    Language.it => "Non ci sono gilde registrate",
                    _ => "There are no guilds registered",
                },
                APIError.OnlyUsableInAGuild => PreferredLanguage switch
                {
                    Language.de => "Diese Aktion ist nur in einer Gilde nutzbar",
                    Language.at => "De Aktion is nua in ana Güdn nutzboa",
                    Language.fr => "Cette action n'est utilisable que dans une guilde",
                    Language.es => "Esta acción solo se puede usar en un gremio.",
                    Language.ru => "Это действие доступно только в гильдии.",
                    Language.it => "Questa azione è utilizzabile solo in una gilda",
                    _ => "This action is only usable in a guild",
                },
                APIError.InvalidAuditLogEvent => PreferredLanguage switch
                {
                    Language.de => "Ungültiger Auditlogeventstyp",
                    Language.at => "Ungütiga oduitlogeventstyp",
                    Language.fr => "Type d'événement auditlog non valide",
                    Language.es => "Tipo de evento de auditoría no válido",
                    Language.ru => "Неверный тип auditlogevent",
                    Language.it => "Tipo di evento auditlog non valido",
                    _ => "Invalid auditlogevent type",
                },
                _ => "Unknown",
            };
        }
        public string Enum(CaseCreationType enumValue)
        {
            return enumValue switch
            {
                CaseCreationType.Default => PreferredLanguage switch
                {
                    Language.de => "Default",
                    Language.at => "Default",
                    Language.fr => "Défaut",
                    Language.es => "Defecto",
                    Language.ru => "Дефолт",
                    Language.it => "Predefinito",
                    _ => "Default",
                },
                CaseCreationType.AutoModeration => PreferredLanguage switch
                {
                    Language.de => "Automoderiert.",
                    Language.at => "Automodariat.",
                    Language.fr => "Le cas est automodéré.",
                    Language.es => "El caso está autoderado.",
                    Language.ru => "Корпус автоматический.",
                    Language.it => "Il caso è moderato automaticamente.",
                    _ => "Case is automoderated.",
                },
                CaseCreationType.Imported => PreferredLanguage switch
                {
                    Language.de => "Importiert.",
                    Language.at => "Importiat",
                    Language.fr => "Le cas est importé.",
                    Language.es => "El caso es importado.",
                    Language.ru => "Корпус импортный.",
                    Language.it => "Il caso è importato.",
                    _ => "Case is imported.",
                },
                CaseCreationType.ByCommand => PreferredLanguage switch
                {
                    Language.de => "Durch Befehl erstellt.",
                    Language.at => "Durch an Beföh erstöt.",
                    Language.fr => "Cas créé par commande.",
                    Language.es => "Caso creado por comando.",
                    Language.ru => "Дело создано командой.",
                    Language.it => "Caso creato da comando.",
                    _ => "Case created by command.",
                },
                _ => "Unknown",
            };
        }
        public string Enum(Language enumValue)
        {
            return enumValue switch
            {
                Language.en => PreferredLanguage switch
                {
                    Language.de => "Englisch",
                    Language.at => "Englisch",
                    Language.fr => "Anglais",
                    Language.es => "inglés",
                    Language.ru => "английский",
                    Language.it => "inglese",
                    _ => "English",
                },
                Language.de => PreferredLanguage switch
                {
                    Language.de => "Deutsch",
                    Language.at => "Piefchinesisch",
                    Language.fr => "Allemand",
                    Language.es => "alemán",
                    Language.ru => "Немецкий",
                    Language.it => "Tedesco",
                    _ => "German",
                },
                Language.fr => PreferredLanguage switch
                {
                    Language.de => "Französisch",
                    Language.at => "Franzesisch",
                    Language.fr => "français",
                    Language.es => "francés",
                    Language.ru => "французкий язык",
                    Language.it => "francese",
                    _ => "French",
                },
                Language.es => PreferredLanguage switch
                {
                    Language.de => "Spanisch",
                    Language.at => "Spanisch",
                    Language.fr => "Espagnol",
                    Language.es => "Español",
                    Language.ru => "испанский",
                    Language.it => "spagnolo",
                    _ => "Spanish",
                },
                Language.it => PreferredLanguage switch
                {
                    Language.de => "Italienisch",
                    Language.at => "Italienisch",
                    Language.fr => "italien",
                    Language.es => "italiano",
                    Language.ru => "Итальянский",
                    Language.it => "italiano",
                    _ => "Italian",
                },
                Language.at => PreferredLanguage switch
                {
                    Language.de => "Österreich",
                    Language.at => "Esterreichisch",
                    Language.fr => "autrichien",
                    Language.es => "austriaco",
                    Language.ru => "Австрийский",
                    Language.it => "austriaco",
                    _ => "Austrian",
                },
                Language.ru => PreferredLanguage switch
                {
                    Language.de => "Russisch",
                    Language.at => "Rusisch",
                    Language.fr => "Russe",
                    Language.es => "Ruso",
                    Language.ru => "Русский",
                    Language.it => "Russo",
                    _ => "Russian",
                },
                _ => "Unknown",
            };
        }
        public string Enum(AutoModerationChannelNotificationBehavior enumValue)
        {
            return enumValue switch
            {
                AutoModerationChannelNotificationBehavior.SendNotification => PreferredLanguage switch
                {
                    Language.de => "Kanalbenachrichtigung",
                    _ => "Channel notification",
                },
                AutoModerationChannelNotificationBehavior.SendNotificationAndDelete => PreferredLanguage switch
                {
                    Language.de => "Temporäre Kanalbenachrichtigung",
                    _ => "Temporary channel notification",
                },
                AutoModerationChannelNotificationBehavior.NoNotification => PreferredLanguage switch
                {
                    Language.de => "Keine Kanalbenachrichtigung",
                    _ => "No channel notification",
                },
                _ => "Unknown",
            };
        }
        public string Enum(EditStatus enumValue)
        {
            return enumValue switch
            {
                EditStatus.None => PreferredLanguage switch
                {
                    Language.de => "Unbestimmt",
                    _ => "None",
                },
                EditStatus.Unedited => PreferredLanguage switch
                {
                    Language.de => "Nicht bearbeitet",
                    _ => "Not edited",
                },
                EditStatus.Edited => PreferredLanguage switch
                {
                    Language.de => "Bearbeitet",
                    _ => "Edited",
                },
                _ => "Unknown",
            };
        }
        public string Enum(LockedCommentStatus enumValue)
        {
            return enumValue switch
            {
                LockedCommentStatus.None => PreferredLanguage switch
                {
                    Language.de => "Unbestimmt",
                    _ => "None",
                },
                LockedCommentStatus.Locked => PreferredLanguage switch
                {
                    Language.de => "Gesperrt",
                    _ => "Locked",
                },
                LockedCommentStatus.Unlocked => PreferredLanguage switch
                {
                    Language.de => "Entsperrt",
                    _ => "Unlocked",
                },
                _ => "Unknown",
            };
        }
        public string Enum(MarkedToDeleteStatus enumValue)
        {
            return enumValue switch
            {
                MarkedToDeleteStatus.None => PreferredLanguage switch
                {
                    Language.de => "Unbestimmt",
                    _ => "None",
                },
                MarkedToDeleteStatus.Marked => PreferredLanguage switch
                {
                    Language.de => "Zu löschen markiert",
                    _ => "Marked to delete",
                },
                MarkedToDeleteStatus.Unmarked => PreferredLanguage switch
                {
                    Language.de => "Nicht zu löschen markiert",
                    _ => "Not marked to delete",
                },
                _ => "Unknown",
            };
        }
        public string Enum(PunishmentActiveStatus enumValue)
        {
            return enumValue switch
            {
                PunishmentActiveStatus.None => PreferredLanguage switch
                {
                    Language.de => "Unbestimmt",
                    _ => "None",
                },
                PunishmentActiveStatus.Active => PreferredLanguage switch
                {
                    Language.de => "Aktiv",
                    _ => "Active",
                },
                PunishmentActiveStatus.Inactive => PreferredLanguage switch
                {
                    Language.de => "Inaktiv",
                    _ => "Inactive",
                },
                _ => "Unknown",
            };
        }

    }
}

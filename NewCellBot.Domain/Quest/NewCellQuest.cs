using System.Linq;
using Bot.Quests;
using NewCellBot.Domain.Quest;
using NewCellBot.Domain.Quest.Model;
using NewCellBot.Domain.Quest.Stories;

namespace Bot
{
    public static class NewCellQuest
    {
        public static string Map = ToshikStory.Map + NastyaStory.Map;
        public static Inventory GetStartingInventory() => new Inventory();
        public static Journal GetStartingJournal() => new Journal().Open(Quest.EnterHall);

        public static DialogQuestion[] GetDialogs()
        {
            var toshikDialogs = ToshikStory.GetDialogs();
            foreach (var dialogQuestion in toshikDialogs) {
                dialogQuestion.ForPlayer = "@Insomnov;@MistifliQ;@starteleport;@svsokrat;296536101;cloudpaper_girl";
                dialogQuestion.PlayerIcon = MapIcon.Toshik;
            }

            var nastyaDialogs = NastyaStory.GetDialogs();
            foreach (var dialogQuestion in nastyaDialogs) {
                dialogQuestion.ForPlayer = "@Naimushina;255239749;@MistifliQ;@starteleport;@svsokrat;296536101;cloudpaper_girl";
                dialogQuestion.PlayerIcon = MapIcon.Nastya;
            }

            return toshikDialogs.Concat(nastyaDialogs).ToArray();
        }
    }
}

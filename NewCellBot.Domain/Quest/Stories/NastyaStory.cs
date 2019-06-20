using System.Linq;
using NewCellBot.Domain.Quest.Model;

namespace Bot.Quests
{
    public static class NastyaStory
    {
        public static string Map =
            @"
%%%%%%%%%%%%%
%%%%%%%%%%%%%
%%%%----%%%%%
%%%%-N-R%%%%%
%%%%----%%%%%
%%%%%%%%%%%%%
%%%%%%%%%%%%%
";

        public static DialogQuestion[] GetDialogs()
        {
            var mapDialog = new DialogQuestion {
                Name = Dialog.MapNastya,
                Message = "_Перемещайтесь по карте, или выберите действие_",
                Answers = new[] {
                    new DialogAnswer {
                        Message = MapButtons.Inventory.ToString(),
                        MoveToDialog = Dialog.InventoryNastya,
                    },
                    new DialogAnswer {
                        Message = MapButtons.Up.ToString(),
                        MoveToDialog = Dialog.MapNastya,
                        MoveToPos = (pos, map) => map.PosUp(pos)
                    },
                    new DialogAnswer {
                        Message = MapButtons.Journal.ToString(),
                        MoveToDialog = Dialog.JournalNastya,
                    },
                    new DialogAnswer {
                        Message = MapButtons.Left.ToString(),
                        MoveToDialog = Dialog.MapNastya,
                        MoveToPos = (pos, map) => pos - 1
                    },
                    new DialogAnswer {
                        Message = MapButtons.Down.ToString(),
                        MoveToDialog = Dialog.MapNastya,
                        MoveToPos = (pos, map) => map.PosDown(pos)
                    },
                    new DialogAnswer {
                        Message = MapButtons.Right.ToString(),
                        MoveToDialog = Dialog.MapNastya,
                        MoveToPos = (pos, map) => pos + 1
                    },
                },
                DisplayMap = true
            };

            var inventoryDialog = new DialogQuestion {
                Name = Dialog.InventoryNastya,
                DynamicMessage = (i, j) => {
                    return "*Инвентарь*:\n";
                    ;
                },
                Answers = new[] {
                    new DialogAnswer {
                        Message = "<<<",
                        MoveToDialog = Dialog.MapNastya
                    },
                }
            };

            var journalDialog = new DialogQuestion {
                Name = Dialog.JournalNastya,
                DynamicMessage = (i, j) => {
                    var done = "\u2714\ufe0f";
                    var pending = "\u2716\ufe0f";

                    string RenderQuest(string quest, string message)
                    {
                        return (j.IsKnown(quest) ? $"{(j.IsFinished(quest) ? done : pending)} {message}\n" : "");
                    }

                    return "*Журнал*:\n"
                        ;
                },
                Answers = new[] {
                    new DialogAnswer {
                        Message = "<<<",
                        MoveToDialog = Dialog.MapNastya
                    },
                }
            };

            var randomDialogs = new[] {
                new DialogQuestion {
                    Name = Dialog.FoundWallNastya,
                    Message = "Уперлась в стену",
                    Answers = mapDialog.Answers,
                    DisplayMap = true,
                    PreventMove = true,
                    MapIcon = MapIcon.WallNastya
                },
            };

            var repaDialogs = new[] {
                new DialogQuestion {
                    Name = Dialog.Repa1,
                    Message = "Ик пук",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Кряк?",
                            MoveToDialog = Dialog.Repa2,
                            MoveToPos = (pos, map) => map.PosLeft(MapIcon.Repa)
                        },
                        new DialogAnswer {
                            Message = "Кряк!",
                            MoveToDialog = Dialog.MapNastya,
                            MoveToPos = (pos, map) => map.PosLeft(MapIcon.Repa)
                        },
                    },
                    MapIcon = MapIcon.Repa,
                    DisplayMap = true
                },
                new DialogQuestion {
                    Name = Dialog.Repa2,
                    Message = "ПУУУУУК??",
                    Answers = mapDialog.Answers,
                    DisplayMap = true
                },
            };

            return new[] {
                    mapDialog,
                    inventoryDialog,
                    journalDialog
                }.Concat(repaDialogs)
                .Concat(randomDialogs)
                .ToArray();
        }
    }
}
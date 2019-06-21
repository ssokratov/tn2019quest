using System.Linq;
using Bot;
using NewCellBot.Domain.Quest.Model;

namespace NewCellBot.Domain.Quest.Stories
{
    public static class FinaleStory
    {
        public static string Map =
            @"
%%%%%%%%%%%%%
%%%!%%%%%%%%%
%%%!%%%%%%%%%
%%%!%%%%%%%%%
%%!Е!%%%%%%%%
%%!-%%%%%%%%%
%%--з--%%%%%%
%%ж-O-к%%%%%%
%%----с%%%%%%
%%К--!!%--%%%
%%!!!!!!--%%%
%%%!%%%%%%%%%
";

        public static DialogQuestion[] GetDialogs()
        {
            var startDialog = new DialogQuestion {
                Name = Dialog.FinaleStart,
                Message = "_Есин_: Хмм... Кажется, мы приземлились как раз в нужном месте. Ну, так и планировалось в общем-то.",
                Answers = new [] {
                    new DialogAnswer {
                        Message = "Бежать к Тошику",
                        MoveToDialog = Dialog.MapNastya,
                        MoveToPos = (pos, map) => map.PosLeft(MapIcon.Toshik),
                        ChangeJournal = j => j.Open(Bot.Quest.StartWedding)
                    }
                },
                DisplayMap = true,
                MapIcon = MapIcon.EsinFinale
            };

            var randomDialogs = new[] {
                new DialogQuestion {
                    Name = Dialog.Finale1,
                    Message = "_Тошик_: ... (бессвязный лепет про чувства, эмоции, морепродукты)",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Поцеловать",
                            MoveToDialog = Dialog.FinaleKiss
                        }
                    },
                    DisplayMap = true,
                    MapIcon = MapIcon.Toshik
                },
                new DialogQuestion {
                    Name = Dialog.FinaleKiss,
                    Message = "Гости охнули в умилении",
                    Photo = "Resourses/Finale2.jpg",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Вернуться к гостям",
                            MoveToDialog = Dialog.MapNastya,
                            MoveToPos = (pos, map) => map.PosDown(MapIcon.Toshik),
                        }
                    }
                },
                new DialogQuestion {
                    Name = Dialog.Finale2,
                    Message = "_Капитан_: Что происходит? Что-то я не разобрался...",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Это вторчик стайл!",
                            MoveToDialog = Dialog.MapNastya,
                            MoveToPos = (pos, map) => map.PosRight(MapIcon.SedoshFinale),
                        }
                    },
                    DisplayMap = true,
                    MapIcon = MapIcon.SedoshFinale
                },
                new DialogQuestion {
                    Name = Dialog.Finale3,
                    Message = "_Яков_: Поздравлем вас от чистого сердца! Мы хотели отсечься уже, но все выходы в огне.",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Посидите ещё",
                            MoveToDialog = Dialog.MapNastya,
                            MoveToPos = (pos, map) => map.PosRight(MapIcon.JacobFinale),
                        }
                    },
                    DisplayMap = true,
                    MapIcon = MapIcon.JacobFinale
                },
                new DialogQuestion {
                    Name = Dialog.Finale4,
                    Message = "Сократ (задротит)",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Не отвлекать",
                            MoveToDialog = Dialog.MapNastya,
                            MoveToPos = (pos, map) => map.PosLeft(MapIcon.SokratFinale),
                        }
                    },
                    DisplayMap = true,
                    MapIcon = MapIcon.SokratFinale
                },
                new DialogQuestion {
                    Name = Dialog.Finale5,
                    Message = "_Колян_: В штанах у вас свадьба.",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "И то верно",
                            MoveToDialog = Dialog.MapNastya,
                            MoveToPos = (pos, map) => map.PosLeft(MapIcon.KolyanFinale),
                        }
                    },
                    DisplayMap = true,
                    MapIcon = MapIcon.KolyanFinale
                },
                new DialogQuestion {
                    Name = Dialog.Flame3,
                    Message = "Здание в огне и вот-вот рухнет",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Не страшно",
                            MoveToDialog = Dialog.MapNastya,
                        }
                    },
                    DisplayMap = true,
                    PreventMove = true,
                    MapIcon = MapIcon.FlameFinale
                }
            };

            var zagsDialogs = new[] {
                new DialogQuestion {
                    Name = Dialog.Finale6,
                    Photo = "Resourses/Tamada.jpg",
                    Message = "_Работница ЗАГСа:_ Ну вот и невеста подоспела!",
                    Answers = new[] {
                        new DialogAnswer {
                            Available = (i, j) => !j.IsFinished(Bot.Quest.StartWedding),
                            Message = "Начать церемонию!",
                            MoveToDialog = Dialog.Finale7
                        },
                        new DialogAnswer {
                            Available = (i, j) => j.IsFinished(Bot.Quest.StartWedding),
                            Message = "Напомнить код",
                            MoveToDialog = Dialog.Finale11
                        },
                        new DialogAnswer {
                            Message = "Уйти",
                            MoveToDialog = Dialog.MapNastya
                        }
                    },
                    PreventMove = true,
                    MapIcon = MapIcon.ZagsWorkerfinale
                },
                new DialogQuestion {
                    Name = Dialog.Finale7,
                    Message = "_Работница ЗАГСа:_ Есть два стула. Занимайте их, пожалуйста. Подпишите вот тут, " +
                              "вот тут и вот там.",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Сесть, подписать",
                            MoveToDialog = Dialog.Finale8
                        }
                    }
                },
                new DialogQuestion {
                    Name = Dialog.Finale8,
                    Message = "_Работница ЗАГСа:_ Подпишите также соглашение о неразглашении информации и об отказе " +
                              "от медицинской помощи.",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Подписать всё",
                            MoveToDialog = Dialog.Finale9
                        }
                    }
                },
                new DialogQuestion {
                    Name = Dialog.Finale9,
                    Message = "_Работница ЗАГСа:_ Давайте свидетельство о рождении, СНИЛС, медицинский полис, выписку " +
                              "с места работы, пошлину, ксерокопии паспортов.",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Отдать всё",
                            MoveToDialog = Dialog.Finale10
                        }
                    }
                },
                new DialogQuestion {
                    Name = Dialog.Finale10,
                    Message = "_Работница ЗАГСа:_ Какие-то у вас ксерокопии бледные... Ладно, сойдет. В следующий раз " +
                              "не приму! Ну, вот и всё, объявляю вас *мужем и женой*! Поздравляю!",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "УРА!!!",
                            MoveToDialog = Dialog.Finale11,
                            ChangeJournal = j => j.Finish(Bot.Quest.StartWedding)
                        }
                    }
                },
                new DialogQuestion {
                    Name = Dialog.Finale11,
                    Photo = "Resources/Finale.jpg",
                    Message = "",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Конец истории",
                            MoveToDialog = Dialog.Finale12
                        }
                    }
                },
                new DialogQuestion {
                    Name = Dialog.Finale12,
                    Message = "_Этот квест, безусловно, уже является самодостаточным подарком. Но у нас для вас есть ещё " +
                              "кое-что. Свяжитесь с Коляном и назовите ему код:_ *НОВАЯ ЯЧЕЙКА СОЗДАНА*_, для получения " +
                              "оставшейся части подарка_",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Ясно",
                            MoveToDialog = Dialog.MapNastya
                        }
                    }
                },
            };

            return new[] {
                    startDialog,
                }.Concat(randomDialogs)
                .Concat(zagsDialogs)
                .ToArray();
        }
    }
}

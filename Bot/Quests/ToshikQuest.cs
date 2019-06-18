using System.Linq;

namespace Bot
{
    public static class ToshikQuest
    {
        public static string Map =
            @"
#############
#############
##--Z--######
##J---R######
##----S######
##K----#f-b##
##-v---п---##
###П#########
##-O#########
##8-#########
##~~#########
##--#########
";

        public static Inventory GetStartingInventory () => new Inventory();
        public static Journal GetStartingJournal () => new Journal().Open(Quest.EnterHall);

        public static DialogQuestion[] GetDialogs()
        {
            var mapDialog = new DialogQuestion {
                Name = Dialog.Map,
                Message = "_Перемещайтесь по карте, или выберите действие_",
                Answers = new[] {
                    new DialogAnswer {
                        Message = MapButtons.Inventory.ToString(),
                        MoveToDialog = Dialog.Inventory,
                    },
                    new DialogAnswer {
                        Message = MapButtons.Up.ToString(),
                        MoveToDialog = Dialog.Map,
                        MoveToPos = (pos, map) => map.PosUp(pos)
                    },
                    new DialogAnswer {
                        Message = MapButtons.Journal.ToString(),
                        MoveToDialog = Dialog.Journal,
                    },
                    new DialogAnswer {
                        Message = MapButtons.Left.ToString(),
                        MoveToDialog = Dialog.Map,
                        MoveToPos = (pos, map) => pos - 1
                    },
                    new DialogAnswer {
                        Message = MapButtons.Down.ToString(),
                        MoveToDialog = Dialog.Map,
                        MoveToPos = (pos, map) => map.PosDown(pos)
                    },
                    new DialogAnswer {
                        Message = MapButtons.Right.ToString(),
                        MoveToDialog = Dialog.Map,
                        MoveToPos = (pos, map) => pos + 1
                    },
                    new DialogAnswer {
                        Available = (i, j) => j.IsFinished(Quest.EnterHall),
                        Message = $"{MapIcon.Self.ToSmile()} идти к...",
                        MoveToDialog = Dialog.MapMoveTo
                    },
                    new DialogAnswer {
                        Available = (i, j) => i.Has(Item.PhoneNumber),
                        Message = "\ud83d\udcde набрать на цифры",
                        MoveToDialog = Dialog.FoundFear,
                    },
                    new DialogAnswer {
                        Available = (i, j) => j.IsOpen(Quest.Demianov),
                        Message = "\ud83d\udcde позвонить Демьянову",
                        MoveToDialog = Dialog.Demianov1,
                    }
                },
                DisplayMap = true
            };

            var moveToDialog = new DialogQuestion {
                Name = Dialog.MapMoveTo,
                Message = "_Хорошо, куда пойдём?_",
                Answers = new[] {
                    new DialogAnswer {
                        Message = "К работнице ЗАГСа",
                        MoveToDialog = Dialog.ZagsWorker1,
                        MoveToPos = (pos, map) => map.PosDown(MapIcon.ZagsWorker)
                    },
                    new DialogAnswer {
                        Message = "К Якову",
                        MoveToDialog = Dialog.Jacob1,
                        MoveToPos = (pos, map) => map.PosRight(MapIcon.Jacob)
                    },
                    new DialogAnswer {
                        Message = "К Репе",
                        MoveToDialog = Dialog.Repa1,
                        MoveToPos = (pos, map) => map.PosLeft(MapIcon.Repa)
                    },
                    new DialogAnswer {
                        Message = "К Капитану",
                        MoveToDialog = Dialog.Sedosh1,
                        MoveToPos = (pos, map) => map.PosRight(MapIcon.Sedosh)
                    },
                    new DialogAnswer {
                        Message = "К Сократу",
                        Available = (i, j) => !j.IsOpen(Quest.FireAlarm),
                        MoveToDialog = Dialog.Sokrat1,
                        MoveToPos = (pos, map) => map.PosLeft(MapIcon.Sokrat)
                    },
                    new DialogAnswer {
                        Message = "В самое начало",
                        MoveToPos = (pos, map) => Map.IndexOf(MapIcon.Self)
                    },
                    new DialogAnswer {
                        Message = "<<<",
                        MoveToDialog = Dialog.Map
                    },
                },
                DisplayMap = true
            };

            var inventoryDialog = new DialogQuestion {
                Name = Dialog.Inventory,
                DynamicMessage = (i, j) => {
                    return "*Инвентарь*:\n"
                           + (i.Has(Item.Phone) ? "\ud83d\udcf1 телефон\n" : "")
                           + (i.Has(Item.Project) ? "\ud83d\udcc3 проект\n" : "")
                           + (i.Has(Item.Stick) ? "\u26a1\ufe0f жезл\n" : "")
                           + (i.Has(Item.FireExtinguisher) ? $"{MapIcon.FireExtinguisher.ToSmile()} огнетушитель\n" : "")
                           + (i.Has(Item.Glasses) ? $"{MapIcon.Glasses.ToSmile()} монокль\n" : "")
                           + (i.Has(Item.Boots) ? $"{MapIcon.Boots.ToSmile()} сапоги\n" : "")
                           + (i.Has(Item.Hat) ? $"\ud83c\udfa9 шляпа\n" : "")
                        ;
                },
                Answers = new[] {
                    new DialogAnswer {
                        Message = "<<<",
                        MoveToDialog = Dialog.Map
                    },
                }
            };

            var journalDialog = new DialogQuestion {
                Name = Dialog.Journal,
                DynamicMessage = (i, j) => {
                    var done = "\u2714\ufe0f";
                    var pending = "\u2716\ufe0f";
                    string RenderQuest(string quest, string message) {
                        return (j.IsKnown(quest) ? $"{(j.IsFinished(quest) ? done : pending)} {message}\n" : "");
                    }
                    return "*Журнал*:\n"
                           + RenderQuest(Quest.EnterHall, "Войти в зал")
                           + RenderQuest(Quest.AskForWedding, "Поговорить с работником ЗАГСа")
                           + RenderQuest(Quest.Jacob, "Замутить Якову телефон")
                           + RenderQuest(Quest.Sedosh, "Обсчитать проект капитанской хижины")
                           + RenderQuest(Quest.FireAlarm, "Потушить пожар")
                           + RenderQuest(Quest.Sokrat, "Починить Сократу роутер")
                           + RenderQuest(Quest.DressForWedding, "Приодеться к свадьбе")
                        ;
                },
                Answers = new[] {
                    new DialogAnswer {
                        Message = "<<<",
                        MoveToDialog = Dialog.Map
                    },
                }
            };

            var monocleDialogs = new[] {
                new DialogQuestion {
                    Name = Dialog.Glasses1,
                    Message = "Под ногами лежит *монокль* в золотой оправе. Чуть не наступил.",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Поднять",
                            MoveToDialog = Dialog.Glasses2,
                            ChangeInventory = i => i.Give(Item.Glasses),
                            ChangeMap = (pos, map) => map.ClearPos(pos)
                        },
                        new DialogAnswer {
                            Message = "Оставить",
                            MoveToDialog = Dialog.Map
                        },
                    },
                    MapIcon = MapIcon.Glasses,
                    DisplayMap = true
                },
                new DialogQuestion {
                    Name = Dialog.Glasses2,
                    Message = "Дорогая вещица. И удобная! Теперь я вижу немного дальше!",
                    Answers = mapDialog.Answers,
                    DisplayMap = true
                },
            };

            var bootsDialogs = new[] {
                new DialogQuestion {
                    Name = Dialog.Boots1,
                    Message = "Перед вами старенькие резиновые сапоги.",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Надеть",
                            MoveToDialog = Dialog.Boots2,
                            ChangeInventory = i => i.Give(Item.Boots),
                            ChangeMap = (pos, map) => map.ClearPos(pos)
                        },
                        new DialogAnswer {
                            Message = "Оставить",
                            MoveToDialog = Dialog.Map
                        },
                    },
                    MapIcon = MapIcon.Boots,
                    DisplayMap = true
                },
                new DialogQuestion {
                    Name = Dialog.Boots2,
                    Message = "Ну... Не очень удобно, но зато скрипят при ходьбе!",
                    Answers = mapDialog.Answers,
                    DisplayMap = true
                },
            };

            var startDoorDialogs = new[] {
                new DialogQuestion {
                    Name = Dialog.StartDoors1,
                    Message = "Перед вами высокая двойная дверь из белого дерева с резными узорами.",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Потянуть за ручку",
                            MoveToDialog = Dialog.Map,
                            ChangeMap = (pos, map) => map.ClearPos(pos)
                        },
                        new DialogAnswer {
                            Message = "Прочитать табличку",
                            MoveToDialog = Dialog.StartDoors2
                        },
                        new DialogAnswer {
                            Message = "Уйти",
                            MoveToDialog = Dialog.Map,
                            MoveToPos = (pos, map) => map.PosDown(pos)
                        },
                    },
                    MapIcon = MapIcon.StartDoors
                },
                new DialogQuestion {
                    Name = Dialog.StartDoors2,
                    Message = "На красной ламинированной табличке переливаются 4 золотые буквы: *ЗАГС*.",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Решительно потянуть за ручку",
                            MoveToDialog = Dialog.Map,
                            ChangeMap = (pos, map) => map.ClearPos(pos)
                        },
                        new DialogAnswer {
                            Message = "Осмотреться",
                            MoveToDialog = Dialog.StartDoors3
                        },
                        new DialogAnswer {
                            Message = "Уйти",
                            MoveToDialog = Dialog.Map,
                            MoveToPos = (pos, map) => map.PosDown(pos)
                        },
                    },
                },
                new DialogQuestion {
                    Name = Dialog.StartDoors3,
                    Message = "Дверь расположена в конце коридора." +
                              " Вглядываясь вдаль вы не видите его начала. Вы ощущаете прилив одиночества и тоски.",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Потянуть за ручку двери",
                            MoveToDialog = Dialog.Map,
                            ChangeMap = (pos, map) => map.ClearPos(pos)
                        },
                        new DialogAnswer {
                            Message = "Замереть в нерешительности",
                            MoveToDialog = Dialog.StartDoors4
                        },
                        new DialogAnswer {
                            Message = "Уйти",
                            MoveToDialog = Dialog.Map,
                            MoveToPos = (pos, map) => map.PosDown(pos)
                        },
                    },
                },
                new DialogQuestion {
                    Name = Dialog.StartDoors4,
                    Message = "Ничего не происходит. Хватит мять сиськи.",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Потянуть за ручку двери",
                            MoveToDialog = Dialog.Map,
                            ChangeMap = (pos, map) => map.ClearPos(pos)
                        },
                        new DialogAnswer {
                            Message = "Уйти",
                            MoveToDialog = Dialog.Map,
                            MoveToPos = (pos, map) => map.PosDown(pos)
                        },
                    },
                },
            };

            var veilDialogs = new[] {
                new DialogQuestion {
                    Name = Dialog.Veil1,
                    Message = "Вы входите в большой светлый зал. Вдруг вам под ноги бросают фату.",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Поднять",
                            MoveToDialog = Dialog.Veil2,
                            ChangeMap = (pos, map) => map.ClearPos(pos),
                            ChangeJournal = j => j.Finish(Quest.EnterHall).Open(Quest.AskForWedding)
                        },
                        new DialogAnswer {
                            Message = "Перешагнуть",
                            MoveToDialog = Dialog.EnteredHall,
                            ChangeMap = (pos, map) => map.ClearPos(pos),
                            ChangeJournal = j => j.Finish(Quest.EnterHall).Open(Quest.AskForWedding)
                        }
                    },
                    MapIcon = MapIcon.Veil,
                    DisplayMap = true,
                },
                new DialogQuestion {
                    Name = Dialog.Veil2,
                    Message =
                        "Вы обращаете внимание на то, что деревянный пол весь в отметинах от каблуков. Чуть в стороне кто-то обронил фотокарточку.",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Посмотреть",
                            MoveToDialog = Dialog.Veil3
                        },
                    },
                },
                new DialogQuestion {
                    Name = Dialog.Veil3,
                    Photo = "Resources/Kabluk.jpg;AgADAgADkaoxG1sH0Uu7QaMcUhByutN4Xw8ABDuyScUAAeRl2Zu7BQABAg",
                    Message = "Вы поднимаете фотографию.",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "ФФФФУУУУУУ",
                            MoveToDialog = Dialog.EnteredHall,
                        },
                    },
                },
                new DialogQuestion {
                    Name = Dialog.EnteredHall,
                    Message = "В зале собрались гости, все очень рады вас видеть. Пожалуй, стоит поговорить с работницей ЗАГСа насчет начала церемонии.",
                    Answers = mapDialog.Answers,
                    DisplayMap = true
                },
            };

            var demianovDiagogs = new[] {
                new DialogQuestion {
                    Name = Dialog.Demianov1,
                    Message = "Привет, Антош. Да, у нас есть точки по салу. Я тебе отправлю одну курьером. " +
                              "Помоги только разобраться с недосдачей по FOBO. Нам завезли новых пыликов. Сейчас на витрине вижу 7. " +
                              "По NTS WINCASH продали 9, а в FOBO только 1. *Недосдача сколько?*" +
                              "\n_(ответьте сообщением)_",
                    Answers = new[] {
                        new DialogAnswer {
                            IsHidden = true,
                            Message = "В жопе столько",
                            MoveToDialog = Dialog.Demianov2
                        },
                        new DialogAnswer {
                            IsHidden = true,
                            Message = "На складе столько",
                            MoveToDialog = Dialog.Demianov2
                        },
                        new DialogAnswer {
                            Message = "Сбросить вызов",
                            MoveToDialog = Dialog.Map
                        },
                        new DialogAnswer {
                            Message = "Нужна подсказка",
                            MoveToDialog = Dialog.Demianov3
                        },
                    }
                },
                new DialogQuestion {
                    Name = Dialog.Demianov2,
                    Message = "Точно, так оно и есть! Отправил к тебе курьера с *телефоном*. Поздравляю!",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Поблагодарить",
                            ChangeInventory = i => i.Give(Item.Phone),
                            ChangeJournal = j => j.Finish(Quest.Demianov),
                            MoveToDialog = Dialog.Map
                        },
                    }
                },
                new DialogQuestion {
                    Name = Dialog.Demianov3,
                    Message = "Повторю вопрос. Недосдача сколько?",
                    Photo = "Resources/Skolko.jpg;AgADAgADO60xG1B3CUivpD44cbs7aK5rXw8ABNee4rv3h__EzfoFAAEC",
                    Answers = new[] {
                        new DialogAnswer {
                            IsHidden = true,
                            Message = "В жопе столько",
                            MoveToDialog = Dialog.Demianov2
                        },
                        new DialogAnswer {
                            IsHidden = true,
                            Message = "На складе столько",
                            MoveToDialog = Dialog.Demianov2
                        },
                        new DialogAnswer {
                            Message = "Сбросить вызов",
                            MoveToDialog = Dialog.Map
                        },
                        new DialogAnswer {
                            Message = "Ещё подсказку",
                            MoveToDialog = Dialog.Demianov4
                        },
                    }
                },
                new DialogQuestion {
                    Name = Dialog.Demianov4,
                    Message = "Недосдача сколько?",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "На складе столько",
                            MoveToDialog = Dialog.Demianov2
                        },
                        new DialogAnswer {
                            Message = "В жопе столько",
                            MoveToDialog = Dialog.Demianov2
                        },
                        new DialogAnswer {
                            Message = "Сбросить вызов",
                            MoveToDialog = Dialog.Map
                        },
                    }
                },
            };

            var repaDiagogs = new[] {
                new DialogQuestion {
                    Name = Dialog.Repa1,
                    Message = "Репа, приодетый и гладкий как шёлк, даёт вам пятюню и поздравляет.",
                    Answers = new[] {
                        new DialogAnswer {
                            Available = (i, j) => !i.Has(Item.PhoneNumber),
                            Message = "Узнать за мутки",
                            MoveToDialog = Dialog.Repa2
                        },
                        new DialogAnswer {
                            Message = "Уйти",
                            MoveToDialog = Dialog.Map,
                            MoveToPos = (pos, map) => map.PosLeft(MapIcon.Repa)
                        },
                    },
                    MapIcon = MapIcon.Repa,
                    DisplayMap = true,
                },
                new DialogQuestion {
                    Name = Dialog.Repa2,
                    Message = "_Репа:_ Бротиш, есть возможность подзаработать. Держи визитку человечка. " +
                              "Набери ему на *цифры*, он всё объяснит.",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Поблагодарить и уйти",
                            MoveToDialog = Dialog.Map,
                            ChangeInventory = i => i.Give(Item.PhoneNumber),
                            ChangeJournal = j => j.Open(Quest.FireAlarm),
                            MoveToPos = (pos, map) => map.PosLeft(MapIcon.Repa),
                            ChangeMap = (pos, map) => map
                                .Replace(map.PosLeft(MapIcon.Sokrat), MapIcon.Flame)
                                .Replace(map.PosDown(MapIcon.Sokrat), MapIcon.Flame)
                                .Replace(map.PosLeft(map.PosDown(MapIcon.Sokrat)), MapIcon.Flame)
                        },
                    }
                },
            };

            var sedoshDiagogs = new[] {
                new DialogQuestion {
                    Name = Dialog.Sedosh1,
                    Message = "Капитан Владимир Седошенко поздравляет вас от всего сердца.",
                    Answers = new[] {
                        new DialogAnswer {
                            Available = (i, j) => !i.Has(Item.Stick),
                            Message = "Поговорить",
                            MoveToDialog = Dialog.Sedosh2
                        },
                        new DialogAnswer {
                            Available = (i, j) => i.Has(Item.Project),
                            Message = "Отдать проект от Якова",
                            ChangeInventory = i => i.Take(Item.Project),
                            MoveToDialog = Dialog.Sedosh3
                        },
                        new DialogAnswer {
                            Message = "Уйти",
                            MoveToDialog = Dialog.Map,
                            MoveToPos = (pos, map) => map.PosRight(MapIcon.Sedosh)
                        },
                    },
                    MapIcon = MapIcon.Sedosh,
                    DisplayMap = true,
                },
                new DialogQuestion {
                    Name = Dialog.Sedosh2,
                    Message =
                        "Капитан Владимир Седошенко делится своими планами. Ему необходима помощь в " +
                        "*проектировании* таунхауса из палок и связующего материала.",
                    Answers = new[] {
                        new DialogAnswer {
                            Available = (i, j) => !j.IsOpen(Quest.Sedosh),
                            Message = "Пообещать помочь",
                            MoveToDialog = Dialog.Map,
                            ChangeJournal = j => j.Open(Quest.Sedosh),
                            MoveToPos = (pos, map) => map.PosRight(MapIcon.Sedosh)
                        },
                        new DialogAnswer {
                            Available = (i, j) => i.Has(Item.Project),
                            Message = "Отдать проект от Якова",
                            ChangeInventory = i => i.Take(Item.Project),
                            MoveToDialog = Dialog.Sedosh3
                        },
                        new DialogAnswer {
                            Available = (i, j) => j.IsOpen(Quest.Sedosh),
                            Message = "Уйти",
                            MoveToDialog = Dialog.Map,
                            MoveToPos = (pos, map) => map.PosRight(MapIcon.Sedosh)
                        },
                    }
                },
                new DialogQuestion {
                    Name = Dialog.Sedosh3,
                    Message = "Капитан Владимир Седошенко благодарит вас за оказанную услугу. " +
                              "Он с гордым видом достает из заднего кармана заряженый *эбонитовый стержень* " +
                              "и торжественно вручает со словами: \"Этот могущественный артефакт я нашел, " +
                              "перекапывая глиняный участок на заднем дворе своего будущего особняка. " +
                              "Думаю, ты найдешь ему применение.\"",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Поблагодарить и уйти",
                            MoveToDialog = Dialog.Map,
                            ChangeInventory = i => i.Give(Item.Stick),
                            ChangeJournal = j => j.Finish(Quest.Sedosh),
                            MoveToPos = (pos, map) => map.PosRight(MapIcon.Sedosh)
                        },
                    }
                },
            };

            var jacobDialogs = new[] {
                new DialogQuestion {
                    Name = Dialog.Jacob1,
                    Message = "_Яков:_ Антон, я очень рад тоум, что ты пригалсил нас с Леной на свадьбу. " +
                              "Мы тут облутались оптимизмом и не отсечемся ещё трлько минут через 20-30.",
                    Answers = new[] {
                        new DialogAnswer {
                            Available = (i, j) => j.IsOpen(Quest.Jacob),
                            Message = "Попросить обсчитать проетк",
                            MoveToDialog = Dialog.Jacob2
                        },
                        new DialogAnswer {
                            Available = (i, j) => i.Has(Item.Phone),
                            Message = "Дать тефлвон",
                            ChangeInventory = i => i.Take(Item.Phone),
                            MoveToDialog = Dialog.Jacob3
                        },
                        new DialogAnswer {
                            Message = "Откланяться",
                            MoveToDialog = Dialog.Map,
                            MoveToPos = (pos, map) => map.PosRight(MapIcon.Jacob)
                        },
                    },
                    MapIcon = MapIcon.Jacob,
                    DisplayMap = true,
                },
                new DialogQuestion {
                    Name = Dialog.Jacob2,
                    Message = "_Яков:_ Да легко, но я только что разбил свой *телефон*. " +
                              "Замути мне новый по салу, и я всё быстренькол бcчитаю.",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Пообещать помочь",
                            MoveToDialog = Dialog.Map,
                            ChangeJournal = j => j.Open(Quest.Demianov),
                            MoveToPos = (pos, map) => map.PosRight(MapIcon.Jacob)
                        }
                    }
                },
                new DialogQuestion {
                    Name = Dialog.Jacob3,
                    Message = "_Яков:_ Топово! Сейчас изи всё посчитаем. Такс... " +
                              "У нас есть кран-балка на 25, а шпонки только на 3 и на 4." +
                              "\nСколько шпонок взять, чтобы закрутить кран-балку?",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "три на 15",
                            MoveToDialog = Dialog.Jacob4
                        },
                        new DialogAnswer {
                            Message = "квадратную на 3 и на 4",
                            MoveToDialog = Dialog.Jacob5
                        },
                        new DialogAnswer {
                            Message = "сруб на 30 и пару шпонок",
                            MoveToDialog = Dialog.Jacob4
                        },
                        new DialogAnswer {
                            Message = "треугольную и 8 эксцентриков",
                            MoveToDialog = Dialog.Jacob4
                        },
                    }
                },
                new DialogQuestion {
                    Name = Dialog.Jacob4,
                    Message = "_Яков:_ Что-то не сходится...",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Посчитат ьещё раз",
                            MoveToDialog = Dialog.Jacob3
                        }
                    }
                },
                new DialogQuestion {
                    Name = Dialog.Jacob5,
                    Message = "_Яков:_ Да, всё сошлоь. Держи *провджкт*!",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Откланяться",
                            MoveToDialog = Dialog.Map,
                            ChangeInventory = i => i.Give(Item.Project),
                            ChangeJournal = j => j.Finish(Quest.Jacob),
                            MoveToPos = (pos, map) => map.PosRight(MapIcon.Jacob)
                        }
                    }
                },
            };

            var sokratDialogs = new[] {
                new DialogQuestion {
                    Name = Dialog.Sokrat1,
                    Message = "Сократ (задротит)",
                    Answers = new[] {
                        new DialogAnswer {
                            Available = (i, j) => j.IsFinished(Quest.FireAlarm) && !j.IsKnown(Quest.Sokrat),
                            Message = "Сократ, что это было?!",
                            MoveToDialog = Dialog.Sokrat2
                        },
                        new DialogAnswer {
                            Available = (i, j) => j.IsOpen(Quest.Sokrat),
                            Message = "Помочь с роутером",
                            MoveToDialog = Dialog.Sokrat3
                        },
                        new DialogAnswer {
                            Message = "Уйти",
                            MoveToDialog = Dialog.Map,
                            MoveToPos = (pos, map) => map.PosLeft(MapIcon.Sokrat)
                        },
                    },
                    MapIcon = MapIcon.Sokrat,
                    DisplayMap = true,
                },
                new DialogQuestion {
                    Name = Dialog.Sokrat2,
                    Message = "_Сократ:_ Сорян, чёт пригорел немного. Опять вайфай роутер не работает, я из-за этого катку в лолец слил!",
                    Answers = new[] {
                        new DialogAnswer {
                            ChangeJournal = j => j.Open(Quest.Sokrat),
                            Message = "Предложить помощь",
                            MoveToDialog = Dialog.Sokrat3
                        },
                        new DialogAnswer {
                            Message = "Не связываться с психом",
                            MoveToDialog = Dialog.Map,
                            MoveToPos = (pos, map) => map.PosLeft(MapIcon.Sokrat)
                        },
                    },
                },
                new DialogQuestion {
                    Name = Dialog.Sokrat3,
                    Message = "_Сократ:_ Поможешь починить роутер? Я уже всё перепробовал. И молотком его бил, и об стену. Не помогает.",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Поправить антенны",
                            MoveToDialog = Dialog.Sokrat4
                        },
                        new DialogAnswer {
                            Message = "Перезагрузить роутер",
                            MoveToDialog = Dialog.Sokrat4
                        },
                        new DialogAnswer {
                            Available = (i, j) => i.Has(Item.Stick),
                            Message = "Использовать жезл",
                            ChangeInventory = i => i.Take(Item.Stick),
                            MoveToDialog = Dialog.Sokrat5
                        },
                    },
                },
                new DialogQuestion {
                    Name = Dialog.Sokrat4,
                    Message = "_Сократ:_ Кажется, ничего не изменилось",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Поправить антенны",
                            MoveToDialog = Dialog.Sokrat4
                        },
                        new DialogAnswer {
                            Message = "Перезагрузить роутер",
                            MoveToDialog = Dialog.Sokrat4
                        },
                        new DialogAnswer {
                            Available = (i, j) => i.Has(Item.Stick),
                            Message = "Использовать жезл",
                            MoveToDialog = Dialog.Sokrat5
                        },
                        new DialogAnswer {
                            Message = "Уйти",
                            MoveToDialog = Dialog.Map,
                            MoveToPos = (pos, map) => map.PosLeft(MapIcon.Sokrat)
                        },
                    },
                },
                new DialogQuestion {
                    Name = Dialog.Sokrat5,
                    Message = "_Сократ:_ Ух ты! " +
                              "Никогда бы сам не догадался вставить заряженный эбонитовый жезл погбы в свой роутер! " +
                              "Но с этой антенной интернет стал работать просто идеально! Не знаю, как тебя благодарить. " +
                              "На вот, держи мою *шляпу* из Феодосии. Она до сих пор почти как новая.",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Поблагодарить",
                            ChangeInventory = i => i.Give(Item.Hat),
                            ChangeJournal = j => j.Finish(Quest.Sokrat),
                            MoveToDialog = Dialog.Map,
                            MoveToPos = (pos, map) => map.PosLeft(MapIcon.Sokrat)
                        },
                    },
                },
                new DialogQuestion {
                    Name = Dialog.Flame1,
                    Message = "Деревянный паркет полыхает перед вашими ногами. Вы слышите пожарную сирену.",
                    Answers = new[] {
                        new DialogAnswer {
                            Available = (i, j) => i.Has(Item.FireExtinguisher),
                            Message = "Использовать огнетушитель",
                            MoveToDialog = Dialog.Flame2,
                            ChangeJournal = j => j.Finish(Quest.FireAlarm),
                            ChangeInventory = i => i.Take(Item.FireExtinguisher),
                            ChangeMap = (pos, map) => map
                                .Replace(map.PosLeft(MapIcon.Sokrat), MapIcon.Empty)
                                .Replace(map.PosDown(MapIcon.Sokrat), MapIcon.Empty)
                                .Replace(map.PosLeft(map.PosDown(MapIcon.Sokrat)), MapIcon.Empty)
                        },
                        new DialogAnswer {
                            Message = "Нужно что-то придумать...",
                            MoveToDialog = Dialog.Map
                        },
                    },
                    DisplayMap = true,
                    PreventMove = true,
                    MapIcon = MapIcon.Flame
                },
                new DialogQuestion {
                    Name = Dialog.Flame2,
                    Message = "Уффф, кажется получилось избежать катастрофы. Надо спросить у Сократа о произошедшем.",
                    Answers = mapDialog.Answers,
                    DisplayMap = true
                },
                new DialogQuestion {
                    Name = Dialog.SmallDoor,
                    Message = "На двери табличка: СЛУЖЕБНОЕ ПОМЕЩЕНИЕ.",
                    Answers = new[] {
                        new DialogAnswer {
                            Available = (i, j) => j.IsOpen(Quest.FireAlarm),
                            Message = "Выбить дверь ногой!",
                            MoveToDialog = Dialog.Map,
                            ChangeMap = (pos, map) => map.ClearPos(pos)
                        },
                        new DialogAnswer {
                            Message = "Уйти",
                            MoveToDialog = Dialog.Map,
                            MoveToPos = (pos, map) => map.PosLeft(MapIcon.SmallDoor)
                        },
                    },
                    MapIcon = MapIcon.SmallDoor
                },
                new DialogQuestion {
                    Name = Dialog.FireExtinguisher,
                    Message = "На стене висит огнетушитель.",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Схватить",
                            MoveToDialog = Dialog.Map,
                            ChangeMap = (pos, map) => map.ClearPos(pos),
                            ChangeInventory = i => i.Give(Item.FireExtinguisher)
                        },
                    },
                    DisplayMap = true,
                    MapIcon = MapIcon.FireExtinguisher
                },
            };

            var zagsWorkerDialogs = new[] {
                new DialogQuestion {
                    Name = Dialog.ZagsWorker1,
                    Message = "Работница ЗАГСа сидит за столом и поправляет прическу.",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Поговорить",
                            Available = (i, j) => !j.IsFinished(Quest.DressForWedding),
                            ChangeJournal = j => j.Finish(Quest.AskForWedding).Open(Quest.DressForWedding),
                            MoveToDialog = Dialog.ZagsWorker2
                        },
                        new DialogAnswer {
                            Message = "Приодеться!",
                            Available = (i, j) => j.IsOpen(Quest.DressForWedding) && i.Has(Item.Boots) && i.Has(Item.Hat) && i.Has(Item.Glasses),
                            MoveToDialog = Dialog.ZagsWorker3
                        },
                        new DialogAnswer {
                            Message = "Уйти",
                            MoveToDialog = Dialog.Map,
                            MoveToPos = (pos, map) => map.PosDown(MapIcon.ZagsWorker)
                        },
                    },
                    MapIcon = MapIcon.ZagsWorker
                },
                new DialogQuestion {
                    Name = Dialog.ZagsWorker2,
                    Message = "Она окидывает вас оценивающим взглядом и произносит:" +
                              "\nВы не можете начинать церемонию в таком виде! В правилах нашего загса " +
                              "ясно сказано, что брачующиеся должны быть одеты в строго определённой форме! " +
                              "Для жениха это: *шляпа*, *монокль* и *высокие сапоги*. Идите ищите.",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Приодеться!",
                            Available = (i, j) => j.IsOpen(Quest.DressForWedding) && i.Has(Item.Boots) && i.Has(Item.Hat) && i.Has(Item.Glasses),
                            MoveToDialog = Dialog.ZagsWorker3
                        },
                        new DialogAnswer {
                            Message = "Уйти",
                            MoveToDialog = Dialog.Map,
                            MoveToPos = (pos, map) => map.PosDown(MapIcon.ZagsWorker)
                        },
                    }
                },
                new DialogQuestion {
                    Name = Dialog.ZagsWorker3,
                    Message = "Вот, теперь можно и начинать! А где невеста?",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "WHAT?! FFFUUUUUU",
                            MoveToDialog = Dialog.ZagsWorker4,
                            ChangeJournal = j => j.Finish(Quest.DressForWedding),
                            ChangeMap = (pos, map) => map
                                .Replace(map.PosLeft(MapIcon.ZagsWorker), MapIcon.Flame)
                                .Replace(map.PosDown(MapIcon.ZagsWorker), MapIcon.Flame)
                                .Replace(map.PosDown(MapIcon.ZagsWorker) - 1, MapIcon.Flame)
                                .Replace(map.PosDown(MapIcon.ZagsWorker) + 1, MapIcon.Flame)
                                .Replace(map.PosDown(map.PosDown(MapIcon.ZagsWorker)), MapIcon.Flame)
                                .Replace(map.PosRight(MapIcon.ZagsWorker), MapIcon.Flame)
                                .Replace(map.PosRight(MapIcon.Jacob), MapIcon.Flame)
                                .Replace(map.PosRight(MapIcon.Sedosh), MapIcon.Flame)
                                .Replace(map.PosLeft(MapIcon.Repa), MapIcon.Flame)
                                .Replace(map.PosLeft(MapIcon.Sokrat), MapIcon.Flame)
                        },
                    },
                },
                new DialogQuestion {
                    Name = Dialog.ZagsWorker4,
                    Message = "\ud83d\udc70\ud83c\udffc???????",
                    DisplayMap = true,
                },
            };

            var randomDialogs = new[] {
                new DialogQuestion {
                    Name = Dialog.FoundWall,
                    Message = "Уперся в стену",
                    Answers = mapDialog.Answers,
                    DisplayMap = true,
                    PreventMove = true,
                    MapIcon = MapIcon.Wall
                },
                new DialogQuestion {
                    Name = Dialog.FoundFear,
                    Message = "Вы чувствуете прилив одиночества и отчаяния." +
                              " Ваши ноги не хотят идти в этом направлении.",
                    Answers = mapDialog.Answers,
                    DisplayMap = true,
                    PreventMove = true,
                    MapIcon = MapIcon.Fear
                },
            };

            var toshikDialogs = new[] {
                    mapDialog,
                    moveToDialog,
                    inventoryDialog,
                    journalDialog
                }.Concat(monocleDialogs)
                .Concat(startDoorDialogs)
                .Concat(veilDialogs)
                .Concat(randomDialogs)
                .Concat(demianovDiagogs)
                .Concat(repaDiagogs)
                .Concat(sokratDialogs)
                .Concat(jacobDialogs)
                .Concat(sedoshDiagogs)
                .Concat(bootsDialogs)
                .Concat(zagsWorkerDialogs)
                .ToArray();
            foreach (var dialogQuestion in toshikDialogs) {
                dialogQuestion.ForPlayer = "@Insomnov;@MistifliQ;@starteleport;@svsokrat;296536101";
            }
            return toshikDialogs;
        }
    }
}

using System.Linq;

namespace Bot
{
    public static class ToshikQuest
    {
        static string initialMap =
            @"
#########
#########
##--Z--##
##J---R##
##-----##
##K---S##
##-v---##
###П#####
##O-#####
##--#####
##8-#####
##~~#####
##--#####
";
        public static QuestService Initialize()
        {
            var mapDialog = new DialogQuestion {
                Name = Dialog.Map,
                Answers = new[] {
                    new DialogAnswer {
                        Message = Directions.Left.ToString(),
                        NextDialogName = Dialog.Map,
                        Move = (pos, map) => pos - 1
                    },
                    new DialogAnswer {
                        Message = Directions.Up.ToString(),
                        NextDialogName = Dialog.Map,
                        Move = (pos, map) => map.PosUp(pos)
                    },
                    new DialogAnswer {
                        Message = Directions.Down.ToString(),
                        NextDialogName = Dialog.Map,
                        Move = (pos, map) => map.PosDown(pos)
                    },
                    new DialogAnswer {
                        Message = Directions.Right.ToString(),
                        NextDialogName = Dialog.Map,
                        Move = (pos, map) => pos + 1
                    },
                    new DialogAnswer {
                        IsAvailable = i => i.Has(Item.Veil),
                        Message = "Подойти к...",
                        NextDialogName = Dialog.MapMoveTo
                    },
                    new DialogAnswer {
                        IsAvailable = i => i.Has(Item.PhoneRequest) && !i.Has(Item.Phone),
                        Message = "Позвонить Жене Демьянову",
                        NextDialogName = Dialog.Demianov1,
                    },
                    new DialogAnswer {
                        IsAvailable = i => i.Has(Item.PhoneNumber),
                        Message = "Позвонить на цифры",
                        NextDialogName = Dialog.FoundFear,
                    }
                },
                DisplayMap = true
            };

            var moveToDialog = new DialogQuestion {
                Name = Dialog.MapMoveTo,
                Message = "Куда идем?",
                Answers = new[] {
                    new DialogAnswer {
                        Message = "К Капитану",
                        NextDialogName = Dialog.Sedosh1,
                        Move = (pos, map) => map.PosRight(MapIcon.Sedosh)
                    },
                    new DialogAnswer {
                        Message = "К Сократу",
                        NextDialogName = Dialog.Sokrat1,
                        Move = (pos, map) => map.PosLeft(MapIcon.Sokrat)
                    },
                    new DialogAnswer {
                        Message = "К Репе",
                        NextDialogName = Dialog.Repa1,
                        Move = (pos, map) => map.PosLeft(MapIcon.Repa)
                    },
                    new DialogAnswer {
                        Message = "К Якову",
                        NextDialogName = Dialog.Jacob1,
                        Move = (pos, map) => map.PosRight(MapIcon.Jacob)
                    },
                    new DialogAnswer {
                        Message = "К работнице ЗАГСа",
                        NextDialogName = Dialog.ZagsWorker1,
                        Move = (pos, map) => map.PosDown(MapIcon.ZagsWorker)
                    },
                    new DialogAnswer {
                        Message = "<<<",
                        NextDialogName = Dialog.Map
                    },
                },
                DisplayMap = true
            };

            var monocleDialogs = new[] {
                new DialogQuestion {
                    Name = Dialog.Glasses,
                    Message = "Под ногами лежит *монокль* в золотой оправе. Чуть не наступил.",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Поднять",
                            NextDialogName = Dialog.FoundGlasses,
                            ChangeInventory = i => i.Give(Item.Glasses),
                            ClearMapCell = true
                        },
                        new DialogAnswer {
                            Message = "Оставить",
                            NextDialogName = Dialog.Map
                        },
                    },
                    MapIcon = MapIcon.Glasses
                },
                new DialogQuestion {
                    Name = Dialog.FoundGlasses,
                    Message = "Дорогая вещица. И удобная! Теперь я вижу немного дальше!",
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
                            NextDialogName = Dialog.Map,
                            ClearMapCell = true
                        },
                        new DialogAnswer {
                            Message = "Прочитать табличку",
                            NextDialogName = Dialog.StartDoors2
                        },
                        new DialogAnswer {
                            Message = "Уйти",
                            NextDialogName = Dialog.Map,
                            Move = (pos, map) => map.PosDown(pos)
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
                            NextDialogName = Dialog.Map,
                            ClearMapCell = true
                        },
                        new DialogAnswer {
                            Message = "Осмотреться",
                            NextDialogName = Dialog.StartDoors3
                        },
                        new DialogAnswer {
                            Message = "Уйти",
                            NextDialogName = Dialog.Map,
                            Move = (pos, map) => map.PosDown(pos)
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
                            NextDialogName = Dialog.Map,
                            ClearMapCell = true
                        },
                        new DialogAnswer {
                            Message = "Замереть в нерешительности",
                            NextDialogName = Dialog.StartDoors4
                        },
                        new DialogAnswer {
                            Message = "Уйти",
                            NextDialogName = Dialog.Map,
                            Move = (pos, map) => map.PosDown(pos)
                        },
                    },
                },
                new DialogQuestion {
                    Name = Dialog.StartDoors4,
                    Message = "Ничего не происходит. Хватит мять сиськи.",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Потянуть за ручку двери",
                            NextDialogName = Dialog.Map,
                            ClearMapCell = true
                        },
                        new DialogAnswer {
                            Message = "Уйти",
                            NextDialogName = Dialog.Map,
                            Move = (pos, map) => map.PosDown(pos)
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
                            NextDialogName = Dialog.Veil2,
                            ClearMapCell = true,
                            ChangeInventory = i => i.Give(Item.Veil)
                        },
                        new DialogAnswer {
                            Message = "Перешагнуть",
                            NextDialogName = Dialog.EnteredHall,
                            ClearMapCell = true,
                            ChangeInventory = i => i.Give(Item.Veil)
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
                            NextDialogName = Dialog.Veil3
                        },
                    },
                },
                new DialogQuestion {
                    Name = Dialog.Veil3,
                    Photo = "Resources/Kabluk.jpg***AgADAgADkaoxG1sH0Uu7QaMcUhByutN4Xw8ABDuyScUAAeRl2Zu7BQABAg",
                    Message = "Вы поднимаете фотографию.",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "ФФФФУУУУУУ",
                            NextDialogName = Dialog.EnteredHall,
                        },
                    },
                },
                new DialogQuestion {
                    Name = Dialog.EnteredHall,
                    Message = "В зале собрались гости, все очень рады вас видеть. Пожалуй, стоит пообщаться с гостями перед тем, как начинать церемонию.",
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
                            NextDialogName = Dialog.Demianov2
                        },
                        new DialogAnswer {
                            IsHidden = true,
                            Message = "На складе столько",
                            NextDialogName = Dialog.Demianov2
                        },
                        new DialogAnswer {
                            Message = "Сбросить вызов",
                            NextDialogName = Dialog.Map
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
                            NextDialogName = Dialog.Map
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
                            IsAvailable = i => !i.Has(Item.PhoneNumber),
                            Message = "Узнать за мутки",
                            NextDialogName = Dialog.Repa2
                        },
                        new DialogAnswer {
                            Message = "Уйти",
                            NextDialogName = Dialog.Map,
                            Move = (pos, map) => map.PosLeft(MapIcon.Repa)
                        },
                    },
                    MapIcon = MapIcon.Repa,
                    DisplayMap = true,
                },
                new DialogQuestion {
                    Name = Dialog.Repa2,
                    Message = "Бротиш, есть возможность подзаработать. Держи визитку человечка. " +
                              "Набери ему на *цифры*, он всё объяснит.",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Поблагодарить и уйти",
                            NextDialogName = Dialog.Map,
                            ChangeInventory = i => i.Give(Item.PhoneNumber),
                            Move = (pos, map) => map.PosLeft(MapIcon.Repa)
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
                            IsAvailable = i => !i.Has(Item.Stick),
                            Message = "Поговорить",
                            NextDialogName = Dialog.Sedosh2
                        },
                        new DialogAnswer {
                            IsAvailable = i => i.Has(Item.Project) && !i.Has(Item.Stick),
                            Message = "Отдать проект, от Якова",
                            NextDialogName = Dialog.Sedosh3
                        },
                        new DialogAnswer {
                            Message = "Уйти",
                            NextDialogName = Dialog.Map,
                            Move = (pos, map) => map.PosRight(MapIcon.Sedosh)
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
                            IsAvailable = i => !i.Has(Item.ProjectRequest),
                            Message = "Пообещать помочь",
                            NextDialogName = Dialog.Map,
                            ChangeInventory = i => i.Give(Item.ProjectRequest),
                            Move = (pos, map) => map.PosRight(MapIcon.Sedosh)
                        },
                        new DialogAnswer {
                            IsAvailable = i => i.Has(Item.Project),
                            Message = "Отдать проект от Якова",
                            NextDialogName = Dialog.Sedosh3
                        },
                        new DialogAnswer {
                            IsAvailable = i => i.Has(Item.ProjectRequest),
                            Message = "Уйти",
                            NextDialogName = Dialog.Map,
                            Move = (pos, map) => map.PosRight(MapIcon.Sedosh)
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
                            Message = "Поблагодарить и, уйти",
                            NextDialogName = Dialog.Map,
                            ChangeInventory = i => i.Give(Item.Stick),
                            Move = (pos, map) => map.PosRight(MapIcon.Sedosh)
                        },
                    }
                },
            };

            var jacobDialogs = new[] {
                new DialogQuestion {
                    Name = Dialog.Jacob1,
                    Message = "_Яков: _Антон, я очень рад тоум, что ты пригалсил нас с Леной на свадьбу. " +
                              "Мы тут облутались оптимизмом и не отсечемся ещё трлько минут через 20-30.",
                    Answers = new[] {
                        new DialogAnswer {
                            IsAvailable = i => i.Has(Item.ProjectRequest) && !i.Has(Item.Project),
                            Message = "Попросить обсчитать проетк",
                            NextDialogName = Dialog.Jacob2
                        },
                        new DialogAnswer {
                            IsAvailable = i => i.Has(Item.Phone) && !i.Has(Item.Project),
                            Message = "Дать тефлвон",
                            NextDialogName = Dialog.Jacob3
                        },
                        new DialogAnswer {
                            Message = "Откланяться",
                            NextDialogName = Dialog.Map,
                            Move = (pos, map) => map.PosRight(MapIcon.Jacob)
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
                            NextDialogName = Dialog.Map,
                            ChangeInventory = i => i.Give(Item.PhoneRequest),
                            Move = (pos, map) => map.PosRight(MapIcon.Jacob)
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
                            Message = "Три на 15",
                            NextDialogName = Dialog.Jacob4
                        },
                        new DialogAnswer {
                            Message = "Квадратную на 3 и Квадратную на 4",
                            NextDialogName = Dialog.Jacob5
                        },
                        new DialogAnswer {
                            Message = "сруб на 30 и пару шпонок",
                            NextDialogName = Dialog.Jacob4
                        },
                        new DialogAnswer {
                            Message = "1 треугольную и 8 эксцентриков",
                            NextDialogName = Dialog.Jacob4
                        },
                    }
                },
                new DialogQuestion {
                    Name = Dialog.Jacob4,
                    Message = "_Яков:_ Что-то не сходится...",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Посчитат ьещё раз",
                            NextDialogName = Dialog.Jacob3
                        }
                    }
                },
                new DialogQuestion {
                    Name = Dialog.Jacob5,
                    Message = "_Яков:_ Да, всё сошлоь. Держи *провджкт*!",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Откланяться",
                            NextDialogName = Dialog.Map,
                            ChangeInventory = i => i.Give(Item.Project)
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
                            Message = "Уйти",
                            NextDialogName = Dialog.Map,
                            Move = (pos, map) => map.PosLeft(MapIcon.Sokrat)
                        },
                    },
                    MapIcon = MapIcon.Sokrat,
                    DisplayMap = true,
                },
            };

            var zagsWorkerDialogs = new[] {
                new DialogQuestion {
                    Name = Dialog.ZagsWorker1,
                    Message = "Работница загса сидит за столом и поправляет прическу.",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Уйти",
                            NextDialogName = Dialog.Map,
                            Move = (pos, map) => map.PosDown(MapIcon.ZagsWorker)
                        },
                    },
                    MapIcon = MapIcon.ZagsWorker,
                    DisplayMap = true,
                },
            };

            var randomDialogs = new[] {
                new DialogQuestion {
                    Name = Dialog.FoundWall,
                    Message = "Впереди стена.",
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

            var dialogs = new[] {
                    mapDialog,
                    moveToDialog
                }.Concat(monocleDialogs)
                .Concat(startDoorDialogs)
                .Concat(veilDialogs)
                .Concat(randomDialogs)
                .Concat(demianovDiagogs)
                .Concat(repaDiagogs)
                .Concat(sokratDialogs)
                .Concat(jacobDialogs)
                .Concat(sedoshDiagogs)
                .Concat(zagsWorkerDialogs);

            var inventory = new Inventory();
            return new QuestService(initialMap, inventory, dialogs.ToArray());
        }
    }
}

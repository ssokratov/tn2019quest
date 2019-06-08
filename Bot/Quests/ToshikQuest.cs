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
                        MoveToDialog = Dialog.Map,
                        MoveToPos = (pos, map) => pos - 1
                    },
                    new DialogAnswer {
                        Message = Directions.Up.ToString(),
                        MoveToDialog = Dialog.Map,
                        MoveToPos = (pos, map) => map.PosUp(pos)
                    },
                    new DialogAnswer {
                        Message = Directions.Down.ToString(),
                        MoveToDialog = Dialog.Map,
                        MoveToPos = (pos, map) => map.PosDown(pos)
                    },
                    new DialogAnswer {
                        Message = Directions.Right.ToString(),
                        MoveToDialog = Dialog.Map,
                        MoveToPos = (pos, map) => pos + 1
                    },
                    new DialogAnswer {
                        Available = i => i.Has(Item.Veil),
                        Message = "Подойти к...",
                        MoveToDialog = Dialog.MapMoveTo
                    },
                    new DialogAnswer {
                        Available = i => i.Has(Item.PhoneRequest) && !i.Has(Item.Phone),
                        Message = "Позвонить Жене Демьянову",
                        MoveToDialog = Dialog.Demianov1,
                    },
                    new DialogAnswer {
                        Available = i => i.Has(Item.PhoneNumber),
                        Message = "Позвонить на цифры",
                        MoveToDialog = Dialog.FoundFear,
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
                        MoveToDialog = Dialog.Sedosh1,
                        MoveToPos = (pos, map) => map.PosRight(MapIcon.Sedosh)
                    },
                    new DialogAnswer {
                        Message = "К Сократу",
                        MoveToDialog = Dialog.Sokrat1,
                        MoveToPos = (pos, map) => map.PosLeft(MapIcon.Sokrat)
                    },
                    new DialogAnswer {
                        Message = "К Репе",
                        MoveToDialog = Dialog.Repa1,
                        MoveToPos = (pos, map) => map.PosLeft(MapIcon.Repa)
                    },
                    new DialogAnswer {
                        Message = "К Якову",
                        MoveToDialog = Dialog.Jacob1,
                        MoveToPos = (pos, map) => map.PosRight(MapIcon.Jacob)
                    },
                    new DialogAnswer {
                        Message = "К работнице ЗАГСа",
                        MoveToDialog = Dialog.ZagsWorker1,
                        MoveToPos = (pos, map) => map.PosDown(MapIcon.ZagsWorker)
                    },
                    new DialogAnswer {
                        Message = "<<<",
                        MoveToDialog = Dialog.Map
                    },
                },
                DisplayMap = true
            };

            var monocleDialogs = new[] {
                new DialogQuestion {
                    Name = Dialog.Glasses_1,
                    Message = "Под ногами лежит *монокль* в золотой оправе. Чуть не наступил.",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Поднять",
                            MoveToDialog = Dialog.Glasses_2,
                            ChangeInventory = i => i.Give(Item.Glasses),
                            ClearMapCell = true
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
                    Name = Dialog.Glasses_2,
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
                            MoveToDialog = Dialog.Map,
                            ClearMapCell = true
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
                            ClearMapCell = true
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
                            ClearMapCell = true
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
                            ClearMapCell = true
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
                            ClearMapCell = true,
                            ChangeInventory = i => i.Give(Item.Veil)
                        },
                        new DialogAnswer {
                            Message = "Перешагнуть",
                            MoveToDialog = Dialog.EnteredHall,
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
                            MoveToDialog = Dialog.Veil3
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
                            MoveToDialog = Dialog.EnteredHall,
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
                    }
                },
                new DialogQuestion {
                    Name = Dialog.Demianov2,
                    Message = "Точно, так оно и есть! Отправил к тебе курьера с *телефоном*. Поздравляю!",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Поблагодарить",
                            ChangeInventory = i => i.Give(Item.Phone),
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
                            Available = i => !i.Has(Item.PhoneNumber),
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
                    Message = "Бротиш, есть возможность подзаработать. Держи визитку человечка. " +
                              "Набери ему на *цифры*, он всё объяснит.",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Поблагодарить и уйти",
                            MoveToDialog = Dialog.Map,
                            ChangeInventory = i => i.Give(Item.PhoneNumber),
                            MoveToPos = (pos, map) => map.PosLeft(MapIcon.Repa)
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
                            Available = i => !i.Has(Item.Stick),
                            Message = "Поговорить",
                            MoveToDialog = Dialog.Sedosh2
                        },
                        new DialogAnswer {
                            Available = i => i.Has(Item.Project) && !i.Has(Item.Stick),
                            Message = "Отдать проект, от Якова",
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
                            Available = i => !i.Has(Item.ProjectRequest),
                            Message = "Пообещать помочь",
                            MoveToDialog = Dialog.Map,
                            ChangeInventory = i => i.Give(Item.ProjectRequest),
                            MoveToPos = (pos, map) => map.PosRight(MapIcon.Sedosh)
                        },
                        new DialogAnswer {
                            Available = i => i.Has(Item.Project),
                            Message = "Отдать проект от Якова",
                            MoveToDialog = Dialog.Sedosh3
                        },
                        new DialogAnswer {
                            Available = i => i.Has(Item.ProjectRequest),
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
                            Message = "Поблагодарить и, уйти",
                            MoveToDialog = Dialog.Map,
                            ChangeInventory = i => i.Give(Item.Stick),
                            MoveToPos = (pos, map) => map.PosRight(MapIcon.Sedosh)
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
                            Available = i => i.Has(Item.ProjectRequest) && !i.Has(Item.Project),
                            Message = "Попросить обсчитать проетк",
                            MoveToDialog = Dialog.Jacob2
                        },
                        new DialogAnswer {
                            Available = i => i.Has(Item.Phone) && !i.Has(Item.Project),
                            Message = "Дать тефлвон",
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
                            ChangeInventory = i => i.Give(Item.PhoneRequest),
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
                            Message = "Три на 15",
                            MoveToDialog = Dialog.Jacob4
                        },
                        new DialogAnswer {
                            Message = "Квадратную на 3 и Квадратную на 4",
                            MoveToDialog = Dialog.Jacob5
                        },
                        new DialogAnswer {
                            Message = "сруб на 30 и пару шпонок",
                            MoveToDialog = Dialog.Jacob4
                        },
                        new DialogAnswer {
                            Message = "1 треугольную и 8 эксцентриков",
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
                            MoveToDialog = Dialog.Map,
                            MoveToPos = (pos, map) => map.PosLeft(MapIcon.Sokrat)
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
                            MoveToDialog = Dialog.Map,
                            MoveToPos = (pos, map) => map.PosDown(MapIcon.ZagsWorker)
                        },
                    },
                    MapIcon = MapIcon.ZagsWorker,
                    DisplayMap = true,
                },
            };

            var randomDialogs = new[] {
                new DialogQuestion {
                    Name = Dialog.FoundWall,
                    Message = "Уперся в стену.",
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

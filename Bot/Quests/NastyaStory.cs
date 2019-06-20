using System;
using System.Linq;

namespace Bot.Quests
{
    public static class NastyaStory
    {
        public static string Map =
            @"
%%%%%%%%%=====
%%%%%%%%%=====
%%TCc--%%--R%%
%%TCcN-P----%%
%%TCc--%%%%%%%
%%%%%%%%%%%%%%
%%%%%%%%%%%%%%
";

        public static DialogQuestion[] GetDialogs()
        {
            var initialMap = NewCellQuest.Map;
            var startPosition = initialMap.IndexOf(MapIcon.Nastya);

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

            var startDialog = new DialogQuestion {
                Name = Dialog.StartNastya,
                Message = "Концерт Триагрутрики только что закончился. Вы, конечно, не могли его пропустить, " +
                          "даже в день свадьбы. В любом случае это не проблема, надо только как-то такси заказать, " +
                          "а телефон сел. Кажется, на улице был таксофон.",
                Answers = mapDialog.Answers,
                DisplayMap = true,
            };

            var inventoryDialog = new DialogQuestion {
                Name = Dialog.InventoryNastya,
                DynamicMessage = (i, j) => {
                    return "*Инвентарь*:\n"
                           + (i.Has(Item.Beer) ? "\ud83c\udf7b два пива\n" : "")
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
                        + RenderQuest(Quest.Police, "Обойти полицейского")
                        + RenderQuest(Quest.BuyBeer, "Купить пива")
                        + RenderQuest(Quest.OrderTaxi, "Заказать такси")
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
                new DialogQuestion {
                    Name = Dialog.FoundRoad,
                    Message = "Перед вами дорога. Открытых люков на ней больше, чем асфальта, так что на каблуках лучше не соваться.",
                    Answers = mapDialog.Answers,
                    DisplayMap = true,
                    PreventMove = true,
                    MapIcon = MapIcon.Road
                },
            };

            var policeDialogs = new[] {
                new DialogQuestion {
                    Name = Dialog.Police1,
                    Message = "На выходе из зала стоит сурового вида Полицейский. Похоже, что кто-то пронёс " +
                              "на концерт незаконные вещества и теперь никого не выпустят, пока не прошманают " +
                              "каждый угол и каждый карман. Как некстати!",
                    Answers = new [] {
                        new DialogAnswer {
                            Message = "Найти выход",
                            Available = (i, q) => !q.IsFinished(Quest.Police),
                            ChangeJournal = j => j.Open(Quest.Police),
                            MoveToDialog = Dialog.MapNastya
                        },
                        new DialogAnswer {
                            Message = "Уйти",
                            Available = (i, q) => q.IsFinished(Quest.Police),
                            MoveToDialog = Dialog.MapNastya
                        }
                    },
                    PreventMove = true,
                    MapIcon = MapIcon.Policeman
                }
            };

            var crowdDialogs = new[] {
                new DialogQuestion {
                    Name = Dialog.Crowd0,
                    Photo = "Resources/Crowd_gop.jpg;AgADAgADGKsxG-dMWUjpNceQFaqyvoPKtw8ABIgIW5WVMsnDdBIAAgI",
                    Message = "Перед вами толпа фанатов Триагрутрики",
                    Answers = new [] {
                        new DialogAnswer {
                            Message = "Идти напролом",
                            Available = (i, q) => q.IsOpen(Quest.Police),
                            ChangeMap = (pos, map) => map.Replace(MapIcon.Crowd0, MapIcon.Empty),
                            MoveToDialog = Dialog.MapNastya
                        },
                        new DialogAnswer {
                            Message = "Не связываться",
                            MoveToPos = (pos, map) => map.PosRight(pos),
                            MoveToDialog = Dialog.MapNastya
                        },
                    },
                    MapIcon = MapIcon.Crowd0
                },
                new DialogQuestion {
                    Name = Dialog.Crowd1,
                    Photo = "Resources/Crowd_vata.jpg;AgADAgADGasxG-dMWUgDA2Yh1_yHZei9UQ8ABHYQ4rVOnfaZcMIEAAEC",
                    Message = "Перед вами особо агрессивная толпа фанатов Триагрутрики",
                    Answers = new [] {
                        new DialogAnswer {
                            Message = "Идти с боем",
                            Available = (i, q) => q.IsOpen(Quest.Police),
                            ChangeMap = (pos, map) => map.Replace(MapIcon.Crowd1, MapIcon.Empty),
                            MoveToDialog = Dialog.MapNastya
                        },
                        new DialogAnswer {
                            Message = "Развернуться",
                            MoveToPos = (pos, map) => map.PosRight(pos),
                            MoveToDialog = Dialog.MapNastya
                        },
                    },
                    MapIcon = MapIcon.Crowd1
                },
                new DialogQuestion {
                    Name = Dialog.Crowd2,
                    Photo = "Resources/Triagrutrika.jpg;AgADAgADGqsxG-dMWUgU85zEe_gY0IvVtw8ABFvJ-AhqptKv2RIAAgI",
                    Message = "Перед вами какие-то непонятные чуваки",
                    Answers = new [] {
                        new DialogAnswer {
                            Message = "Пройти мимо",
                            ChangeMap = (pos, map) => map
                                .Replace(MapIcon.Crowd2, MapIcon.Empty)
                                .Replace(map.PosUp(map.PosLeft(map.PosLeft(map.PosLeft(startPosition)))), MapIcon.Genich)
                                .Replace(map.PosDown(map.PosLeft(map.PosLeft(map.PosLeft(startPosition)))), MapIcon.Bartender),
                            MoveToPos = (pos, map) => map.PosRight(pos),
                            MoveToDialog = Dialog.MapNastya
                        }
                    },
                    MapIcon = MapIcon.Crowd2
                },
            };

            var bartenderDialogs = new[] {
                new DialogQuestion {
                    Name = Dialog.Bartender1,
                    Message = "За деревянной стойкой бармен томно потирает бокалы",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Купить пива",
                            Available = (i, j) => j.IsOpen(Quest.BuyBeer),
                            MoveToDialog = Dialog.Bartender2,
                            MoveToPos = (pos, map) => map.PosUp(MapIcon.Bartender)
                        },
                        new DialogAnswer {
                            Message = "Уйти",
                            MoveToDialog = Dialog.MapNastya,
                            MoveToPos = (pos, map) => map.PosUp(MapIcon.Bartender)
                        },
                    },
                    MapIcon = MapIcon.Bartender,
                    DisplayMap = true
                },
                new DialogQuestion {
                    Name = Dialog.Bartender2,
                    Message = "_Бармен_: не проблема! Правда, касса уже закрылась, так что рубли не принимаю. " +
                              "Галактическими кредитами сможете расплатиться? За 2 пива с вас 30 кредитов.",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Дать 30",
                            MoveToDialog = Dialog.Bartender3
                        },
                        new DialogAnswer {
                            Message = "Уйти",
                            MoveToDialog = Dialog.MapNastya,
                            MoveToPos = (pos, map) => map.PosUp(MapIcon.Bartender)
                        },
                        new DialogAnswer {
                            Message = "Попросить скидку",
                            MoveToDialog = Dialog.Bartender4
                        },
                    }
                },
                new DialogQuestion {
                    Name = Dialog.Bartender3,
                    Message = "Вы открываете кошелёк и понимаете, что у вас не хватит. Всё, что у вас есть с собой - это 20 " +
                              "кредитов и 4000 рублей.",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Попросить скидку",
                            MoveToDialog = Dialog.Bartender4
                        },
                    }
                },
                new DialogQuestion {
                    Name = Dialog.Bartender4,
                    Message = "_Бармен_: я сделаю вам скидку в 5 кредитов, если сможете отгадать загадку: \n" +
                              "Приходят в бар Михалков, Алексей Герман, Дарен Аранофски и начинают заказывать " +
                              "отсылочки к библии. Михалков заказал гвоздь в ладонь и 5 хлебов. Герман " +
                              "заказал предательство друга и брошенный камень, а Аранофски поправил венок и достал айфон. " +
                              "У бармена от такого случился апокалипсис, пиво стало полынью и чуть не умер брат.\n" +
                              "*Вопрос*: сколько отсылочек к библии было заказано?\n" +
                              "_(ответьте сообщением)_",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "1",
                            MoveToDialog = Dialog.Bartender7
                        },
                        new DialogAnswer {
                            Message = "2",
                            MoveToDialog = Dialog.Bartender7
                        },
                        new DialogAnswer {
                            Message = "3",
                            MoveToDialog = Dialog.Bartender7
                        },
                        new DialogAnswer {
                            Message = "4",
                            MoveToDialog = Dialog.Bartender5
                        },
                        new DialogAnswer {
                            Message = "5",
                            MoveToDialog = Dialog.Bartender6
                        },
                        new DialogAnswer {
                            Message = "6",
                            MoveToDialog = Dialog.Bartender6
                        },
                        new DialogAnswer {
                            Message = "7",
                            MoveToDialog = Dialog.Bartender6
                        },
                        new DialogAnswer {
                            Message = "8",
                            MoveToDialog = Dialog.Bartender6
                        },
                        new DialogAnswer {
                            Message = "9",
                            MoveToDialog = Dialog.Bartender6
                        },
                        new DialogAnswer {
                            Message = "10",
                            MoveToDialog = Dialog.Bartender6
                        },
                        new DialogAnswer {
                            Message = "11",
                            MoveToDialog = Dialog.Bartender6
                        },
                    }
                },
                new DialogQuestion {
                    Name = Dialog.Bartender5,
                    Message = "_Бармен_: Точно, 4! Заказано было ровно столько. Остальные отсылочки в загадку добавил её автор" +
                              ", потому что он так видит. Теперь я готов продать вам пиво за *25 кредитов*.",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Ещё скидку",
                            MoveToDialog = Dialog.Bartender8
                        },
                    }
                },
                new DialogQuestion {
                    Name = Dialog.Bartender6,
                    Message = "_Бармен_: Что-то многовато...",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "<<<",
                            MoveToDialog = Dialog.Bartender4
                        },
                    }
                },
                new DialogQuestion {
                    Name = Dialog.Bartender7,
                    Message = "_Бармен_: Хм, всё таки побольше",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "<<<",
                            MoveToDialog = Dialog.Bartender4
                        },
                    }
                },
                new DialogQuestion {
                    Name = Dialog.Bartender8,
                    Message = "_Бармен_: тогда ещё одна загадка. Приходит бармен на работу устраиваться. Какой первый вопрос ему задают " +
                              "на собеседовании?",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Есть медкнижка?",
                            MoveToDialog = Dialog.Bartender10
                        },
                        new DialogAnswer {
                            Message = "Посчитать сдачу",
                            MoveToDialog = Dialog.Bartender10
                        },
                        new DialogAnswer {
                            Message = "i++ + ++i",
                            MoveToDialog = Dialog.Bartender9
                        },
                        new DialogAnswer {
                            Message = "Образование?",
                            MoveToDialog = Dialog.Bartender10
                        },
                    }
                },
                new DialogQuestion {
                    Name = Dialog.Bartender9,
                    Message = "_Бармен_: Ну естественно! Этот вопрос идеален на любом собеседовании. " +
                              "Хорошо, моё последнее предложение: *20 кредитов* за две кружки пива.",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Беру",
                            ChangeJournal = j => j.Finish(Quest.BuyBeer),
                            ChangeInventory = i => i.Give(Item.Beer),
                            MoveToDialog = Dialog.MapNastya,
                            MoveToPos = (pos, map) => map.PosUp(MapIcon.Bartender)
                        },
                    }
                },
                new DialogQuestion {
                    Name = Dialog.Bartender10,
                    Message = "_Бармен_: Нет, это вряд ли.",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "<<<",
                            MoveToDialog = Dialog.Bartender8
                        },
                    }
                },
            };

            var genichDialogs = new[] {
                new DialogQuestion {
                    Name = Dialog.Genich1,
                    Message = "Перед вами Женич. Как водится, пьян и весел.",
                    Answers = new[] {
                        new DialogAnswer {
                            Available = (i, j) => i.Has(Item.Beer),
                            Message = "\ud83c\udf7b Дать пиво",
                            ChangeInventory = i => i.Take(Item.Beer),
                            MoveToDialog = Dialog.Genich7
                        },
                        new DialogAnswer {
                            Message = "Поговорить",
                            Available = (i, j) => j.IsOpen(Quest.Police),
                            MoveToDialog = Dialog.Genich2,
                        },
                        new DialogAnswer {
                            Message = "Слушать",
                            Available = (i, j) => j.IsFinished(Quest.Police),
                            MoveToDialog = Dialog.GenichPolice1,
                        },
                        new DialogAnswer {
                            Message = "Уйти",
                            MoveToDialog = Dialog.MapNastya,
                            MoveToPos = (pos, map) => map.PosDown(MapIcon.Genich)
                        },
                    },
                    MapIcon = MapIcon.Genich,
                    DisplayMap = true
                },
                new DialogQuestion {
                    Name = Dialog.Genich2,
                    Message = "_Женич_: Оооо, какие люди! А я тут как раз думаю что на концерте Триагрутрики по-любому " +
                              "тебя встречу. Тут ещё Лёнька где-то ходит. Мы с ним офигенно затусили с понедельника, а " +
                              "сегодня какой день недели вообще? Мы тут просто тусовались ваще ваще ваще просто уууу...",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Слушать ещё 5 минут",
                            MoveToDialog = Dialog.Genich3
                        },
                    }
                },
                new DialogQuestion {
                    Name = Dialog.Genich3,
                    Message = "_Женич_: ...дак вот в фифе сейчас карточки золотые сделали, так что приходится фармить их " +
                              "по квестам. Ага, только читеры сразу замутили себе по 100 этих карт и теперь вообще играть " +
                              "нереально. Я, считай, за две недели фарма открыл только Кержакова и Онопко. Да и то они " +
                              "из сундуков выпали. А за Weekend лигу выпал бриллиантовый Месси. Ну вот это да, это, конечно " +
                              "круто было!..",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Слушать ещё 20 минут",
                            MoveToDialog = Dialog.Genich4
                        },
                    }
                },
                new DialogQuestion {
                    Name = Dialog.Genich4,
                    Message = "_Женич_: ...и у нас на складе сейчас Антона, считай, повысили. А кого на его место поставить? " +
                              "Ну ясно понятно сестру Карягиной. Одно слово, каряга. Её как поставили у нас пол смены хотели " +
                              "заявление написать. Тоху сразу все зауважали. Ну там были за ним косяки тоже, но как Палну начальником " +
                              "сделали, у всех сразу премии порезали. А палет-то меньше не становится. У нас вообще, 6-я линия открылась...",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Слушать",
                            MoveToDialog = Dialog.Genich5
                        },
                        new DialogAnswer {
                            Message = "Перебить",
                            MoveToDialog = Dialog.Genich6
                        },
                    }
                },
                new DialogQuestion {
                    Name = Dialog.Genich5,
                    Message = "_Женич_: ...Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed in placerat justo. " +
                              "Mauris et neque sit amet dolor interdum hendrerit eu nec purus. Nam suscipit fermentum vestibulum. " +
                              "Nam nec mi sem. Duis vel interdum justo. Curabitur sollicitudin fermentum lectus sit amet tempus. " +
                              "Vivamus rhoncus et mauris ut posuere. Donec tempus sodales diam, nec vulputate augue vestibulum et. " +
                              "Pellentesque interdum orci at neque sodales dictum. Suspendisse tempus leo nec euismod placerat. " +
                              "Etiam ultrices laoreet lacinia....",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Перебить",
                            MoveToDialog = Dialog.Genich6
                        },
                    }
                },
                new DialogQuestion {
                    Name = Dialog.Genich6,
                    Message = "Вы просите Женича помочь как-то отвлечь полицейского.\n" +
                              "_Женич_: да без проблем! Только так дела не делаются. Давай ты нам *пивка* купишь, мы красиво-душевно " +
                              "поговорим и что-нибудь придумаем.",
                    Answers = new[] {
                        new DialogAnswer {
                            Available = (i, j) => i.Has(Item.Beer),
                            Message = "\ud83c\udf7b Дать пиво",
                            ChangeInventory = i => i.Take(Item.Beer),
                            MoveToDialog = Dialog.Genich7
                        },
                        new DialogAnswer {
                            Message = "Уйти",
                            ChangeJournal = j => j.Open(Quest.BuyBeer),
                            MoveToDialog = Dialog.MapNastya,
                            MoveToPos = (pos, map) => map.PosDown(MapIcon.Genich)
                        },
                    }
                },
                new DialogQuestion {
                    Name = Dialog.Genich7,
                    Message = "_Женич_: Во, вот это красиво!\n" +
                              "Он забирает у вас оба стакана и отправляется к полицейскому.",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Уйти",
                            MoveToDialog = Dialog.MapNastya,
                            ChangeJournal = j => j.Finish(Quest.Police),
                            MoveToPos = (pos, map) => map.PosLeft(map.PosLeft(startPosition)),
                            ChangeMap = (pos, map) => map
                                .Replace(map.IndexOf(MapIcon.Genich), MapIcon.Empty)
                                .Replace(map.PosLeft(MapIcon.Policeman), MapIcon.Genich)
                        },
                    }
                },
                new DialogQuestion {
                    Name = Dialog.GenichPolice1,
                    Message = "_Женич_: Вы вот, товарищ полицейский, понимаете сути своих обвинений? Я выдвигаю только те " +
                              "обвинения, за которые могу предъявить, показать доказательтва. А вы мне тут обвиняете того, " +
                              "чего не сможете доказать. Давайте вот лучше пивка пивнём душевно в душу...",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Прокрасться мимо",
                            MoveToDialog = Dialog.GenichPolice2,
                            MoveToPos = (pos, map) => map.PosRight(MapIcon.Policeman)
                        },
                    }
                },
                new DialogQuestion {
                    Name = Dialog.GenichPolice2,
                    Message = "Пока полицейского отвлекли, вам удаётся незаметно выскочить на улицу",
                    Answers = mapDialog.Answers,
                    DisplayMap = true,
                }
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
                    startDialog,
                    mapDialog,
                    inventoryDialog,
                    journalDialog
                }.Concat(policeDialogs)
                .Concat(crowdDialogs)
                .Concat(genichDialogs)
                .Concat(bartenderDialogs)
                .Concat(repaDialogs)
                .Concat(randomDialogs)
                .ToArray();
        }
    }
}
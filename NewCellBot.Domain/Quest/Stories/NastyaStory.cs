using System.Linq;
using NewCellBot.Domain.Quest.Model;

namespace Bot.Quests
{
    public static class NastyaStory
    {
        public static string Map =
            @"
%%%%%%%%%=====
%%%%%%%%%=====
%%TCc--%%z-R%%
%%TCcN-P----%%
%%TCc--%%%%%%%
%%%%%%%%%%%%%%
%%%%%%%%%%%%%%
";

        public static DialogQuestion[] GetDialogs()
        {
            var initialMap = NewCellQuest.Map;
            var startPosition = initialMap.IndexOf(MapIcon.Nastya);

            var mapDialog = new DialogQuestion
            {
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

            var startDialog = new DialogQuestion
            {
                Name = Dialog.StartNastya,
                Message = "Концерт Триагрутрики только что закончился. Вы, конечно, не могли его пропустить, " +
                          "даже в день свадьбы. В любом случае это не проблема, надо только как-то такси заказать, " +
                          "а телефон сел. Кажется, на улице был таксофон.",
                Answers = mapDialog.Answers,
                DisplayMap = true,
            };

            var inventoryDialog = new DialogQuestion
            {
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

            var journalDialog = new DialogQuestion
            {
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
                        + RenderQuest(Quest.FindCredits, "Достать 20 кредитов")
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
                            Message = "Метнуться",
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
                            MoveToPos = (pos, map) => map.PosRight(MapIcon.Policeman),
                            ChangeJournal = j => j.Open(Quest.OrderTaxi)
                        },
                    }
                },
                new DialogQuestion {
                    Name = Dialog.GenichPolice2,
                    Message = "Пока полицейского отвлекли, вам удаётся незаметно выскочить на улицу. " +
                              "Начало свадьбы через 30 минут, надо бы вызвать такси, но телефон всё так же сел.",
                    Answers = mapDialog.Answers,
                    DisplayMap = true,
                }
            };

            var taxofonDialogs = new[] {
                new DialogQuestion {
                    Name = Dialog.Taxofon1,
                    Message = "_Таксофон:_ Здравствуйте! Спасибо, что позвонили в нашу таксомоторную компанию! " +
                              "Ваш звонок очень важен для нас. Оставайтесь на линии, первый освободившийся " +
                              "оператор примет ваш звонок. Примерное время ожидания 28 минут.",
                    Answers = new [] {
                        new DialogAnswer {
                            Available = (i, j) => j.IsOpen(Quest.OrderTaxi),
                            Message = "Ждать",
                            MoveToDialog = Dialog.Taxofon2,
                        },
                        new DialogAnswer {
                            Available = (i, j) => j.IsOpen(Quest.OrderTaxi),
                            Message = "Нетерпеливо топнуть ножкой",
                            MoveToDialog = Dialog.Taxofon2,
                        },
                        new DialogAnswer {
                            Available = (i, j) => i.Has(Item.PhoneNumber),
                            Message = "Набрать корешу",
                            MoveToDialog = Dialog.Taxofon9,
                        },
                        new DialogAnswer {
                            Available = (i, j) => j.IsFinished(Quest.OrderTaxi),
                            Message = "Уже не нужно",
                            MoveToDialog = Dialog.MapNastya,
                            MoveToPos = (pos, map) => map.PosDown(MapIcon.Taxofon)
                        }
                    }
                },
                new DialogQuestion {
                    Name = Dialog.Taxofon2,
                    Message = "_Таксофон:_ Примерное время ожидания 27 минут.",
                    Answers = new [] {
                        new DialogAnswer {
                            Message = "Ждать",
                            MoveToDialog = Dialog.Taxofon3,
                        },
                        new DialogAnswer {
                            Message = "Пнуть",
                            MoveToDialog = Dialog.Taxofon6,
                        }
                    }
                },
                new DialogQuestion {
                    Name = Dialog.Taxofon3,
                    Message = "_Таксофон:_ Примерное время ожидания 26 минут.",
                    Answers = new [] {
                        new DialogAnswer {
                            Message = "Ждать",
                            MoveToDialog = Dialog.Taxofon4,
                        },
                        new DialogAnswer {
                            Message = "Пнуть",
                            MoveToDialog = Dialog.Taxofon6,
                        }
                    }
                },
                new DialogQuestion {
                    Name = Dialog.Taxofon4,
                    Message = "_Таксофон:_ Примерное время ожидания 25 минут.",
                    Answers = new [] {
                        new DialogAnswer {
                            Message = "Ждать",
                            MoveToDialog = Dialog.Taxofon5,
                        },
                        new DialogAnswer {
                            Message = "Пнуть",
                            MoveToDialog = Dialog.Taxofon6,
                        }
                    }
                },
                new DialogQuestion {
                    Name = Dialog.Taxofon5,
                    Message = "_Таксофон:_ Пшшшшшш... [звонок сорвался]",
                    Answers = new [] {
                        new DialogAnswer {
                            Message = "Набрать снова",
                            MoveToDialog = Dialog.Taxofon1,
                        }
                    }
                },
                new DialogQuestion {
                    Name = Dialog.Taxofon6,
                    Message = "_Таксофон:_ Здравствуйте! Трансгалактическая таксомоторная компания рада " +
                              "приветствовать вас! Нами была зафиксирована вспышка ярости при попытке вызвать " +
                              "такси. Не нужно свирепствовать, трансгалактическая таксомоторная компания всегда " +
                              "придёт на помощь!",
                    Answers = new [] {
                        new DialogAnswer {
                            Message = "Заказать такси",
                            MoveToDialog = Dialog.Taxofon7,
                        }
                    }
                },
                new DialogQuestion {
                    Name = Dialog.Taxofon7,
                    Message = "_Таксофон:_ Водитель приедет в ближайшее время! С вас 20 галактических кредитов.",
                    Answers = new [] {
                        new DialogAnswer {
                            Message = "У меня только рубли",
                            MoveToDialog = Dialog.Taxofon8,
                            ChangeJournal = j => j.Open(Quest.FindCredits),
                            MoveToPos = (pos, map) => map.PosDown(MapIcon.Taxofon)
                        }
                    }
                },
                new DialogQuestion {
                    Name = Dialog.Taxofon8,
                    Message = "_Таксофон:_ Что ж, извините, всего хорошего.",
                    Answers = mapDialog.Answers,
                    DisplayMap = true
                },
                new DialogQuestion {
                    Name = Dialog.Taxofon9,
                    Message = "_Таксофон:_ Дорогой вы мой человечек, категорически вас приветствую! Если вам " +
                              "необходимо произвести обмен рублей на галактические кредиты, то вы обратились " +
                              "по нужному адресу! Ведь так много зависит от правильного выбора надёждного поставщика " +
                              "финансовых услуг. Оставайтесь на месте, наш человечек уже выдвинулся в Вашу сторону " +
                              "чтобы порешать вопросик!",
                    Answers = new [] {
                        new DialogAnswer {
                            Message = "Окей...",
                            MoveToDialog = Dialog.MapNastya,
                            MoveToPos = (pos, map) => map.PosDown(MapIcon.Taxofon),
                            ChangeMap = (pos, map) => map
                                .Replace(map.PosUp(map.PosRight(MapIcon.Taxofon)), MapIcon.Okusheva)
                        }
                    }
                },
            };

            var repaDialogs = new[] {
                new DialogQuestion {
                    Name = Dialog.Repa1,
                    Message = "_Репа_: Привет, Настён!",
                    Answers = new[] {
                        new DialogAnswer {
                            Message = "Есть 20 кредитов?",
                            Available = (i, j) => j.IsOpen(Quest.FindCredits) && !i.Has(Item.PhoneNumber),
                            MoveToDialog = Dialog.Repa2,
                            MoveToPos = (pos, map) => map.PosLeft(MapIcon.Repa)
                        },
                        new DialogAnswer {
                            Message = "Уйти",
                            MoveToDialog = Dialog.MapNastya,
                            MoveToPos = (pos, map) => map.PosLeft(MapIcon.Repa)
                        },
                    },
                    MapIcon = MapIcon.Repa,
                    DisplayMap = true
                },
                new DialogQuestion {
                    Name = Dialog.Repa2,
                    Message = "_Репа_: 20 Галактических кредитов до ЗАГСа?! Офигеть! Нет, у меня нет налика. " +
                              "Но если у тебя есть немного рублей, то у меня есть кент, готовый их превратить " +
                              "в некоторое количество кредитов. *Набери ему на цифры*, он подскочет, обкашляете.",
                    Answers = new [] {
                        new DialogAnswer {
                            Message = "Спасибо!",
                            ChangeInventory = i => i.Give(Item.PhoneNumber),
                            MoveToDialog = Dialog.MapNastya,
                            MoveToPos = (pos, map) => map.PosLeft(MapIcon.Repa)
                        }
                    }
                },
            };

            var okushevaDialogs = new[] {
                new DialogQuestion {
                    Name = Dialog.Okusheva1,
                    Message = "_Ксения Окушева_: Добрейший вечерочек! Приветствуем вас в нашей " +
                              "компании \"Бизнес зрелость\", меня зовут Ксения Окушева! Мы очень " +
                              "рады за вас, что вы смогли выбрать именно того финансового партнера, " +
                              "который научит Вас правильно распоряжаться свобоными средствами, поможет " +
                              "вам создать финансовую подушку безопасности и вселит уверенность в " +
                              "завтрашнем дне.",
                    Answers = new [] {
                        new DialogAnswer {
                            Message = "Распросить подробнеe",
                            MoveToDialog = Dialog.Okusheva2,
                        },
                        new DialogAnswer {
                            Message = "Спросить про кредиты",
                            MoveToDialog = Dialog.Okusheva2,
                        },
                    },
                    DisplayMap = true,
                    MapIcon = MapIcon.Okusheva
                },
                new DialogQuestion {
                    Name = Dialog.Okusheva2,
                    Message = "Наша компания насчитывает уже более чем двухсотлетнюю историю и зарекомендавала " +
                              "себя на рынке как настоящий мастодонт среди прочих финансовых компаний. Мы оказываем " +
                              "весь спектр финансовых услуг вплоть до конверсии рубей в галактические кредиты. Для начала " +
                              "мы рекомендуем вам записаться на личную финансовую консультацию к Андрею Сергеевичу, нашему " +
                              "лучшему и самому опытному эксперту. С ним вы разберете своё финансовое положение в реальной " +
                              "жизни \"под микроскопом\", определите точку \"А\", внедрите в жизнь инструменты по эффективному " +
                              "управлению и формированию капиталом. С ним вы научитесь ставить финансовые цели, выстраивать " +
                              "стратегии и идти к их реализации без срывов и страхов.",
                    Answers = new [] {
                        new DialogAnswer {
                            Message = "Насчет кредитов...",
                            MoveToDialog = Dialog.Okusheva3,
                        },
                    },
                },
                new DialogQuestion {
                    Name = Dialog.Okusheva3,
                    Message = "Мы вам можем предложить лучшие условия на рынке! Именно наша компания имеет " +
                              "прямой доступ к галактической бирже. Кстати о биржах, предлагаем вам пройти " +
                              "персональную школу трейдеров \"Греби лопатой\" и тогда вы сможете начать зарабатывать " +
                              "в среднем по 200% в минуту. Сейчас созвонимся с Аркадием Леонидовичем и узнаем когда он " +
                              "свободен, чтобы он показал вам азы биржевого дела.",
                    Answers = new [] {
                        new DialogAnswer {
                            Message = "ГАЛАКТИЧЕСКИЕ КРЕДИТЫ МНЕ",
                            MoveToDialog = Dialog.Okusheva4,
                        },
                    },
                },
                new DialogQuestion {
                    Name = Dialog.Okusheva4,
                    Message = "Хорошо, что ж вы сразу не сказали что Вам необходимо купить кредиты? У нас самый " +
                              "лучший курс 1:200. Если вам нужно 20 кредитов, с вас 8000 рублей.",
                    Answers = new [] {
                        new DialogAnswer {
                            Message = "Что там про 200%/мин?",
                            MoveToDialog = Dialog.Okusheva5,
                        },
                    },
                },
                new DialogQuestion {
                    Name = Dialog.Okusheva5,
                    Message = "Смотрите, здесь всё очень просто. Сейчас я вам всё покажу в моменте. " +
                              "Сегодня хорошим спросом пользуются собачьи ценные бумаги, предлагаем рассмотреть варианты:\n" +
                              "1. Купить акции Коляна\n" +
                              "2. Купить фьючерсы Репы\n" +
                              "3. Купить опцион на Володю\n" +
                              "4. Вложиться в облигационный ПИФ \"Яков&Сократ\"",
                    Answers = new [] {
                        new DialogAnswer {
                            Message = "1",
                            MoveToDialog = Dialog.Okusheva6,
                        },
                        new DialogAnswer {
                            Message = "2",
                            MoveToDialog = Dialog.Okusheva7,
                        },
                        new DialogAnswer {
                            Message = "3",
                            MoveToDialog = Dialog.Okusheva9,
                        },
                        new DialogAnswer {
                            Message = "4",
                            MoveToDialog = Dialog.Okusheva8,
                        },
                    },
                },
                new DialogQuestion {
                    Name = Dialog.Okusheva6,
                    Message = "Поздравляем! Ваши вложения за минуту упали на 50%",
                    Answers = new [] {
                        new DialogAnswer {
                            Message = "Трейдить дальше",
                            MoveToDialog = Dialog.Okusheva5,
                        },
                    },
                },
                new DialogQuestion {
                    Name = Dialog.Okusheva7,
                    Message = "Поздравляем! Ваши вложения обесценились за минуту на 25%",
                    Answers = new [] {
                        new DialogAnswer {
                            Message = "Трейдить дальше",
                            MoveToDialog = Dialog.Okusheva5,
                        },
                    },
                },
                new DialogQuestion {
                    Name = Dialog.Okusheva8,
                    Message = "Поздравляем! Ваши сбережения увеличились на 0,07% за минуту",
                    Answers = new [] {
                        new DialogAnswer {
                            Message = "Трейдить дальше",
                            MoveToDialog = Dialog.Okusheva5,
                        },
                    },
                },
                new DialogQuestion {
                    Name = Dialog.Okusheva9,
                    Message = "Поздравляем! Опцион Володи купленный за 150 исполняется за 8000",
                    Answers = new [] {
                        new DialogAnswer {
                            Message = "Обменять на кредиты!",
                            MoveToDialog = Dialog.Okusheva10,
                        },
                    },
                },
                new DialogQuestion {
                    Name = Dialog.Okusheva10,
                    Message = "О! Я смотрю вы быстро разобрались, Вам точно надо посетить нашу школу трейдеров. " +
                              "Завтра у нас будет проходить семинар в отеле \"Гранд Будапешт\", вам обязательно " +
                              "надо его постетить. Держите ваши *20 галактический кредитов* До встречи!",
                    Answers = new [] {
                        new DialogAnswer {
                            Message = "Прощайте",
                            MoveToDialog = Dialog.MapNastya,
                            MoveToPos = (pos, map) => map.PosDown(pos),
                            ChangeJournal = j => j.Finish(Quest.FindCredits),
                            ChangeMap = (pos, map) => map.Replace(MapIcon.Okusheva, MapIcon.Empty)
                        },
                    },
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
                .Concat(taxofonDialogs)
                .Concat(okushevaDialogs)
                .Concat(repaDialogs)
                .Concat(randomDialogs)
                .ToArray();
        }
    }
}
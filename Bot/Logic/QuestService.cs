using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bot
{
    public class PlayerValidationResult
    {
        public bool CanPlay { get; set; }
        public string Reason { get; set; }
    }

    public class QuestState
    {
        public int Pos { get; set; }
        public string Map { get; set; }
        public Inventory Inventory { get; set; }
        public Journal Journal { get; set; }
        public string OpenDialogName { get; set; }
    }

    public class QuestService
    {
        public QuestState State;

        private string Map => State.Map;
        private int Pos => State.Pos;
        private Inventory Inventory => State.Inventory;
        private Journal Journal => State.Journal;
        public DialogQuestion OpenDialog { get; set; }
        public Dictionary<string, DialogQuestion> Dialogs { get; set; }
        public Dictionary<char, DialogQuestion> DialogsByMapIcons { get; set; }

        public PlayerValidationResult CanPlay(string player)
        {
            if (OpenDialog.ForPlayer != null
                && (player == null || !OpenDialog.ForPlayer.Contains(player))) {
                return new PlayerValidationResult {
                    CanPlay = false,
                    Reason = "Сейчас ход " + OpenDialog.ForPlayer.Split(';').FirstOrDefault()
                };
            }

            return new PlayerValidationResult { CanPlay = true };
        }

        public VisibleState ProcessAnswer(string answer)
        {
            var dialogAnswer = OpenDialog.Answers
                .Where(a => a.Available(Inventory, Journal))
                .FirstOrDefault(a => a.Message.SameAs(answer));
            if (dialogAnswer != null) {
                dialogAnswer.ChangeInventory?.Invoke(Inventory);
                dialogAnswer.ChangeJournal?.Invoke(Journal);
                if (dialogAnswer.MoveToDialog != null) {
                    OpenDialog = Dialogs[dialogAnswer.MoveToDialog];
                    State.OpenDialogName = OpenDialog.Name;
                }

                if (dialogAnswer.ChangeMap != null) {
                    State.Map = dialogAnswer.ChangeMap(Pos, Map);
                }

                if (dialogAnswer.MoveToPos != null) {
                    var newPos = dialogAnswer.MoveToPos(Pos, Map);
                    Move(newPos);
                }
            }

            return GetVisibleState();
        }

        private VisibleState GetVisibleState()
        {
            return new VisibleState {
                Message = GetVisibleMap() + (OpenDialog.Message ?? OpenDialog.DynamicMessage?.Invoke(Inventory, Journal) ?? ""),
                Answers = OpenDialog.Answers
                    .Where(a => a.Available(Inventory, Journal))
                    .Where(a => !a.IsHidden)
                    .Select(a => a.Message)
                    .ToArray(),
                Photo = OpenDialog.Photo,
            };
        }

        private void Move(int newPos)
        {
            var newPosObject = Map[newPos];
            if (DialogsByMapIcons.ContainsKey(newPosObject)) {
                OpenDialog = DialogsByMapIcons[newPosObject];
                State.OpenDialogName = OpenDialog.Name;
            }

            if (!OpenDialog.PreventMove) {
                State.Pos = newPos;
            }
        }

        private void ClearCell()
        {
            var newMap = new StringBuilder(Map) {
                [Pos] = MapIcon.Empty
            };
            State.Map = newMap.ToString();
        }

        private string GetVisibleMap() => !OpenDialog.DisplayMap
            ? ""
            : (Inventory.Has(Item.Glasses) ? GetVisibleMapBig() : GetVisibleMapSmall());

        // 5 x 5 vision range
        private string GetVisibleMapBig()
        {
            var up = Map.PosUp(Pos);
            var up2 = Map.PosUp(up);
            var down = Map.PosDown(Pos);
            var down2 = Map.PosDown(down);
            var visibleMap = new StringBuilder();
            visibleMap.AppendLine(Map.FetchSymbols(up2 - 2, up2 - 1, up2, up2 + 1, up2 + 2));
            visibleMap.AppendLine(Map.FetchSymbols(up - 2, up - 1, up, up + 1, up + 2));
            visibleMap.Append(Map.FetchSymbols(Pos - 2, Pos - 1));
            visibleMap.Append(MapIcon.Self);
            visibleMap.AppendLine(Map.FetchSymbols(Pos + 1, Pos + 2));
            visibleMap.AppendLine(Map.FetchSymbols(down - 2, down - 1, down, down + 1, down + 2));
            visibleMap.AppendLine(Map.FetchSymbols(down2 - 2, down2 - 1, down2, down2 + 1, down2 + 2));
            return visibleMap.ToString().ToSmileAll();
        }

        // 3 x 3 vision range
        private string GetVisibleMapSmall()
        {
            var up = Map.PosUp(Pos);
            var down = Map.PosDown(Pos);
            var visibleMap = new StringBuilder();
            visibleMap.AppendLine(Map.FetchSymbols(up - 1, up, up + 1));
            visibleMap.Append(Map.FetchSymbols(Pos - 1));
            visibleMap.Append(MapIcon.Self);
            visibleMap.AppendLine(Map.FetchSymbols(Pos + 1));
            visibleMap.AppendLine(Map.FetchSymbols(down - 1, down, down + 1));
            return visibleMap.ToString().ToSmileAll();
        }

        public QuestService(string map, Inventory inventory, Journal journal, DialogQuestion[] dialogs)
        {
            Dialogs = dialogs.ToDictionary(d => d.Name);
            DialogsByMapIcons = dialogs.Where(d => d.MapIcon.HasValue).ToDictionary(d => d.MapIcon.Value);
            OpenDialog = dialogs.First();
            State = new QuestState {
                Map = map,
                Inventory = inventory,
                Journal = journal,
                Pos = map.IndexOf(MapIcon.Self),
                OpenDialogName = OpenDialog.Name
            };
            ClearCell();
        }

        public QuestService(DialogQuestion[] dialogs, QuestState state)
        {
            Dialogs = dialogs.ToDictionary(d => d.Name);
            DialogsByMapIcons = dialogs.Where(d => d.MapIcon.HasValue).ToDictionary(d => d.MapIcon.Value);
            OpenDialog = Dialogs[state.OpenDialogName];
            this.State = state;
        }
    }
}
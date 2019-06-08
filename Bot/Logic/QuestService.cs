using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bot
{
    public class QuestService
    {
        private int Pos { get; set; }
        private DialogQuestion OpenDialog { get; set; }

        private string Map { get; set; }
        private Inventory Inventory { get; }
        private Dictionary<string, DialogQuestion> Dialogs { get; }
        private Dictionary<char, DialogQuestion> DialogsByMapIcons { get; }

        public VisibleState ProcessAnswer(string answer)
        {
            var dialogAnswer = OpenDialog.Answers.Where(a => a.Available(Inventory))
                .FirstOrDefault(a => a.Message.SameAs(answer));
            if (dialogAnswer != null) {
                dialogAnswer.ChangeInventory?.Invoke(Inventory);
                if (dialogAnswer.MoveToDialog != null) {
                    OpenDialog = Dialogs[dialogAnswer.MoveToDialog];
                }

                if (dialogAnswer.ClearMapCell) {
                    ClearCell();
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
                Message = GetVisibleMap() + (OpenDialog.Message ?? ""),
                Answers = OpenDialog.Answers.Where(a => a.Available(Inventory)).Where(a => !a.IsHidden)
                    .Select(a => a.Message).ToArray(),
                Photo = OpenDialog.Photo,
            };
        }

        private void Move(int newPos)
        {
            var newPosObject = Map[newPos];
            if (DialogsByMapIcons.ContainsKey(newPosObject)) {
                OpenDialog = DialogsByMapIcons[newPosObject];
            }

            if (!OpenDialog.PreventMove) {
                Pos = newPos;
            }
        }

        private void ClearCell()
        {
            var newMap = new StringBuilder(Map) {
                [Pos] = MapIcon.Empty
            };
            Map = newMap.ToString();
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

        private void SetStartingPosition()
        {
            Pos = Map.IndexOf('O');
            ClearCell();
        }

        public QuestService(string map, Inventory inventory, DialogQuestion[] dialogs)
        {
            Map = map;
            Dialogs = dialogs.ToDictionary(d => d.Name);
            DialogsByMapIcons = dialogs.Where(d => d.MapIcon.HasValue).ToDictionary(d => d.MapIcon.Value);
            OpenDialog = dialogs.First();
            Inventory = inventory;
            SetStartingPosition();
        }
    }
}
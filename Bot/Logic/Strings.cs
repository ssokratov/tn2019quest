namespace Bot
{
    public static class MapButtons
    {
        public const char Left = 'l';
        public const char Up = 't';
        public const char Right = 'r';
        public const char Down = 'd';
        public const char Inventory = 'i';
        public const char Journal = 'j';
    }

    public static class MapIcon
    {
        public const char Toshik = 'O';
        public const char Nastya = 'N';
        public const char WallToshik = '#';
        public const char WallNastya = '%';
        public const char Fear = '~';
        public const char Empty = '-';
        public const char Flame = 'F';

        public const char SmallDoor = 'п';
        public const char FireExtinguisher = 'f';
        public const char Boots = 'b';

        public const char Repa = 'R';
        public const char Kolyan = 'k';
        public const char KolyanDacha = 'U';
        public const char KolyanDachaKey = '?';
        public const char Sedosh = 'K';
        public const char Sokrat = 'S';
        public const char Jacob = 'J';
        public const char ZagsWorker = 'Z';
        public const char Genich = 'G';
        public const char Bartender = 'B';
        public const char Policeman = 'P';
        public const char Crowd0 = 'c';
        public const char Crowd1 = 'C';
        public const char Crowd2 = 'T';
        public const char Road = '=';

        public const char KolyanDachaDoor = 'D';
        public const char StartDoors = 'П';
        public const char Veil = 'v';
        public const char Glasses = 'M';
    }

    public static class Item
    {
        public const string Glasses = nameof(Glasses);
        public const string FireExtinguisher = nameof(FireExtinguisher);
        public const string Boots = nameof(Boots);
        
        public const string Stick = nameof(Stick);
        public const string Hat = nameof(Hat);

        public const string PhoneNumber = nameof(PhoneNumber);
        public const string Phone = nameof(Phone);
        public const string Project = nameof(Project);

        public const string KolyanDachaKey = nameof(KolyanDachaKey);
        public const string Beer = nameof(Beer);
    }

    public static class Quest
    {
        public const string EnterHall = nameof(EnterHall);
        public const string AskForWedding = nameof(AskForWedding);
        public const string DressForWedding = nameof(DressForWedding);
        public const string FireAlarm = nameof(FireAlarm);
        public const string Sokrat = nameof(Sokrat);
        public const string Demianov = nameof(Demianov);
        public const string Jacob = nameof(Jacob);
        public const string Sedosh = nameof(Sedosh);
        public const string Kolyan = nameof(Kolyan);
        public const string KolyanDachaOpenDoor = nameof(KolyanDachaOpenDoor);

        public const string Police = nameof(Police);
        public const string BuyBeer = nameof(BuyBeer);
        public const string OrderTaxi = nameof(OrderTaxi);
    }

    public static class Dialog
    {
        public const string StartDoors1 = nameof(StartDoors1);
        public const string StartDoors2 = nameof(StartDoors2);
        public const string StartDoors3 = nameof(StartDoors3);
        public const string StartDoors4 = nameof(StartDoors4);

        public const string Veil1 = nameof(Veil1);
        public const string Veil2 = nameof(Veil2);
        public const string Veil3 = nameof(Veil3);

        public static string EnteredHall = nameof(EnteredHall);

        public const string Jacob1 = nameof(Jacob1);
        public const string Jacob2 = nameof(Jacob2);
        public const string Jacob3 = nameof(Jacob3);
        public const string Jacob4 = nameof(Jacob4);
        public const string Jacob5 = nameof(Jacob5);
        public const string Police1 = nameof(Police1);
        public const string Police2 = nameof(Police2);
        public const string Police3 = nameof(Police3);
        public const string Crowd0 = nameof(Crowd0);
        public const string Crowd1 = nameof(Crowd1);
        public const string Crowd2 = nameof(Crowd2);
        public const string Bartender1 = nameof(Bartender1);
        public const string Bartender2 = nameof(Bartender2);
        public const string Bartender3 = nameof(Bartender3);
        public const string Bartender4 = nameof(Bartender4);
        public const string Bartender5 = nameof(Bartender5);
        public const string Bartender6 = nameof(Bartender6);
        public const string Bartender7 = nameof(Bartender7);
        public const string Bartender8 = nameof(Bartender8);
        public const string Bartender9 = nameof(Bartender9);
        public const string Bartender10 = nameof(Bartender10);
        public const string Bartender11 = nameof(Bartender11);
        public const string Genich1 = nameof(Genich1);
        public const string Genich2 = nameof(Genich2);
        public const string Genich3 = nameof(Genich3);
        public const string Genich4 = nameof(Genich4);
        public const string Genich5 = nameof(Genich5);
        public const string Genich6 = nameof(Genich6);
        public const string Genich7 = nameof(Genich7);
        public const string Genich8 = nameof(Genich8);
        public const string GenichPolice1 = nameof(GenichPolice1);
        public const string GenichPolice2 = nameof(GenichPolice2);
        public const string GenichPolice3 = nameof(GenichPolice3);
        public const string Repa1 = nameof(Repa1);
        public const string Repa2 = nameof(Repa2);
        public const string Kolyan1 = nameof(Kolyan1);
        public const string Kolyan2 = nameof(Kolyan2);
        public const string Kolyan3 = nameof(Kolyan3);
        public const string Kolyan4 = nameof(Kolyan4);
        public const string Kolyan5 = nameof(Kolyan5);
        public const string Kolyan6 = nameof(Kolyan6);
        public const string KolyanDacha1 = nameof(KolyanDacha1);
        public const string KolyanDacha2 = nameof(KolyanDacha2);
        public const string KolyanDacha3 = nameof(KolyanDacha3);
        public const string KolyanDacha4 = nameof(KolyanDacha4);
        public const string KolyanDacha5 = nameof(KolyanDacha5);
        public const string KolyanDacha6 = nameof(KolyanDacha6);
        public const string KolyanDacha7 = nameof(KolyanDacha7);
        public const string KolyanDacha8 = nameof(KolyanDacha8);
        public const string Sedosh1 = nameof(Sedosh1);
        public const string Sedosh2 = nameof(Sedosh2);
        public const string Sedosh3 = nameof(Sedosh3);
        public const string Demianov1 = nameof(Demianov1);
        public const string Demianov2 = nameof(Demianov2);
        public const string Demianov3 = nameof(Demianov3);
        public const string Demianov4 = nameof(Demianov4);
        public const string Sokrat1 = nameof(Sokrat1);
        public const string Sokrat2 = nameof(Sokrat2);
        public const string Sokrat3 = nameof(Sokrat3);
        public const string Sokrat4 = nameof(Sokrat4);
        public const string Sokrat5 = nameof(Sokrat5);
        public const string ZagsWorker1 = nameof(ZagsWorker1);
        public const string ZagsWorker2 = nameof(ZagsWorker2);
        public const string ZagsWorker3 = nameof(ZagsWorker3);
        public const string ZagsEnd = nameof(ZagsEnd);
 
        public const string Glasses1 = nameof(Glasses1);
        public const string Glasses2 = nameof(Glasses2);

        public const string Boots1 = nameof(Boots1);
        public const string Boots2 = nameof(Boots2);

        public const string Flame1 = nameof(Flame1);
        public const string Flame2 = nameof(Flame2);
        public const string SmallDoor = nameof(SmallDoor);
        public const string FireExtinguisher = nameof(FireExtinguisher);

        public const string StartToshik = nameof(StartToshik);
        public const string MapToshik = nameof(MapToshik);
        public const string MapMoveTo = nameof(MapMoveTo);
        public const string InventoryToshik = nameof(InventoryToshik);
        public const string JournalToshik = nameof(JournalToshik);
        public const string FoundWallToshik = nameof(FoundWallToshik);
        public const string FoundFear = nameof(FoundFear);
        public const string FoundRoad = nameof(FoundRoad);

        public const string StartNastya = nameof(StartNastya);
        public const string MapNastya = nameof(MapNastya);
        public const string InventoryNastya = nameof(InventoryNastya);
        public const string JournalNastya = nameof(JournalNastya);
        public const string FoundWallNastya = nameof(FoundWallNastya);
    }

}

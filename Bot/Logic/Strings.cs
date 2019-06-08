namespace Bot
{
    public static class Directions
    {
        public const char Left = 'l';
        public const char Up = 't';
        public const char Right = 'r';
        public const char Down = 'd';
    }

    public static class MapIcon
    {
        public const char Self = 'O';
        public const char Wall = '#';
        public const char Fear = '~';
        public const char Empty = '-';

        public const char Repa = 'R';
        public const char Sedosh = 'K';
        public const char Sokrat = 'S';
        public const char Jacob = 'J';
        public const char ZagsWorker = 'Z';

        public const char StartDoors = 'П';
        public const char Veil = 'v';
        public const char Glasses = '8';
    }

    public static class Item
    {
        public const string Glasses = nameof(Glasses);
        public const string Veil = nameof(Veil);
        public const string Stick = nameof(Stick);

        public const string PhoneNumber = nameof(PhoneNumber);
        public const string PhoneRequest = nameof(PhoneRequest);
        public const string Phone = nameof(Phone);
        public const string ProjectRequest = nameof(ProjectRequest);
        public const string Project = nameof(Project);
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
        public const string Repa1 = nameof(Repa1);
        public const string Repa2 = nameof(Repa2);
        public const string Sedosh1 = nameof(Sedosh1);
        public const string Sedosh2 = nameof(Sedosh2);
        public const string Sedosh3 = nameof(Sedosh3);
        public const string Demianov1 = nameof(Demianov1);
        public const string Demianov2 = nameof(Demianov2);
        public const string Demianov3 = nameof(Demianov3);
        public const string Sokrat1 = nameof(Sokrat1);
        public const string ZagsWorker1 = nameof(ZagsWorker1);
 
        public const string Glasses_1 = nameof(Glasses_1);
        public const string Glasses_2 = nameof(Glasses_2);

        public const string FoundWall = nameof(FoundWall);
        public const string FoundFear = nameof(FoundFear);

        public const string Map = nameof(Map);
        public const string MapMoveTo = nameof(MapMoveTo);
    }

}

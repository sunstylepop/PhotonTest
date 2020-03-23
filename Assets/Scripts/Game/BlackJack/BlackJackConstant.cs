using ExitGames.Client.Photon;

namespace Assets.Scripts
{
    class BlackJackConstant
    {

    }

    class BlackJackRoomProperty
    {
        //C0 - C9固定屬性, 不能更改

        /// <summary> 房間等級 </summary>
        public const string LEVEL = "C0";
        /// <summary> 所需金額 </summary>
        public const string MIN_COST = "C1";

        public static Hashtable LowRoom = new Hashtable() {
            { MIN_COST, 0},
            { LEVEL, (int)RoomLevel.Low}
        };

        public static Hashtable MidRoom = new Hashtable() {
            { MIN_COST, 50},
            { LEVEL, (int)RoomLevel.Mid}
        };

        public static Hashtable HighRoom = new Hashtable() {
            { MIN_COST, 100},
            { LEVEL, (int)RoomLevel.High}
        };
    }

}

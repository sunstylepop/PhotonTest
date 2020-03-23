
namespace Assets.Scripts
{
    public class PlayerInfo
    {
        /// <summary>
        /// 遊戲位置
        /// </summary>
        public int Location { get; set; }

        /// <summary>
        /// server端入房遊戲編號
        /// </summary>
        public int ActorNr { get; set; }

        /// <summary>
        /// 暱稱
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// 基本手牌
        /// </summary>
        public int[] BaseCards { get; set; }

        /// <summary>
        /// 額外手牌
        /// </summary>
        public int[] ExtraCards { get; set; }
    }

    public class PersonalRoundEvent
    {
        public int Location { get; set; }
        public int TimeOut { get; set; }
    }

    public class GetCardEvent
    {
        public int Location { get; set; }
        public int[] ExtraCard { get; set; }
    }

    public class PassEvent
    {
        public int Location { get; set; }
    }
}

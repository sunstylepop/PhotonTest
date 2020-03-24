
using System;

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

        public int TotalPoint{
            get
            {
                int _point = 0;

                Action<int> AddPoint = (p) => { _point += (p + 1) > 10 ? 10 : (p + 1); };

                if(BaseCards != null && BaseCards.Length > 0)
                {
                    foreach (var p in BaseCards)
                        AddPoint(p);
                }

                if (ExtraCards != null && ExtraCards.Length > 0)
                {
                    foreach (var p in ExtraCards)
                        AddPoint(p);
                }
                return _point;
            }
        }
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

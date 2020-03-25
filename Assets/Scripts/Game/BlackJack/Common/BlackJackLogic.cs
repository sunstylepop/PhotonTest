using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Game.BlackJack.Common
{
    class BlackJackLogic
    {

        public static (int, BlackJackCardType) CalculatResult(int[] BaseCards, int[] ExtraCards)
        {
            int _point = 0;
            int _countCards = 0;

            if(ExtraCards == null || ExtraCards.Length == 0)
            {
                var IsBlackJack = BaseCards.Any(x => x % 13 == 0) && BaseCards.Any(x => x % 13 >= 10);
                if(IsBlackJack)
                    return (21, BlackJackCardType.BlackJack);
            }

            //A先當1點計算

            Action<int> AddPoint = (p) => {
                var _cp = (p % 13) + 1;
                _point += _cp > 10 ? 10 : _cp; 
            };

            if (BaseCards != null && BaseCards.Length > 0)
            {
                _countCards += BaseCards.Length;
                foreach (var p in BaseCards)
                    AddPoint(p);
            }

            if (ExtraCards != null && ExtraCards.Length > 0)
            {
                _countCards += ExtraCards.Length;
                foreach (var p in ExtraCards)
                    AddPoint(p);
            }

            if (_point <= 21 && _countCards == 5)
            {
                return (_point, BlackJackCardType.FiveCard);
            }

            if(_point > 21)
            {
                return (_point, BlackJackCardType.OverTwentyOne);
            }

            return (_point, BlackJackCardType.None);
        }
    }
}

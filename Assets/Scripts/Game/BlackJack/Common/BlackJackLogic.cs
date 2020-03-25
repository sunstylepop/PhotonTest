using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Game.BlackJack.Common
{
    class BlackJackLogic
    {

        public static (int, bool) CalculatResult(int[] BaseCards, int[] ExtraCards)
        {
            int _point = 0;
            int _countCards = 0;

            if(BaseCards != null && BaseCards.Length == 2)
            {
                var IsBlackJack = BaseCards.Any(x => x % 13 == 0) && BaseCards.Any(x => x % 13 >= 10);
                return (21, true);
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
                return (_point, true);
            }

            return (_point, false);
        }
    }
}

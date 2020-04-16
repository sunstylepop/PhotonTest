
namespace Assets.Scripts
{
    public class BlackJackRoom
    {
        public string GameCode => "BlackJack";

        public int ID { get; set; }

        public string Name { get; set; }

        public int Position { set; get; }

        public int Bet { get; set; }

        public int RequirMoney { get; set; }
    }
}

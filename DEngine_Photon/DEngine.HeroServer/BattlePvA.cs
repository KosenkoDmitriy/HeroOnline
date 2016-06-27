using DEngine.Common.GameLogic;

//AI opponent mode
namespace DEngine.HeroServer
{
    public class BattlePvA : BattleService
    {
        public override BattleMode Mode { get { return BattleMode.RandomPvA; } }

        public BattlePvA()
        {
        }

        protected override void OnBattleEnd(int result, BattleCode code = BattleCode.Start)
        {
            base.OnBattleEnd(result, code);
            if (GameUser1.GamePlay != null)
            {
                if (result < 0)
                    GameUser1.GamePlay.OnItemDrop();

                GameUser1.GamePlay.OnBattleEnd(this, result);

            }
        }
    }
}

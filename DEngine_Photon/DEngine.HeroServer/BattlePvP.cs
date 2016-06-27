using DEngine.Common.Config;
using DEngine.Common.GameLogic;
using UnityEngine;

namespace DEngine.HeroServer
{
    public class BattlePvP : BattleService
    {
        public override BattleMode Mode { get { return BattleMode.RandomPvP; } }

        public BattlePvP()
        {
        }

        //calculated for rewards when end game!

        protected override void OnBattleEnd(int result, BattleCode code = BattleCode.Start)
        {
            base.OnBattleEnd(result, code);

            int levelDif = GameUser2.Base.Level - GameUser1.Base.Level;
            int winSilver = GameConfig.ARENAWINSILVER;
            int lostSilver = GameConfig.ARENALOSTSILVER;

            GameAward gameAward1 = new GameAward();
            GameAward gameAward2 = new GameAward();

            if (result < 0) // GameUser1 win
            {
                gameAward1.Honor = (100 + levelDif * 5);
                gameAward2.Honor = -(50 + levelDif * 2 / 5);
                GameUser1.Base.TotalWon += 1;
                GameUser2.Base.TotalLost += 1;
                
                GameUser1.AddCash(winSilver, 0);
                GameUser2.AddCash(-lostSilver, 0);
            }
            else if (result == 0)
            {
                gameAward1.Honor = (20 + 2 * levelDif);
                gameAward2.Honor = (20 - 2 * levelDif);
                GameUser1.Base.TotalDraw += 1;
                GameUser2.Base.TotalDraw += 1;
            }
            else
            {
                gameAward2.Honor = (100 + levelDif * 5);
                gameAward1.Honor = -(50 + levelDif * 2 / 5);
                GameUser1.Base.TotalLost += 1;
                GameUser2.Base.TotalWon += 1;

                GameUser2.AddCash(winSilver, 0);
                GameUser1.AddCash(-lostSilver, 0);
            }

            gameAward1.Honor = Mathf.Clamp(gameAward1.Honor, -100, 200);
            gameAward2.Honor = Mathf.Clamp(gameAward2.Honor, -100, 200);
                
            GameUser1.AddHonor(gameAward1.Honor);
            Zone.SendUserSync(GameUser1, gameAward1);

            GameUser2.AddHonor(gameAward2.Honor);
            Zone.SendUserSync(GameUser2, gameAward2);

            Zone.HeroDatabase.AddArenaLog(GameUser1, GameUser2, result, gameAward1.Honor, gameAward2.Honor);
        }
    }
}

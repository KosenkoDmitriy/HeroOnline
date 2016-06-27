using System.Collections.Generic;

namespace DEngine.Common.GameLogic
{
    public class GameAward
    {
        public int UserExp;
        public int RoleExp;
        public int Silver;
        public int Gold;
        public int Honor;
        public int SilverEx;

        public Dictionary<int, int> Items = new Dictionary<int, int>();
        public Dictionary<int, int> Roles = new Dictionary<int, int>();

        public bool IsEmpty
        {
            get { return (UserExp == 0 && RoleExp == 0 && Silver == 0 && Gold == 0 && Items.Count == 0 && Roles.Count == 0); }
        }

        public void Clear()
        {
            UserExp = 0;
            RoleExp = 0;
            Silver = 0;
            Gold = 0;
            Items.Clear();
            Roles.Clear();
            SilverEx = 0;
        }

        public void Clone(GameAward rData)
        {
            UserExp = rData.UserExp;
            RoleExp = rData.RoleExp;
            Silver = rData.Silver;
            Gold = rData.Gold;
        }
    }
}

using DEngine.Common.GameLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEngine.HeroServer
{
    public abstract class ItemScript
    {
        public GameUser GameUser { get; set; }

        public abstract bool OnUse(UserRole userRole);
    }
}

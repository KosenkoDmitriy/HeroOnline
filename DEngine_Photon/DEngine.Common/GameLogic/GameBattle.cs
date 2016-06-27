using System.Collections.Generic;

namespace DEngine.Common.GameLogic
{
    public class GameBattle : IDataSerializable
    {
        public BattleMode Mode;
        public GameUser User01;
        public GameUser User02;

        public void Serialize(BinSerializer serializer)
        {
            if (serializer.Mode == SerializerMode.Write)
                serializer.Writer.Write((int)Mode);
            else
                Mode = (BattleMode)serializer.Reader.ReadInt32();

            serializer.Serialize(ref User01);
            serializer.Serialize(ref User02);
        }
    }
}

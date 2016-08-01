using System;

namespace DEngine.Common.GameLogic
{
    public static class Global
    {
        public static readonly Random Random;

        public static GameObjCollection GameRoles;
        public static GameObjCollection GameItems;
        public static GameObjCollection GameSkills;
        public static GameObjCollection ChargeShop;

        public static GameObjList GameHeroes;
        public static GameObjList GameMobs;
        public static GameObjList[] TopUsers;

        public static string key = "";

        static Global()
        {
            Random = new Random();
            GameRoles = new GameObjCollection();
            GameItems = new GameObjCollection();
            GameSkills = new GameObjCollection();
            ChargeShop = new GameObjCollection();

            GameHeroes = new GameObjList();
            GameMobs = new GameObjList();

            TopUsers = new GameObjList[5];
            for (int i = 0; i < 5; i++)
                TopUsers[i] = new GameObjList() { Tag = DateTime.Today.AddDays(-1) };
        }
    }
}

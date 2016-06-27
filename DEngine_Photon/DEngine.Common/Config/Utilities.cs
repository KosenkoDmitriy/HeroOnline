using System;

namespace DEngine.Common.Config
{
    public static class Utilities
    {
        public static readonly Random Random;

        public static readonly string[] CsvSpliter;

        static Utilities()
        {
            Random = new Random();
            CsvSpliter = new string[] { "\",\"" };
        }

        public static int[] GetArrayInt(string input, char split = ';')
        {
            string[] allFields = input.Split(split);

            int[] outArray = new int[allFields.Length];
            for (int i = 0; i < allFields.Length; i++)
            {
                int value;
                if (Int32.TryParse(allFields[i], out value))
                    outArray[i] = value;
            }

            return outArray;
        }

        public static float[] GetArrayFloat(string input, char split = ';')
        {
            string[] allFields = input.Split(split);

            float[] outArray = new float[allFields.Length];
            for (int i = 0; i < allFields.Length; i++)
            {
                float value;
                if (Single.TryParse(allFields[i], out value))
                    outArray[i] = value;
            }

            return outArray;
        }
    }
}

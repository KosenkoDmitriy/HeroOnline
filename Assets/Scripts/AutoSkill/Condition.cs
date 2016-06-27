using UnityEngine;
using System.Collections;

public class Condition  {

    public enum ConditionType
    {
        None,   //0
        Equal,  //1
        Greater,    //2
        Less,       //3
        LessOrEqual,  //4
        GreaterOrEqual   //5
    }

    public static bool Compair(float value1, ConditionType type, float value2)
    {
        switch (type)
        {
            case ConditionType.Equal: return value1 == value2;
            case ConditionType.Greater: return value1 > value2;
            case ConditionType.Less: return value1 < value2;
            case ConditionType.LessOrEqual: return value1 <= value2;
            case ConditionType.GreaterOrEqual: return value1 >= value2;
            default: return false;
        }
    }
}

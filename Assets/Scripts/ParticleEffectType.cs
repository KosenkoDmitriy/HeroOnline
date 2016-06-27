using UnityEngine;
using System.Collections;


public enum EffectInitType
{
    None,
    AllPlayer,
    CurrentPlayer,
    AtPosZero
}
public class ParticleEffectType : MonoBehaviour {

    public EffectInitType initType= EffectInitType.None;

}

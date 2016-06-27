using UnityEngine;
using System.Collections;

public interface IState
{

    //public Controller controller { get; set; }

    void OnEnter();
    void OnUpdate();
    void OnExit();
}

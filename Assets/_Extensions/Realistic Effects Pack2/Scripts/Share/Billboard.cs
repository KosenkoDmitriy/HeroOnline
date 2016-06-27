using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour
{
  public Camera Camera;
  public bool Active = true;
  public bool AutoInitCamera = true;

  private GameObject myContainer;
  private Transform t, camT;

  private void Awake()
  {
    if (AutoInitCamera) {
      Camera = Camera.main;
      Active = true;
    }

    t = transform;
    camT = Camera.transform;
    var parent = t.root;
    myContainer = new GameObject {name = "Billboard_" + t.gameObject.name};
    myContainer.transform.position = t.position;
    t.parent = myContainer.transform;
    myContainer.transform.parent = parent;
  }

  private void Update()
  {
    if (Active)
      myContainer.transform.LookAt(myContainer.transform.position + camT.rotation * Vector3.back, camT.rotation * Vector3.up);
  }
}
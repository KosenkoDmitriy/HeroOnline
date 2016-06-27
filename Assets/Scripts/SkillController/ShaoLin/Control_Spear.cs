using UnityEngine;
using System.Collections;

public class Control_Spear : MonoBehaviour {
    public GameObject spear;
    public int nSpear=3;
    public Vector3 Radius;
    public float AngleSpear=270;
    public float Speed = 0.5f;
    private Vector3 To;
    private GameObject simpleSpear;
    private GameObject[] listSpear;

    //
    private float Angle, AngleCount;

    private GameObject CreateSimpleSpear()
    {
        GameObject _spear=new GameObject();
        _spear.name = "Spimple Spear";
        GameObject _chiSpear = Instantiate(spear, Radius, _spear.transform.rotation) as GameObject;
      
 
        _chiSpear.transform.position = Radius;
        _chiSpear.transform.Rotate(AngleSpear, 0, 0);
        _chiSpear.transform.parent = _spear.transform;
        return _spear;
    }
    private void CreateSpear()
    {
        simpleSpear = CreateSimpleSpear();
        Angle = 360 / nSpear;
        listSpear = new GameObject[nSpear];
        for (int i = 0; i < nSpear; i++)
        {
            listSpear[i] = Instantiate(simpleSpear, transform.position, transform.rotation) as GameObject;
            AngleCount += Angle;
            listSpear[i].transform.parent = gameObject.transform;
            listSpear[i].transform.Rotate(0, AngleCount, 0);

        }
        Destroy(simpleSpear);
    }
	// Use this for initialization
	void Start () {
        transform.position = new Vector3(0, -10, 0);
        To = Vector3.zero;
        CreateSpear();
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(0, Speed, 0);
        transform.localPosition = Vector3.Slerp(transform.localPosition, To, 0.1f);
	}
}

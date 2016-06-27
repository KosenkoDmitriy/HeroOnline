/// <summary>
/// This script use for control a touch controller
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class TouchController : MonoBehaviour
{

    public int layerTouch; //layer ground
    public float distaceRay; //distance raycast
    public Camera cameraTarget; //camera layer cast
    public GameObject circle_Ring_Pref; //circle on bottom of character
    public GameObject circle_Touch_Pref; //circle when drag to target

    //Variable private field 
    private Controller controller;
    public LineRenderer lineRenderer;
    private bool readyDrag;
    private Ray ray;
    private RaycastHit hit;
    private Touch touch;
    private Vector3 radiausPos;
    [HideInInspector]
    public Vector3 posCircleStart;
    [HideInInspector]
    public Vector3 posCircleEnd;

    public GameObject transformRootStart { get; set; }
    public GameObject transformLookStart { get; set; }
    public GameObject transformRootEnd { get; set; }
    public GameObject transformLookEnd { get; set; }
    public GameObject circleRingStart { get; set; }
    public GameObject circleRingEnd { get; set; }
    public GameObject circleTouch { get; set; }

    [HideInInspector]
    public Controller controllerGetSkill;

    public static TouchController instance;

    public GameplayManager gamePlayManager;

    public UIHeroSlotController selectedSlot;

    public bool clickOnImage;

    [HideInInspector]
    public Controller oldContoller;
    [HideInInspector]
    public Controller oldEnemyContoller;

  //  [HideInInspector]
  //  public Enemy oldEnemy;

    private enum StatInput
    {
        Down, Move, Up
    }

    void Start()
    {
        instance = this;
        lineRenderer = GetComponent<LineRenderer>();
        clickOnImage = false;
    }

    void Update()
    {
        if (GameplayManager.battleStatus != GameplayManager.BattleStatus.Playing)//UICamera.hoveredObject != null ||
        {
            if (GameManager.tutorial.step != TutorialManager.TutorialStep.Control_NPCFinshed)
                return;
        }

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            InputTouch();
        }
        else
        {
            InpuMouse();
        }

        
    }
    public void SetTouchController(Controller controllerSelected)
    {
        controller = controllerSelected;
        controllerGetSkill = controller;
        OnSelectedPlayer(controller);
    }

    void InputTouch()
    {
        int touchIndex = Input.touchCount;
        for (int i = 0; i < touchIndex; i++)
        {
            touch = Input.GetTouch(i);
            if (touch.phase == TouchPhase.Began)
            {
                DoRayCast(touch.position, StatInput.Down);
            }

            if (touch.phase == TouchPhase.Moved)
            {
                DoRayCast(Input.mousePosition, StatInput.Move);
            }

            if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
            {
                DoRayCast(Input.mousePosition, StatInput.Up);
            }
        }
    }

    void InpuMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (controller == null || controllerGetSkill == null) 
                ClearCircle();
            DoRayCast(Input.mousePosition, StatInput.Down);
        }

        if (Input.GetMouseButton(0))
        {
            if (controller == null || controllerGetSkill == null)
                ClearCircle();
            DoRayCast(Input.mousePosition, StatInput.Move);
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (controller == null || controllerGetSkill == null)
                ClearCircle();
            DoRayCast(Input.mousePosition, StatInput.Up);
        }
    }

    private IEnumerator HideSelectedEnemy(Controller curControler)
    {
        yield return new WaitForSeconds(0.5f);

        curControler.OnDeSelected();
    }

    private void DoRayCast(Vector3 position, StatInput statInput)
    {

        ray = cameraTarget.ScreenPointToRay(position);

        RaycastHit[] hits = Physics.RaycastAll(ray, distaceRay, 1 << layerTouch);
        if (hits.Length > 0)
        {
            RaycastHit firstEnemy = new RaycastHit();
            bool bfirstEnemy = false;

            foreach (RaycastHit curHit in hits)
            {
                if (curHit.collider.tag == GameManager.PlayerTagName && statInput == StatInput.Down)
                {

                    hit = curHit;
                    bfirstEnemy = false;

                    if (curHit.collider.GetComponent<Controller>().roleType != RoleType.Building)
                    {
                        break;
                    }
                }
                else if (!bfirstEnemy && curHit.collider.tag == GameManager.EnemyTagName)
                {
                    firstEnemy = curHit;
                    bfirstEnemy = true;
                }
                else
                {
                    hit = curHit;
                }
            }

            if (bfirstEnemy == true)
                hit = firstEnemy;

            switch (statInput)
            {
                case StatInput.Down:
                    {
                        if (hit.collider.tag == GameManager.PlayerTagName)
                        {
                            clickOnImage = false;
                            controller = hit.collider.GetComponent<Controller>();
                            if (controller.roleType == RoleType.Building || controller.role.Base.Type == DEngine.Common.GameLogic.RoleType.Hostage)
                            {
                                controller = null;
                                return;
                            }
                            OnSelectedPlayer(controller);
                            OnMouseDown(controller);

                        }                      
                    }
                    break;

                case StatInput.Move:
                    {
                        
                        if (controller != null)
                        {
                            controller.target = null;
                            if (controller.actionStat == Controller.ActionStat.Dead)
                            {
                                return;
                            }

                            if (clickOnImage) return;

                            lineRenderer.enabled = true;
                            if (transformRootStart == null || hit.transform == null || controller == null || transformLookStart==null) return;
                            Vector3 pointStart = CirclePoint(transformRootStart.transform, transformLookStart.transform, controller.transform.position, hit.point);
                            posCircleStart.x = hit.point.x;
                            posCircleStart.z = hit.point.z;
                            if (hit.collider.tag == GameManager.EnemyTagName)
                            {
                                if (controller.typeCharacter != Controller.TypeChatacter.Healer)
                                    OnMouseOverTarget(pointStart, hit.collider.gameObject);

                                Controller enemyController = hit.collider.gameObject.GetComponent<Controller>();
                                if (enemyController != null)
                                {
                                    OnOverEnemy(enemyController);
                                }
                            }
                            else if (hit.collider.tag == GameManager.PlayerTagName && controller != hit.collider.GetComponent<Controller>())
                            {
                                if (controller.typeCharacter == Controller.TypeChatacter.Healer)
                                    OnMouseOverTarget(pointStart, hit.collider.gameObject);
                            }
                            else
                            {
                                OnExitEnemy();

                                posCircleEnd.x = 1000;
                                posCircleEnd.z = 1000;
                                circleTouch.transform.position = posCircleStart;
                                circleRingEnd.transform.position = posCircleEnd;
                                //controller.positionWay = hit.point;
                                lineRenderer.SetPosition(0, new Vector3(pointStart.x, 0.05f, pointStart.z));
                                lineRenderer.SetPosition(1, new Vector3(hit.point.x, 0.05f, hit.point.z));

                            }
                        }
                        else
                        {
                            controller = null;
                            lineRenderer.SetPosition(1, Vector3.zero);
                            lineRenderer.enabled = false;
                            if (transformRootStart != null)
                                Destroy(transformRootStart.gameObject);
                            if (transformRootEnd != null)
                                Destroy(transformRootEnd.gameObject);
                            if (transformLookEnd != null)
                                Destroy(transformLookEnd.gameObject);
                            Destroy(circleRingEnd);
                            Destroy(circleTouch);
                        }
                    }
                    break;

                case StatInput.Up:
                    {

                        if (controller != null)
                        {
                            if (clickOnImage == true)
                            {
                                return;
                            }
                            if (controller.actionStat == Controller.ActionStat.Dead)
                            {
                                return;
                            }


                            if (hit.collider.tag != GameManager.PlayerTagName && hit.collider.tag != GameManager.EnemyTagName)
                            {
                                //Debug.Log("OnMove");

                                if (GameManager.tutorial.step == TutorialManager.TutorialStep.Control_NPCFinshed && UIBattleManager.Instance.heroFirst != null
                                    && UIBattleManager.Instance.tutStep < UIBattleManager.TutorialStepForBattle.AutoSkill )
                                {
                                    if (controllerGetSkill == UIBattleManager.Instance.heroFirst.controller)
                                    {
                                        controller.OnMove(hit.point);
                                        UIBattleManager.Instance.OnMoveHero();
                                    }
                                }
                                else
                                {
                                    controller.OnMove(hit.point);
                                }
                            }
                            else
                            {

                                if (controller.gameObject != hit.collider.gameObject)
                                {
                                    if (controller.typeCharacter == Controller.TypeChatacter.Healer)
                                    {
                                        if (hit.collider.tag == controller.tag)
                                            controller.OnAction(hit.collider.gameObject);
                                    }
                                    else
                                    {
                                        if (hit.collider.tag != controller.tag)
                                            controller.OnAction(hit.collider.gameObject);
                                    }

                                    UIBattleManager.Instance.OnFinishedTargetToMonster();

                                }
                            }


                            controller = null;

                            lineRenderer.SetPosition(1, Vector3.zero);
                            lineRenderer.enabled = false;
                            if (transformRootStart != null)
                                Destroy(transformRootStart.gameObject);
                            if (transformRootEnd != null)
                                Destroy(transformRootEnd.gameObject);
                            if (transformLookEnd != null)
                                Destroy(transformLookEnd.gameObject);
                            if (circleRingEnd != null)
                                Destroy(circleRingEnd);
                            if (circleTouch != null)
                                Destroy(circleTouch);
                            if (selectedSlot != null)
                                selectedSlot.isSlected = false;
                        }
                    }
                    break;
            }
        }
    }
      
    public void OnMouseOverTarget(Vector3 pointStart, GameObject targetObject)
    {
        controller.target = targetObject;
      //  controller.positionWay = controller.target.transform.position;
        posCircleEnd.x = controller.target.transform.position.x;
        posCircleEnd.z = controller.target.transform.position.z;
        circleTouch.transform.position = posCircleEnd;
        circleRingEnd.transform.position = posCircleEnd;
        Vector3 pointEnd = CirclePoint(transformRootEnd.transform, transformLookEnd.transform, controller.target.transform.position, controller.transform.position);
        lineRenderer.SetPosition(0, new Vector3(pointStart.x, 0.05f, pointStart.z));
        lineRenderer.SetPosition(1, new Vector3(pointEnd.x, 0.05f, pointEnd.z));
    }

    public void OnMouseDown(Controller selectController)
    {
        if (selectController == null) return;
        controller = selectController;

        if (controller.actionStat != Controller.ActionStat.Dead)
        {
          //  controller.actionStat = Controller.ActionStat.Idle;
        }
        else
        {
            return;
        }
        controllerGetSkill = controller;

        UIBattleManager.Instance.OnTouchHeroFinish();

        transformRootStart = new GameObject("Root_Start");
        transformLookStart = new GameObject("Emtry_Start");
        transformRootEnd = new GameObject("Root_End");
        transformLookEnd = new GameObject("Emtry_End");
        posCircleStart.x = controller.transform.position.x;
        posCircleStart.y = 0.05f;
        posCircleStart.z = controller.transform.position.z;
        if (circleRingStart == null)
        {
            if (circleRingStart != null) Destroy(circleRingStart);
            circleRingStart = Instantiate(circle_Ring_Pref, posCircleStart, Quaternion.identity) as GameObject;
            circleRingStart.transform.parent = controller.transform;
        }
        else
        {
            circleRingStart.transform.parent = controller.transform;
            posCircleStart.x = 0;
            posCircleStart.z = 0;
            circleRingStart.transform.localPosition = posCircleStart;
        }
        posCircleStart.x = hit.point.x;
        posCircleStart.z = hit.point.z;
        if (circleTouch != null) Destroy(circleTouch);
        circleTouch = Instantiate(circle_Touch_Pref, posCircleStart, Quaternion.identity) as GameObject;
        posCircleEnd.x = 1000;
        posCircleEnd.y = 0.05f;
        posCircleEnd.z = 1000;
        if (circleRingEnd != null) Destroy(circleRingEnd);
        circleRingEnd = Instantiate(circle_Ring_Pref, posCircleEnd, Quaternion.identity) as GameObject;
    }

    public Vector3 CirclePoint(Transform transformRoot, Transform transformLook, Vector3 center, Vector3 lookAtPos)
    {
        transformRoot.transform.position = center;
        transformLook.transform.parent = transformRoot.transform;
        transformLook.transform.localPosition = new Vector3(0, 0, 0.45f);
        transformRoot.transform.LookAt(lookAtPos);
        return transformLook.transform.position;
    }

    public void OnSelectedPlayer(Controller curController)
    {
        if (oldContoller != null)
        {
            oldContoller.OnDeSelected();
        }
        oldContoller = curController;
        oldContoller.OnSelected();
    }
      
    public void OnOverEnemy(Controller curController)
    {
        
        if (oldEnemyContoller != null)
        {
            if (oldEnemyContoller.name == curController.name) return;
            oldEnemyContoller.OnDeSelected();
        }
        oldEnemyContoller = curController;
        oldEnemyContoller.OnSelected();
    }
    public void OnExitEnemy()
    {
        if (oldEnemyContoller != null)
        {
            oldEnemyContoller.OnDeSelected();
            oldEnemyContoller = null;
        }

      /*  if (oldEnemy != null)
        {
            oldEnemy.OnDeSelected();
            oldEnemy = null;
        }   */  
    }

    private void ClearCircle()
    {
        controller = null;
        lineRenderer.SetPosition(1, Vector3.zero);
        lineRenderer.enabled = false;
        if (transformRootStart != null)
            Destroy(transformRootStart.gameObject);
        if (transformRootEnd != null)
            Destroy(transformRootEnd.gameObject);
        if (transformLookEnd != null)
            Destroy(transformLookEnd.gameObject);
        Destroy(circleRingEnd);
        Destroy(circleTouch);
    }
}

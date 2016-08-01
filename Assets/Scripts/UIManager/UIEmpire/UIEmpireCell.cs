using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DEngine.Common.Config;

public class UIEmpireCell : MonoBehaviour {

    public enum State
    {
        Lock,
        Free,
        Building,
        Obtacle,
    };

    public Image border;
    public Image cone;

    public Vector2 position;

    private State _state;
    private UIEmpireMap _mapManager;
    private Color _oldColor;
    private float timerClick;
    private static List<UIEmpireCell> cellsCheckBuilding = new List<UIEmpireCell>();
    private Vector3 _dragPos;
    private Vector3 _dropPos;
    private bool _isDrag;

    public void Init(UIEmpireMap manager, Vector2 pos, State state)
    {
        _isDrag = false;
        _mapManager = manager;
        position = pos;
        ChangeState(state);
    }

    #region Event
    public void OnClick()
    {
        if (UIEmpireManager.isFriend || _state != State.Lock) return;

        if (MessageBox.dialogSet.Count > 0) return;
        UINoticeManager.OnButtonOK_click += OnCorfirmOpenCell;
        MessageBox.ShowDialog(string.Format( GameManager.localization.GetText("Land_OpenCellConfirm"),LandConfig.OPEN_SILVER), UINoticeManager.NoticeType.YesNo);        
      

        //float dt = Time.time - timerClick;
        //if (dt < 0.5f)
        //{
        //_mapManager.OnClickCell(this);
        //}
        //else
        //    timerClick = Time.time;

        ResetMoveMap();
    }

    public void OnMouseOver()
    {
        if (UIEmpireManager.isFriend) return;
        _oldColor = cone.color;
        if (UIEmpireManager.Instance.HasDragBuilding())
        {
            _mapManager.OnMouseOverCellWithBuiding(this, UIEmpireManager.Instance.houseDataSelected);
        }
    }

    public void OnMouseExit()
    {
        if (UIEmpireManager.isFriend) return;
        _mapManager.CanNotBuilding();
        foreach (UIEmpireCell cell in cellsCheckBuilding)
        {
            cell.ChangeState(cell._state);
        }
        cellsCheckBuilding.Clear();
    }

    public void OnDragBegin()
    {
        _isDrag = true;
        _dragPos = Helper.GetCursorPos();
    }

    public void OnDragMove()
    {
        if (!_isDrag || UIEmpireManager.Instance.togCustomBuilding.isOn) return;

        _dropPos = Helper.GetCursorPos();
        Vector3 dir = (_dropPos - _dragPos);
        dir.z = 0;
        dir.Normalize();
        UIEmpireManager.Instance.MoveMap(dir);
    }

    public void OnDragEnd()
    {
        if (!_isDrag) return;

        //_dropPos = Helper.GetCursorPos();
        //Vector3 dir = (_dropPos - _dragPos);
        //dir.z = 0;       
        //UIEmpireManager.Instance.MoveMap(dir);
        ResetMoveMap();
    }
    #endregion

    #region public methods
    public void FinishedBuilding()
    {
        foreach (UIEmpireCell cell in cellsCheckBuilding)
        {
            cell.ChangeState(State.Building);
        }
        cellsCheckBuilding.Clear();
    }
    public bool CheckBuiding()
    {
        bool canBuild = false;
        cellsCheckBuilding.Add(this);
        if (_state == State.Free)
        {
            cone.color = Color.green;
            canBuild = true;
        }
        else
        {
            cone.color = Color.red;
            canBuild = false;
        }
        return canBuild;
    }
    public void Lock()
    {
        _state = State.Lock;
        cone.color = Color.black;
        _oldColor = cone.color;
    }
    public void UnLock()
    {
        _state = State.Free;
        cone.color = Color.white;
        _oldColor = cone.color;
    }
    public void StateBuilding()
    {
        Color c = Color.yellow;
        c.a = 1f;
        cone.color = c;
        _oldColor = cone.color;
    }
    #endregion

    #region private methods

    private void OnCorfirmOpenCell()
    {
        _mapManager.OnClickCell(this);
    }
    private void ResetMoveMap()
    {
        _dragPos = _dropPos;
        _isDrag = false;
    }

    public void ChangeState(State state)
    {
        cone.color = Color.white;
        switch (state)
        {
            case State.Lock: Lock(); break;
            case State.Free: UnLock(); break;
            case State.Building: StateBuilding(); break;
        }
        _state = state;
    }
    #endregion
}

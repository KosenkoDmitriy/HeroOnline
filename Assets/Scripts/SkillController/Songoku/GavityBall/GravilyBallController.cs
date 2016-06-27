using UnityEngine;
using System.Collections;
using DEngine.Common.GameLogic;

public enum TrangThai{
TaoKK,BanKK,No,nothing
}
public class GravilyBallController : MonoBehaviour {

    private Vector3 PointCreateKhinhKhi;
    private TrangThai trangThai;
    private float Height = 2;
    private float TimeCreateCount;

    public Transform Player;
    public Vector3 HitPoint;
    public GameObject KhinhKhi;
    public GameObject KhinhKhiBoom;
    public float TimeCreateKhinhKhi = 2;

    public Controller controllerAttack { get; set; }
    public int skillIndex { get; set; }
    public RoleSkill roleSkill;

    public float HightForce = 2;
    public float ForceAmount = 200;

	// Use this for initialization
	void Start () {
        trangThai = TrangThai.TaoKK;
        if (Player != null)
        {
            PointCreateKhinhKhi = Player.position;
        }
        else
        {
            Debug.Log("Skill_E_Sogoku: set player!!");
        }
        PointCreateKhinhKhi.y += Height;
        KhinhKhi = Instantiate(KhinhKhi, PointCreateKhinhKhi, transform.rotation) as GameObject;
        HitPoint = PointCreateKhinhKhi;

        if(controllerAttack!=null)
            if (controllerAttack.target != null)
            {
                HitPoint = controllerAttack.target.transform.position;
            }
	}

    private Vector3 GetCenterPointEnemy()
    {
        Vector3 centerPoint = Vector3.zero;
        string tagName = Helper.GetTagEnemy(controllerAttack);
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(tagName);
        foreach (GameObject enemy in enemies)
        {
            centerPoint += enemy.transform.position;
        }

        if (enemies.Length > 0)
            centerPoint /= enemies.Length;
        return centerPoint;
    }


    public void setTrangThai(TrangThai _trangThai)
    {
        trangThai = _trangThai;
    }
    public void setHitPoint(Vector3 _Point)
    {
        HitPoint = _Point;
    }
    private void UpdateTrangThai()
    {
        switch (trangThai)
        {
            case TrangThai.TaoKK:
               //s Debug.Log("tao boom");
                TimeCreateCount += Time.deltaTime;
                if (TimeCreateCount >= TimeCreateKhinhKhi)
                    trangThai = TrangThai.BanKK;
                    break;
            case TrangThai.BanKK:
                  //  Debug.Log("ban boom");
                KhinhKhi.transform.position = Vector3.MoveTowards(KhinhKhi.transform.position, HitPoint, 0.5f);
                if (Vector3.Distance(KhinhKhi.transform.position, HitPoint) <= 0.1f)
                {
                    trangThai = TrangThai.No;
                }
                break;
            case TrangThai.No:
                Destroy(KhinhKhi);
                Instantiate(KhinhKhiBoom, HitPoint, transform.rotation);
                Destroy(gameObject, 2);
                HandleSkillHit();
                trangThai = TrangThai.nothing;
                break;
            
        }
    }
	// Update is called once per frame
	void Update () {
        UpdateTrangThai();
	}

    private void HandleSkillHit()
    {
        float Range = roleSkill.GameSkill.EffectRange;
        SkillManager.Instance.CreateRangeOfSkill(HitPoint, Range);

        HeroSet heroSet = GameplayManager.Instance.getEnemySet(controllerAttack);

        if (controllerAttack != null)
            if (controllerAttack.target != null)
            {
                HitPoint = controllerAttack.target.transform.position;
            }

        SoundManager.Instance.PlaySkillEnd(roleSkill, controllerAttack);

        foreach (Hero hero in heroSet)
        {
            if (hero.gameObject != null)
            {
                float distance = Vector3.Distance(HitPoint, hero.gameObject.transform.position);
                if (distance <= Range)
                {
                    Vector3 addForce = (-hero.gameObject.transform.forward.normalized + Vector3.up * HightForce).normalized;
                    hero.controller.AddForce(addForce * ForceAmount);
                    GameplayManager.Instance.SendSkillHit(controllerAttack.role.Id, controllerAttack.index, hero.controller, roleSkill);
                }
                else
                {
                    Debug.Log("out");
                }
            }
        }
    }

}

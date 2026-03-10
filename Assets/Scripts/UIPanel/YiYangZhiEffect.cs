
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YiYangZhiEffect : SingleViewBase
{
    bool left;
    public Transform trans_content;
    public float speed;
    public Collider2D theCollider;
    PeopleData belong;
    Vector3 pos;
    public override void Init(params object[] args)
    {
        base.Init(args);
        left = (bool)args[0];
        pos = (Vector3)args[1];
        belong = args[2] as PeopleData;

    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        transform.position = pos;
        if (left)
        {
            trans_content.localEulerAngles = new Vector3(0, 180, 0);
            theCollider.offset = new Vector2(50, 0);
            speed = Mathf.Abs(speed) * -1;
        }
        else
        {
            speed = Mathf.Abs(speed);
            trans_content.localEulerAngles = Vector3.zero;
            theCollider.offset = new Vector2(-50, 0);

        }
    }

    private void FixedUpdate()
    {
        transform.localPosition = new Vector2(transform.localPosition.x + speed * Time.fixedDeltaTime, transform.localPosition.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BattlePeopleView battlePeopleView = collision.GetComponent<BattlePeopleView>();
        if (battlePeopleView != null)
        {
            ////打中了
            //if (battlePeopleView.PeopleData.OnlyId != belong.OnlyId)
            //{
            //    BattleManager.Instance.SingleAttack(belong, battlePeopleView.PeopleData);

            //    HitEffect hitEffect = PanelManager.Instance.OpenSingle<HitEffect>(battlePeopleView.parentPanel.trans_bulletParent, transform.position);
            //    PanelManager.Instance.CloseSingle(this);
            //}
        }
    }
}

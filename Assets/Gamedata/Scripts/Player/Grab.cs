
using System;
using UnityEngine;
using UnityEngine.InputSystem;
[Serializable]
public struct ThrowStatus
{
    [Range(0f, 90f), Header("射出角度")]
    public float ThrowAngle;
    [Header("直線状に投げる力")]
    public float StraightPower;
    [Header("放物線の表示用")]
    public int linePoint;

}

public class Grab : MonoBehaviour
{
    public ThrowStatus throwStatus;
    public enum ThrowType { Parabola, Straight }
    public ThrowType throwType;
    public static Grab Instance;
    [SerializeField]
    GameObject[] targetBuildings;
    [SerializeField]
    GameObject grabArea,GrabbingBuilding,targetSpot,targetDirection;
    public int targetBuildNum;
    [SerializeField]
    private float cursoleSpeed;
    [SerializeField]
    Vector2 cursoleInput;
    public Vector3 cursolevel;
    public LineRenderer lineRenderer;
    [SerializeField, Header("掴む/投げる音")]
    AudioClip[] grabSEs;

    const int torqueMin = 45;
    const int torqueMax = 90;
    const int enemyThrowPower = 75;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        targetBuildNum = 0;
        cursolevel =  new Vector3 (0,0,0);
        lineRenderer=GetComponent<LineRenderer>();
    }

    void Update()
    {
        
        for(int i = 0; i < targetBuildings.Length; i++)
        {
            if (targetBuildings[i]!=null&&GrabbingBuilding==null)
            {
                if(i==targetBuildNum)
                {
                    if (targetBuildings[i] != null && targetBuildings[i].GetComponent<Outline>() != null)
                    {
                        targetBuildings[i].GetComponent<Outline>().enabled = true;
                    }
                }else
                {
                    if (targetBuildings[i] != null && targetBuildings[i].GetComponent<Outline>() != null)
                    {
                        targetBuildings[i].GetComponent<Outline>().enabled = false;
                    }
                }
            }
            
        }
        switch (throwType)
        {
            /// 放物線状に投げる場合
            case ThrowType.Parabola:
                targetDirection.SetActive(false);
                //lineRenderer.enabled = true;
                if (GrabbingBuilding == null)
                {
                    targetSpot.transform.position = new Vector3(transform.position.x, targetSpot.transform.position.y, transform.position.z-10);
                    targetSpot.SetActive(false);
                }
                else
                {
                    GrabbingBuilding.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    targetSpot.SetActive(true);
                    if (cursoleInput != Vector2.zero)
                    {
                        Vector3 cursoleVelocity = new Vector3(cursoleInput.x * cursoleSpeed, 0, cursoleInput.y * cursoleSpeed);
                        var cursoleDelta = cursoleVelocity * Time.deltaTime;
                        cursolevel += cursoleDelta;
                        targetSpot.transform.position = new Vector3(transform.position.x + cursolevel.x, targetSpot.transform.position.y, transform.position.z -10+ cursolevel.z);
                    }
                    else
                    {
                        if (cursolevel == Vector3.zero)
                        {
                            targetSpot.transform.position = new Vector3(transform.position.x, targetSpot.transform.position.y, transform.position.z - 10);
                        }
                        else
                        {
                            targetSpot.transform.position = new Vector3(transform.position.x + cursolevel.x, transform.position.y, transform.position.z - 10 + cursolevel.z);

                        }
                    }
                    DrawTrajecition();
                }
                break;
            
            ///直線状に投げる場合
            case ThrowType.Straight:
                targetSpot.SetActive(false);
                lineRenderer.enabled = false;
                if(GrabbingBuilding==null)
                {
                    targetDirection.transform.position = new Vector3(transform.position.x, transform.position.y-1, transform.position.z);
                    targetDirection.SetActive(false);
                }else
                {
                    targetDirection.transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
                    targetDirection.SetActive(true);
                    if(cursoleInput!=Vector2.zero)//入力がある間
                    {
                        var DirectionY=-Mathf.Atan2(cursoleInput.y, cursoleInput.x)*Mathf.Rad2Deg+90;
                        targetDirection.transform.rotation = Quaternion.Euler(0, DirectionY, 0);
                    }
                }
                break;
        }

        

    }

    /// <summary>
    /// 建物を掴んでいない場合は掴み、掴んでいる場合は投げる
    /// </summary>
    public void OnFire()
    {
        if (!GetComponent<PlayercharaControll>().isAlive) return;
        if(GrabbingBuilding == null)
        {
            
            if (targetBuildings[targetBuildNum] == null) return;
            SoundSys.instance.PlaySE(grabSEs[0]);
            var target = targetBuildings[targetBuildNum];
            var targetRig = target.GetComponent<Rigidbody>();
            var targetCol = target.GetComponent<Collider>();
            var targetStatus=target.GetComponent<BuildingStatus>();
            //エフェクト生成
            if(target.tag!="Enemy")
            {
                targetStatus.PuffGenerate(target.transform.position, target);
                MainGameSys.instance.SmokeSpawn(target.transform.position);
                MainGameSys.instance.RubbleSpawn(target.transform.position,true);
                MainGameSys.instance.RubbleSpawn(target.transform.position, false);
            }
            else
            {
                target.GetComponent<EnemyMovement>().BlowTextSet();
                target.transform.GetComponentInChildren<ParticleSystem>().Stop();
            }
            
            if (targetStatus!=null&&targetStatus.status.type == buildingStatus.BuildingType.Decoration) return;
            if (targetRig != null)
            {
                targetCol.isTrigger = true;
                targetRig.useGravity = false;
                targetRig.constraints = RigidbodyConstraints.FreezeAll;
                targetRig.velocity = Vector3.zero;
                target.transform.parent = transform;
                target.transform.position = new Vector3(transform.position.x, transform.position.y+5
                    + targetBuildings[targetBuildNum].transform.lossyScale.x+transform.lossyScale.x, transform.position.z);
                GrabbingBuilding = target;
                if (GrabbingBuilding.tag != "Enemy")
                {
                    targetStatus.status.NowGrabbing = true;
                    if (targetStatus.status.type.ToString() == "Boaling"&&targetStatus.status.horizontalGrabFlag)
                    {
                        //ボーリング状に吹っ飛ばすものだけ横にして持つ
                        target.transform.localRotation = Quaternion.Euler(0, 0, 90);
                        target.transform.localPosition = new Vector3(3.25f, target.transform.localPosition.y, target.transform.localPosition.z);
                    }
                    else
                    {
                        target.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    }
                    targetStatus.Materialchange(true);
                    targetStatus.StopFade();
                    if(targetStatus.status.throwType==buildingStatus.ThrowType.Parabola)
                    {
                        lineRenderer.enabled = true;
                    }
                }
                
               
                
                //ここで持ってる間色を半透明にしたい
                
                
                //一旦掴んでるものが敵や建物と当たらないようにする
                GrabbingBuilding.gameObject.layer = LayerMask.NameToLayer("DirectionSelecter");
                ThrowTypeCheck(target);
            }
        }
        //既に建物を掴んでいる場合
        else
        {
            
            lineRenderer.enabled = false;
            var grabbingRig=GrabbingBuilding.GetComponent<Rigidbody>();
            var grabbingCol = GrabbingBuilding.GetComponent<Collider>();
            grabbingCol.isTrigger = false;
            grabbingRig.useGravity = true;
            grabbingRig.constraints= RigidbodyConstraints.None;
            grabbingRig.velocity = Vector3.zero;
            //GrabbingBuilding.transform.localPosition = new Vector3(0, transform.position.y+2, 0);
            GrabbingBuilding.transform.parent = null;
            switch (throwType)
            {
                case ThrowType.Parabola:
                    ThrowBuilding("Parabola", GrabbingBuilding);
                    break;

                case ThrowType.Straight:
                    ThrowBuilding("Straight", GrabbingBuilding);
                    break;
            }
            if(GrabbingBuilding.CompareTag("Enemy"))
            {
                GrabbingBuilding.GetComponent<EnemyMovement>().Defeat();
                SoundSys.instance.PlaySE(grabSEs[2]);
            }else
            {
                SoundSys.instance.PlaySE(grabSEs[1]);
            }
            PlayercharaControll.instance.AttackAnim();
            cursolevel = Vector3.zero;
            GrabbingBuilding = null;
            targetBuildings[targetBuildNum] = null;
            
        }
        

    }
    public void OnThrowDirection(InputValue value)
    {
        if (!GetComponent<PlayercharaControll>().isAlive) return;
        cursoleInput = value.Get<Vector2>();
    }

    public void OnRemoveSpot()
    {
        //カーソルを初期位置に戻す
        cursolevel = Vector2.zero;
        targetSpot.transform.position = new Vector3
            (transform.position.x, targetSpot.transform.position.y, transform.position.z - 10);
    }
    void ThrowTypeCheck(GameObject throwBuilding)
    {
        //放物線状に投げるか、直線状に投げるかの判定を行う
        //敵のフィニッシュ時には直線で固定
        if(throwBuilding.tag=="Enemy")
        {
            throwType = ThrowType.Straight;
            return;
        }    
        var throwtype = throwBuilding.GetComponent<BuildingStatus>().status.throwType;
        switch(throwtype)
        {
            case buildingStatus.ThrowType.Parabola:
                throwType = ThrowType.Parabola;
                break;
            case buildingStatus.ThrowType.Straight:
                throwType = ThrowType.Straight;
                break;
        }
    }

    public void Targeting(GameObject target)
    {
        if (!GetComponent<PlayercharaControll>().isAlive) return;
        for (int i = 0;i< targetBuildings.Length; i++)
        {
            if (targetBuildings[i] == target) return;
            if (targetBuildings[i]==null)
            {
                targetBuildings[i] = target;
                targetBuildNum = i;
                return;
            }
        }
    }

    public void TargetOut(GameObject target)
    {
        if (!GetComponent<PlayercharaControll>().isAlive) return;
        for (int i = 0; i< targetBuildings.Length; i++)
        {
            if (targetBuildings[i] == target)
            {
                if (targetBuildings[i].GetComponent<Outline>() != null)
                    targetBuildings[i].GetComponent<Outline>().enabled = false;
                targetBuildings[i] = null;
            }
        }
    }
    void ThrowBuilding(string ThrowType,GameObject throwBuilding)
    {
        if (!GetComponent<PlayercharaControll>().isAlive) return;
        GrabbingBuilding.layer = 6;
        var grabbingRig = GrabbingBuilding.GetComponent<Rigidbody>();
        int torqueX = (int)UnityEngine.Random.Range(torqueMin, torqueMax);
        int torqueY = (int)UnityEngine.Random.Range(torqueMin, torqueMax);
        int torqueZ = (int)UnityEngine.Random.Range(torqueMin, torqueMax);
        Vector3 torque = new Vector3(torqueX, torqueY, torqueZ);
        switch (throwBuilding.tag)
        {
            case "Buildings":
                var grabStatus = throwBuilding.GetComponent<BuildingStatus>();
                grabStatus.MassChange(true,throwBuilding);
                grabStatus.status.NowGrabbing = false;
                grabStatus.Materialchange(false);
                
                switch (ThrowType)
                {
                    case "Parabola":
                        //放物線状に投げた建物に回転を与える
                        
                        grabbingRig.AddTorque(torque, ForceMode.Impulse);
                        Vector3 velocity = CalculateVelocity(GrabbingBuilding.transform.position, targetSpot.transform.position, throwStatus.ThrowAngle);
                        grabbingRig.AddForce(velocity * grabbingRig.mass, ForceMode.Impulse);
                        break;
                    case "Straight":
                        //選んだ方向へ発射
                        if (grabStatus.status.type == buildingStatus.BuildingType.Boaling)
                            throwBuilding.GetComponent<Collider>().isTrigger = true;
                        grabbingRig.AddForce(targetDirection.transform.forward * throwStatus.StraightPower * grabStatus.status.speed, ForceMode.Impulse);
                        break;
                }
                grabStatus.colldierChecker.SetActive(true); //衝突判定のオン
                break;
            
            case "Enemy":
                //カメラが敵に注目する
                MainGameSys.instance.ThrowingEnemyFeature();
                MainGameSys.instance.WallLayerChange();
                GrabbingBuilding.GetComponent<EnemyMovement>().DeathTextSet(false);
                grabbingRig.AddTorque(torque, ForceMode.Impulse);
                grabbingRig.AddForce(targetDirection.transform.forward * throwStatus.StraightPower * Time.deltaTime * enemyThrowPower, ForceMode.Impulse);
                break;
        }


        
    }
    Vector3 CalculateVelocity(Vector3 pointA,Vector3 pointB,float angle)
    {
        //ラジアンへ変換
        float rad=angle*Mathf.PI/180;
        float x = Vector2.Distance(new Vector2(pointA.x, pointA.z), new Vector2(pointB.x, pointB.z));
        float y = pointA.y - pointB.y;
        float speed=Mathf.Sqrt(-Physics.gravity.y * Mathf.Pow(x, 2) / (2 * Mathf.Pow(Mathf.Cos(rad), 2) * (x * Mathf.Tan(rad) + y)));
        if(float.IsNaN(speed) )
        {
            return Vector3.zero;
        }else
        {
            return (new Vector3(pointB.x - pointA.x, x * Mathf.Tan(rad), pointB.z - pointA.z).normalized * speed);
        }
    }
    
    /// <summary>
    /// 放物線状の予測線を描く
    /// </summary>
    void DrawTrajecition()
    {
        Vector3 origin= GrabbingBuilding.transform.position;
        Vector3 target = CalculateVelocity(GrabbingBuilding.transform.position, targetSpot.transform.position, throwStatus.ThrowAngle);
        lineRenderer.positionCount = throwStatus.linePoint;
        Vector3 limitPos=Vector3.zero;
        float time = 0;
        bool isDraw = true;
        for(int i=0;i<throwStatus.linePoint;i++)
        {
            var x = (target.x * time) + (Physics.gravity.x / 2 * time * time);
            var y = (target.y * time) + (Physics.gravity.y / 2 * time * time);
            var z = (target.z * time) + (Physics.gravity.z / 2 * time * time);
            Vector3 point = new Vector3(x, y, z);
            Vector3 pos=origin+ point;
            if(pos.y<targetSpot.transform.position.y)
            {
                pos.y = targetSpot.transform.position.y;

                if(isDraw)
                {
                    limitPos = pos;
                    isDraw = false;

                }
                lineRenderer.SetPosition(i, limitPos);
            }
            else
                lineRenderer.SetPosition(i, pos);

            //else
            time += 0.01f;
        }
    }
}

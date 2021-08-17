using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{

    public enum Type {Melee,Range};
    public Type type;
    public int damage;
    public float rate;
    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;

    public Transform bulletpos;
    public GameObject bullet;

    public Transform bulletcasepose;
    public GameObject bulletCase;

    public int maxAmmo;
    public int curAmmo;

    public void Use()
    {
        if (type == Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }else if (type == Type.Range && curAmmo>0)
        {
            curAmmo--;
            //StopCoroutine("Shot");
            StartCoroutine("Shot");
        }
    }

    IEnumerator Swing()
    {
        //1
        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = true;
        trailEffect.enabled = true;

        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;
        //yield return new WaitForSeconds(0.1f); //0.1프레임 대기
        //yield break //나가기
        //2 
    }

    IEnumerator Shot()
    {
        GameObject instantBullet = Instantiate(bullet,bulletpos.position,bulletpos.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletpos.forward * 50;
        yield return null;

        GameObject instantCase = Instantiate(bulletCase, bulletcasepose.position, bulletcasepose.rotation);
        Rigidbody caseRigid = instantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletcasepose.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2,3);
        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);
    }

    //메인루틴 -> 서브루틴 -> 메인루틴 / 교차실행
    //코루틴 : 메인+서브루틴 
}

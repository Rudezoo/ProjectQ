using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject meshObj;
    public GameObject effectObj;
    public Rigidbody rigid;


    void Start()
    {
        StartCoroutine(Explosion());
    }

    // Update is called once per frame

    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(3f);
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;

        meshObj.SetActive(false);
        effectObj.SetActive(true);

        RaycastHit[] rayhits=Physics.SphereCastAll(transform.position,15,Vector3.up,0f,LayerMask.GetMask("Enemy"));
        foreach(RaycastHit hitobj in rayhits)
        {
            hitobj.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
        }

        Destroy(gameObject, 5);

    }


}

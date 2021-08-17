using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    public EInfo eInfo;
    public Transform Target;

    public bool isChasing;

    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat;

    NavMeshAgent nav;

    Animator anim;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponentInChildren<MeshRenderer>().material; //material은 이렇게 가져와야된다.
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        Invoke("ChaseStart", 2);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            Weapons weapon = other.GetComponent<Weapons>();
            eInfo.health -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec, false));
            Debug.Log("Melee" + weapon.damage);
        }
        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            eInfo.health -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject);
            StartCoroutine(OnDamage(reactVec, false));
            Debug.Log("Range" + bullet.damage);
        }
    }

    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if (eInfo.health > 0)
        {
            mat.color = Color.white;
            /*reactVec = reactVec.normalized;
            reactVec += Vector3.up;
            rigid.AddForce(reactVec * 2, ForceMode.Impulse);*/
            if (isGrenade)
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 2;

                rigid.freezeRotation = false;
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
            }
        }
        else
        {
            mat.color = Color.gray;
            gameObject.layer = 12;

            isChasing = false;
            nav.enabled = false;

            anim.SetTrigger("doDie");
            if (isGrenade)
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up*3;

                rigid.freezeRotation = false;
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
                rigid.AddTorque(reactVec * 15, ForceMode.Impulse);
            }
            else
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up;
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
            }


           
            Destroy(gameObject, 4);
        }
    }

    public void HitByGrenade(Vector3 explosionPos)
    {
        eInfo.health -= 100;
        Vector3 reactVec = transform.position - explosionPos;
        StartCoroutine(OnDamage(reactVec, true));
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    void ChaseStart()
    {
        isChasing = true;
        anim.SetBool("isWalk",true);
    }

    // Update is called once per frame
    void Update()
    {
        if(isChasing)
            nav.SetDestination(Target.position);
    }

    void FreezeVelocity()
    {
        if (isChasing)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
        
    }

    private void FixedUpdate()
    {
        FreezeVelocity();
    }
}

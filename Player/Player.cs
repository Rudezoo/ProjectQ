using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update\

    public float speed;
    public float turnspeed = .01f;
    public float jumpPower = 5;
    public GameObject[] weapons;
    public bool[] hasWeapons;
    public GameObject[] grenades;

    public Camera followCamera;

    public PInfo pInfo;
    public GameObject GrenadeObj;

    float hAxis;
    float vAxis;

    bool wDown;
    bool jDown;
    bool iDown;
    bool sDown1;
    bool sDown2;
    bool sDown3;
    bool fDown;
    bool rDown;

    bool gDown;

    bool isJumping;
    bool isDodging;
    bool isSwaping;
    bool isFireReady=true;
    bool isReloading;

    public bool stop;

    bool isBorder;

    public Vector3 moveVec;
    Vector3 saveVec;
    Vector3 dodgeVec;

    Quaternion rotGoal;

    Rigidbody rigid;
    Animator anim;

    GameObject NearObject;
    Weapons equipWeapon;
    int equipWeaponIdx=-1;

    float fireDelay;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {

        
        if (!stop)
        {
            GetInput();
            move();
            SetMoveState();
            RotateChar();
            Jump();
            Attack();
            Dodge();
            Interaction();
            Swap();
            Reload();
            Grenade();
        }

    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Sprint");
        jDown = Input.GetButtonDown("Jump");
        iDown = Input.GetButtonDown("Interaction");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
        fDown = Input.GetButton("Fire1");
        rDown = Input.GetButtonDown("Reload");
        gDown = Input.GetButtonDown("Fire2");
    }

    void move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        saveVec = moveVec;
        if (isDodging)
            moveVec = dodgeVec;

        if (isSwaping || !isFireReady ||isReloading)
            moveVec = Vector3.zero;

        if(!isBorder)
            transform.position += moveVec * speed * (wDown ? 2.0f : 1.0f) * Time.deltaTime;

    }

    void SetMoveState()
    {
        anim.SetBool("isWalk", moveVec != Vector3.zero);
        anim.SetBool("isRun", wDown);
    }

    void RotateChar()
    {
        if (hAxis != 0 || vAxis != 0)
        {

            rotGoal = Quaternion.LookRotation(saveVec);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotGoal, turnspeed);

            /*transform.LookAt(moveVec+transform.position);*/

            //마우스회전
        }
        if (fDown)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayhit;
            if (Physics.Raycast(ray, out rayhit, 100))
            {
                Vector3 nextVec = rayhit.point - transform.position;
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }
        }
    }

    void Jump()
    {
        if (jDown && !isJumping && !isDodging && moveVec == Vector3.zero)
        {
            isJumping = true;
            rigid.AddForce(Vector3.up * jumpPower,ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
        }
    }

    void Attack()
    {
        if (equipWeapon == null)
            return;

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;

        if(fDown && isFireReady && !isDodging && !isSwaping &&!isReloading) //조건만족
        {
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type==Weapons.Type.Melee?"doSwing":"doShot");
            fireDelay = 0;
        } 
    }

    void Grenade()
    {
        
        if (pInfo.hasGrenades == 0)
            return;

        if(gDown && !isReloading && !isSwaping)
        {
            Debug.Log("Grenade SHoot!");
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayhit;
            if (Physics.Raycast(ray, out rayhit, 100))
            {
                Vector3 nextVec = rayhit.point - transform.position;
                nextVec.y = 15;

                GameObject instantGrenade = Instantiate(GrenadeObj, transform.position, transform.rotation);
                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(nextVec,ForceMode.Impulse);
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                pInfo.hasGrenades --;
                grenades[pInfo.hasGrenades].SetActive(false);
            }
        }
    }

    void Reload()
    {
        if (equipWeapon == null || equipWeapon.type == Weapons.Type.Melee || pInfo.ammo==0)
            return;

        if(rDown && !isJumping && !isDodging && !isSwaping && isFireReady)
        {
            anim.SetTrigger("doReload");
            isReloading = true;

            Invoke("ReloadOut", 2.5f);
        }

    }

    void ReloadOut()
    {
        int reAmmo = pInfo.ammo < equipWeapon.maxAmmo ? pInfo.ammo : equipWeapon.maxAmmo;
        equipWeapon.curAmmo = reAmmo;
        pInfo.ammo -= reAmmo;
        isReloading = false;
    }
    

    void Dodge()
    {
        if (jDown && moveVec!=Vector3.zero && !isJumping && !isDodging)
        {
            isDodging = true;
            dodgeVec = moveVec;

            speed *= 1.5f;
            anim.SetTrigger("doDodge");

            Invoke("DodgeOut", 0.8f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJumping = false;

        }
    }

    void DodgeOut()
    {
        speed=speed/1.5f;
        isDodging = false;
    }

/*    public void LookBack()
    {
        transform.LookAt(-1 *(transform.position + moveVec));
        rigid.AddForce(-1 * (transform.position + moveVec) * 0.005f, ForceMode.Impulse);
    }*/

    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        isBorder = Physics.Raycast(transform.position, moveVec, 5, LayerMask.GetMask("Wall"));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch (item.type)
            {
                case Item.Type.Ammo:
                    pInfo.ammo += item.value;
                    if (pInfo.ammo > pInfo.Maxammo)
                        pInfo.ammo = pInfo.Maxammo;
                    break;
                case Item.Type.Coin:
                    pInfo.coin += item.value;
                    if (pInfo.coin > pInfo.Maxcoin)
                        pInfo.coin = pInfo.Maxcoin;
                    break;
                case Item.Type.Grenade:
                    grenades[pInfo.hasGrenades].SetActive(true);
                    pInfo.hasGrenades += item.value;
                    if (pInfo.hasGrenades > pInfo.MaxhasGrenades)
                        pInfo.hasGrenades = pInfo.MaxhasGrenades;
                    break;
                case Item.Type.Heart:
                    pInfo.health+= item.value;
                    if (pInfo.health > pInfo.MaxHealth)
                        pInfo.health = pInfo.MaxHealth;
                    break;
            }
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon")
        {
            NearObject = other.gameObject;
        }

        //Debug.Log(NearObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
        {
            NearObject = null;
        }
    }

    void Swap()
    {
        int weaponidx = -1;

        if(sDown1 && (!hasWeapons[0] || equipWeaponIdx == 0))
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIdx == 1))
            return;
        if (sDown3 && (!hasWeapons[2] || equipWeaponIdx == 2))
            return;


        if (sDown1) weaponidx = 0;
        if (sDown2) weaponidx = 1;
        if (sDown3) weaponidx = 2;

        if ((sDown1 || sDown2|| sDown3)&&!isJumping&&!isDodging)
        {
            if (hasWeapons[weaponidx] == true)
            {   
                if(equipWeapon)
                    equipWeapon.gameObject.SetActive(false);

                equipWeapon = weapons[weaponidx].GetComponent<Weapons>();
                equipWeaponIdx = weaponidx;
                equipWeapon.gameObject.SetActive(true);
                //pInfo.ammo = equipWeapon.curAmmo;

                anim.SetTrigger("doSwap");
                isSwaping = true;
                Invoke("SwapOut", 0.5f);
            }
                
        }
    }

    void SwapOut()
    {
        isSwaping = false;
    }

    void Interaction()
    {
        if (iDown && NearObject != null && !isJumping && !isDodging)
        {
            if (NearObject.tag == "Weapon")
            {
                Item item = NearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(NearObject);
            }
        }

    }
}

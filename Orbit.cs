using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform target;
    public float OrbitSpeed;
    Vector3 offset;

    void Start()
    {
        offset = transform.position - target.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position + offset;
        transform.RotateAround(target.position,Vector3.up,OrbitSpeed*Time.deltaTime);
        offset = transform.position - target.position;
    }
}

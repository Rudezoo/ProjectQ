using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject other;

  

    private void Update()
    {

            
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("In");
        other = collision.gameObject;
        if (other.tag == "Player")
        {
            
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("Out");
        other = collision.gameObject;
        if (other.tag == "Player")
        {
            
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCheck : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isWeapon;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerWeapon"))
        {
            isWeapon = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
         if (other.CompareTag("PlayerWeapon"))
        {
            isWeapon = false;
        }
    }
}

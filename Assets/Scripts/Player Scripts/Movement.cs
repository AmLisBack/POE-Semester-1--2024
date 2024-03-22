using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f; 
    public Rigidbody rb;
    public Transform blueFlag;
    public Transform redFlag;

    public Transform blueFlagSpawn;
    public Transform redFlagSpawn;


    void Update()
    {
        
        float LeftRightInput = Input.GetAxis("Horizontal");
        float UpDownInput = Input.GetAxis("Vertical");
        
        Vector3 movement = new Vector3(LeftRightInput, 0f, UpDownInput) * moveSpeed * Time.deltaTime;

        rb.MovePosition(rb.position + transform.TransformDirection(movement));
        if (EnemyFinite.playerHasFlag)
        {
            blueFlag.position = Vector3.MoveTowards(blueFlag.position, gameObject.transform.position + new Vector3(0, 2f, 0), 1f);
        }
        if(EnemyFinite.roundEnded)
        {
            gameObject.transform.position = redFlagSpawn.position;
        }
        
        if(EnemyFinite.flagDropped)
        {
            Recapture();
        }

        
    }

    private void Attack()
    {
        EnemyFinite.flagDropped = true;
        
    }
    private void Recapture()
    {
        blueFlag.position = blueFlagSpawn.position;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("BlueFlag"))
        {
            EnemyFinite.playerHasFlag = true;
        }
        if(other.CompareTag("SecureBlue") && EnemyFinite.playerHasFlag)
        {
            EnemyFinite.playerHasFlag = false;
            blueFlag.position = blueFlagSpawn.position;
            EnemyFinite.playerScored = true;
            EnemyFinite.roundEnded = true;
        } 
        if(other.CompareTag("Enemy"))
        {
            Attack();
        }
    }
}

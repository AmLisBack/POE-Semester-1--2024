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

    public bool playerHasEnemyFlag;
    public bool playerHasTheirFlag;

    public bool playerFlagDrop;

    public void Start()
    {
        playerFlagDrop = false;
    }

    void Update()
    {
        
        float LeftRightInput = Input.GetAxis("Horizontal");
        float UpDownInput = Input.GetAxis("Vertical");
        
        Vector3 movement = new Vector3(LeftRightInput, 0f, UpDownInput) * moveSpeed * Time.deltaTime;

        rb.MovePosition(rb.position + transform.TransformDirection(movement));
        if(EnemyFinite.roundEnded)
        {
            playerFlagDrop = false;
            playerHasEnemyFlag = false;
            playerHasTheirFlag = false;
        }
        if (!playerFlagDrop)
        {
            if (playerHasEnemyFlag)
            {
                FlagFollow(blueFlag);
            }
            if (playerHasTheirFlag)
            {
                FlagFollow(redFlag);
            }
        }


        
    }

    /*private void Attack()
    {
        EnemyFinite.flagDrop = true;
       
    }
    */
    private void FlagFollow(Transform flag)
    {
        flag.transform.position = Vector3.MoveTowards(flag.position, gameObject.transform.position, 1f) + new Vector3(0,1f,0);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("BlueFlag"))
        {
            playerFlagDrop = false;
            EnemyFinite.playerHasFlag = true;
            playerHasEnemyFlag = true;
            
        }
        if(other.CompareTag("BlueBase")&& playerHasEnemyFlag)
        {
            playerFlagDrop = false;
            playerHasTheirFlag = false;
            playerHasEnemyFlag = false; 
            EnemyFinite.playerHasFlag = false;
            blueFlag.position = blueFlagSpawn.position;
        } 
        if(other.CompareTag("BlueBase") && playerHasTheirFlag)
        {
            playerFlagDrop = false;
            playerHasTheirFlag = false;
            playerHasEnemyFlag = false;
            redFlag.position = redFlagSpawn.position;
        }
        if(other.CompareTag("Enemy"))
        {
            //Attack();
            playerFlagDrop = true;
            playerHasTheirFlag = false;
            playerHasEnemyFlag = false;
        }
        if(other.CompareTag("RedFlag"))
        {
            playerFlagDrop = false;
            playerHasTheirFlag = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFinite : MonoBehaviour
{
    public Transform player;
    public Transform redFlag;//Flag position based on the Gameobjects transform - For Navmesh agent AI
    public Transform blueFlag;
    public Transform secureRedFlag;
    //These are used to spawn the flag back once captured or resecured
    public Transform blueFlagSpawn;
    public Transform redFlagSpawn;
    

    public NavMeshAgent enemy;

    private float distanceToPlayer;
    private float distanceToBlueFlag;
    private float distanceToRedFlag;
    private float keepDistance = 3f;

    public bool playerHasFlag;//true if the player is carrying the Red flag
    public bool returnRedFlag;
    public bool chasePlayer;//used to decide whether to chase the player
    public bool enemyHasFlag;//if the enemy/agent is carrying a flag

    private States currentState;

    
    private enum States
    {
        Take, Recapture, Chase, Avoid, Secure
    }
    // Start is called before the first frame update
    void Start()
    {
        currentState = States.Take; //Enemy goes for players flag from the start
        playerHasFlag = false;
        enemyHasFlag = false;
        chasePlayer = false;
        returnRedFlag = false;
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position,player.position); //Gets the distance between agent and player
        distanceToRedFlag = Vector3.Distance(transform.position,redFlag.position);//to make decision based on offensive or attack
        distanceToBlueFlag = Vector3.Distance(transform.position,blueFlag.position);//distance to objective

        switch(currentState)
        {
            case States.Take:
                Take();
                if(distanceToPlayer <= 5f  && !enemyHasFlag && distanceToPlayer < distanceToBlueFlag)
                {
                    currentState = States.Chase;
                }
                if(enemyHasFlag)
                {
                    Secure();
                }
                break;
            case States.Chase:
                Chase();
                if(enemyHasFlag)
                {
                    currentState = States.Secure;
                }
                break;
            case States.Recapture:
                Recapture();
                break;
            case States.Avoid:
                Avoid();
                break;
            case States.Secure:
                Secure();
                if(distanceToPlayer <= keepDistance)
                {
                    currentState = States.Avoid;
                }
                break;
        }
    }

    private void Take()
    {
        enemy.destination = blueFlag.position;
    }
    private void Recapture()
    {
        enemy.destination = redFlag.position;
    }
    private void Chase()
    {
        enemy.destination = player.position;
    }
    private void Avoid()
    {
       
        Vector3 directionOfPlayer = transform.position - player.position;
        Vector3 avoidDirectionPoint = transform.position + directionOfPlayer.normalized * keepDistance;
        enemy.destination = avoidDirectionPoint;
        
    }
    private void Secure()
    {
        enemy.destination = secureRedFlag.position;
        StartCoroutine(flagFollow());
    }

    private void OnTriggerEnter(Collider other)//Possible issue if too many triggers being activated therefor tags not working// Possible solution have a deactivated flag on the enemy and player and activate them once they are 'carrying' the flag
    {
        if(other.CompareTag("BlueFlag"))
        {
            enemyHasFlag = true;
        }
        else if(other.CompareTag("SecureRed"))
        {
            Debug.Log("entered Red secure trigger");
            blueFlag.position = blueFlagSpawn.position;
            enemyHasFlag = false;
            StopCoroutine(flagFollow());
        }
        else if(other.CompareTag("SecureBlue"))
        {
            Debug.Log("Entered blue secure");
        }
        
    }
    IEnumerator flagFollow()
    {
        blueFlag.position = Vector3.MoveTowards(blueFlag.position, enemy.transform.position, 1f);
        yield return null;
    }
}

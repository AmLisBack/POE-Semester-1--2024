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
    public static float roundNumber = 1;// Set to 1 because game starts on Round 1
    public static float playerScore = 0;
    public static float enemyScore = 0;

    
    public bool returnRedFlag;
    public bool chasePlayer;//used to decide whether to chase the player
    public bool returnBlueFlag;//Used to start returning the flag
    public bool recaptureFlag;//Used to set the state to go fetch the enemies dropped flag

    public static bool enemyHasFlag;//if the enemy has the players flag
    public static bool enemyScored;
    public static bool roundEnded;
    public static bool playerScored;
    public static bool playerHasFlag;//true if the player is carrying the Red flag
    public static bool flagDropped;//If the enemy drops the flag this is true
    public static bool playerDroppedFlag;//Set in the Movement script, for when the player drops their flag


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
        playerScored = false;
        enemyScored = false;
        roundEnded = false;
        flagDropped = false;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log($"playerScored {playerScored} |||||| EnemyScored {enemyScored}");
        if(roundEnded)
        {
            flagReset();
            playerHasFlag = false;
            enemyHasFlag = false;
            roundNumber += 1;
            flagDropped = false;
            enemy.transform.position = blueFlagSpawn.position + new Vector3(0f,2f,0f);
            if(enemyScored)
            {
                enemyScore += 1;
                enemyScored = false;
                
            }
            if(playerScored)
            {
                playerScore += 1;
                playerScored = false;
                
            }
            roundEnded = false ;
        }
        if(flagDropped) //Constantly running causing the AI to freeze in the air
        {
            currentState = States.Take;
            flagDropped = false;
        }
         
        if(enemyHasFlag && !flagDropped)
        {
            redFlag.position = Vector3.MoveTowards(redFlag.position, enemy.transform.position, 1f);
        }
        if(playerDroppedFlag && recaptureFlag && !enemyHasFlag)
        {
            blueFlag.position = Vector3.MoveTowards(blueFlag.position, enemy.transform.position, 1f);
            returnBlueFlag = true;
        }
        distanceToPlayer = Vector3.Distance(transform.position,player.position); //Gets the distance between agent and player
        distanceToRedFlag = Vector3.Distance(transform.position,redFlag.position);//to make decision based on offensive or attack
        distanceToBlueFlag = Vector3.Distance(transform.position,blueFlag.position);//distance to objective
        
        switch(currentState)
        {
            case States.Take:
                Take();
                if (distanceToPlayer <= 5f && !enemyHasFlag && distanceToPlayer < distanceToBlueFlag && playerHasFlag)
                {
                    currentState = States.Chase;
                }
                if(enemyHasFlag)
                {
                    currentState = States.Secure;
                }
                if(recaptureFlag && playerDroppedFlag)
                {
                    currentState = States.Recapture;
                }
                break;
            case States.Chase:
                Chase();
                if(enemyHasFlag)
                {
                    currentState = States.Secure;
                }
                if (distanceToPlayer > 8f)
                {
                    currentState = States.Take;
                }
                if(flagDropped)
                {
                    currentState = States.Take;
                }
                if(playerDroppedFlag && recaptureFlag)
                {
                    currentState = States.Recapture;
                }
                break;
            case States.Recapture:
                Recapture();
                break;
            case States.Avoid:
                Avoid();
                if(distanceToPlayer > 8f)
                {
                    currentState = States.Secure;
                }
                if(distanceToPlayer >8f && playerDroppedFlag)
                {
                    currentState = States.Recapture;
                }
                break;
            case States.Secure:
                Secure();
                if(!enemyHasFlag)
                {
                    currentState = States.Take;
                }
                if(distanceToPlayer <= keepDistance)
                {
                    currentState = States.Avoid;
                }
                break;
        }
    }

    private void Take()
    {
        enemy.destination = redFlag.position;
    }
    private void Recapture()
    {
        enemy.destination = blueFlag.position;
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
        
       
    }   
    private void flagReset()
    {
        redFlag.position = redFlagSpawn.position;
        blueFlag.position = blueFlagSpawn.position; 
    }
    private void enemyAttack()
    {
        playerDroppedFlag = true;
    }

    private void OnTriggerEnter(Collider other)// Add a rb to all waypoints and freeze positions
    {
        Debug.Log(other.tag);
        
        /*
        else if(other.CompareTag("SecureRed") && enemyHasFlag)
        {
            
            redFlag.position = redFlagSpawn.position;
            enemyScored = true;
            roundEnded = true;
            enemyHasFlag = false;
           
        }
        */
        if(other.CompareTag("Player") && !enemyHasFlag)
        {
            enemyAttack();
            
        }
        if(other.CompareTag("Player") && enemyHasFlag)
        {
            enemy.transform.position = blueFlagSpawn.position + new Vector3(0f, 2f, 0f);
        }
        if(other.CompareTag("BlueFlag") && currentState == States.Recapture)
        {
            blueFlag.position = blueFlagSpawn.position;
            returnBlueFlag = true;//Players flag is no longer dropped
            
        }
        if(other.CompareTag("SecureBlue") && returnBlueFlag && currentState == States.Recapture)
        {
            blueFlag.position = blueFlagSpawn.position;
            returnBlueFlag = false;
            currentState = States.Take;//Resets to starting state to make decisions based on other params in switch case
            playerDroppedFlag = false;
            recaptureFlag = false;
            
        }
        
       
        
    }
   
}

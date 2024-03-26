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
    public Transform blueBase;
    public Transform redBase;
    

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

    //RED FLAG TO TAKE, BLUE FLAG TO PROTECT
    //BLUE SECURE IS ON PLAYERS SIDE, RED SECURE IS ON ENEMIES SIDE


    
    public static bool enemyWon;
    public static bool roundEnded;
    public static bool playerWon;
    public static bool playerHasFlag;//true if the player is carrying the Red flag
    public static bool flagDrop;//If the enemy drops the flag this is true
    public static bool playerDroppedFlag;//Set in the Movement script, for when the player drops their flag

    public bool enemyHasOwnFlag;// if the enemy has its own flag (Blue)
    public bool enemyHasPlayersFlag;//if the enemy has the players flag (Red)

    public static bool enemyCarryingFlag;//This is for the redFlag script to check if its been secured
    public  bool redFollow;
    public bool blueFollow;

    private States currentState;

    
    private enum States
    {
        Take, Chase, Avoid, Secure
    }
    // Start is called before the first frame update
    void Start()
    {
        currentState = States.Take; //Enemy goes for players flag from the start
        playerHasFlag = false;
        enemyHasPlayersFlag = false;
        chasePlayer = false;
        returnRedFlag = false;
        playerWon = false;
        enemyWon = false;
        roundEnded = false;
        flagDrop = false;
    }

    // Update is called once per frame
    void Update()
    {
        #region Round Ended
        Debug.Log($"playerScored {playerWon} |||||| EnemyScored {enemyWon} [Current State]: {currentState}");
        if (roundEnded)
        {
            flagReset();
            playerHasFlag = false;
            enemyHasPlayersFlag = false;
            enemyHasOwnFlag = false;
            roundNumber += 1;
            flagDrop = false;
            if (enemyWon)
            {
                enemyScore += 1;
                enemyWon = false;

            }
            if (playerWon)
            {
                playerScore += 1;
                playerWon = false;

            }
            roundEnded = false;
        }
        #endregion


        distanceToPlayer = Vector3.Distance(transform.position, player.position); //Gets the distance between agent and player
        distanceToRedFlag = Vector3.Distance(transform.position, redFlag.position);//to make decision based on offensive or attack
        distanceToBlueFlag = Vector3.Distance(transform.position, blueFlag.position);//distance to objective

        switch (currentState)
        {
            case States.Take:
                Take();
                if (distanceToPlayer <= 5f && !enemyHasPlayersFlag && distanceToPlayer < distanceToBlueFlag && playerHasFlag)
                {
                    currentState = States.Chase;
                }
                if (enemyHasPlayersFlag)
                {
                    currentState = States.Secure;
                }
                
                break;
            case States.Chase:
                Chase();
                if (enemyHasPlayersFlag)
                {
                    currentState = States.Secure;
                }
                if (distanceToPlayer > 8f)
                {
                    currentState = States.Take;
                }
                if (flagDrop)
                {
                    currentState = States.Take;
                }
                if(redFollow || blueFollow)
                {
                    currentState = States.Secure;
                }
                
                break;
            case States.Avoid:
                Avoid();
                if(distanceToPlayer > 8f && flagDrop)
                {
                    currentState = States.Take;
                    flagDrop = false;
                }
                if (distanceToPlayer > 8f)
                {
                    currentState = States.Secure;
                }
                break;
            case States.Secure:
                if (blueFollow)
                {
                    Secure(redBase);
                }
                if (redFollow)
                {
                    Secure(redBase);
                }
                if (!enemyHasPlayersFlag)
                {
                    currentState = States.Take;
                }
                if (distanceToPlayer <= keepDistance)
                {
                    currentState = States.Avoid;
                }
                break;
        }if(flagDrop)
        {
            if(redFollow)
            {
                FlagDrop(redFlag);
            }
            if(blueFollow)
            {
                FlagDrop(blueFlag);
            }
            redFollow = false;
            blueFollow = false;
            enemyHasOwnFlag = false;
            enemyHasPlayersFlag = false;
            
            currentState = States.Avoid;
            
        }
        if (!flagDrop)
        {
            if (redFollow)
            {
                FlagFollow(redFlag);
            }
            if (blueFollow)
            {
                FlagFollow(blueFlag);
            }
        }
        
    }

    private void Take()//Go get the enemies flag(Red)
    {
        enemy.destination = redFlag.position;
    }
    private void Recapture()//Pick up and defend own flag(Blue)
    {
        
        enemy.destination = redBase.position;
    }
    private void Chase()// Try and get to the player to Attack
    {
        enemy.destination = player.position;
    }
    private void Avoid()
    {
       
        Vector3 directionOfPlayer = transform.position - player.position;
        Vector3 avoidDirectionPoint = transform.position + directionOfPlayer.normalized * keepDistance;
        enemy.destination = avoidDirectionPoint;
        
    }
    private void Secure(Transform baseToSecureAt)
    {
        enemy.destination = baseToSecureAt.position;
        
    }   
    private void flagReset()
    {
        redFollow = false;
        blueFollow = false; 
        redFlag.position = blueBase.position;
        blueFlag.position = redBase.position; 
    }
    private void enemyAttack()
    {
        playerDroppedFlag = true;
    }

    private void FlagFollow(Transform flag)
    {
        flag.transform.position = Vector3.MoveTowards(flag.position, gameObject.transform.position, 1f) + new Vector3(0, 1f, 0);
    }
    private void FlagDrop(Transform flag)
    {
        flag.position = gameObject.transform.position + new Vector3(0, 1.5f, 0);
    }

    private void OnTriggerEnter(Collider other)// Add a rb to all waypoints and freeze positions
    {
        //Debug.Log(other.tag);
        if(other.CompareTag("Player"))
        {
            if(!enemyHasPlayersFlag || !enemyHasOwnFlag)
            {
                enemyAttack();
            }
            if(enemyHasPlayersFlag|| enemyHasOwnFlag)
            {
                flagDrop = true;
                if(blueFollow)
                {
                    blueFlag.position += new Vector3(0, 1f, 0f);
                }
                if(redFollow)
                {
                    redFlag.position += new Vector3(0, 1f, 0f);
                }
            }
        }
        if(other.CompareTag("BlueFlag"))
        {
            if(flagDrop)
            {
                enemyHasOwnFlag = true;
                blueFollow = true;
            }
        }
        if(other.CompareTag("RedFlag"))
        {
            redFollow = true;
            enemyHasPlayersFlag = true;
        }
        
        
        
        
       
        
    }
   
}

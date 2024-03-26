using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class redFlag : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform spawnLocation;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("RedBase"))
        {
            EnemyFinite.roundEnded = true;
            EnemyFinite.enemyWon = true;
            
        }
        
        
    }
}

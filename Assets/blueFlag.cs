using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blueFlag : MonoBehaviour
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
        if (other.CompareTag("BlueBase"))
        {
            EnemyFinite.roundEnded = true;
            EnemyFinite.playerWon = true;
        }
        
        
    }
}

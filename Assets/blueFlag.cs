using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blueFlag : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SecureBlue"))
        {
            EnemyFinite.roundEnded = true;
            EnemyFinite.playerScored = true;
        }
        if(other.CompareTag("Enemy") && EnemyFinite.flagDropped)
        {

        }
    }
}

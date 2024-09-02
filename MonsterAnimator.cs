using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimator : MonoBehaviour
{
    private Monster monster;
    private bool Isdead = false;

    // Start is called before the first frame update
    void Start()
    {
        if (monster == null)
        {
            monster = FindObjectOfType<Monster>();
        }
        else
        {
            Debug.LogError("Hero not found in the scene!");
        }
       
    }
    
    
        void Update()
    {
        
        if (!Isdead && (monster.health <= 0))
        {
            Isdead = true;
            GetComponent<Animator>().SetTrigger("Die");
        }
        
    }
}
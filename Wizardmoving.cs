using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wizardmoving : MonoBehaviour
{

    public float wizardMovespeed;
    public Rigidbody2D wizardRB;
    public Collider2D wizardColl;
   
    void Start()
    {
        wizardColl = GetComponent<Collider2D>();
        wizardRB = GetComponent<Rigidbody2D>();
    
    }

  
    void Update()
    {
         wizardMove();
    }
  
    
     void wizardMove()
    {
        float horizontalNum = Input.GetAxis("Horizontal");
        float verticalNum = Input.GetAxis("Vertical");
        wizardRB.velocity = new Vector2(wizardMovespeed * horizontalNum,wizardRB.velocity.y);
        wizardRB.velocity = new Vector2(wizardRB.velocity.x,wizardMovespeed * verticalNum);
    }
}

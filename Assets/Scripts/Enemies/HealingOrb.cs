﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingOrb : MonoBehaviour
{
    public int healPoints;
    // Start is called before the first frame update
    void Start()
    {
        healPoints = 5;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("DESTROYYYYY");
            other.gameObject.GetComponentInChildren<PlayerHealth>().Heal(healPoints);
            Destroy(gameObject);
        }
    }
   
}
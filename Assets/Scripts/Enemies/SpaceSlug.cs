﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SpaceSlug : MonoBehaviour
{
    public int damage; //damage dealt to Player health

    public CinemachineSmoothPath path = default;
    public CinemachineSmoothPath playerPath;
    CinemachineDollyCart playerCart;
    public CinemachineDollyCart slugCart;
    System.Random rng;
    Vector3 start;
    Vector3 middle;
    Vector3 end;
    public bool timerReset;

    public float timeAhead;
    public float maxHeight; //max height of the slug track
    public float minHeight; //min height of the slug track
    public float speed;
    public int outerWidthRange; //horizontal range of the slug track
    public int closeWidthMin;
    public int closeWidthMax;
    public float maxTime = 10;
    public float chancesOfAttack;
    public float speedAdjustmentMultiplier;
    public bool attack;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        playerCart = PlayerInfo.singleton.GetComponentInParent<CinemachineDollyCart>();
        rng = new System.Random();
        start = new Vector3();
        middle = new Vector3();
        end = new Vector3();
        timer = maxTime;
        SpawnSlug();
    }

    // Update is called once per frame
    void Update()
    {   
        if (slugCart.transform.position == end)timerReset = true;
        if (timerReset) timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = maxTime;
            timerReset = false;
            attack = (rng.Next(100) >= chancesOfAttack) ? false : true;
            SpawnSlug();
        } 
    }

    private void SpawnSlug()
    {
        int side = rng.Next(2); //0 = left side of player track, 1 = right side of player track
        middle = playerPath.EvaluatePositionAtUnit(playerCart.m_Position + playerCart.m_Speed * timeAhead, playerCart.m_PositionUnits + (int)(playerCart.m_Speed * timeAhead)) + Vector3.up * maxHeight;
        if (side == 0)
        {
            start = middle - minHeight * Vector3.up * maxHeight - playerCart.transform.right.normalized * rng.Next(outerWidthRange);
            end = middle - minHeight * Vector3.up * maxHeight + playerCart.transform.right.normalized * ((attack) ? rng.Next(closeWidthMin, closeWidthMax) : rng.Next(outerWidthRange));
        }
        else
        {
            start = middle - minHeight * Vector3.up * maxHeight + playerCart.transform.right.normalized * rng.Next(outerWidthRange);
            end = middle - minHeight * Vector3.up * maxHeight - playerCart.transform.right.normalized * ((attack) ? rng.Next(closeWidthMin, closeWidthMax) : rng.Next(outerWidthRange));
        }
        speed = path.PathLength / timeAhead - 3;
        if (attack)
        {
            Vector3 direction = start - new Vector3(middle.x, end.y, middle.z);
            middle +=new Vector3(direction.x*.25f,0, direction.z/2f * .25f);
            //Vector3 depthAdjustment= playerPath.EvaluatePositionAtUnit(playerCart.m_Position + playerCart.m_Speed * timeAhead+2, playerCart.m_PositionUnits + (int)(playerCart.m_Speed * timeAhead)+2);
            //start += new Vector3(depthAdjustment.x, 0, depthAdjustment.z) * .25f;
        }

        path.m_Waypoints[0].position = start;
        path.m_Waypoints[1].position = middle;
        path.m_Waypoints[2].position = end;

        path.InvalidateDistanceCache();
        slugCart.m_Position = 0;

        if (attack) speed = path.PathLength / (timeAhead*speedAdjustmentMultiplier);
        else speed = path.PathLength / timeAhead;

        //speed
        slugCart.m_Speed = speed;
    }

    //If player collides with SpaceSlug, Player takes damage
    void OnCollision(GameObject other)
    {
        if (other.tag == "Player")
        {
            //call Player's TakeDamage() method
            other.GetComponent<PlayerInfo>().TakeDamage(damage);
        }
    }
}

﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour {


    public int health;
    public int maxHealth;
    public bool isAlive = true;
    public Transform headPos;
    public DamagePopUp popText;
    public ParticleSystem deathParticles;
    public Animator deathAnim;
    private int playerID;
    public int winCount;
    private void Start(){
        winCount = 0;
    }
    public void TakeDamage(int val)
    {
        if (isAlive)
        {
            health -= val;
            GameObject pop = Instantiate(popText.gameObject, headPos.position, Quaternion.identity, transform.parent);
            pop.GetComponent<DamagePopUp>().SetDamage(val);
            if (health <= 0)
            {
                isAlive = !isAlive;
                GetComponent<AttackBehavior>().enabled = false;
                GetComponent<MovementBehavior>().enabled = false;
                GetComponentInChildren<SpriteRenderer>().enabled = false;
                deathParticles.Play();
                deathAnim.SetTrigger("alive");
                GameManager.Instance.PlayerDeath();
            }
        }
    }

    public void Reset(Vector3 position){
        health = maxHealth;
        isAlive = true;
        GetComponent<AttackBehavior>().enabled = true;
        GetComponent<MovementBehavior>().enabled = true;
        GetComponentInChildren<SpriteRenderer>().enabled = true;
        transform.position = position;
    }

    public void RegisterPlayerID(int ID){
        playerID = ID;
        Debug.Log(gameObject + " ID: " + playerID);
    }
    
    public void Pause(){
        GetComponent<AttackBehavior>().enabled = false;
        GetComponent<MovementBehavior>().enabled = false;
    }
    public void Resume(){
        if(isAlive){
            GetComponent<AttackBehavior>().enabled = true;
            GetComponent<MovementBehavior>().enabled = true;
        }
    }
}

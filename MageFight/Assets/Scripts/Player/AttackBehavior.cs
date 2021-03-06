﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBehavior : MonoBehaviour {

    [Header("Spell Manager")]
    public SpellsManager spellManager;
    public MovementBehavior playerMovement;
    public InputManager input;
    public bool canAttack = true;
    public bool isHolding = false;
    public bool aiming = false;
    public bool invoking;
    public float timeAfterAttack;
    private float timerAfterAttack;
    private Vector3 lastHandPos;
    public Transform handPos;
    public GameObject arrowSprite;
    public ParticleSystem invokeParticles;
    public AnimationBehavior anim;
    private ParticleSystem.MainModule invokeParticlesMain;
    public int spellIndex = -1;
    public Vector2 spellDir = Vector2.zero;
    // Use this for initialization

    void Start () {
        invokeParticlesMain = invokeParticles.GetComponent<ParticleSystem>().main;
    }

    // Update is called once per frame
    void Update () {
        GetInput();
        CanAttackCheck();
        UpdateSpellDir();
    }

    private void UpdateArrow()
    {
        float arrowAngle = Mathf.Atan2(spellDir.y, spellDir.x) * Mathf.Rad2Deg;
        arrowSprite.transform.rotation = Quaternion.Euler(0, 0, arrowAngle);
    }

    private void CanAttackCheck()
    {
        if (!canAttack)
        {
            timerAfterAttack += Time.deltaTime;
            if (timerAfterAttack > timeAfterAttack)
            {
                timerAfterAttack = 0;
                canAttack = !canAttack;
            }
        }
    }

    private void GetInput()
    {
        GetSpellsInputs(input.firstSkillButton, 0);
        GetSpellsInputs(input.secondSkillButton, 1);
        GetSpellsInputs(input.thirdSkillButton, 2);
    }

    public void SpawnSpell()
    {
        spellManager.ThrowSpell(spellIndex, lastHandPos, spellDir, gameObject);
        spellDir = Vector2.zero;
    }

    private void InvokeSpell(int spellIndex)
    {
        if (canAttack)
        {
            invokeParticles.Play();
            isHolding = true;
            invoking = true;
        }
    }

    private void ThrowSpell(int _spellIndex)
    {
        if (canAttack)
        {
            isHolding = false;
            spellIndex = _spellIndex;
            spellDir = playerMovement.aimDirection;
            lastHandPos = handPos.position;
            anim.PlaySpellAnim(spellManager.GetSpellByIdx(spellIndex));
            invokeParticles.Stop();
            canAttack = !canAttack;
            invoking = false;
            aiming = false;
        }
    }

    private void UpdateSpellDir()
    {
        if (isHolding && invoking)
        {
            aiming = playerMovement.aimDirection != Vector2.zero;
            if (aiming)
                spellDir = playerMovement.aimDirection;
            UpdateArrow();
        }
    }

    private void GetSpellsInputs(string input,int spellIndex)
    {
        if (Input.GetButtonDown(input) && spellManager.CanInvokeSpell(spellIndex))
        {
            if (!invoking && canAttack)
            {
                if (spellManager.GetSpellCastType(spellIndex) == Spell.CastType.OneTap)
                {
                    arrowSprite.gameObject.SetActive(false);
                    ThrowSpell(spellIndex);
                }
                else if (spellManager.GetSpellCastType(spellIndex) == Spell.CastType.Hold)
                {
                    arrowSprite.gameObject.SetActive(true);
                    InvokeSpell(spellIndex);
                }
                spellDir = new Vector2(playerMovement.currentDirection, 0);
            }
        }
        if (Input.GetButtonUp(input) && spellManager.GetSpellCastType(spellIndex) == Spell.CastType.Hold && invoking)
        {
            arrowSprite.gameObject.SetActive(false);
            ThrowSpell(spellIndex);
        }
    }

    public void SetCanAttack(bool val)
    {
        canAttack = val;
    }

    public void SetActive(bool v)
    {
        enabled = v;
    }

}

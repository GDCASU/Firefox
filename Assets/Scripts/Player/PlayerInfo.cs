﻿/*
 * Handles the status of the Player, including permanent attributes and temporary effects.
 * 
 * NOTE: All attributes are public, so they can be changed permanently.
 * 
 * Author: Cristion Dominguez
 * Date: 22 Jan. 2021
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A list of possible status effects that can be cast onto the Player.
public enum StatusEffects
{
    FOCUS,  // alter bullet damage
    FORITIFY,  // alter defense
    FRENZY,  // alter fire rate
    VULNERABILITY  // alter incoming damage
}

public class PlayerInfo : MonoBehaviour
{
    public static PlayerInfo singleton;

    public int maxHealth = 60;  // health Player should not surpass
    public int health = 50;  // current health of Player

    public float defense;  // how much damage should be shaved off; EXAMPLE: 0.25 = 25% damage reduced

    public float fireRate = 0.2f;  // how fast the Player can shoot

    public int bulletDamage;  // how much damage a Player bullet should deal

    // Placeholders for original values of certain attributes altered by status effects.
    private float originalDefense;
    private float originalFireRate;
    private int originalBulletDamage;

    private float damageMultiplier = 1f;  // amount to multiply incoming damage by

    private Dictionary<StatusEffects, IEnumerator> statuses = new Dictionary<StatusEffects, IEnumerator>();  // a list of status effects and their respective coroutines
                                                                                                             // that shall deactive them once a certain time is reached
    /// <summary>
    /// Upon loading script, transforms script into a singleton and saves original values of certain attributes.
    /// </summary>
    private void Awake()
    {
        if (singleton == null)
            singleton = this;
        else
            Destroy(gameObject);

        originalDefense = defense;
        originalFireRate = fireRate;
        originalBulletDamage = bulletDamage;
    }

    #region Health Functions
    /// <summary>
    /// Removes health amount by a combination of incoming damage, the damage multipler, and defense.
    /// </summary>
    /// <param name="damage"> incoming damage </param>
    public void TakeDamage(int damage)
    {
        health -= Mathf.RoundToInt((float)damage * damageMultiplier * (1f - defense));
        if (health <= 0)
        {
            KillPlayer();
        }
    }

    /// <summary>
    /// Increases health amount by heal value without succeeding max health.
    /// </summary>
    /// <param name="heal"> amount to increase health </param>
    public void Heal(int heal)
    {
        if (health + heal > maxHealth) health = maxHealth;
        else health += heal;
    }

    /// <summary>
    /// Destroys Player gameobject.
    /// </summary>
    public void KillPlayer()
    {
        //Destroy(gameObject);
    }
    #endregion

    #region Status Effect Functions
    /// <summary>
    /// Adds or resets a status effect on the gameobject for a specified amount of time.
    /// </summary>
    /// <param name="effect"> a status effect </param>
    /// <param name="effectValue"> value that shall alter a certain attribute </param>
    /// <param name="activeTime"> time the effect should last </param>
    public void AddStatusEffect(StatusEffects effect, float effectValue, float activeTime)
    {
        // If the gameobject already has a certain status effect, then stop the coroutine currently attached to that effect and initiate a new one.
        // Otherwise, add the status effect to the statuses list and start the coroutine.
        if (statuses.ContainsKey(effect))
        {
            StopCoroutine(statuses[effect]);
            statuses[effect] = StartEffectLifecycle(effect, effectValue, activeTime);
        }
        else
            statuses.Add(effect, StartEffectLifecycle(effect, effectValue, activeTime));

        // Start the life cycle of the status effect.
        StartCoroutine(statuses[effect]);
    }

    /// <summary>
    /// Applies the status effect to the Player until active time is reached, at which point the status effect shall be removed from the Player (statuses list).
    /// </summary>
    /// <param name="effect"> a status effect </param>
    /// <param name="effectValue"> value that shall alter a certain attribute </param>
    /// <param name="activeTime"> time the effect should last </param>
    /// <returns></returns>
    private IEnumerator StartEffectLifecycle(StatusEffects effect, float effectValue, float activeTime)
    {
        AffectPlayer(effect, effectValue, false);  // alter Player attributes
        yield return new WaitForSeconds(activeTime);
        AffectPlayer(effect, effectValue, true);  // return Player attributes to normal
        statuses.Remove(effect);  // remove status effect from statuses list
    }

    /// <summary>
    /// Alters Player attributes based on the status effect. If the status effect is new (has not ended), then modify the specified attribute value(s) by effect value;
    /// otherwise, restore the original attribute value(s).
    /// </summary>
    /// <param name="effect"> a status effect </param>
    /// <param name="effectValue"> value that shall alter a certain attribute </param>
    /// <param name="hasEnded"> Has the status effect ended? </param>
    private void AffectPlayer(StatusEffects effect, float effectValue, bool hasEnded)
    {
        switch (effect)
        {
            // Alter damage.
            case StatusEffects.FOCUS:
                if (hasEnded)
                    bulletDamage = originalBulletDamage;
                else
                    bulletDamage = (int) effectValue;
                break;

            // Alter defense.
            case StatusEffects.FORITIFY:
                if (hasEnded)
                    defense = originalDefense;
                else
                    defense = effectValue;
                break;
            
            // Alter fire rate.
            case StatusEffects.FRENZY:
                if (hasEnded)
                    fireRate = originalFireRate;
                else
                    fireRate = effectValue;
                break;
           
            // Alter damage multiplier.
            case StatusEffects.VULNERABILITY:
                if (hasEnded)
                    damageMultiplier = 1f;
                else
                    damageMultiplier = effectValue;
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Answers whether the Player possesses a certain status effect.
    /// </summary>
    /// <param name="effect"> a status effect </param>
    /// <returns> a true or false </returns>
    public bool HasStatusEffect(StatusEffects effect) => statuses.ContainsKey(effect);

    /// <summary>
    /// Removes an existing status effect from Player. Returns 1 if status effect has been successfully removed and returns 0 if status effect
    /// did not exist.
    /// </summary>
    /// <param name="effect"> a status effect </param>
    /// <returns> 1 for successful removal; 0 for status effect not existing </returns>
    public int RemoveStatusEffect(StatusEffects effect)
    {
        if (statuses.ContainsKey(effect))
        {
            StopCoroutine(statuses[effect]);
            AffectPlayer(effect, 0, true);  // return Player attributes to normal
            statuses.Remove(effect);  // remove status effect from statuses list
            return 1;
        }

        return 0;
    }
    #endregion
}
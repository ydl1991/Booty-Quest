using System;
using System.Collections.Generic;
using UnityEngine;

//==========================================================================================================
// Set target on fire cannon
//==========================================================================================================
public class FlameCannon : Cannon
{
    FlameCannon()
    {
        m_weight = GameplayManager.kFlameCannonWeight;
        m_damage = GameplayManager.kFlameCannonDamage;
    }

    //----------------------------------------------------------------------------
    // Set other on fire for fixed time
    //----------------------------------------------------------------------------
    protected override void SpecialEffects()
    {
        // Attempt one: 
        //  Create a flame debuff component, add that to target, diminish target's health, remove component when time's up
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStatsApply : ASkill
{
    public void Apply(GameObject target, WeaponItem weapon)
    {
        PlayerStats player = target.GetComponent<PlayerStats>();
        player.attack += weapon.attack;
        player.critChance += weapon.critChance;
        if (player.critChance > 100)
            player.critChance = 100;
        player.defense += weapon.defense;
    }

    public void Remove(GameObject target, WeaponItem weapon)
    {
        PlayerStats player = target.GetComponent<PlayerStats>();
        player.attack -= weapon.attack;
        player.critChance -= weapon.critChance;
        if (player.critChance < -100)
            player.critChance = -100;
        player.defense -= weapon.defense;        
    }

    public override void Apply(GameObject target)
    {

    }

    public override void Remove(GameObject target)
    {
        
    }
}

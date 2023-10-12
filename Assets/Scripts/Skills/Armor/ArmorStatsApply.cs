using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorStatsApply : ASkill
{
    public void Apply(GameObject target, ArmorItem armor)
    {
        PlayerStats player = target.GetComponent<PlayerStats>();
        player.defense += armor.defense;
        foreach(var skill in armor.skills)
        {
            skill.Apply(target);
        }
    }

    public void Remove(GameObject target, ArmorItem armor)
    {
        PlayerStats player = target.GetComponent<PlayerStats>();
        player.defense -= armor.defense;
        foreach(var skill in armor.skills)
        {
            skill.Remove(target);
        }
    }

    public override void Apply(GameObject target)
    {

    }

    public override void Remove(GameObject target)
    {

    }
}

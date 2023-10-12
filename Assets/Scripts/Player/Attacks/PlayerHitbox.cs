using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    public enum attackType
    {
        Cut,
        Blunt,
        Projectile
    }
    public attackType hitType;
    public int motionValue = 100;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Hitbox"))
        {
            GetComponentInParent<WeaponManager>().InflictDamage(other.GetComponent<Hurtbox>(), other.gameObject.GetComponentInParent<MonsterHealth>(), hitType, motionValue);
        }
    }
}

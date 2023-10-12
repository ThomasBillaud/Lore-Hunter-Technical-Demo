using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Hurtbox : MonoBehaviour
{
    public int cutHitzoneWeakness = 50;
    public int bluntHitzoneWeakness = 50;
    public int projHitzoneWeakness = 50;
}

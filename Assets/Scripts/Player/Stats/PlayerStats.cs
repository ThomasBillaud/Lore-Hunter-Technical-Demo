using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public Health health;
    public Stamina stamina;
    public int attack = 1;
    public int critChance = 0;
    public int defense = 1;

    public TextMeshProUGUI DebugDisplayAttack;
    public TextMeshProUGUI DebugDisplayDefense;

	private void Update()
	{
        DebugDisplayAttack.text = "Attack : " + attack;
        DebugDisplayDefense.text = "Defense : " + defense;
	}
}

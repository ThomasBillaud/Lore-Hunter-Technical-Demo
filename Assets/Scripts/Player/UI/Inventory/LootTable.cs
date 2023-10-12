using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LootTable : MonoBehaviour
{
    public List<Loot> lootTable;
    private float totalWeight;

    private void Awake() {
        totalWeight = lootTable.Sum(item => item.percentage);
    }

    public Loot DiceRoll() {
        float diceRoll = Random.Range(0f, totalWeight);
        foreach(Loot item in lootTable)
        {
            if (item.percentage >= diceRoll)
                return item;
            diceRoll -= item.percentage;
        }
        throw new System.Exception("Reward generation failed !");
    }
}

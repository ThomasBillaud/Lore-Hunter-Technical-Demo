using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorSlot : MonoBehaviour
{
    public Transform parentOverride;
    public GameObject[] currentModel = new GameObject[5];
    public Transform rootBone;

    public int ChooseArmorPiece(ArmorItem.armorType type)
    {
        switch(type)
        {
            case ArmorItem.armorType.Helmet:
                return 0;
            case ArmorItem.armorType.Torso:
                return 1;
            case ArmorItem.armorType.Gauntlets:
                return 2;
            case ArmorItem.armorType.Belt:
                return 3;
            case ArmorItem.armorType.Legs:
                return 4;
            default:
                return 5;
        }
    }

    public void UnloadArmorPiece(ArmorItem.armorType type)
    {
        int piece = ChooseArmorPiece(type);
        if (currentModel[piece] != null)
            currentModel[piece].SetActive(false);
    }

    public void UnloadArmorPieceAndDestroy(ArmorItem.armorType type)
    {
        int piece = ChooseArmorPiece(type);

        if (currentModel[piece] != null)
            Destroy(currentModel[piece]);
    }

    public void LoadArmorPieceModel(ArmorItem armorItem)
    {
        UnloadArmorPieceAndDestroy(armorItem.type);
        int piece = ChooseArmorPiece(armorItem.type);

        if (armorItem == null)
        {
            UnloadArmorPiece(armorItem.type);
            return;
        }

        GameObject model = Instantiate(armorItem.modelPrefab) as GameObject;
        
        if (model != null)
        {
            if (parentOverride != null)
            {
                model.transform.parent = parentOverride;
            } else {
                model.transform.parent = transform;
            }
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
            model.transform.localScale = Vector3.one;

            SkinnedMeshRenderer mesh = model.GetComponentInChildren<SkinnedMeshRenderer>();
            Transform[] newBones = new Transform[mesh.bones.Length];
            mesh.rootBone = rootBone;
            for (int i = 0; i < mesh.bones.Length; i++)
            {
                foreach (Transform newBone in rootBone.GetComponentsInChildren<Transform>())
                {
                    if (newBone.name == mesh.bones[i].name)
                    {
                        newBones[i] = newBone;
                        continue;
                    }
                }
            }
            mesh.bones = newBones;
        }
        currentModel[piece] = model;
    }


}

using UnityEngine;
using System.Collections.Generic;

public class SkinnedMeshReplacement : MonoBehaviour
{
    public SkinnedMeshRenderer sourceRenderer; // le renderer source
    public SkinnedMeshRenderer targetRenderer; // le renderer cible

    [System.Serializable]
    public struct BoneReplacement
    {
        public Transform sourceBone;
        public Transform targetBone;
    }

    public List<BoneReplacement> boneReplacements = new List<BoneReplacement>(); // liste des correspondances de bones

    void Start()
    {
        // Crée un tableau pour stocker les bones correspondants
        Transform[] boneArray = new Transform[sourceRenderer.bones.Length];

        // Boucle à travers les bones du SkinnedMeshRenderer source
        for (int i = 0; i < sourceRenderer.bones.Length; i++)
        {
            // Recherche le bone correspondant dans la liste de correspondances
            BoneReplacement replacement = boneReplacements.Find(r => r.sourceBone == sourceRenderer.bones[i]);

            if (replacement.targetBone != null)
            {
                // Si un bone correspondant est trouvé, l'ajoute au tableau de bones correspondants
                boneArray[i] = replacement.targetBone;
            }
            else
            {
                Debug.LogError($"Le bone {sourceRenderer.bones[i].name} n'a pas de correspondance dans la liste de correspondances.");
            }
        }

        // Copie la configuration de bind pose et les bones à partir du SkinnedMeshRenderer source
        targetRenderer.rootBone = sourceRenderer.rootBone;
        targetRenderer.bones = boneArray;
        targetRenderer.sharedMesh.bindposes = sourceRenderer.sharedMesh.bindposes;

        // Calcule la mesh dans la configuration de bind pose cible
        Mesh newMesh = new Mesh();
        sourceRenderer.BakeMesh(newMesh);

        // Assigne la nouvelle mesh au SkinnedMeshRenderer cible
        targetRenderer.sharedMesh = newMesh;
    }
}

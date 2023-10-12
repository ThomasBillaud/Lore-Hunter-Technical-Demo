using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class Description : MonoBehaviour
{
    public Image itemIcon;
    public LocalizeStringEvent itemName;
    public LocalizeStringEvent description;

    public void ChangeDescription(ItemData item)
    {

        if (item != null)
        {
            itemIcon.sprite = item.itemIcon;
            itemName.StringReference.TableEntryReference = item.itemName;
            itemName.RefreshString();
            description.StringReference.TableEntryReference = item.descriptionKey;
            description.RefreshString();
        } else
        {
            itemIcon.sprite = null;
            itemName.StringReference.TableEntryReference = "Empty";
            itemName.RefreshString();
            description.StringReference.TableEntryReference = "Empty";
            description.RefreshString();
        }
    }
}

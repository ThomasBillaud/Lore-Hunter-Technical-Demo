using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapWindow : MonoBehaviour
{
    public Transform border;
    private Transform player;
    private static MinimapWindow instance;

    private void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player").transform;
    }

    void LateUpdate()
    {
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;

        transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
        border.rotation = Quaternion.Euler(0f, 0f, player.eulerAngles.y);
    }

    //public static void Show()
    //{
    //    instance.gameObject.SetActive(true);
    //}
//
    //public static void Hide()
    //{
    //    instance.gameObject.SetActive(false);
    //}
}

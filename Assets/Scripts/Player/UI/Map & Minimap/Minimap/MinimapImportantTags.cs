using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class MinimapImportantTags : MonoBehaviour
{
    public Transform minimapCamera;
    public float minimapSize;
    Vector3 temp;
    private MapManager map;
    private PositionConstraint constraint;

    void Start()
    {
        map = FindObjectOfType<MapManager>();
        constraint = GetComponent<PositionConstraint>();
    }

    void LateUpdate()
    {
        temp = transform.parent.transform.position;
        temp.y = transform.position.y;
        transform.position = temp;

        if (map.isMapOpen == false)
        {
            if (constraint.constraintActive == true)
                constraint.constraintActive = false;
            Vector3 centerPosition = minimapCamera.transform.localPosition;
            centerPosition.y = 1f;

            float distance = Vector3.Distance(transform.position, centerPosition);

            if (distance > minimapSize)
            {
                Vector3 fromOriginToObject = transform.position - centerPosition;
                fromOriginToObject *= minimapSize / distance;
                transform.position = centerPosition + fromOriginToObject;
                transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
            }
        } else {
            GetComponent<PositionConstraint>().constraintActive = true;
        }
    }
}

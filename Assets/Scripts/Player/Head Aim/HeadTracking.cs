using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class HeadTracking : MonoBehaviour
{
    public Transform Target;
    public float Radius = 10f;
    public float retargetSpeed = 5f;
    public float maxAngle = 75f;
    public Rig headRig;
    List<PointOfInterest> POIs;
    float RadiusSqr;
    bool isRigged = true;

    // Start is called before the first frame update
    void Start()
    {
        POIs = FindObjectsOfType<PointOfInterest>().ToList();
        RadiusSqr = Radius * Radius;
    }

    // Update is called once per frame
    void Update()
    {
        if (isRigged)
        {
            Transform tracking = null;
            foreach(PointOfInterest poi in POIs)
            {
                Vector3 delta = poi.transform.position - transform.position;
                if (delta.sqrMagnitude < RadiusSqr)
                {
                    float angle = Vector3.Angle(transform.forward, delta);
                    if (angle < maxAngle)
                    {
                        tracking = poi.transform;
                        break;
                    }
                }
            }
            float rigWeight = 0;
            Vector3 targetPos = transform.position + (transform.forward * 2f);
            if(tracking != null)
            {
                targetPos = tracking.position;
                rigWeight = 1;
            }
            Target.position = Vector3.Lerp(Target.position, targetPos, Time.deltaTime * retargetSpeed);
            headRig.weight = Mathf.Lerp(headRig.weight, rigWeight, Time.deltaTime);
        }
    }

    public void StartHeadRig()
    {
        headRig.weight = Mathf.Lerp(headRig.weight, 1, Time.deltaTime);
        isRigged = true;
    }

    public void StopHeadRig()
    {
        headRig.weight = Mathf.Lerp(headRig.weight, 0, Time.deltaTime);
        isRigged = false;
    }
}

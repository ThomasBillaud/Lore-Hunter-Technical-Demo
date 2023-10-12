using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static void CrossFade(this Animator animator, CrossFadeSettings settings)
    {
        animator.CrossFade(settings.stateName, settings.transitionDuration, settings.layer, settings.timeOffset);
    }

    public static void CrossFadeInFixedTime(this Animator animator, CrossFadeSettings settings)
    {
        animator.CrossFadeInFixedTime(settings.stateName, settings.transitionDuration, settings.layer, settings.timeOffset);
    }
}

using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    [SpineAnimation]
    public string deathAimingAnimationName;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        skeletonAnimation.AnimationState.SetAnimation(0, deathAimingAnimationName, false);
        StartCoroutine(Destroy());
    }
    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(1.6f);
        Destroy(gameObject);
    }
}

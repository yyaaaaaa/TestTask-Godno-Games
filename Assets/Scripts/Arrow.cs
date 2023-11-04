using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SpineAnimation]
    public string attackAnimationName;
    public SkeletonAnimation skeletonAnimation;
    private Rigidbody2D rb;
    private bool hit = false;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if (!hit)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }      
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        StartCoroutine(Collision());
    }

    IEnumerator Collision()
    {
        hit = true;
        gameObject.transform.rotation = Quaternion.identity;
        rb.bodyType = RigidbodyType2D.Static;
        skeletonAnimation.AnimationState.SetAnimation(0, attackAnimationName, false);
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}

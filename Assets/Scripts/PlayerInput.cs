using Spine.Unity;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    #region Names
    [SpineAnimation]
    public string startAimingAnimationName;

    [SpineAnimation]
    public string targetAnimationName;

    [SpineAnimation]
    public string endAimingAnimationName;

    [SpineAnimation]
    public string idleAnimationName;

    #endregion

    public SkeletonAnimation skeletonAnimation;
    public BowController bowController;

    private void Update()
    {
        HandleInput();
        bowController.UpdateTrajectory();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) && !bowController.isDragging)
        {
            bowController.StartAiming();
            StartAimingAnim();
        }
        else if (Input.GetMouseButton(0) && bowController.isDragging)
        {
            bowController.Aiming();
        }
        else if (Input.GetMouseButtonUp(0) && bowController.isDragging)
        {
            bowController.Shoot();
            ShootAnim();
        }
    }

    void StartAimingAnim()
    {
        skeletonAnimation.AnimationState.SetAnimation(0, startAimingAnimationName, false);
        skeletonAnimation.AnimationState.AddAnimation(0, targetAnimationName, true, 0f);
    }
    void ShootAnim()
    {
        skeletonAnimation.AnimationState.SetAnimation(0, endAimingAnimationName, false);
        skeletonAnimation.AnimationState.AddAnimation(0, idleAnimationName, true, 0f);
    }

}

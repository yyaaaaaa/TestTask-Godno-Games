using Spine;
using Spine.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BowController : MonoBehaviour
{
    #region Inspector
    public GameObject arrowPrefab;
    public Transform arrowSpawnPoint;
    public float maxDragDistance = 3f;
    public int maxTrajectoryPoints = 30;
    public float stepDistance;
    public GameObject trajectoryPointPrefab;
    public bool isDragging = false;
    public SkeletonAnimation skeletonAnimation;
    [SpineBone(dataField: "skeletonAnimation")]
    public string boneName;
    #endregion
    #region Private fields 
    private Vector2 startPoint;
    private Vector2 endPoint;
    private float shootingPower;
    private GameObject[] trajectoryPoints;
    private Rigidbody2D arrowRigidbody;
    private Camera maincam;
    Vector2 direction;
    [SerializeField]
    private float offsetX;
    [SerializeField]
    private float offsetY;
    Bone bone;
    #endregion
    void Start()
    {
        bone = skeletonAnimation.Skeleton.FindBone(boneName);
        maincam = Camera.main;
        trajectoryPoints = new GameObject[maxTrajectoryPoints];
        for (int i = 0; i < maxTrajectoryPoints; i++)
        {
            trajectoryPoints[i] = Instantiate(trajectoryPointPrefab, transform.position, Quaternion.identity);
            trajectoryPoints[i].SetActive(false);
        }
    }

    public void Shoot()
    {
        isDragging = false;
        arrowRigidbody.GetComponent<MeshRenderer>().enabled = true;
        arrowRigidbody.bodyType = RigidbodyType2D.Dynamic;  
        arrowRigidbody.velocity = direction * shootingPower;
        startPoint = Vector2.zero;
        endPoint = Vector2.zero;
        shootingPower = 0f;
        HideTrajectory();
    }

    public void Aiming()
    {
        endPoint = maincam.ScreenToWorldPoint(Input.mousePosition);
        shootingPower = Mathf.Clamp(Vector2.Distance(startPoint, endPoint) * 2f, 0f, maxDragDistance);
        direction = (startPoint - endPoint).normalized;
      
        float angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) % 360f;
        bone.Rotation = angle;

        Vector3 newPosition = Quaternion.Euler(0, 0, angle) * Vector3.right;
        newPosition += new Vector3(offsetX, offsetY, 0); 
        arrowSpawnPoint.transform.localPosition = newPosition;

    }

    public void StartAiming()
    {
        startPoint = maincam.ScreenToWorldPoint(Input.mousePosition);
        isDragging = true;
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.identity);
        arrow.GetComponent<MeshRenderer>().enabled = false;
        arrowRigidbody = arrow.GetComponent<Rigidbody2D>();
        arrowRigidbody.bodyType = RigidbodyType2D.Static;
    }

    public void UpdateTrajectory()
    {
        if (isDragging)
        {
            Vector2[] pointsPosition = Plot(arrowRigidbody,arrowSpawnPoint.position, direction*shootingPower, maxTrajectoryPoints);
            float pointSizeStep = (0.3f - 0.1f) / maxTrajectoryPoints;

            for (int i = 0; i < maxTrajectoryPoints; i++)
            {
                trajectoryPoints[i].transform.position = pointsPosition[i];
                trajectoryPoints[i].SetActive(true);

                float pointSize = Mathf.Lerp(0.3f, 0.1f, i * pointSizeStep); // Размеры точек траектории уменьшаются к концу траектории
                trajectoryPoints[i].transform.localScale = new Vector3(pointSize, pointSize, 1f);
            }

            for (int i = maxTrajectoryPoints; i < trajectoryPoints.Length; i++)
            {
                trajectoryPoints[i].SetActive(false);
            }
        }
        else
        {
            HideTrajectory();
        }
    }

    void HideTrajectory()
    {
        foreach (var point in trajectoryPoints)
        {
            point.SetActive(false);
        }
    }


    private Vector2[] Plot(Rigidbody2D rigidbody, Vector2 pos, Vector2 velocity, int steps)
    {
        Vector2[] result = new Vector2[steps];
        float timestep = Time.fixedDeltaTime / Physics2D.velocityIterations * stepDistance;
        Vector2 gravityAcceleration = Physics2D.gravity * rigidbody.gravityScale * timestep * timestep;

        float drag = 1f - timestep * rigidbody.drag;
        Vector2 movestep = velocity * timestep;
        for (int i = 0; i < steps; i++)
        {
            movestep += gravityAcceleration;
            movestep *= drag;
            pos += movestep;
            result[i] = pos;
        }
        return result;
    }


}

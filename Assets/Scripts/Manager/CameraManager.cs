using Unity.Mathematics;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    float m_Distance = 10f;
    [Range(0f, 10f)]
    [SerializeField] float distanceVal;
    float base_FOV = 50f;
    Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, Distance() + base_FOV, Time.deltaTime * 4f);
    }

    float Distance()
    {
        var players = Spawner.m_Players.ToArray();
        float maxDistance  = m_Distance;

        foreach(var player in players)
        {
            float targetDistance = Vector3.Distance(Vector3.zero, player.transform.position) + distanceVal;
            if(targetDistance > maxDistance)
            {
                maxDistance = targetDistance;
            }
        }
        return maxDistance;
    }
}

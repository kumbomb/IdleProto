using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance = null;

    float m_Distance = 10f;
    [Range(0f, 10f)]
    [SerializeField] float distanceVal;
    
    [Space(20f)]
    [Header("Camera Shake")]
    Vector3 originCamPos;   // 카메라 원래 위치
    bool isCamShake = false;        // 카메라 흔들기 동작중인지 체크
    [Range(0f,10f)]
    [SerializeField] float shakeDuration;
    [Range(0f,10f)]
    [SerializeField] float shakePower;

    float base_FOV = 50f;
    Camera cam;

    private void Awake() 
    {
        if(instance == null) instance = this;
    }

    private void Start()
    {
        cam = GetComponent<Camera>();
        originCamPos = transform.localPosition;
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

#region  Camera Shake
    public void CameraShake()
    {
        if(isCamShake) return;
        isCamShake = true;
        StartCoroutine(Co_CameraShake());
    }
    IEnumerator Co_CameraShake()
    {
        float timer = 0f;
        while (timer <= shakeDuration)
        {
            transform.localPosition = Random.insideUnitSphere * shakePower + originCamPos;
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originCamPos;
        isCamShake = false;
    }
#endregion
}

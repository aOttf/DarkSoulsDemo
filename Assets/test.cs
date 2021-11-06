using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    private bool tmp2;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        tmp2 = Input.GetKeyDown(KeyCode.A);
    }

    private void FixedUpdate()
    {
        bool tmp = Input.GetKeyDown(KeyCode.A);
        print(tmp == tmp2);
    }

    //private Vector3 Convert2PointOnCircle(Vector3 origin, float radius, Vector3 pos)
    //{
    //    return (origin + (pos - origin).normalized * radius);
    //}

    //// Damping Function for rotating camera
    //private static Vector3 SmoothCircleDamp(Vector3 origin, float radius, Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime, [DefaultValue("Mathf.infinity")] float maxSpeed, [DefaultValue("Time.deltaTime")] float deltaTime)
    //{
    //    smoothTime = Mathf.Max(0.0001f, smoothTime);
    //    float num = 2f / smoothTime;
    //    float num2 = num * deltaTime;
    //    float num3 = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
    //    Vector3 vec4 = current - target;
    //    Vector3 vec5 = target;
    //    float num6 = maxSpeed * smoothTime;
    //    vec4 = Vector3.ClampMagnitude(vec4, num6);
    //    target = current - vec4;
    //    Vector3 vec7 = (currentVelocity + num * vec4) * deltaTime;
    //    currentVelocity = (currentVelocity - num * vec7) * num3;
    //    Vector3 vec8 = target + (vec4 + vec7) * num3;
    //    if ((vec5 - current).magnitude > 0f == vec8.magnitude > vec5.magnitude)
    //    {
    //        vec8 = vec5;
    //        currentVelocity = (vec8 - vec5) / deltaTime;
    //    }

    //    return vec8;
    //}
}
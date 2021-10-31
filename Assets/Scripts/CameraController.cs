using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Components")]
    public GameObject playerHandler;

    public GameObject model;
    public PlayerInput pi;
    public Camera cam;

    [Header("Moving Speed")]
    public float horiSpd = 20.0f;

    public float vertSpd = 20.0f;

    [Header("Adjust Variables")]
    public float smoothFlwFac; //Smooth Factor for camera following

    public float smoothHoriFac;
    public float smoothVertFac;

    private float tgtLocalXEuler = 0f; //current x value of localeularangle of the camera
    private float curLocalXEuler = 0f;
    private float xEulerRef;

    private float tgtWorldYEuler = 0f;
    private float curWorldYEuler = 0f;
    private float yEulerRef;

    private float modelYEuler;  //current y value of eularangle of the model in world space

    private Vector3 camSpd;   //current speed of camera, used for damp function

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        //Unmove the face of the model
        model.transform.eulerAngles = new Vector3(model.transform.eulerAngles.x,
            modelYEuler, model.transform.eulerAngles.z);
    }

    private void FixedUpdate()
    {
        //get current model orientation
        modelYEuler = model.transform.eulerAngles.y;

        //targetYEulerDiff += pi.DJright * horiSpd * Time.deltaTime;
        //YEulerDiff = Mathf.SmoothDamp(YEulerDiff, targetYEulerDiff, ref diffSpd, smoothFac);
        ////Horizontal Rotation of the camera position
        //playerHandler.transform.Rotate(Vector3.up, YEulerDiff);

        //Vertical Rotation of the camera position
        tgtLocalXEuler -= pi.DJup * vertSpd * Time.deltaTime;
        //Limit the range of the vertical rotation
        tgtLocalXEuler = Mathf.Clamp(tgtLocalXEuler, -40, 40);
        curLocalXEuler = Mathf.SmoothDamp(curLocalXEuler, tgtLocalXEuler, ref xEulerRef, smoothVertFac);
        transform.localEulerAngles = new Vector3(curLocalXEuler, 0, 0);

        tgtWorldYEuler -= pi.DJright * horiSpd * Time.fixedDeltaTime;
        curWorldYEuler = Mathf.SmoothDamp(curWorldYEuler, tgtWorldYEuler, ref yEulerRef, smoothHoriFac);
        playerHandler.transform.localEulerAngles = new Vector3(0, curWorldYEuler, 0);

        //Damp the camera to the position
        cam.transform.position = Vector3.SmoothDamp(cam.transform.position, transform.position, ref camSpd, smoothFlwFac);
        cam.transform.eulerAngles = transform.eulerAngles;
    }
}
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

    [Header("===== Camera Movement Key Settings ======")]
    public string keyJUp;

    public string keyJDown;
    public string keyJLeft;
    public string keyJRight;

    [Header("===== Camera Control Signals =====")]
    [SerializeField]
    private float DJup = 0f;

    [SerializeField]
    private float DJright = 0f;

    [Header("Moving Speed")]
    public float horiSpd = 20.0f;

    public float vertSpd = 20.0f;

    [Header("Adjust Variables")]
    public float camDist;

    public float smoothFlwFac; //Smooth Factor for camera following

    public float smoothHoriFac;
    public float smoothVertFac;

    private float tgtWorldXEuler = 0f; //current x value of localeularangle of the camera
    private float curLocalXEuler = 0f;
    private float xEulerRef;

    private float tgtWorldYEuler = 0f;
    private float curWorldYEuler = 0f;
    private float yEulerRef;

    private float modelYEuler;  //current y value of eularangle of the model in world space

    private Vector3 camSpd;   //current speed of camera, used for damp function
    private Vector3 TgtCamPos => transform.position - transform.forward * camDist;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        //Camera Movement Input
        DJup = Input.GetKey(keyJUp) ? 1 : 0 - (Input.GetKey(keyJDown) ? 1 : 0);
        DJright = Input.GetKey(keyJRight) ? 1 : 0 - (Input.GetKey(keyJLeft) ? 1 : 0);
    }

    private void FixedUpdate()
    {
        RotatePlayerHandler();
        RotateCameraHandler();
        MoveCamera();
    }

    private void RotateCameraHandler()
    {
        //Vertical Rotation
        tgtWorldXEuler -= DJup * vertSpd * Time.deltaTime;
        tgtWorldXEuler = Mathf.Clamp(tgtWorldXEuler, -40, 40);
        curLocalXEuler = Mathf.SmoothDamp(curLocalXEuler, tgtWorldXEuler, ref xEulerRef, smoothVertFac);
        transform.localEulerAngles = new Vector3(curLocalXEuler, 0, 0);
    }

    private void MoveCamera()
    {
        cam.transform.position = Vector3.SmoothDamp(cam.transform.position, TgtCamPos, ref camSpd, smoothFlwFac);
        cam.transform.eulerAngles = transform.eulerAngles;
    }

    private void RotatePlayerHandler()
    {
        //Horizontal Rotation
        tgtWorldYEuler -= DJright * horiSpd * Time.fixedDeltaTime;
        curWorldYEuler = Mathf.SmoothDamp(curWorldYEuler, tgtWorldYEuler, ref yEulerRef, smoothHoriFac);

        //get current model horizontal orientation
        modelYEuler = model.transform.eulerAngles.y;
        playerHandler.transform.localEulerAngles = new Vector3(0, curWorldYEuler, 0);
        //Unmove the face of the model
        model.transform.eulerAngles = new Vector3(model.transform.eulerAngles.x,
            modelYEuler, model.transform.eulerAngles.z);
    }
}
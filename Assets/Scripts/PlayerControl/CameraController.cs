using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Internal;

public class CameraController : MonoBehaviour
{
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

    private Vector3 camSpd;   //current speed of camera, used for damp function
    private Vector3 TgtCamPos => transform.position - transform.forward * camDist;

    private Vector3 playerPos;

    // Start is called before the first frame update
    private void Start()
    {
        playerPos = playerHandler.transform.position;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        //If character just moved in a physical frame, adjust the camera position proportionally
        AdjustCameraPosition();

        //Camera Movement Input
        DJup = Input.GetKey(keyJUp) ? 1 : 0 - (Input.GetKey(keyJDown) ? 1 : 0);
        DJright = Input.GetKey(keyJRight) ? 1 : 0 - (Input.GetKey(keyJLeft) ? 1 : 0);

        //Horizontal Rotation
        RotatePlayerHandler();
        //Vertical Rotation
        RotateCameraHandler();
        //Lerp Camera Position
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

    private void RotatePlayerHandler(float scalar = 1f)
    {
        //Horizontal Rotation
        tgtWorldYEuler -= DJright * horiSpd * Time.deltaTime * scalar;
        curWorldYEuler = Mathf.SmoothDamp(curWorldYEuler, tgtWorldYEuler, ref yEulerRef, smoothHoriFac);

        //get current model horizontal orientation
        Vector3 tmpEuler = model.transform.eulerAngles;

        playerHandler.transform.localEulerAngles = new Vector3(0, curWorldYEuler, 0);

        //Unmove the face of the model
        model.transform.eulerAngles = tmpEuler;
    }

    private void AdjustCameraPosition()
    {
        Vector3 deltaPos = playerHandler.transform.position - playerPos;
        playerPos = playerHandler.transform.position;
        cam.transform.position += deltaPos;
    }
}
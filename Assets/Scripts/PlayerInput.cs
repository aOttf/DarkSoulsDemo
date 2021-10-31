using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [Header("===== Player Movement Key Settings =====")]
    public string keyUp = "w";

    public string keyDown = "s";
    public string keyLeft = "a";
    public string keyRight = "d";

    [Header("===== Camera Movement Key Settings ======")]
    public string keyJUp;

    public string keyJDown;
    public string keyJLeft;
    public string keyJRight;

    [Header("===== Ability Key Settings ======")]
    public string keyShift = "leftshift";

    public string keySpace;
    public string keyMouse0;
    public string keyMouse1;

    [Header("===== Intermediate Singals =====")]
    [SerializeField]
    private float Dup = 0f;

    [SerializeField]
    private float Dright = 0f;

    [SerializeField]
    public float DJup = 0f;

    [SerializeField]
    public float DJright = 0f;

    [Header("===== Output Signals =====")]
    public float Mmag;    //Movement magnitude

    public Vector3 Dvec;    //Directional vector

    public bool runSignal;
    public bool jrbSignal;  //Jump, Roll and BackJump; Triggered by jump key
    public bool attackSignal;

    //trigger once signal

    //double trigger

    [Header("===== Others =====")]
    //Variables for Smoothdamp function
    private float targetDup;

    private float targetDright;
    private float velocityDup;
    private float velocityDright;

    public float smoothTime = 1f;

    public bool isLocked = false;

    // Start is called before the first frame update
    private void Start()
    {
        print(transform.position);
        Vector3 tmp = transform.position;
        tmp += new Vector3(1, 10, 1);
        print(transform.position);
        print(tmp);
    }

    // Update is called once per frame
    private void Update()
    {
        //Handle Player Inputs

        //Camera Movement Input
        DJup = Input.GetKey(keyJUp) ? 1 : 0 - (Input.GetKey(keyJDown) ? 1 : 0);
        DJright = Input.GetKey(keyJRight) ? 1 : 0 - (Input.GetKey(keyJLeft) ? 1 : 0);

        //catch movement input
        targetDup = Input.GetKey(keyUp) ? 1 : 0 - (Input.GetKey(keyDown) ? 1 : 0);
        targetDright = Input.GetKey(keyRight) ? 1 : 0 - (Input.GetKey(keyLeft) ? 1 : 0);

        //Handle Intermediate Signals

        //calculate directional singals on hori and vertical axis
        Dup = Mathf.SmoothDamp(Dup, targetDup, ref velocityDup, smoothTime);
        Dright = Mathf.SmoothDamp(Dright, targetDright, ref velocityDright, smoothTime);

        //Calculate movement vector and directional vector singals
        //Approach 1
        //Mmag = Mathf.Max(Mathf.Abs(Dup), Mathf.Abs(Dright));
        //Approach 2 using mapping

        //Handle Output Signals
        Vector2 targetPos = Square2Circle(new Vector2(Dup, Dright));
        Mmag = Mathf.Sqrt(targetPos.x * targetPos.x + targetPos.y * targetPos.y);
        Dvec = Dup * transform.forward + Dright * transform.right;

        //Handle Jump, Roll, Back, Run, Attack Inputs
        runSignal = Input.GetKey(keyShift);
        jrbSignal = Input.GetKeyDown(keySpace);
        attackSignal = Input.GetKeyDown(keyMouse0);

        //Reset Signals if necessary
        if (!isLocked)
            ResetSignals();
    }

    private Vector2 Square2Circle(Vector2 squarePos)
    {
        //Using Elliptical Grid Mapping to map points in square to a circle
        float u = squarePos.x * Mathf.Sqrt(1 - squarePos.y * squarePos.y / 2);
        float v = squarePos.y * Mathf.Sqrt(1 - squarePos.x * squarePos.x / 2);
        return new Vector2(u, v);
    }

    private void ResetSignals()
    {
        //Intermediate Singals Clear
        targetDup = targetDright = 0;

        //Output Signals Clear
        Mmag = 0;
        Dvec = Vector3.zero;
        jrbSignal = runSignal = attackSignal = false;
    }
}
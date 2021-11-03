using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private ActorController ac;

    [Header("===== Player Movement Key Settings =====")]
    public string keyUp = "w";

    public string keyDown = "s";
    public string keyLeft = "a";
    public string keyRight = "d";
    public string keyShift = "leftshift";
    public string keySpace;

    [Header("===== Attack Key Settings ======")]
    public string keyMouse0;

    public string keyMouse1;

    [Header("===== Move Signals =====")]
    [SerializeField]
    private float Dup = 0f;

    [SerializeField]
    private float Dright = 0f;

    [SerializeField]
    private float targetDup;

    [SerializeField]
    private float targetDright;

    [SerializeField]
    private float velocityDup;

    [SerializeField]
    private float velocityDright;

    public float Mmag;    //Movement magnitude

    public Vector3 Dvec;    //Directional vector

    public bool runSignal;
    public bool jrbSignal;  //Jump, Roll and BackJump; Triggered by jump key

    [Header("===== Attack Signals =====")]
    public bool attackSignal;

    //Variables for Smoothdamp function
    public float smoothTime = 1f;

    [Header("===== Flags for controlling signals lock")]
    [SerializeField]
    private bool isMoveLocked = true;

    [SerializeField]
    public bool isAttackLocked = true;

    public bool IsAllLocked => isMoveLocked && isAttackLocked;

    public void SetAllSignalsActive(bool value) => isAttackLocked = isMoveLocked = !value;

    public void SetMoveSignalsActive(bool value) => isMoveLocked = !value;

    public void SetAttackSignalsActive(bool value) => isAttackLocked = !value;

    private void Awake()
    {
        ac = GetComponent<ActorController>();
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        //Handle Player Inputs

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
        if (isAttackLocked)
            ResetAttackSignals();
        if (isMoveLocked)
            ResetMovementSignals();
    }

    // =================== Private Methods ================================//
    private Vector2 Square2Circle(Vector2 squarePos)
    {
        //Using Elliptical Grid Mapping to map points in square to a circle
        float u = squarePos.x * Mathf.Sqrt(1 - squarePos.y * squarePos.y / 2);
        float v = squarePos.y * Mathf.Sqrt(1 - squarePos.x * squarePos.x / 2);
        return new Vector2(u, v);
    }

    private void ResetMovementSignals()
    {
        //Intermediate Singals Clear
        targetDup = targetDright = 0;

        //Output Signals Clear
        Mmag = 0;
        Dvec = Vector3.zero;
        jrbSignal = runSignal = false;
    }

    private void ResetAttackSignals()
    {
        attackSignal = false;
    }
}
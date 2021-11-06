using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    public GameObject model;
    public GameObject cameraHandler;

    private PlayerInput pi;
    private Animator anim;
    private Rigidbody rg;
    private CapsuleCollider cap;

    [Space(10)]
    [Header("========= Physics Materials ==========")]
    public PhysicMaterial frictionOne;
    public PhysicMaterial frictionZero;

    [Space(10)]
    [Header("========= Moving Speeds ===========")]
    public float walkingSpd = 1.0f;
    public float runningSpd = 2.0f;

    [Space(10)]
    [Header("========== Impluses =============")]
    public float rollFrc = 10.0f;
    public float jumpFrc = 20.0f;
    public float backFrc = 2.0f;

    [Space(10)]
    [Header("========== Cooldowns =============")]
    public float rollCold = 3f;
    public float jumpCold = 2f;
    public float backCold = 0.5f;
    public float attackCold = 1.2f;

    private float CurrentSpd => (pi.runSignal ? runningSpd : walkingSpd) * pi.Mmag;

    private Vector3 movingVec;  //moving vector of the actor

    [Header("========= Flags ==================")]
    private bool lockPlanar;    //flag variable for locking the moving vector
    public bool isRoll;
    public bool isJump;
    public bool isBack;
    public bool isAttack;

    private bool rollRdy = true;
    private bool jumpRdy = true;   //flag variable for jump colddown
    private bool backRdy = true;
    private bool attackRdy = true;

    [Header("======= Smoothing Factors ================")]
    [SerializeField]
    private float smoothTurn = 0.07f;

    [SerializeField]
    private float smoothWeight;

    [Header("======= Animator State Machine Info ========")]
    private int moveLayerIndex;
    private int attackLayerIndex;
    private string moveLayerName;
    private string attackLayerName;

    // Root Motion Adjust Variables
    private Vector3 m_deltaP;
    private Vector3 m_deltaR;

    private void Awake()
    {
        cap = GetComponent<CapsuleCollider>();
        pi = GetComponent<PlayerInput>();
        anim = model.GetComponent<Animator>();
        rg = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        anim.SetLayerWeight(anim.GetLayerIndex("Attack"), 0f);

        moveLayerIndex = anim.GetLayerIndex("Move");
        attackLayerIndex = anim.GetLayerIndex("Attack");
        moveLayerName = anim.GetLayerName(moveLayerIndex);
        attackLayerName = anim.GetLayerName(attackLayerIndex);
    }

    // Update is called once per frame
    private void Update()
    {
        /*
         * The following input parsing part should be moved and tuned into
         * PlayerInput Class
         */

        //Starts Here
        if (pi.jrbSignal && IsGrounded())
        {
            float forward = anim.GetFloat("forward");

            //First detect if is a back jump
            if (backRdy && forward < .5f)
                isBack = true;
            //Then detect if is a roll
            else if (rollRdy && forward > .5f && forward < 1.1f)
                isRoll = true;
            else if (jumpRdy && forward > 1.1f)
                isJump = true;
        }

        if (pi.attackSignal && attackRdy)
            isAttack = true;
        //Ends Here

        SetAnimParams();

        //Set facing direction of the player
        if (pi.Dvec != Vector3.zero)
            //Lock direction if user doesn't input any move
            //model.transform.forward = pi.Dvec;
            model.transform.forward = Vector3.Slerp(model.transform.forward, pi.Dvec, smoothTurn);

        //Calculate moving vector
        movingVec = (lockPlanar) ? movingVec : CurrentSpd * model.transform.forward;
    }

    //-----------------------------Physics Related Functions------------------------------//

    private void FixedUpdate()
    {
        //print(cameraHandler.transform.position);
        print(transform.position);
        Move();
        JumpBackJumpRoll();

        //print(cameraHandler.transform.position);
    }

    private void Move()
    {
        //Moving the actor
        rg.position += Time.fixedDeltaTime * movingVec + m_deltaP;
        m_deltaP = Vector3.zero;
    }

    private void JumpBackJumpRoll()
    {
        /* The following code commented out should be implemented instead in the future together with the implementation
        * of handling triggers in animation
       */
        //Vector3 impluse = model.transform.forward * anim.GetFloat("backFrc");
        //rg.AddForce(impluse, ForceMode.Impulse);

        /*handling triggers should be done in animation start and exit in the future instead
         * By in the FSM Enter and Exit Class, sending messages to this class
         */
        if (isBack)
        {
            rg.AddForce((Vector3.up - model.transform.forward) * backFrc, ForceMode.Impulse);

            isBack = false;

            backRdy = false;
            StartCoroutine("cooldownBack");
        }
        //Jumping the actor
        else if (isJump)
        {
            rg.AddForce(Vector3.up * jumpFrc, ForceMode.Impulse);
            isJump = false;

            jumpRdy = false;
            StartCoroutine("cooldownJump");
        }

        //Rolling the actor
        else if (isRoll)
        {
            rg.AddForce(model.transform.forward * rollFrc, ForceMode.Impulse);
            isRoll = false;

            rollRdy = false;
            StartCoroutine("cooldownRoll");
        }
        else if (isAttack)
        {
            isAttack = false;

            attackRdy = false;
            StartCoroutine("cooldownAttack");
        }
    }

    //private bool IsGrounded()
    //{
    //    Vector3 start = transform.position + transform.up * cap.radius;
    //    Vector3 end = start + transform.up * (cap.height - 2 * cap.radius);
    //    Collider[] hitCols = Physics.OverlapCapsule(start, end, cap.radius, LayerMask.GetMask("Ground"));

    //    return hitCols != null && hitCols.Length != 0;

    //}

    private bool IsGrounded()
    {
        float extraHeightText = .1f;
        int layerMask = ~(1 << 8);
        bool isHit;

        isHit = Physics.Raycast(cap.bounds.center, -transform.up, cap.bounds.extents.y + extraHeightText, layerMask);

        return isHit;
    }

    //Cooldown for forward jump, backward jump and roll
    private IEnumerator cooldownJump()
    {
        yield return new WaitForSeconds(jumpCold);
        jumpRdy = true;
    }

    private IEnumerator cooldownBack()
    {
        yield return new WaitForSeconds(backCold);
        backRdy = true;
    }

    private IEnumerator cooldownRoll()
    {
        yield return new WaitForSeconds(rollCold);
        rollRdy = true;
    }

    private IEnumerator cooldownAttack()
    {
        yield return new WaitForSeconds(attackCold);
        attackRdy = true;
    }

    //--------------------------Functions for setting Animation Params---------------------------//

    private void SetAnimParams()
    {
        //Set animation of moving(blending of idle, walk and run)
        anim.SetFloat("forward", Mathf.Lerp(anim.GetFloat("forward"), CurrentSpd, 0.05f));

        //Set animation of Back, Jump and Roll
        if (isBack)
            anim.SetTrigger("back");
        else if (isJump)
            anim.SetTrigger("jump");
        else if (isRoll)
            anim.SetTrigger("roll");
        else if (isAttack)
            anim.SetTrigger("attack");

        anim.SetBool("isGround", IsGrounded());
    }

    //-------------------------------------------Message Functions to Control Input--------------------------//

    private void SetMoveLock(bool value) => lockPlanar = value;

    public void OnMoveGroundEnter()
    {
        pi.SetAllSignalsActive(true);
        SetMoveLock(false);
        cap.material = frictionOne;
    }

    public void OnMoveEnter()
    {
        pi.SetAllSignalsActive(false);
        SetMoveLock(true);
        cap.material = frictionZero;
    }

    public void OnAttackIdleEnter()
    {
    }

    public void OnAttackIdleUpdate()
    {
        float curAttackWeight = Mathf.Lerp(anim.GetLayerWeight(attackLayerIndex), 0f, smoothWeight);
        anim.SetLayerWeight(attackLayerIndex, curAttackWeight);
    }

    public void OnAttackEnter()
    {
        pi.SetMoveSignalsActive(false);
        pi.SetAttackSignalsActive(true);
        SetMoveLock(false);
    }

    public void OnAttack1Update()
    {
        float curAttackWeight = Mathf.Lerp(anim.GetLayerWeight(attackLayerIndex), 1f, smoothWeight);
        anim.SetLayerWeight(attackLayerIndex, curAttackWeight);
    }

    public void OnAttackExit()
    {
        if (IsState("Attack.Idle", "Attack"))
            OnMoveGroundEnter();
    }

    //====================== Callbacks by Root Motion Controller ==========================//
    public void FollowRM(Vector3 deltaP)
    {
        /*
         * Below Should be applied to Move also in the future.
         */
        if (IsState("Attack.Attack1HandC", "Attack"))
            m_deltaP += deltaP;
    }

    //====================== Check Current Animation and Transition Info ==========================//
    private bool IsTransition(string transitionName, string layerName = "Move")
    {
        int layerIndex = anim.GetLayerIndex(layerName);
        return anim.GetAnimatorTransitionInfo(layerIndex).IsUserName(transitionName);
    }

    private bool IsState(string stateName, string layerName = "Move")
    {
        int layerIndex = anim.GetLayerIndex(layerName);
        return anim.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName);
    }
}
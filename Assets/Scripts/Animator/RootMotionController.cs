using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootMotionController : MonoBehaviour
{
    private Animator anim;
    private ActorController ac;

    private Vector3 m_acceDeltaPos;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        ac = transform.parent.gameObject.GetComponent<ActorController>();
    }

    private void OnAnimatorMove()
    {
        ac.FollowRM(anim.deltaPosition);
    }
}
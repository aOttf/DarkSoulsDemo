using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimTriggerController : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void SetTrigger(string trigName) => anim.SetTrigger(trigName);

    public void ResetTrigger(string trigName) => anim.ResetTrigger(trigName);
}
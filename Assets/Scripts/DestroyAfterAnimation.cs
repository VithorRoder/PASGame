using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterAnimation : MonoBehaviour
{
    void Start()
    {
        Animator animator = GetComponent<Animator>();
        float animationTime = animator.GetCurrentAnimatorStateInfo(0).length;
        Destroy(gameObject, animationTime);
    }
}

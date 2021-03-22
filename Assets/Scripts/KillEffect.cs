using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillEffect : MonoBehaviour
{
    [SerializeField]
    private Animator KillAnimation;
    private void Start(){
        KillAnimation.SetBool("EarnedAKill",true);
        KillAnimation.SetBool("EarnedAKill",false);
        StartCoroutine(Wait(0.5f));
        this.enabled = false;
    }
    IEnumerator Wait(float time){
        yield return new WaitForSeconds(time);
        this.gameObject.SetActive(false);
    }
    private void OnEnable(){
        KillAnimation.SetBool("EarnedAKill",true);
        KillAnimation.SetBool("EarnedAKill",false);
        StartCoroutine(Wait(0.5f));
        this.enabled = false;
    }
}

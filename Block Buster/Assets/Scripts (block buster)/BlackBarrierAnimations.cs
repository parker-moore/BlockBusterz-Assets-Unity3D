using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackBarrierAnimations : MonoBehaviour {

    /*
     * Class Description: public class BlackBarrierAnimations
     *  script that enables/disables animation specifically for black box bordering gameplay
     */

    private Animator ani;

    void Start()
    {
        ani = GetComponent<Animator>();
    }

    public void animationEnded()
    {
        ani.SetBool("enable", false);
    }

    public void EnableAnimation()
    {
        if (ani.GetBool("enable") == true)
            ani.SetBool("enable", false);
        else
            ani.SetBool("enable", true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space)) {
        //     print("Waving");
        //     anim.SetTrigger("MakeWave");
        // }

        if (Input.GetKeyDown(KeyCode.Return)) {
            print("Walking");
            anim.SetTrigger("DemoGesture");
            //anim.SetTrigger("avatar_0_fbx_tmp");
        }

        // if (Input.GetKeyDown(KeyCode.Backspace)) {
        //     print("Idling");
        //     anim.SetTrigger("MakeIdle");
        // }
    }

    void RunAnimation(string textInput) {
        
    }
}

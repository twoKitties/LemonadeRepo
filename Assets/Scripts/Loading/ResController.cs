using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResController : MonoBehaviour {

#if UNITY_STANDALONE
    private void Awake()
    {
        Screen.SetResolution(506, 900, false);
    }
#endif
}

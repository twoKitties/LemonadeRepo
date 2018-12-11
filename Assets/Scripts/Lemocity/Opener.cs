using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opener : MonoBehaviour
{
    public void Open(GameObject objToOpen)
    {
        objToOpen.SetActive(true);
    }
    public void Close(GameObject objToClose)
    {
        objToClose.SetActive(false);
    }
}

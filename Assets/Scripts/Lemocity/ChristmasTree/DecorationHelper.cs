using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorationHelper : MonoBehaviour
{
    [SerializeField]
    private GameObject[] littleTreeDecorations;

    public void SetDecoration(int decorationID)
    {
        for(int i = 0; i < littleTreeDecorations.Length; i++)
        {
            if (i == decorationID)
                littleTreeDecorations[i].SetActive(true);
            else
                littleTreeDecorations[i].SetActive(false);
        }
    }
}

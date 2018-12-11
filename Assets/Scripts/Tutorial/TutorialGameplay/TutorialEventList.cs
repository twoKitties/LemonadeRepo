using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class TutorialEventList
{
    public static event Action OnGameTilesBurned;
    public static void CallOnGameTilesBurned()
    {
        if (OnGameTilesBurned != null)
            OnGameTilesBurned();
    }
    public static event Action OnGameTileSwaped;
    public static void CallOnGameTileSwaped()
    {
        if (OnGameTileSwaped != null)
            OnGameTileSwaped();
    }
    public static event Action OnGameTilesReachedBottom;
    public static void CallOnGameTilesReachedBottom()
    {
        if (OnGameTilesReachedBottom != null)
            OnGameTilesReachedBottom();
    }
    public static event Action OnBonusButtonHorizontalUsed;
    public static void CallOnBonusButtonHorizontalUsed()
    {
        if (OnBonusButtonHorizontalUsed != null)
            OnBonusButtonHorizontalUsed();
    }
    public static event Action OnBonusButtonVerticalUsed;
    public static void CallOnBonusButtonVerticalUsed()
    {
        if (OnBonusButtonVerticalUsed != null)
            OnBonusButtonVerticalUsed();
    }
}

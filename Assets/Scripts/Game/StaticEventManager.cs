using System;
using UnityEngine;

public static class StaticEventManager
{
    //public static event Action<GameTile> OnTileDestroy;

    //public static void CallOnTileDestroy(GameTile tile)
    //{
    //    if (OnTileDestroy != null)
    //        OnTileDestroy(tile);
    //}

    public static event Action OnGameLose;

    public static void CallOnGameLose()
    {
        if (OnGameLose != null)
            OnGameLose();
    }

    public static event Action<int> OnTileDead;

    public static void CallOnTileDead(int bonusAmount)
    {
        if (OnTileDead != null)
            OnTileDead(bonusAmount);
    }

    /// <summary>
    /// BackgroundController must be subscribed to this event
    /// </summary>
    public static event Action<int> OnBackgroundColorChanged;

    public static void CallOnBackgroundColorChanged(int backgroundColorIndex)
    {
        if (OnBackgroundColorChanged != null)
            OnBackgroundColorChanged(backgroundColorIndex);
    }

    public static event Action OnMenuEnter;

    public static void CallOnMenuEnter()
    {
        if (OnMenuEnter != null)
            OnMenuEnter();
    }

    public static event Action OnMenuExit;

    public static void CallOnMenuExit()
    {
        if (OnMenuExit != null)
            OnMenuExit();
    }

    public static event Action<int> OnBottleImageRefresh;

    public static void CallOnBottleImageRefresh(int bottleImageIndex)
    {
        if (OnBottleImageRefresh != null)
            OnBottleImageRefresh(bottleImageIndex);
    }
}

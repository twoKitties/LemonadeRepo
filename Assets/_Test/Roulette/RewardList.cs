using UnityEngine;

[CreateAssetMenu(fileName = "RewardListItem", menuName = "Rewards/RewardListItem")]
public class RewardList : ScriptableObject
{
    public CandyReward[] candyRewards;
}
[System.Serializable]
public class CandyReward
{
    public Sprite Sprite;
    public string DescriptionEn;
    public string DescriptionRu;
    public int Amount;

    public CandyReward(Sprite sprite, string descriptionEn, string descriptionRu, int amount)
    {
        Sprite = sprite;
        DescriptionEn = descriptionEn;
        DescriptionRu = descriptionRu;
        Amount = amount;
    }
}

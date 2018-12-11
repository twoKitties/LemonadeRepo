using UnityEngine;
using UnityEngine.UI;

public class RewardWindow : MonoBehaviour
{
    [SerializeField]
    private Image rewardImage;
    [SerializeField]
    private Text rewardText;
    [SerializeField]
    private int rewardAmount;

    public void SetRewardItemData(Sprite rewardImage, string rewardText, int rewardAmount)
    {
        this.rewardImage.sprite = rewardImage;
        this.rewardText.text = rewardText;
        this.rewardAmount = rewardAmount;
    }

    public void ClaimReward()
    {
        var pointsContainer = FindObjectOfType<LemonadeStore.UpgradeWorker>();
        if (pointsContainer != null)
            pointsContainer.Points += rewardAmount;
        if (PointsCount.OnPointsChange != null)
            PointsCount.OnPointsChange();
        // Maybe worth adding here some cash sound

        // Clear data
        //rewardImage = null;
        //rewardText = null;
        //rewardAmount = 0;

        gameObject.SetActive(false);
    }
}

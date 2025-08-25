using System.Collections.Generic;

namespace SSB.Quests
{
    public class SaveQuestData
    {
        public int Status;
        public int RewardStatus;
        public int CurrentValue;

        public SaveQuestData(int status,int rewardStatus, int currentValue)
        {
            Status = status;
            RewardStatus = rewardStatus;
            CurrentValue = currentValue;
        }
    }
}
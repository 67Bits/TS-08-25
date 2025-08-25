using UnityEngine;

namespace KainCobra.Talents
{
    [CreateAssetMenu(menuName = "Talents/TalentsDataManager", fileName = "TalentsDataManager")]
    public class TalentsDataManager : ScriptableObject
    {
        public TalentBase[] AllTalents;

        public void LoadAllTalents()
        {
            for (int i = 0; i < AllTalents.Length; i++)
            {
                var talent = AllTalents[i];
                talent.TalentData = null;
                var saveData = LoadData(talent.Id);
                if (saveData != null)
                {
                    var talentData = new TalentData(
                        saveData.IdTalent,
                        saveData.Level);
                    talent.TalentData = talentData;
                }
            }
        }
        public void ResetTalents()
        {
            for (int i = 0; i < AllTalents.Length; i++)
            {
                var talent = AllTalents[i];
                talent.IsApplied = false;
            }
        }
        public void SaveData(TalentData talentData)
        {
            SaveTalentData saveTalentData = new SaveTalentData(
                talentData.IdTalent,
                talentData.Level);
            SaveSystem.SaveDataById(saveTalentData, talentData.IdTalent);
        }
        public SaveTalentData LoadData(string Id)
        {
            var saveData = SaveSystem.LoadDataById<SaveTalentData>(Id);
            return saveData;
        }
        private void OnValidate()
        {
            if (AllTalents.Length <= 0)
            {
                AllTalents = Resources.LoadAll<TalentBase>("Talents");
            }
        }
    }
}
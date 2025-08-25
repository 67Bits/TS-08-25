namespace KainCobra.Talents
{
    public class SaveTalentData
    {
        public string IdTalent { get; private set; }
        public int Level { get; private set; }

        public SaveTalentData(string idTalent, int level)
        {
            IdTalent = idTalent;
            Level = level;
        }
    }
}

namespace DataStorage
{
    [System.Serializable]
    public class Item
    {
        public string Key;
        public int CurrentGradeID;
        public bool[] GradeStates;

        public Item(string Key, int CurrentGradeID, bool[] GradeStates)
        {
            this.Key = Key;
            this.CurrentGradeID = CurrentGradeID;
            this.GradeStates = GradeStates;
        }
    }
}

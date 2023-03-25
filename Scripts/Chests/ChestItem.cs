using RPG.Inventories;
using UnityEngine;

namespace RPG.Chests
{
    [CreateAssetMenu(fileName = "New Chest", menuName = "Chests/Chest", order = 0)]
    public class ChestItem : ScriptableObject
    {

        [SerializeField] private MathProblem mathProblem;
        [SerializeField] private Chest chestPrefab;
        [SerializeField] private GameObject chestProtectorPrefab;
        [SerializeField] private AudioClip questionVoiceLine;
        [SerializeField] private DropLibrary dropLibrary;
        [SerializeField] private int level = 1;

        #region Main Methods

        public bool CheckAnswer(string input)
        {
            return mathProblem.CheckAnswer(input);
        }
        
        public Chest SpawnChest(Vector3 position)
        {
            Chest chest = Instantiate(this.chestPrefab);
            chest.transform.position = position;
            chest.Setup(this);
            return chest;
        }

        #endregion

        #region Getters/Setters

        public string GetQuestion()
        {
            return mathProblem.GetQuestion();
        }

     
        public int GetLevel()
        {
            return mathProblem.GetDifficulty();
        }

        public DropLibrary GetDropTable()
        {
            return dropLibrary;
        }


        public AudioClip GetVoice()
        {
            return questionVoiceLine;
        }

        #endregion

    }
}

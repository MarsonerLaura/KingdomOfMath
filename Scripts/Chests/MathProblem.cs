using UnityEngine;

namespace RPG.Chests
{
    [CreateAssetMenu(fileName = "New MathProblem", menuName = "Chests/MathProblem", order = 0)]
    public class MathProblem : ScriptableObject
    {
        enum Difficulty
        {
            Easy,
            Medium,
            Hard
        }

        [SerializeField] private Difficulty difficulty;
        [SerializeField] [Multiline(10)] private string question;
        [SerializeField] [Multiline(3)] private string solution;
        [SerializeField] private string[] hints;

        private int _hintCount = 0;

        #region Main Methods

        public bool CheckAnswer(string answer)
        {
            return answer.Equals(solution);
        }

        public string GiveHint()
        {
            return hints[_hintCount];
        }

        #endregion

        #region Getters/Setters

        public string GetQuestion()
        {
            return question;
        }

        public int GetDifficulty()
        {
            switch (difficulty)
            {
                case Difficulty.Easy:
                    return 1;
                case Difficulty.Medium:
                    return 2;
                case Difficulty.Hard:
                    return 3;
                default:
                    return 0;
            }
        }

        #endregion

        
    }

}
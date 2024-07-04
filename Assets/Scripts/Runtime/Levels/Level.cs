using UnityEngine;

namespace MP.Levels
{
    [CreateAssetMenu(fileName = "Level", menuName = "Memory Puzzle", order = 1)]
    public class Level : ScriptableObject
    {
        public int Width;
        public int Height;

        /// <summary>
        /// Generates a new level data model by difficulty
        /// </summary>
        /// <param name="difficulty">value between 0 and 1. Where 1 is the maximum difficulty</param>
        public Level(float difficulty)
        {
            CalculateGridSize(difficulty);
        }

        private void CalculateGridSize(float difficulty)
        {
            var lm = LevelManager.Instance;
            var maxWidth = lm.MaximumWidth;
            var minWidth = lm.MinimumWidth;
            var maxHeight = lm.MaximumHeight;
            var minHeight = lm.MinimumHeight;

            var widthValue = Mathf.Lerp(minWidth, maxWidth, difficulty);
            var heightValue = Mathf.Lerp(minHeight, maxHeight, difficulty);
            
            Width = Mathf.RoundToInt(widthValue);
            Height = Mathf.RoundToInt(heightValue);

            // Check if the area is multiplier of 2.
            var area = Width * Height;
            if (area % 2 == 0)
                return;

            // If area is not a multiplier of 2 we need to reproduce
            if (Width > Height)
            {
                Width--;
            }
            else
            {
                Height--;
            }
        }
    }
}
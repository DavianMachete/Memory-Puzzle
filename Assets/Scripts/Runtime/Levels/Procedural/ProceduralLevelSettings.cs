using System;
using MP.Cards;

namespace MP.Levels.Procedural
{
    [Serializable]
    public struct ProceduralLevelSettings
    {
        public int maximumLevel;
        public GridSettings gridSettings;
        public CardsHolder cards;
    }
}
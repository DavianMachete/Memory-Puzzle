using System;

namespace MP.Levels.Procedural
{
    [Serializable]
    public struct ProceduralLevelSettings
    {
        public int maximumLevel;
        public GridSettings gridSettings;
        public CardData[] cards;
    }
}
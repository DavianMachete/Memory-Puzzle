using System.Collections;
using System.Collections.Generic;
using MP.Levels;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.TestTools;

namespace MP.Tests
{

    public class LevelDataTests
    {
        [UnityTest]
        public IEnumerator TestLevelGridSizeOnMaxLevel()
        {
            var lm = LevelManager.Instance;
            yield return new WaitUntil(() => lm != null && lm.IsInitialized);

            var settings = lm.ProceduralLevelSettings;
            var maxLevel = settings.maximumLevel;
            var gridSettings = settings.gridSettings;

            // Get level by bigger number than maximumLevel
            var data = lm.GetLevel(maxLevel * 10);
            
            Assert.IsTrue(data.ColumnsCount == gridSettings.maximumColumns);
            Assert.IsTrue(data.RowsCount == gridSettings.maximumRows);
            Assert.IsTrue(data.CardsTypeCount == settings.cards.CardsDataCount);
        }
    }
}
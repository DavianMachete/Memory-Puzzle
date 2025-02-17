using UnityEditor;
using UnityEngine;

namespace MP.Levels
{
    [CustomEditor(typeof(LevelData))]
    public class LevelDataEditor : Editor
    {
        private LevelData _levelData;
        private SerializedProperty _level;
        private SerializedProperty _columnsCount;
        private SerializedProperty _rowsCount;
        
        private void OnEnable()
        {
            _levelData = target as LevelData;
            _level = serializedObject.FindProperty("level");
            _columnsCount = serializedObject.FindProperty("columnsCount");
            _rowsCount = serializedObject.FindProperty("rowsCount");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.PropertyField(_level);
            
            EditorGUILayout.Space();

            EditorGUILayout.IntSlider(_columnsCount, 2,10);
            EditorGUILayout.IntSlider(_rowsCount, 2, 10);
            
            EditorGUILayout.EndVertical();

            DrawGrid();

            // if (GUILayout.Button("Check grid sizes"))
            // {
            //     Debug.Log($"_levelData.ColumnsCount = {_levelData.ColumnsCount}");
            //     Debug.Log($"_levelData.RowsCount = {_levelData.RowsCount}");
            //     Debug.Log($"_levelData.GetGridSizeText() = {_levelData.GetGridSizeText()}");
            // }
            
            serializedObject.ApplyModifiedProperties();
            if (serializedObject.hasModifiedProperties)
                EditorUtility.SetDirty(target);
        }


        private void DrawGrid()
        {
            if (_levelData.RowsCount <= 1 || _levelData.ColumnsCount <= 1) 
                return;
            
            _levelData.ValidateGrid();

            // Get the current inspector width
            var inspectorWidth = EditorGUIUtility.currentViewWidth;
            
            // Calculate cell size based on the inspector width
            var cellSize = (inspectorWidth - 100) / _levelData.ColumnsCount; // 100 is arbitrary padding
            GUILayout.Space(10); // spacing before the grid
                
            // Draw the columns numbers
            GUILayout.BeginHorizontal();
            GUILayout.Space(cellSize); // for the corner space
            for (var col = 0; col < _levelData.ColumnsCount; col++)
            {
                GUILayout.Label(col.ToString(), GUILayout.Width(cellSize));
            }
            GUILayout.EndHorizontal();

            for (var row = 0; row < _levelData.RowsCount; row++)
            {
                GUILayout.BeginHorizontal();
                for (var col = 0; col < _levelData.ColumnsCount; col++)
                {
                    // Row number
                    if (col == 0)
                        GUILayout.Label(row.ToString(), GUILayout.Width(cellSize/2));
                    
                    var cardId = _levelData.GetCardId(row, col);
                    var newCardId = EditorGUILayout.TextField(cardId, GUILayout.Width(cellSize));
                    _levelData.SetCardId(newCardId, row, col);
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(10); // spacing after the grid
        }
    }
}
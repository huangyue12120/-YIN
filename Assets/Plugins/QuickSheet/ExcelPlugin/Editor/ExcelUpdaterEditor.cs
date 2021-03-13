using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace UnityQuickSheet
{
    /// <summary>
    /// Custom editor script class for excel file setting.
    /// </summary>
    [CustomEditor(typeof(ExcelUpdater))]
    public class ExcelUpdaterEditor : Editor
    {
        // Start is called before the first frame update
        void Start()
        {
            
        }
        // Update is called once per frame
        void Update()
        {
            
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUI.changed = false;

            EditorGUILayout.Separator();
            if(GUILayout.Button("Update"))
            {
                Object[] targetsUnderxls;
                
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(ExcelSettings.Instance);
            }
        }
}
}

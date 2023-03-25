using System.Collections.Generic;
using RPG.Utils;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        [SerializeField] private string text;
        [SerializeField] private List<string> children = new List<string>();
        [SerializeField] private Rect rect = new Rect(0,0, 200, 150);
        [SerializeField] private bool isPlayerSpeaking = false;
        [SerializeField] private string onEnterAction;
        [SerializeField] private string onExitAction;
        [SerializeField] private Condition condition;
        


        #region Getters and Setters

        public Rect GetRect()
        {
            return rect;
        }
        public string GetText()
        {
            return text;
        }

        public List<string> GetChildren()
        {
            return children;
        }

        public bool IsPlayerSpeaking()
        {
            return isPlayerSpeaking;
        }

        public string GetOnEnterAction()
        {
            return onEnterAction;
        }
        
        public string GetOnExitAction()
        {
            return onExitAction;
        }

        #endregion

        #region Main Methods
        
        public bool CheckCondition(IEnumerable<IPredicateEvaluator> evaluators)
        {
            return condition.Check(evaluators);
        }

        #endregion

#if UNITY_EDITOR
        public void SetPosition(Vector2 newPosition)
        {
            Undo.RecordObject(this, "Move Dialogue Node");
            rect.position = newPosition;
            EditorUtility.SetDirty(this);
        }
        
        public void SetText(string newText)
        {
            if (newText != text)
            {
                Undo.RecordObject(this, "Update Dialogue Text");
                text = newText;
                EditorUtility.SetDirty(this);
            }
        }

        public void SetPlayerSpeaking(bool speaking)
        {
            Undo.RecordObject(this, "Change Dialogue Speaker");
            isPlayerSpeaking = speaking;
            EditorUtility.SetDirty(this);
        }
        
        public void AddChild(string childID)
        {
            Undo.RecordObject(this, "Add Dialogue Link");
            children.Add(childID);
            EditorUtility.SetDirty(this);
        }

        public void RemoveChild(string childID)
        {
            Undo.RecordObject(this, "Remove Dialogue Link");
            children.Remove(childID);
            EditorUtility.SetDirty(this);
        }
        
        
#endif


    }
}


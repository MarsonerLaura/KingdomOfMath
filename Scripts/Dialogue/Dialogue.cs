using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "DialogueSystem/Dialogue", order = 0)]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] private List<DialogueNode> nodes = new List<DialogueNode>();
        [SerializeField] private Vector2 newNodeOffset = new Vector2(250,0);
        
        private Dictionary<string, DialogueNode> _nodeLookup = new Dictionary<string, DialogueNode>();

#if UNITY_EDITOR
        public void CreateNode(DialogueNode parent)
        {
            DialogueNode child = MakeNode(parent);

            Undo.RegisterCreatedObjectUndo(child, "Created Dialogue Node");
            if (AssetDatabase.GetAssetPath(this) != "")
            {
                Undo.RecordObject(this, "Added Dialogue Node");
            }
            
            AddNode(child);
        }
        
        public void DeleteNode(DialogueNode nodeToDelete)
        {
            Undo.RecordObject(this, "Removed Dialogue Node");
            nodes.Remove(nodeToDelete);
            CleanDeletedChildren(nodeToDelete);
            OnValidate();
            Undo.DestroyObjectImmediate(nodeToDelete);
        }
        
        private DialogueNode MakeNode(DialogueNode parent)
        {
            DialogueNode child = CreateInstance<DialogueNode>();
            child.name = Guid.NewGuid().ToString();

            if (parent != null)
            {
                parent.AddChild(child.name);
                child.SetPlayerSpeaking(!parent.IsPlayerSpeaking());
                child.SetPosition(parent.GetRect().position + newNodeOffset);
            }

            return child;
        }
        
        private void AddNode(DialogueNode child)
        {
            nodes.Add(child);

            OnValidate();
        }
        
        private void CleanDeletedChildren(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode node in GetAllNodes())
            {
                node.RemoveChild(nodeToDelete.name);
            }
        }
#endif

        #region Unity Event Methods

        private void Awake()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            if(_nodeLookup!=null) _nodeLookup.Clear();
            foreach (DialogueNode node in GetAllNodes())
            {
                if(node!=null)
                {
                    _nodeLookup[node.name] = node;
                }
            }
        }

        #endregion
        
        #region Main Methods

        public DialogueNode GetRootNode()
        {
            return nodes[0];
        }
        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return nodes;
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            foreach (string childID in parentNode.GetChildren())
            {
                if (_nodeLookup.ContainsKey(childID))
                {
                    yield return _nodeLookup[childID];  
                }
            }
        }

        public IEnumerable<DialogueNode> GetPlayerChildren(DialogueNode currentNode)
        {
            foreach (DialogueNode child in GetAllChildren(currentNode))
            {
                if (child.IsPlayerSpeaking())
                {
                    yield return child;
                }
            }
        }

        public IEnumerable<DialogueNode> GetAIChildren(DialogueNode currentNode)
        {
            foreach (DialogueNode child in GetAllChildren(currentNode))
            {
                if (!child.IsPlayerSpeaking()){
                    
                    yield return child;
                }
            }
        }

        #endregion
        
        #region Serialization Methods

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (nodes.Count == 0)
            {
                DialogueNode child = MakeNode(null);
                AddNode(child);
            }
            if(!AssetDatabase.GetAssetPath(this).Equals(""))
            {
                foreach (DialogueNode node in GetAllNodes())
                {
                    if (AssetDatabase.GetAssetPath(node).Equals(""))
                    {
                        AssetDatabase.AddObjectToAsset(node,this);
                    }
                }
            }
#endif
        }

        public void OnAfterDeserialize()
        {
            
        }

        #endregion
        
    }

}
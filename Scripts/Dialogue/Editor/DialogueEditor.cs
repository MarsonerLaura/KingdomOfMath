using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        [NonSerialized] private GUIStyle _npc1NodeStyle;
        [NonSerialized] private GUIStyle _playerNodeStyle;
        [NonSerialized] private DialogueNode _draggingNode = null;
        [NonSerialized] private Vector2 _draggingOffset;
        [NonSerialized] private DialogueNode _creatingNode = null;
        [NonSerialized] private DialogueNode _deletingNode = null;
        [NonSerialized] private DialogueNode _linkingNode = null;
        [NonSerialized] private bool _draggingCanvas = false;
        [NonSerialized] private Vector2 _draggingCanvasOffset;

        private Vector2 _scrollPosition;

        private const float CanvasSize = 4000f;
        private const float BackgroundSize = 50f;
        
        private static Dialogue _selectedDialogue = null;
       
        #region Unity Event Methods

        private void OnGUI()
        {
            if (_selectedDialogue == null)
            {
                EditorGUILayout.LabelField("No Dialogue Selected.");
            }
            else
            {
                ProcessEvents();

                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
                
                Rect canvas = GUILayoutUtility.GetRect(CanvasSize, CanvasSize);
                Texture2D backgroundTexture = Resources.Load("background") as Texture2D;
                Rect textureCoordinates = new Rect(0, 0, CanvasSize/BackgroundSize, CanvasSize/BackgroundSize);
                GUI.DrawTextureWithTexCoords(canvas, backgroundTexture, textureCoordinates);
                
                foreach (DialogueNode node in _selectedDialogue.GetAllNodes())
                {
                    DrawConnections(node);
                }
                foreach (DialogueNode node in _selectedDialogue.GetAllNodes())
                {
                    DrawNode(node);
                }

                EditorGUILayout.EndScrollView();
                
                if (_creatingNode != null)
                {
                    _selectedDialogue.CreateNode(_creatingNode);
                    _creatingNode = null;
                }
                
                if (_deletingNode != null)
                {
                    _selectedDialogue.DeleteNode(_deletingNode);
                    _deletingNode = null;
                } 
                
            }
        }
        
        private void OnEnable()
        {
            SetupNodeStyles();
        }

        private void OnSelectionChange()
        {
            Dialogue newDialogue = Selection.activeObject as Dialogue;
            if (newDialogue != null)
            {
                _selectedDialogue = newDialogue;
                Repaint();
            }
        }

        #endregion

        #region Main Methods

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAsset(1)]
        public static bool OpenDialogue(int instanceID, int line)
        {
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
            
            if (dialogue != null)
            {
                ShowEditorWindow();
                _selectedDialogue = dialogue;
                return true;
            }

            return false;
        }

        
        private void DrawNode(DialogueNode node)
        {
            GUIStyle style = _npc1NodeStyle;
            if (node.IsPlayerSpeaking())
            {
                style = _playerNodeStyle;
            }
            GUILayout.BeginArea(node.GetRect(), style);
            
            GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea);
            textAreaStyle.wordWrap = true;
            node.SetText(EditorGUILayout.TextArea(node.GetText(), textAreaStyle));

            GUILayout.BeginHorizontal();
            
            if (GUILayout.Button("x"))
            {
                _deletingNode = node;
            }

            DrawLinkButtons(node);

            if (GUILayout.Button("+"))
            {
                _creatingNode = node;
            }
            
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void DrawLinkButtons(DialogueNode node)
        {
            if (_linkingNode == null)
            {
                if (GUILayout.Button("link"))
                {
                    _linkingNode = node;
                }
            }
            else if (_linkingNode == node)
            {
                if (GUILayout.Button("cancel"))
                {
                    _linkingNode = null;
                }
            }
            else if (_linkingNode.GetChildren().Contains(node.name))
            {
                if (GUILayout.Button("unlink"))
                {
                    _linkingNode.RemoveChild(node.name);
                    _linkingNode = null;
                }
            }
            else
            {
                if (GUILayout.Button("child"))
                {
                    _linkingNode.AddChild(node.name);
                    _linkingNode = null;
                }
            }
        }

        private void DrawConnections(DialogueNode node)
        {
            Vector3 startPosition = new Vector3(node.GetRect().xMax, node.GetRect().center.y,0);
            foreach (DialogueNode childNode in _selectedDialogue.GetAllChildren(node))
            {
                Vector3 endPosition = new Vector3(childNode.GetRect().xMin, childNode.GetRect().center.y,0);
                Vector3 controlPointOffset = endPosition - startPosition;
                controlPointOffset.y = 0;
                controlPointOffset.x *= 0.9f;
                Handles.DrawBezier(startPosition, endPosition, startPosition + controlPointOffset,
                    endPosition - controlPointOffset, Color.white, null, 4f);
            }
        }

        private void SetupNodeStyles()
        {
            _npc1NodeStyle = new GUIStyle();
            _npc1NodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            _npc1NodeStyle.padding = new RectOffset(20, 20, 20, 20);
            _npc1NodeStyle.border = new RectOffset(12, 12, 12, 12);
            
            _playerNodeStyle = new GUIStyle();
            _playerNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
            _playerNodeStyle.padding = new RectOffset(20, 20, 20, 20);
            _playerNodeStyle.border = new RectOffset(12, 12, 12, 12);
        }
        
        private void ProcessEvents()
        {
            if (Event.current.type == EventType.MouseDown && _draggingNode == null)
            {
                _draggingNode = GetNodeAtPoint(Event.current.mousePosition + _scrollPosition);
                if (_draggingNode != null)
                {
                    _draggingOffset = _draggingNode.GetRect().position - Event.current.mousePosition;
                    Selection.activeObject = _draggingNode;
                }
                else
                {
                    _draggingCanvas = true;
                    _draggingCanvasOffset = Event.current.mousePosition + _scrollPosition;
                    Selection.activeObject = _selectedDialogue;
                }
            }
            else if (Event.current.type == EventType.MouseDrag && _draggingNode != null)
            {

                _draggingNode.SetPosition(Event.current.mousePosition + _draggingOffset);
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseDrag && _draggingCanvas)
            {
                _scrollPosition = _draggingCanvasOffset - Event.current.mousePosition;
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && _draggingNode != null)
            {
                _draggingNode = null;
                
            }
            else if(Event.current.type==EventType.MouseUp && _draggingCanvas)
            {
                _draggingCanvas = false;
            }
        }

        private DialogueNode GetNodeAtPoint(Vector2 currentMousePosition)
        {
            DialogueNode retValue = null;
            foreach (DialogueNode node in _selectedDialogue.GetAllNodes())
            {
                if (node.GetRect().Contains(currentMousePosition))
                {
                    retValue = node;
                }
            }
            return retValue;
        }

        #endregion
        
    }
}

// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Main Quest Editor window. This script handles window management and
    /// references to objects such as the current selection, window instance,
    /// and current inspector editor. It delegates most of the actual GUI 
    /// work to QuestCanvasGUI.
    /// </summary>
    public class QuestEditorWindow : EditorWindow
    {

        [MenuItem("Tools/Pixel Crushers/Quest Machine/Quest Editor", false, 1)]
        public static void ShowWindow()
        {
            m_instance = GetWindow<QuestEditorWindow>();
        }

        #region Static Properties & Methods

        private static QuestEditorWindow m_instance;

        /// <summary>
        /// Reference to the current instance of QuestEditorWindow.
        /// </summary>
        public static QuestEditorWindow instance { get { return m_instance; } }

        /// <summary>
        /// True if the Quest Editor window is open.
        /// </summary>
        public static bool isOpen { get { return m_instance != null; } }

        /// <summary>
        /// Reference to current inspector. When things change in the Quest Editor window, we may
        /// also need to request the inspector to update its display. When an inspector becomes
        /// active, it sets this property.
        /// </summary>
        public static Editor currentEditor { get; set; }

        /// <summary>
        /// The quest currently selected for editing.
        /// </summary>
        public static Quest selectedQuest { get { return (m_instance != null) ? m_instance.m_quest : null; } }

        /// <summary>
        /// The serialized object representation of the current quest.
        /// </summary>
        public static SerializedObject selectedQuestSerializedObject { get { return (m_instance != null) ? m_instance.m_questSerializedObject : null; } }

        /// <summary>
        /// The index of the main selected node in the current quest's nodeList.
        /// </summary>
        public static int selectedNodeListIndex
        {
            get 
            {
                return (m_instance != null) ? m_instance.m_selectedNodeListIndex : -1; 
            }
            set
            {
                if (m_instance != null)
                {
                    m_instance.m_selectedNodeListIndex = value;
                    if (value == -1) m_instance.m_selectedNodeListIndices.Clear();
                }
            }
        }
        /// <summary>
        /// The indices of all selected nodes in the current quest's nodeList.
        /// </summary>
        public static List<int> selectedNodeListIndices
        {
            get { return (m_instance != null) ? m_instance.m_selectedNodeListIndices : null; }
        }

        public static List<string> nodeClipboard { get { return (m_instance != null) ? m_instance.m_nodeClipboard : null; } }
        public static List<int> nodeIndexClipboard { get { return (m_instance != null) ? m_instance.m_nodeIndexClipboard : null; } }

        public static QuestListContainer selectedQuestListContainer { get { return (m_instance != null) ? m_instance.m_selectedQuestListContainer : null; } }

        public static void RepaintNow()
        {
            if (m_instance != null) m_instance.Repaint();
        }

        public static void RepaintCurrentEditorNow()
        {
            if (currentEditor != null) currentEditor.Repaint();
        }

        public static void SetSelectionToQuest()
        {
            if (m_instance == null) return;
            m_instance.FocusSelectionOnQuest();
        }

        public static void UpdateSelectedQuestSerializedObject()
        {
            if (selectedQuestSerializedObject != null) selectedQuestSerializedObject.Update();
        }

        public static void ApplyModifiedPropertiesFromSelectedQuestSerializedObject()
        {
            if (selectedQuestSerializedObject != null) selectedQuestSerializedObject.ApplyModifiedProperties();
        }

        public static bool IsSelectedQuest(SerializedObject serializedObject)
        {
            return selectedQuestSerializedObject != null && serializedObject != null &&
                selectedQuestSerializedObject.targetObject != null && serializedObject.targetObject != null &&
                selectedQuestSerializedObject.targetObject.GetInstanceID() == serializedObject.targetObject.GetInstanceID();
        }

        #endregion

        #region Private Fields

        private const int CurrentFileVersion = 1;

        // Instance ID of the current quest asset, or 0 if none.
        [SerializeField]
        private int m_questInstanceID;

        // Index into current quest's nodeList of the main selected node.
        [SerializeField]
        private int m_selectedNodeListIndex = -1;

        [SerializeField]
        private List<int> m_selectedNodeListIndices = new List<int>();

        // Instance ID of the current QuestListContainer, or 0 if none.
        [SerializeField]
        private int m_questListContainerInstanceID;

        // Index into the current QuestListContainer's questList. Specifies which quest to edit.
        [SerializeField]
        private int m_questListIndex = -1;

        [SerializeField]
        private Vector2 m_canvasScrollPosition = Vector2.zero;
        public Vector2 canvasScrollPosition
        {
            get { return m_canvasScrollPosition; }
            set { m_canvasScrollPosition = value; }
        }

        // These must be kept in sync. nodeClipboard contains serialized nodes. 
        // nodeIndexClipboard contains original indices of nodeClipboard nodes.
        [SerializeField]
        private List<string> m_nodeClipboard = new List<string>();
        [SerializeField]
        private List<int> m_nodeIndexClipboard = new List<int>();

        private QuestListContainer m_selectedQuestListContainer;

        // The quest currently selected for editing. References don't survive assembly reloads,
        // so it's not serialized. Instead, find it again from instance IDs.
        private Quest m_quest;

        // The serialized object representation of the current quest.
        private SerializedObject m_questSerializedObject;

        // Canvas drawer for current selection. Passed to main GUI drawer.
        private QuestCanvasGUI m_canvasGUI = new QuestCanvasGUI();

        // At runtime, tracks time until next window update.
        private float elapsedTime;

        private bool showQuestRelations = false;

        #endregion

        private void OnEnable()
        {
#if DEBUG_QUEST_EDITOR
            Debug.Log("<color=green>QuestEditorWindow.OnEnable</color>");
#endif
            m_instance = this;
            titleContent.text = "Quest Editor";
            Undo.undoRedoPerformed += Repaint;
#if UNITY_5 || UNITY_2017_1
            EditorApplication.playmodeStateChanged += OnPlaymodeStateChanged;
#else
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
            SelectQuestBasedOnCurrentSelection();
        }

        private void OnDisable()
        {
#if DEBUG_QUEST_EDITOR
            Debug.Log("<color=red>QuestEditorWindow.OnDisable</color>");
#endif
            m_instance = null;
            QuestEditorPrefs.SavePrefs();
            Undo.undoRedoPerformed -= Repaint;
#if UNITY_5 || UNITY_2017_1
            EditorApplication.playmodeStateChanged -= OnPlaymodeStateChanged;
#else
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
#endif
        }

#if UNITY_5 || UNITY_2017_1
        private void OnPlaymodeStateChanged()
        {
            HandlePlayModeStateChanged();
        }
#else
        private void OnPlayModeStateChanged(PlayModeStateChange obj)
        {
            HandlePlayModeStateChanged();
        }
#endif

        private void HandlePlayModeStateChanged()
        {
#if DEBUG_QUEST_EDITOR
            Debug.Log("<color=magenta>QuestEditorWindow.OnPlaymodeStateChanged. Application.isPlaying=" + Application.isPlaying + "</color>");
#endif
            SelectQuestBasedOnCurrentSelection();
            QuestEditorStyles.ResetImages(); // Unity loses dynamically-created textures when switching playmode.
            QuestEditorPrefs.SavePrefs();
            Repaint();
        }

        private void OnSelectionChange()
        {
            SelectQuestBasedOnCurrentSelection();
        }

        public void ShowQuestRelations(QuestDatabase database)
        {
            if (Selection.activeObject != database) Selection.activeObject = database;
            showQuestRelations = true;
            Repaint();
        }

        public void SelectQuest(Quest quest)
        {
            SelectQuest(quest, null, 0);
        }

        public void SelectQuest(QuestListContainer questListContainer)
        {
            if (questListContainer == null)
            {
                SelectQuest(null, null, 0);
                return;
            }
            var questListIndex = (questListContainer.GetInstanceID() == m_questListContainerInstanceID) ? m_questListIndex : 0;
            SelectQuest(questListContainer, Mathf.Max(0, questListIndex));
        }

        public void SelectQuest(QuestListContainer questListContainer, int questListIndex)
        {
            if (questListContainer == null)
            {
                SelectQuest(null, null, 0);
                return;
            }
            var quest = (0 <= questListIndex && questListIndex < questListContainer.questList.Count) ? questListContainer.questList[questListIndex] : null;
            SelectQuest(quest, questListContainer, questListIndex);
        }

        public void SelectQuest(Quest quest, QuestListContainer questListContainer, int questListIndex)
        {
            showQuestRelations = false;
            m_quest = quest;
            m_questSerializedObject = (quest != null) ? new SerializedObject(quest) : null;
            var questInstanceID = (quest != null) ? quest.GetInstanceID() : 0;
            if (quest != null && questInstanceID != m_questInstanceID) m_selectedNodeListIndex = -1;
            m_questInstanceID = questInstanceID;
            m_questListContainerInstanceID = (questListContainer != null) ? questListContainer.GetInstanceID() : 0;
            m_selectedQuestListContainer = questListContainer;
            m_questListIndex = questListIndex;
            m_canvasGUI.AssignQuest(quest);
#if DEBUG_QUEST_EDITOR
            Debug.Log("<color=magenta>QuestEditorWindow.SelectQuest: quest=" + quest + " [instanceID=" + questInstanceID + ", isAsset=" + 
                ((quest != null) ? (!quest.isInstance).ToString() : "NA") + "], questListContainer=" + questListContainer + 
                " [instanceID=" + m_questListContainerInstanceID + "], questListIndex=" + questListIndex + "</color>");
#endif
            CheckQuestFileVersion();
        }

        private void SelectQuestBasedOnCurrentSelection()
        {
#if DEBUG_QUEST_EDITOR
            var activeObjectName = (Selection.activeObject != null) ? Selection.activeObject.name + " (" + Selection.activeObject.GetType().Name + ")" : "<none>";
            Debug.Log("<color=cyan>QuestEditorWindow.SelectQuestBasedOnCurrentSelection: Selection.activeObject=" + activeObjectName + 
                ", activeGO=" + Selection.activeGameObject + "</color>");
#endif
            var selectionAsQuest = Selection.activeObject as Quest;
            var selectionAsQuestListContainer = (Selection.activeGameObject != null) ? Selection.activeGameObject.GetComponent<QuestListContainer>() : null;
            if (selectionAsQuest != null)
            {
                SelectQuest(selectionAsQuest);
            }
            else if (selectionAsQuestListContainer != null)
            {
                SelectQuest(selectionAsQuestListContainer);
            }
            else if (m_questListContainerInstanceID != 0) // Can't use current selection, but can use previously-selected QuestListContainer.
            {
                SelectQuest(EditorUtility.InstanceIDToObject(m_questListContainerInstanceID) as QuestListContainer);
            }
            else if (m_questInstanceID != 0) // Can't use current selection, but can use previously-selected Quest.
            {
                SelectQuest(EditorUtility.InstanceIDToObject(m_questInstanceID) as Quest);
            }
            else
            {
                SelectQuest(null, null, 0);
            }
            Repaint();
        }

        private void FocusSelectionOnQuest()
        {
            var questListContainer = (m_questListContainerInstanceID != 0) ? EditorUtility.InstanceIDToObject(m_questListContainerInstanceID) as QuestListContainer : null;
            if (questListContainer != null && questListContainer.questList != null &&
                0 <= m_questListIndex && m_questListIndex < questListContainer.questList.Count &&
                questListContainer.questList[m_questListIndex].GetInstanceID() == m_questInstanceID)
            {
                SelectQuest(questListContainer);
                Selection.activeGameObject = questListContainer.gameObject;
            }
            else
            {
                SelectQuest(m_quest);
                Selection.activeObject = m_quest;
            }
        }

        private void CheckQuestFileVersion()
        {
            if (m_quest != null && m_quest.isAsset && m_quest.fileVersion < CurrentFileVersion)
            {
                if (m_quest.fileVersion == 0)
                {
                    // A bug in 1.0.0 - 1.0.2 orphaned subassets when deleting a node. 
                    // This method scrubs them from the quest asset:
                    QuestEditorAssetUtility.DeleteUnusedSubassets(m_quest);
                }
                UpdateSelectedQuestSerializedObject();
                selectedQuestSerializedObject.FindProperty("m_fileVersion").intValue = CurrentFileVersion;
                ApplyModifiedPropertiesFromSelectedQuestSerializedObject();
            }
        }

        private void Update()
        {
            if (!Application.isPlaying || showQuestRelations) return;
            elapsedTime += Time.deltaTime;
            var frequency = QuestEditorPrefs.runtimeRepaintFrequency;
            if (frequency > 0 && elapsedTime > frequency) // Repaint at specified frequency.
            {
                elapsedTime = 0;
                if (m_quest == null && Selection.activeObject != null) LookForQuestOrListOnCurrentSelection();
                if (m_quest != null && m_quest.isInstance) UpdateRuntimeView();
            }
        }

        private void UpdateRuntimeView()
        {
            CheckCurrentRuntimeSelection();
            RepaintNow();
            RepaintCurrentEditorNow();
        }

        private void CheckCurrentRuntimeSelection()
        {

            if (m_questListContainerInstanceID == 0) return;
            var questListContainer = EditorUtility.InstanceIDToObject(m_questListContainerInstanceID) as QuestListContainer;
            if (questListContainer == null || questListContainer.questList == null) return;
            var wasQuestRemoved = !questListContainer.questList.Contains(m_quest);
            var gainedQuest = (m_quest == null) && (questListContainer.questList.Count > 0);
            if (wasQuestRemoved || gainedQuest) SelectQuest(questListContainer);
        }

        private void LookForQuestOrListOnCurrentSelection()
        {
            if (Selection.activeObject is Quest)
            {
                SelectQuest(Selection.activeObject as Quest);
                Repaint();
            }
            else if (Selection.activeGameObject != null)
            {
                var selectionAsQuestListContainer = Selection.activeGameObject.GetComponent<QuestListContainer>();
                if (selectionAsQuestListContainer != null)
                {
                    SelectQuest(selectionAsQuestListContainer);
                    Repaint();
                }
            }
        }

        private void OnGUI()
        {
#if DEBUG_QUEST_EDITOR
            //var activeObjectName = (Selection.activeObject != null) ? Selection.activeObject.name + " (" + Selection.activeObject.GetType().Name + ")" : "<none>";
            //EditorGUI.LabelField(new Rect(10, 20, position.width - 10, 200), "Selection.activeObject=" + activeObjectName +
            //    "\nSelection.activeGameObject=" + Selection.activeGameObject +
            //    "\nm_questInstanceID=" + m_questInstanceID + "\nm_questListContainerInstanceID=" + m_questListContainerInstanceID);
#endif
            DrawTitleImage();
            if (showQuestRelations && Selection.activeObject is QuestDatabase)
            {
                m_canvasGUI.DrawQuestRelations(Selection.activeObject as QuestDatabase);
            }
            else if (m_canvasGUI.IsQuestAssigned())
            {
                m_canvasGUI.Draw(position);
            }
            else
            {
                DrawNoSelection();
            }
        }

        public void DrawTitleImage()
        {
            if (instance == null || QuestEditorStyles.titleImage == null) return;
            var width = QuestEditorStyles.titleImage.width / 2;
            var height = QuestEditorStyles.titleImage.height / 2;
            var rect = new Rect(instance.position.width - width, instance.position.height - height, width, height);
            GUI.DrawTexture(rect, QuestEditorStyles.titleImage);
        }

        public void DrawNoSelection()
        {
            if (GUILayout.Button(new GUIContent("New quest asset...", "Create a new quest asset file."), GUILayout.Width(160)))
            {
                var filename = EditorUtility.SaveFilePanelInProject("Create Quest Asset", "New Quest", "asset", "Save new quest asset as:");
                if (!string.IsNullOrEmpty(filename))
                {
                    var type = System.Type.GetType("PixelCrushers.QuestMachine.Wrappers.Quest, Assembly-CSharp-firstpass");
                    if (type == null) type = System.Type.GetType("PixelCrushers.QuestMachine.Wrappers.Quest, Assembly-CSharp");
                    if (type != null)
                    {
                        AssetUtility.CreateAssetWithFilename(type, filename, true);
                    }
                    else
                    {
                        AssetUtility.CreateAssetWithFilename<Quest>(filename, true);
                    }
                }
            }
        }

    }
}

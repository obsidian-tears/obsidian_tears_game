using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    public class QuestGeneratorEditorWindow : EditorWindow
    {

        [MenuItem("Tools/Pixel Crushers/Quest Machine/Quest Generator", false, 2)]
        public static void ShowWindow()
        {
            GetWindow<QuestGeneratorEditorWindow>();
        }

        private static QuestGeneratorEditorWindow m_instance;

        public static QuestGeneratorEditorWindow instance { get { return m_instance; } }

        public static Editor currentEditor { get; set; }

        public static void RepaintNow()
        {
            if (instance != null) instance.Repaint();
        }

        [SerializeField]
        private QuestGeneratorEditorWindowGUI m_windowGUI = null;

        private void OnEnable()
        {
            m_instance = this;
            titleContent.text = "Quest Generator";
            if (m_windowGUI == null) m_windowGUI = new QuestGeneratorEditorWindowGUI();
            Undo.undoRedoPerformed += Repaint;
        }

        private void OnDisable()
        {
            m_instance = null;
            Undo.undoRedoPerformed -= Repaint;
        }

        private void OnGUI()
        {
            if (m_windowGUI == null) return;
            m_windowGUI.Draw(position.width);
        }
    }
}

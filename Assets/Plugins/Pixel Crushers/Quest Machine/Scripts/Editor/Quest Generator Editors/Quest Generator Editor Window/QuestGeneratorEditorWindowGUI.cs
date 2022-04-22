using UnityEngine;
using System;

namespace PixelCrushers.QuestMachine
{

    [Serializable]
    public class QuestGeneratorEditorWindowGUI
    {

        [SerializeField]
        private QuestGeneratorEditorWindowToolbar m_toolbar;

        [SerializeField]
        private Vector2 m_scrollPosition = Vector2.zero;

        [SerializeField]
        private QuestGeneratorEntityTypeListGUI m_entitiesGUI = new QuestGeneratorEntityTypeListGUI();

        [SerializeField]
        private QuestGeneratorFactionListGUI m_factionsGUI = new QuestGeneratorFactionListGUI();

        [SerializeField]
        private QuestGeneratorDriveListGUI m_drivesGUI = new QuestGeneratorDriveListGUI();

        [SerializeField]
        private QuestGeneratorUrgencyFunctionListGUI m_urgencyFunctionsGUI = new QuestGeneratorUrgencyFunctionListGUI();

        [SerializeField]
        private QuestGeneratorActionListGUI m_actionsGUI = new QuestGeneratorActionListGUI();

        [SerializeField]
        private QuestGeneratorDomainListGUI m_domainsGUI = new QuestGeneratorDomainListGUI();

        //[SerializeField]
        //private QuestGeneratorEntitySpecifierListGUI m_entitySpecifiersGUI = new QuestGeneratorEntitySpecifierListGUI();

        //[SerializeField]
        //private QuestGeneratorDomainSpecifierListGUI m_domainSpecifiersGUI = new QuestGeneratorDomainSpecifierListGUI();

        private const float ScrollbarWidth = 30;

        public void Draw(float width)
        {
            if (m_toolbar == null) m_toolbar = new QuestGeneratorEditorWindowToolbar();
            if (m_toolbar.Draw()) m_scrollPosition = Vector2.zero;
            var interiorWidth = width - ScrollbarWidth;
            try
            {
                m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition, false, true);
                switch (m_toolbar.index)
                {
                    case (int)QuestGeneratorEditorWindowToolbar.ToolbarIndex.Entities:
                        m_entitiesGUI.Draw(interiorWidth);
                        break;
                    case (int)QuestGeneratorEditorWindowToolbar.ToolbarIndex.Factions:
                        m_factionsGUI.Draw(interiorWidth);
                        break;
                    case (int)QuestGeneratorEditorWindowToolbar.ToolbarIndex.Drives:
                        m_drivesGUI.Draw(interiorWidth);
                        break;
                    case (int)QuestGeneratorEditorWindowToolbar.ToolbarIndex.Urgencies:
                        m_urgencyFunctionsGUI.Draw(interiorWidth);
                        break;
                    case (int)QuestGeneratorEditorWindowToolbar.ToolbarIndex.Actions:
                        m_actionsGUI.Draw(interiorWidth);
                        break;
                    case (int)QuestGeneratorEditorWindowToolbar.ToolbarIndex.Domains:
                        m_domainsGUI.Draw(interiorWidth);
                        break;
                }
            }
            finally
            {
                GUILayout.EndScrollView();
            }
        }
    }

}
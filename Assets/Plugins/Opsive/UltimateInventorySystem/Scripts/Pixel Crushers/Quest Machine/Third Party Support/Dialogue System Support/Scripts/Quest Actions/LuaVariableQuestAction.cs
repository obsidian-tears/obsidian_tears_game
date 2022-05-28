// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Quest action that sets a Dialogue System variable.
    /// </summary>
    public class LuaVariableQuestAction : QuestAction
    {

        [VariablePopup]
        [SerializeField]
        private string m_variable;

        [SerializeField]
        private MessageValue m_value;

        public string variable
        {
            get { return m_variable; }
            set { m_variable = value; }
        }

        private MessageValue value
        {
            get { return m_value; }
            set { m_value = value; }
        }

        public override void Execute()
        {
            if (string.IsNullOrEmpty(variable) || value == null) return;
            switch (value.valueType)
            {
                case MessageValueType.Int:
                    DialogueLua.SetVariable(variable, value.intValue);
                    break;
                case MessageValueType.String:
                    DialogueLua.SetVariable(variable, value.stringValue);
                    break;
            }
        }

        public override string GetEditorName()
        {
            return "Set Lua variable " + variable + " to " + value;
        }

    }

}

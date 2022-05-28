// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Quest action that runs Lua code.
    /// </summary>
    public class LuaQuestAction : QuestAction
    {

        [LuaScriptWizard]
        [SerializeField]
        private string m_luaCode;

        public string luaCode
        {
            get { return m_luaCode; }
            set { m_luaCode = value; }
        }

        public override void Execute()
        {
            base.Execute();
            Lua.Run(luaCode);
        }

        public override string GetEditorName()
        {
            return "Lua: " + luaCode;
        }

    }

}

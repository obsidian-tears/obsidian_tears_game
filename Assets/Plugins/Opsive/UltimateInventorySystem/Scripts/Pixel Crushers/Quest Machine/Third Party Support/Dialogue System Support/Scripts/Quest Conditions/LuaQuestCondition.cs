// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.QuestMachine.DialogueSystemSupport
{

    /// <summary>
    /// Quest condition that checks whether a Lua expression is true.
    /// </summary>
    public class LuaQuestCondition : QuestCondition
    {

        [LuaConditionsWizard]
        [SerializeField]
        private string m_conditions;

        [Tooltip("Check Conditions at this frequency in seconds.")]
        [SerializeField]
        private float m_frequencyToCheck = 1;

        public string conditions
        {
            get { return m_conditions; }
            set { m_conditions = value; }
        }

        public float frequencyToCheck
        {
            get { return m_frequencyToCheck; }
            set { m_frequencyToCheck = value; }
        }

        public override void StartChecking(System.Action trueAction)
        {
            base.StartChecking(trueAction);
            if (IsLuaConditionTrue())
            {
                SetTrue();
            }
            else if (frequencyToCheck > 0)
            {
                TimedCallbackManager.StartCallback(OnCallback, frequencyToCheck);
            }
        }

        public override void StopChecking()
        {
            base.StopChecking();
            TimedCallbackManager.StopCallback(OnCallback);
        }

        public void OnCallback()
        {
            if (IsLuaConditionTrue()) SetTrue();
        }

        private bool IsLuaConditionTrue()
        {
            return Lua.IsTrue(conditions);
        }

        public override string GetEditorName()
        {
            return "Lua: " + conditions;
        }

    }

}

// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Manages the UI elements for each indicator state. Controlled by a QuestIndicatorManager.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class QuestIndicatorUI : MonoBehaviour
    {

        [SerializeField]
        private GameObject m_offerDisabled = null;

        [SerializeField]
        private GameObject m_offer = null;

        [SerializeField]
        private GameObject m_talkDisabled = null;

        [SerializeField]
        private GameObject m_talk = null;

        [SerializeField]
        private GameObject m_interactDisabled = null;

        [SerializeField]
        private GameObject m_interact = null;

        [SerializeField]
        private GameObject[] m_custom = new GameObject[0];

        public GameObject[] custom
        {
            get { return m_custom; }
        }

        public virtual void SetIndicator(int index, bool value)
        {
            switch (index)
            {
                case (int)QuestIndicatorState.OfferDisabled:
                    SetActive(m_offerDisabled, value);
                    break;
                case (int)QuestIndicatorState.Offer:
                    SetActive(m_offer, value);
                    break;
                case (int)QuestIndicatorState.TalkDisabled:
                    SetActive(m_talkDisabled, value);
                    break;
                case (int)QuestIndicatorState.Talk:
                    SetActive(m_talk, value);
                    break;
                case (int)QuestIndicatorState.InteractDisabled:
                    SetActive(m_interactDisabled, value);
                    break;
                case (int)QuestIndicatorState.Interact:
                    SetActive(m_interact, value);
                    break;
                default:
                    var customIndex = index - (int)QuestIndicatorState.Custom0;
                    if (0 <= customIndex && customIndex < custom.Length) SetActive(custom[customIndex], value);
                    break;
            }
        }

        protected void SetActive(GameObject go, bool value)
        {
            if (go != null) go.SetActive(value);
        }

        public virtual void HideAllIndicators()
        {
            SetIndicator((int)QuestIndicatorState.OfferDisabled, false);
            SetIndicator((int)QuestIndicatorState.Offer, false);
            SetIndicator((int)QuestIndicatorState.TalkDisabled, false);
            SetIndicator((int)QuestIndicatorState.Talk, false);
            SetIndicator((int)QuestIndicatorState.InteractDisabled, false);
            SetIndicator((int)QuestIndicatorState.Interact, false);
            for (int i = 0; i < custom.Length; i++)
            {
                SetIndicator((int)QuestIndicatorState.Custom0 + i, false);
            }
        }

    }

}

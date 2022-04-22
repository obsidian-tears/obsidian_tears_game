using UnityEngine;
using System.Collections;

namespace PixelCrushers.LoveHate.Example
{

	/// <summary>
	/// This script maintains the faction info window in the lower
	/// right of the example scene.
	/// </summary>
	public class InteractionUI : MonoBehaviour 
	{

		public CanvasGroup introCanvasGroup;

		public RectTransform interactionPanel;

		public UnityEngine.UI.Text npcSummaryText;

		public UnityEngine.UI.Button flatterButton;
		public UnityEngine.UI.Button insultButton;
		public UnityEngine.UI.Button giveButton;
		public UnityEngine.UI.Button stealButton;

		private IEnumerator Start()
		{
			SetInteractionPanel(false);

			// Wait for intro canvas to close:
			float elapsed = 0;
			while (elapsed < 5)
			{
				if (IsInterruptKeyDown()) break;
				yield return null;
				elapsed += GameTime.deltaTime;
			}
			if (introCanvasGroup != null && introCanvasGroup.gameObject != null)
			{
				while (introCanvasGroup.alpha > 0.05f)
				{
					if (IsInterruptKeyDown()) break;
					yield return null;
					introCanvasGroup.alpha -= GameTime.deltaTime;
				}
				introCanvasGroup.gameObject.SetActive(false);
			}
		}

		private bool IsInterruptKeyDown()
		{
			return Input.GetKeyDown(KeyCode.Escape) ||
					Input.GetKeyDown(KeyCode.Return) ||
					Input.GetKeyDown(KeyCode.Space) ||
					Input.GetMouseButtonDown(0) ||
					Mathf.Abs(Input.GetAxis("Vertical")) > 0.1 ||
					Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1;
		}

		public void SetInteractionPanel(bool value)
		{
			if (interactionPanel != null && interactionPanel.gameObject != null)
			{
				interactionPanel.gameObject.SetActive(value);
			}
		}

		//public void SetDeedButtons(bool value)
		//{
		//	if (flatterButton != null) flatterButton.gameObject.SetActive(value);
		//	if (insultButton != null) insultButton.gameObject.SetActive(value);
		//	if (giveButton != null) giveButton.gameObject.SetActive(value);
		//	if (stealButton != null) stealButton.gameObject.SetActive(value);
		//}
		
	}

}

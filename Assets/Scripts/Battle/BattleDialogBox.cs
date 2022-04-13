using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleDialogBox : MonoBehaviour
{
    public int lettersPerSecond;

    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject dialogBox;
    [SerializeField] List<Text> actionTexts;
    [SerializeField] Color selectedColor;

    // Start is called before the first frame update
    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
    }

    public void UpdateActionSelection(int selectedIndex)
    {
        for (int i = 0; i < actionTexts.Count; i++)
        {
            if (i == selectedIndex)
            {
                actionTexts[i].color = selectedColor;
            }
            else
            {
                actionTexts[i].color = Color.black;
            }
        }
    }

    public bool selectorEnabled => actionSelector.activeInHierarchy;
    public void EnableActionSelector()
    {
        actionSelector.SetActive(true);
    }
    public void DisableActionSelector()
    {
        actionSelector.SetActive(false);
    }
}

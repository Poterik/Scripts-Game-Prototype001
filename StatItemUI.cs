using TMPro;
using UnityEngine;

public class StatItemUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;

    public void SetData(string name)
    { nameText.text = name;}
}

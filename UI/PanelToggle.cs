using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PanelToggle : MonoBehaviour
{
    public GameObject panel;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(Toggle);
    }

    public void Toggle()
    {
        if (panel == null) return;

        panel.SetActive(!panel.activeSelf);
    }
}

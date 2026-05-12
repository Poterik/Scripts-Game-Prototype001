using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StoryBox : MonoBehaviour
{
    private TextMeshProUGUI storyText;
    private int index = -1;
    private float showDelay = 10f;
    private Collider col;
    private MeshRenderer mr;

    private void Start()
    {
        col = GetComponent<Collider>();
        mr = GetComponent<MeshRenderer>();
    }

    [SerializeField]
    private List<string> storyContainer = new List<string> 
    {
        "Ricochet - Bullets bounce back at nearby enemies, losing 10% damage.",
        "Starting HP is 100 units.",
        "The minimum Attack Speed is 0.1. At the beginning it is 2.5.",
        "The minimum Damage is 25.",
        "Fired bullets have a starting speed that can be improved, but they gain speed with each passing second.",
        "Until you have enough damage and lifesteal, you won't heal.",
        "The minimum Attack Range is 3.",
        "Luck increases the chance of dropping cursed boxes",
        "You cannot lower the stats below the minimum.",
        "The chance of getting a cursed loot box depends on the game difficulty."
    };

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ShowStory());
        }
    }

    private IEnumerator ShowStory()
    {
        storyText.text = storyContainer[index].ToString();
        storyText.gameObject.SetActive(true);
        col.enabled = false;
        mr.enabled = false;
        yield return new WaitForSeconds(showDelay);
        storyText.gameObject.SetActive(false);
        Destroy(gameObject);
    }

    public int GetCount()
    {
        return storyContainer.Count;
    }

    public void SetValue(int value, TextMeshProUGUI text)
    {
        index = value;
        storyText = text;
    }
}

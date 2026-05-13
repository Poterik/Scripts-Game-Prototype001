using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro damageText;
    private float lifetime = 1f;
    private float speed = 2f;
    //private string quoiseColor = "#00FBFF";
    //private string pinkColor = "#FF0076";

    private void Awake()
    {
        damageText = GetComponent<TextMeshPro>();
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    public void SetDamageText(int damage, int color)
    {
        damageText.text = damage.ToString();
        switch (color)
        {
            case 1:
                damageText.color = new Color32(0, 251, 255, 255);//quoise
                break;
            default:
                damageText.color = new Color32(255, 0, 118, 255);//pink
                break;
        }
    }
}

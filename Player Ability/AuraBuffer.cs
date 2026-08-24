using UnityEngine;

public class AuraBuffer : MonoBehaviour
{
    private GameManager gameManager;
    private Transform playerPos;
    private MyPlayerControl playerControl;
    private PlayerFighter playerFighter;

    [Header("Settings")]
    private float lastUsedTime;
    private float buffDuration = 7.5f;
    private float oldSpeed;
    private float newSpeed = 15f;
    private float oldRegenTiming;
    private float newRegenTiming = 1.5f;

    private void Start()
    {
        gameManager = GameManager.Instance;
        playerPos = gameManager.player.transform;
        if (playerPos == null || gameManager == null) return;

        playerControl = gameManager.player.GetComponent<MyPlayerControl>();
        playerFighter = gameManager.player.GetComponent<PlayerFighter>();
        
        transform.SetParent(playerPos);
        transform.localPosition = Vector3.down;

        WriteOldData();
        AcceptBuff();
        Destroy(gameObject, buffDuration);
    }

    private void OnDestroy()
    {
        DeclineBuff();
    }

    private void WriteOldData()
    {
        oldSpeed = playerControl.moveSpeed;
        oldRegenTiming = playerFighter.regenCooldown;
    }

    private void AcceptBuff()
    {
        playerControl.moveSpeed = newSpeed;
        playerFighter.regenCooldown = newRegenTiming;
    }

    private void DeclineBuff()
    {
        playerControl.moveSpeed = oldSpeed;
        playerFighter.regenCooldown = oldRegenTiming;
    }
}

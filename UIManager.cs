using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Settings")]
    public GameObject escapeMenu;
    public GameObject escapeMenuAddon;
    public GameObject settingAddon;
    public TextMeshProUGUI sensivityIntText;
    public Slider sensivitySlider;

    private MyPlayerControl playerControl;
    private UpgradeManager upgradeManager;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeComponent();

        sensivitySlider.value = playerControl.mouseSensivity;
        sensivityIntText.text = sensivitySlider.value.ToString();
        sensivitySlider.onValueChanged.AddListener(OnSensivityChanged);
    }

    private void InitializeComponent()
    {
        //playerControl = GameManager.Instance.player.GetComponent<MyPlayerControl>();
        playerControl = FindAnyObjectByType<MyPlayerControl>();
        upgradeManager = FindAnyObjectByType<UpgradeManager>();

        escapeMenu.SetActive(false);
        escapeMenuAddon.SetActive(false);
        settingAddon.SetActive(false);
    }

    private void Update()
    {
        //if (GameManager.Instance.player == null) return;
        if (GameManager.Instance.gameOver) return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ToggleEscapeMenu();
        }
    }

    private void ToggleEscapeMenu()
    {
        if (UpgradeStatistics.Instance.statsPanel.activeSelf || upgradeManager.upgradeWindow.activeSelf)
        {
            //UpgradeStatistics.Instance.statsPanel.SetActive(false);
            //UpdateGameState(false);
            return;
        }

        bool willOpen = !escapeMenu.activeSelf;

        SetEscapeMenuActive(willOpen);
    }

    private void SetEscapeMenuActive(bool active)
    {
        escapeMenu.SetActive(active);
        escapeMenuAddon.SetActive(false);

        UpdateGameState(active);
    }

    public void UpdateGameState(bool isPaused)
    {
        Time.timeScale = isPaused ? 0f : 1f;
        playerControl.isMenu = isPaused;

        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;
    }

    public void ToggleSettings()
    {
        if (GameManager.Instance.gameOver) return;

        //bool willOpen = !settingAddon.activeSelf;
        bool willOpen = !escapeMenuAddon.activeSelf;
        Debug.Log("ToggleSetting status: " + willOpen);
        settingAddon.SetActive(willOpen);
        escapeMenuAddon.SetActive(willOpen);
    }

    private void OnSensivityChanged(float value)
    {
        playerControl.mouseSensivity = value;

        sensivityIntText.text = $"{value:N2}";  
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}

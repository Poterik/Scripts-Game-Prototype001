using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewUIManager : MonoBehaviour
{
    public enum MenuState { None, Menu, Settings, Handbook, Upgrade, Stats }
    public MenuState currentState = MenuState.None;
    public GameObject[] handbookPages;

    public bool isMenu = false;

    [Header("References")]
    private MyPlayerControl mPlayerControl;

    [Header("Sens")]
    public TextMeshProUGUI sensivityIntText;
    public Slider sensivitySlider;

    [Header("Panels Ref")]
    public GameObject menuPanel;
    public GameObject settingsPanel;
    public GameObject handbookPanel;
    public GameObject endStatPanel;

    public static NewUIManager Instance {  get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        mPlayerControl = GameManager.Instance.player.GetComponent<MyPlayerControl>();
        sensivitySlider.value = mPlayerControl.mouseSensivity;
        sensivityIntText.text = sensivitySlider.value.ToString();
        sensivitySlider.onValueChanged.AddListener(OnSensivityChanged);

        CloseAll();
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            switch (currentState)
            {
                case MenuState.None: OpenMenu(); break;
                case MenuState.Menu: CloseAll(); break;
                case MenuState.Settings: CloseAll(); break;
                case MenuState.Handbook: CloseAll(); break;
                case MenuState.Upgrade: break;
                case MenuState.Stats: break;
            }
        }
    }

    public void OpenMenu()
    {
        SetState(MenuState.Menu);
    }

    public void CloseAll()
    {
        SetState(MenuState.None);
    }

    public void OpenSettings()
    {
        //SetState(MenuState.Settings);
        SetState(currentState == MenuState.Settings ? MenuState.Menu : MenuState.Settings);
    }

    public void OpenHandbook()
    {
        //SetState(MenuState.Handbook);
        SetState(currentState == MenuState.Handbook ? MenuState.Menu : MenuState.Handbook);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void SetState(MenuState newState)
    {
        if (GameManager.Instance.gameOver) return;

        currentState = newState;

        isMenu = newState != MenuState.None;
        Time.timeScale = isMenu ? 0f : 1f;
        Cursor.lockState = isMenu ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isMenu;

        bool parentOpen = newState == MenuState.Menu
                   || newState == MenuState.Settings
                   || newState == MenuState.Handbook;
        menuPanel.SetActive(parentOpen);

        settingsPanel.SetActive(newState == MenuState.Settings);
        handbookPanel.SetActive(newState == MenuState.Handbook);

        foreach (var page in handbookPages)
        {
            page.SetActive(false);
        }
    }

    private void OnSensivityChanged(float value)
    {
        mPlayerControl.mouseSensivity = value;

        sensivityIntText.text = $"{value:N2}";
    }
}

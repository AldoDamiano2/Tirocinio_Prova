using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI refs")]
    [SerializeField] private GameObject pausePanel;     // il pannello da mostrare in pausa
    [SerializeField] private Button btnResume;          // bottone "Riprendi"
    [SerializeField] private Button btnExit;            // bottone "Esci"
    [SerializeField] private Selectable firstSelected;  // selezione iniziale (es. btnResume)

    [Header("Options")]
    [SerializeField] private bool lockCursorDuringPlay = true;

    public static bool IsPaused { get; private set; }

    void Awake()
    {
        // Assicura wiring dei bottoni (anche se ti dimentichi di collegarli via Inspector)
        if (btnResume != null) btnResume.onClick.AddListener(OnClickResume);
        if (btnExit != null)   btnExit.onClick.AddListener(OnClickExitToGui);
    }

    void Start()
    {
        SetPaused(false, force:true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SetPaused(!IsPaused);
    }

    public void OnClickResume()
    {
        SetPaused(false);
    }

    public void OnClickExitToGui()
    {
        // ripristina prima del cambio scena
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("gui", LoadSceneMode.Single);
    }

    private void SetPaused(bool pause, bool force=false)
    {
        if (!force && IsPaused == pause) return;
        IsPaused = pause;

        if (pausePanel != null) pausePanel.SetActive(pause);

        Time.timeScale = pause ? 0f : 1f;

        if (pause)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            if (firstSelected != null)
                EventSystem.current?.SetSelectedGameObject(firstSelected.gameObject);
        }
        else
        {
            Cursor.visible = !lockCursorDuringPlay;
            Cursor.lockState = lockCursorDuringPlay ? CursorLockMode.Locked : CursorLockMode.None;
            EventSystem.current?.SetSelectedGameObject(null);
        }
    }
}

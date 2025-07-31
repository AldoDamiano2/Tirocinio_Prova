using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string simulationSceneName = "Stanza";
    [SerializeField] private GameObject loadingPanel; // nuovo campo per il pannello

    // Questo metodo va collegato al click del pulsante
    public void OnStartButtonPressed()
    {
        // Mostra il pannello di caricamento
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
        }

        // Aspetta 1 secondo prima di caricare la scena
        Invoke("LoadSimulationScene", 1f);
    }

    private void LoadSimulationScene()
    {
        SceneManager.LoadScene(simulationSceneName);
    }

    public void OnQuitButtonPressed()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

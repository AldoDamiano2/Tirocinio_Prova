using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Nome della tua scena di simulazione
    [SerializeField] private string simulationSceneName = "Stanza";

    // Questo metodo va collegato al click del pulsante
    public void OnStartButtonPressed()
    {
        // Carica la scena di simulazione
        SceneManager.LoadScene(simulationSceneName);
    }

    // (Opzionale) Chiude l’applicazione
    public void OnQuitButtonPressed()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}

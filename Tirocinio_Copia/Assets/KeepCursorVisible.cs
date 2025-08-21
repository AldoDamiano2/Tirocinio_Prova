using UnityEngine;

public class KeepCursorVisible : MonoBehaviour
{
    void Start()
    {
        Cursor.lockState = CursorLockMode.None; // non bloccare
        Cursor.visible = true;                  // rendi visibile
    }

    void Update()
    {
        // Sblocco rapido se qualche altro script lo riblocca
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}

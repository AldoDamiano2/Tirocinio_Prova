using UnityEngine;

public class QuickJawTest : MonoBehaviour
{
    public Transform jawBone;
    public float openAngle = 210f;
    private Quaternion closedRot;

    void Start()
    {
        closedRot = jawBone.localRotation;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
            jawBone.localRotation = Quaternion.Euler(0, 0,-140);
        else
            jawBone.localRotation = closedRot;
    }
}

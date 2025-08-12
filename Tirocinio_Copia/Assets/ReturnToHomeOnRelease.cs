using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ReturnToHomeOnRelease : MonoBehaviour
{
    [Header("Dove tornare")]
    public Transform homeAnchor;            // trascina qui l'Empty "Home"

    [Header("Tempi")]
    public float delayAfterRelease = 1.0f;  // attesa prima di iniziare il ritorno
    public float returnDuration = 0.6f;     // quanto dura l'animazione di ritorno

    [Header("Fisica durante il ritorno")]
    public bool makeKinematicWhileReturning = true; // ignora fisica durante il ritorno

    private SimpleMouseGrab grab; 
    private Rigidbody rb;
    private bool returning;
    private float t;
    private Vector3 startPos;
    private Quaternion startRot;
    private float releaseTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grab = GetComponent<SimpleMouseGrab>();
    }

    void Update()
    {
        bool isGrabbed = grab != null && grab.IsGrabbed;

        if (returning)
        {
            if (makeKinematicWhileReturning) rb.isKinematic = true; else rb.useGravity = false;

            t += Time.deltaTime / Mathf.Max(0.0001f, returnDuration);
            Vector3 pos = Vector3.Lerp(startPos, homeAnchor.position, t);
            Quaternion rot = Quaternion.Slerp(startRot, homeAnchor.rotation, t);
            rb.MovePosition(pos);
            rb.MoveRotation(rot);

            if (t >= 1f)
            {
                returning = false;
                if (makeKinematicWhileReturning) rb.isKinematic = false; else rb.useGravity = true;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            return;
        }

        // non sto tenendo l'oggetto → parti il timer
        if (!isGrabbed)
        {
            if (releaseTime <= 0f) releaseTime = Time.time;

            if (Time.time - releaseTime >= delayAfterRelease && homeAnchor != null)
            {
                returning = true;
                t = 0f;
                startPos = transform.position;
                startRot = transform.rotation;
            }
        }
        else
        {
            // azzera il timer quando riprendi l'oggetto
            releaseTime = 0f;
        }
    }
}

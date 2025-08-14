using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SimpleMouseGrab : MonoBehaviour
{
    [Header("Controlli")]
    public KeyCode grabButton = KeyCode.Mouse0;   // afferra/rilascia
    public KeyCode rotateButton = KeyCode.Mouse1; // ruota mentre è preso
    public bool IsGrabbed => grabbed;  // <— ci serve per sapere quando l'hai rilasciato

    [Header("Distanze")]
    public float maxGrabDistance = 8f;    // distanza massima per afferrare
    public float holdDistance = 0.8f;     // distanza davanti alla camera
    public float minHoldDistance = 0.3f;  // minimo con rotellina
    public float maxHoldDistance = 3f;    // massimo con rotellina

    [Header("Movimento/Rotazione")]
    public float moveSpeed = 30f;         // quanto “insegue” il punto
    public float rotateSpeed = 200f;      // gradi/sec durante la rotazione
    public bool keepGrabPoint = true;     // mantiene il punto cliccato come ancoraggio

    [Header("Raycast")]
    public bool useSphereCast = true;     // più permissivo del raycast
    public float sphereRadius = 0.12f;
    public LayerMask raycastMask = ~0;

    private Camera cam;
    private Rigidbody rb;
    private bool grabbed = false;
    private Vector3 localGrabOffset = Vector3.zero;

    void Awake()
    {
        cam = Camera.main;
        rb  = GetComponent<Rigidbody>();

        if (!cam)
            Debug.LogError("SimpleMouseGrab: nessuna Camera taggata 'MainCamera' nella scena.");
        
        // consigli fisici per oggetti presi in mano
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    void Update()
    {
        if (Input.GetKeyDown(grabButton)) TryGrab();
        if (Input.GetKeyUp(grabButton))   Release();

        if (grabbed)
        {
            MoveTowardPointer();
            HandleRotation();
            HandleScrollZoom();
        }
    }

    void TryGrab()
    {
        if (grabbed || cam == null) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        bool hitSomething = false;
        RaycastHit hit;

        if (useSphereCast)
            hitSomething = Physics.SphereCast(ray, sphereRadius, out hit, maxGrabDistance, raycastMask, QueryTriggerInteraction.Collide);
        else
            hitSomething = Physics.Raycast(ray, out hit, maxGrabDistance, raycastMask, QueryTriggerInteraction.Collide);

        if (!hitSomething) return;

        // consenti click su collider figli: risali al Rigidbody del martello
        var hitRb = hit.rigidbody ? hit.rigidbody : hit.collider.GetComponentInParent<Rigidbody>();
        if (hitRb != rb) return; // hai cliccato qualcos'altro

        grabbed = true;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        localGrabOffset = keepGrabPoint
            ? transform.InverseTransformVector(transform.position - hit.point)
            : Vector3.zero;
    }

    void Release()
    {
        if (!grabbed) return;
        grabbed = false;
        rb.useGravity = true; // mantiene l'inerzia per “lanciare”
    }

    void MoveTowardPointer()
    {
        // segui il puntatore nello spazio: proietta il mouse a 'holdDistance'
        Vector3 screen = new Vector3(Input.mousePosition.x, Input.mousePosition.y, holdDistance);
        Vector3 target = cam.ScreenToWorldPoint(screen);

        if (keepGrabPoint)
            target += cam.transform.TransformVector(localGrabOffset);

        Vector3 desiredVel = (target - transform.position) * moveSpeed;
        rb.velocity = desiredVel;
    }

    void HandleRotation()
    {
        if (!Input.GetKey(rotateButton)) return;

        float dx = Input.GetAxis("Mouse X");
        float dy = Input.GetAxis("Mouse Y");

        // yaw intorno all'UP della camera, pitch intorno alla RIGHT
        Quaternion yaw   = Quaternion.AngleAxis(dx * rotateSpeed * Time.deltaTime, cam.transform.up);
        Quaternion pitch = Quaternion.AngleAxis(-dy * rotateSpeed * Time.deltaTime, cam.transform.right);

        rb.MoveRotation(yaw * pitch * rb.rotation);
    }

    void HandleScrollZoom()
    {
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.0001f)
        {
            holdDistance = Mathf.Clamp(holdDistance + scroll * 0.2f, minHoldDistance, maxHoldDistance);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (cam == null) cam = Camera.main;
        if (cam == null) return;

        Gizmos.color = Color.cyan;
        Vector3 p = cam.transform.position + cam.transform.forward * holdDistance;
        Gizmos.DrawWireSphere(p, 0.03f);
    }
#endif
}

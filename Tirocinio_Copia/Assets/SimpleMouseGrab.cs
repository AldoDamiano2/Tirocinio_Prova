using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SimpleMouseGrab : MonoBehaviour
{
    [Header("Controlli")]
    public KeyCode grabButton = KeyCode.Mouse0;   // tasto per afferrare/rilasciare
    public KeyCode rotateButton = KeyCode.Mouse1; // tasto per ruotare mentre è preso

    [Header("Parametri")]
    public float maxGrabDistance = 3f;    // distanza massima del raycast
    public float holdDistance = 0.8f;     // distanza davanti alla camera
    public float moveSpeed = 25f;         // velocità di inseguimento
    public float rotateSpeed = 180f;      // gradi/sec durante la rotazione
    public bool keepGrabPoint = true;     // mantiene il punto cliccato come "ancora"

    [Header("Raycast")]
    public LayerMask raycastMask = ~0;    // quali layer può colpire il raycast

    Camera cam;
    Rigidbody rb;
    bool grabbed = false;
    Vector3 localGrabOffset = Vector3.zero;

    void Awake()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();

        if (!cam)
        {
            Debug.LogError("SimpleMouseGrab: nessuna Camera con tag MainCamera trovata nella scena.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(grabButton)) TryGrab();
        if (Input.GetKeyUp(grabButton)) Release();

        if (grabbed)
        {
            MoveTowardHoldPoint();
            HandleRotation();
        }
    }

    void TryGrab()
    {
        if (grabbed || cam == null) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, maxGrabDistance, raycastMask, QueryTriggerInteraction.Ignore))
        {
            // afferra solo se hai cliccato proprio questo oggetto (il suo rigidbody)
            if (hit.rigidbody == rb)
            {
                grabbed = true;
                rb.useGravity = false;

                if (keepGrabPoint)
                {
                    // memorizza offset locale dal centro al punto cliccato per evitare "snap"
                    localGrabOffset = transform.InverseTransformVector(transform.position - hit.point);
                }
                else
                {
                    localGrabOffset = Vector3.zero;
                }
            }
        }
    }

    void Release()
    {
        if (!grabbed) return;
        grabbed = false;
        rb.useGravity = true; // mantiene la velocità accumulata per "lanciare"
    }

    void MoveTowardHoldPoint()
    {
        // punto target davanti alla camera
        Vector3 target = cam.transform.position + cam.transform.forward * holdDistance;

        // applica offset per mantenere il punto cliccato come "ancora"
        if (keepGrabPoint)
            target += cam.transform.TransformVector(localGrabOffset);

        // insegui il target con la fisica (evita teletrasporti)
        Vector3 vel = (target - transform.position) * moveSpeed;
        rb.velocity = vel;
    }

    void HandleRotation()
    {
        if (!Input.GetKey(rotateButton)) return;

        float dx = Input.GetAxis("Mouse X");
        float dy = Input.GetAxis("Mouse Y");

        // yaw intorno all'UP della camera, pitch intorno alla RIGHT della camera
        Quaternion yaw = Quaternion.AngleAxis(dx * rotateSpeed * Time.deltaTime, cam.transform.up);
        Quaternion pitch = Quaternion.AngleAxis(-dy * rotateSpeed * Time.deltaTime, cam.transform.right);

        rb.MoveRotation(yaw * pitch * rb.rotation);
    }
}

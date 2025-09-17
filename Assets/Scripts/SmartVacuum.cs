using UnityEngine;
using System.Linq;
using System.Collections;

public class SmartVacuumAvoid : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rayDistance = 1.2f;
    [SerializeField] private float rotationSpeed = 200f;
    [SerializeField] private LayerMask obstacleMask;

    private Rigidbody rb;
    private GameObject targetTrash;
    private int collectedTrash = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        FindNextTrash();
    }

    void FixedUpdate()
    {
        if (!targetTrash) return;

        Vector3 toTrash = (targetTrash.transform.position - transform.position).normalized;
        bool hitFront = Physics.Raycast(transform.position, transform.forward, rayDistance, obstacleMask);
        bool hitLeft = Physics.Raycast(transform.position, -transform.right, rayDistance, obstacleMask);
        bool hitRight = Physics.Raycast(transform.position, transform.right, rayDistance, obstacleMask);

        Vector3 moveDir = toTrash;

        if (hitFront)
        {
            float turn = hitLeft && !hitRight ? 1f : (!hitLeft && hitRight ? -1f : (Random.value > 0.5f ? 1f : -1f));
            transform.Rotate(Vector3.up * turn * rotationSpeed * Time.fixedDeltaTime);
        }

        Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime);

        rb.MovePosition(rb.position + transform.forward * moveSpeed * Time.fixedDeltaTime);

        if (Vector3.Distance(transform.position, targetTrash.transform.position) < 0.5f)
        {
            FindNextTrash();
        }
    }

    void FindNextTrash()
    {
        GameObject[] trashObjects = GameObject.FindGameObjectsWithTag("Trash")
            .Where(t => t.activeInHierarchy)
            .ToArray();

        if (trashObjects.Length == 0)
        {
            targetTrash = null;
            return;
        }

        targetTrash = trashObjects.OrderBy(t => Vector3.Distance(transform.position, t.transform.position)).FirstOrDefault();
        if (targetTrash != null)
        {
            Debug.Log($"➡️ Иду к мусору: {targetTrash.name}, ID: {targetTrash.GetInstanceID()}");
        }
        else
        {
            Debug.LogWarning("Не удалось найти следующий мусор!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trash"))
        {
            other.gameObject.SetActive(false);
            Destroy(other.gameObject);
            collectedTrash++;
            StartCoroutine(FindNextTrashWithDelay());
        }
    }

    private IEnumerator FindNextTrashWithDelay()
    {
        yield return new WaitForEndOfFrame();
        FindNextTrash();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * rayDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position - transform.right * rayDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * rayDistance);

        if (targetTrash != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(targetTrash.transform.position, 0.5f);
        }
    }
}











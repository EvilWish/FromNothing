using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 velocity;

    [Header("Movement")]
    [SerializeField] public float speed = 5f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;   
    [SerializeField] private float groundedGravity = -2f; 

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayers;      
    [SerializeField] private float groundProbeDistance = 0.2f; 
    [SerializeField] private float groundProbeSkin = 0.02f;

    private bool grounded;
    private RaycastHit groundHit;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public void ProcessMove(Vector2 input)
    {
        Vector3 move = new Vector3(input.x, 0f, input.y);
        move = transform.TransformDirection(move);
        controller.Move(move * speed * Time.deltaTime);

        grounded = GroundCheck(out groundHit);

        if (grounded)
        {
            if (velocity.y < 0f)
                velocity.y = groundedGravity;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    public void Jump(float jumpHeight = 1.2f)
    {
        if (!grounded) return;
        velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);
    }

    private bool GroundCheck(out RaycastHit hit)
    {
        // Fußpunkt des Controllers (ungefähr tangent an die Kapsel)
        Vector3 feet = controller.bounds.center
                     - Vector3.up * (controller.height * 0.5f - controller.radius)
                     + Vector3.up * groundProbeSkin;

        float sphereRadius = Mathf.Max(0.01f, controller.radius * 0.95f);

        // Cast straight down; QueryTriggerInteraction.Ignore, damit Trigger nicht zählen
        bool hasHit = Physics.SphereCast(
            origin: feet,
            radius: sphereRadius,
            direction: Vector3.down,
            hitInfo: out hit,
            maxDistance: groundProbeDistance,
            layerMask: groundLayers,
            queryTriggerInteraction: QueryTriggerInteraction.Ignore
        );

        return hasHit;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!TryGetComponent(out CharacterController cc)) return;

        Vector3 feet = cc.bounds.center
                     - Vector3.up * (cc.height * 0.5f - cc.radius)
                     + Vector3.up * groundProbeSkin;

        float sphereRadius = Mathf.Max(0.01f, cc.radius * 0.95f);

        // Startkugel
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(feet, sphereRadius);

        // Endkugel
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(feet + Vector3.down * groundProbeDistance, sphereRadius);

        // Linie dazwischen
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(feet, feet + Vector3.down * groundProbeDistance);
    }
#endif
}


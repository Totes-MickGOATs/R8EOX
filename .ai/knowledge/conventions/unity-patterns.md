# Unity MonoBehaviour Patterns

## Lifecycle Methods

```
Awake()         → Cache component references, initialize internal state
OnEnable()      → Subscribe to events, register with systems
Start()         → Initialize state that depends on other objects being ready
Update()        → Per-frame logic (prefer explicit Tick() called by parent)
FixedUpdate()   → Physics operations ONLY
LateUpdate()    → Camera follow, post-physics adjustments
OnDisable()     → Unsubscribe from events, deregister from systems
OnDestroy()     → Final cleanup
```

## Caching

```csharp
// CORRECT — cache in Awake, use in Tick
private Rigidbody rb;
private Camera mainCam;

private void Awake()
{
    rb = GetComponent<Rigidbody>();
    mainCam = Camera.main; // Cache — Camera.main does FindObjectWithTag internally
}

public void Tick()
{
    // Use cached references
    rb.AddForce(Vector3.up);
}
```

```csharp
// WRONG — repeated GetComponent and Camera.main in Update
private void Update()
{
    GetComponent<Rigidbody>().AddForce(Vector3.up);  // Slow
    Camera.main.transform.position = transform.position; // Very slow
}
```

## Tag Comparison

```csharp
// CORRECT
if (other.CompareTag("Player")) { }

// WRONG — allocates string
if (other.tag == "Player") { }
```

## Prefer Explicit Tick() Over Auto-Update()

Per the top-down architecture, system managers should call sub-component methods directly:

```csharp
// System manager drives updates
public class CombatManager : MonoBehaviour
{
    [SerializeField] private List<CombatUnit> units;

    private void Update()
    {
        foreach (var unit in units)
        {
            unit.Tick(Time.deltaTime);
        }
    }
}

// Sub-component has NO Update() — parent calls Tick()
namespace R8EOX.Combat.Internal
{
    internal class CombatUnit
    {
        public void Tick(float deltaTime) { }
    }
}
```

## Physics

- Physics operations go in `FixedUpdate()`, visual updates in `Update()` or `LateUpdate()`
- Use `CompareTag()` instead of string comparison
- Prefer events and callbacks over polling in Update where possible

## Null Safety

- Always null-check `GetComponent<T>()` results
- Always null-check `Find*()` results
- Check serialized references are assigned (consider `[RequireComponent]`)

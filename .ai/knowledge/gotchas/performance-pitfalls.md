# Performance Pitfalls

## Update Loop Abuse

Never do heavy work in `Update()`, `FixedUpdate()`, or `LateUpdate()`:

- **No allocations**: Avoid `new List<>()`, string concatenation (`+`), LINQ queries, `ToString()`
- **No GetComponent**: Cache in `Awake()` — `GetComponent` uses reflection
- **No Camera.main**: Calls `FindGameObjectWithTag` internally — cache in `Awake()`
- **No Find* methods**: `FindObjectOfType`, `FindObjectsOfType` scan the entire scene
- **No tag string comparison**: Use `CompareTag("tag")` not `tag == "tag"` (avoids allocation)

## Caching Pattern

```csharp
// Cache everything in Awake
private Transform cachedTransform;
private Rigidbody rb;
private Camera mainCam;

private void Awake()
{
    cachedTransform = transform;
    rb = GetComponent<Rigidbody>();
    mainCam = Camera.main;
}
```

## Physics

- Physics operations MUST go in `FixedUpdate()` — not `Update()`
- Use `FixedUpdate()` for: `AddForce`, `MovePosition`, raycasts that affect physics
- Use `Update()` for: input reading, visual updates, non-physics logic
- Use `LateUpdate()` for: camera follow, post-physics adjustments

## String Operations

```csharp
// WRONG — allocates new string every frame
void Update() { debugText.text = "Score: " + score; }

// CORRECT — only update when value changes
private int lastScore = -1;
void Update()
{
    if (score != lastScore)
    {
        debugText.text = $"Score: {score}";
        lastScore = score;
    }
}
```

## Collections

- Pre-allocate lists with expected capacity: `new List<T>(expectedSize)`
- Use `List<T>.Clear()` instead of creating new lists
- Avoid LINQ in hot paths — use loops instead
- Consider `NativeArray<T>` for large datasets

## Event-Driven vs Polling

Prefer events/callbacks over per-frame polling:
```csharp
// WRONG — polling every frame
void Update()
{
    if (health <= 0) Die();
}

// CORRECT — event-driven
public void TakeDamage(int amount)
{
    health -= amount;
    if (health <= 0) Die();
}
```

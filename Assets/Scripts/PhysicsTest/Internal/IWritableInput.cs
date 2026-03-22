namespace R8EOX.PhysicsTest.Internal
{
    /// <summary>
    /// Minimal writable input surface used by PathFollower to command a vehicle.
    /// ScriptedInput satisfies this contract; cast and assign at the call site.
    /// </summary>
    internal interface IWritableInput
    {
        float Throttle { get; set; }
        float Brake    { get; set; }
        float Steer    { get; set; }
    }
}

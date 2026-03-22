namespace R8EOX.PhysicsTest.Internal
{
    /// <summary>
    /// Bridges R8EOX.Input.Internal.ScriptedInput to the local IWritableInput contract
    /// without importing a cross-system internal namespace via a using directive.
    /// </summary>
    internal class InputAdapter : IWritableInput
    {
        private readonly R8EOX.Input.Internal.ScriptedInput _si;

        internal InputAdapter(R8EOX.Input.Internal.ScriptedInput si) => _si = si;

        public float Throttle { get => _si.Throttle; set => _si.Throttle = value; }
        public float Brake    { get => _si.Brake;    set => _si.Brake    = value; }
        public float Steer    { get => _si.Steer;    set => _si.Steer    = value; }
    }
}

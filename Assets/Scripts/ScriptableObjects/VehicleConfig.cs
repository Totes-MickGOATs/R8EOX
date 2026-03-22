using UnityEngine;

namespace R8EOX
{
    [CreateAssetMenu(fileName = "NewVehicleConfig", menuName = "R8EOX/VehicleConfig")]
    public class VehicleConfig : ScriptableObject
    {
        [Header("Engine")]
        [SerializeField] private float maxRPM = 8000f;
        [SerializeField] private float idleRPM = 800f;
        [SerializeField] private float maxHorsepower = 450f;
        [SerializeField] private AnimationCurve torqueCurve;

        [Header("Transmission")]
        [SerializeField] private float[] gearRatios = { 3.5f, 2.5f, 1.8f, 1.3f, 1.0f, 0.8f };
        [SerializeField] private float finalDriveRatio = 3.7f;

        [Header("Chassis")]
        [SerializeField] private float mass = 1400f;
        [SerializeField] private Vector3 centerOfMass = new(0f, -0.3f, 0.2f);
        [SerializeField] private float dragCoefficient = 0.35f;

        [Header("Wheels")]
        [SerializeField] private float wheelRadius = 0.34f;
        [SerializeField] private float suspensionTravel = 0.15f;
        [SerializeField] private float suspensionStiffness = 35000f;
        [SerializeField] private float tireFriction = 1.2f;

        [Header("Handling")]
        [SerializeField] private float maxSteerAngle = 35f;
        [SerializeField] private float steerSpeed = 5f;
        [SerializeField] private float brakeTorque = 5000f;

        public float MaxRPM => maxRPM;
        public float IdleRPM => idleRPM;
        public float MaxHorsepower => maxHorsepower;
        public AnimationCurve TorqueCurve => torqueCurve;
        public float[] GearRatios => gearRatios;
        public float FinalDriveRatio => finalDriveRatio;
        public float Mass => mass;
        public Vector3 CenterOfMass => centerOfMass;
        public float DragCoefficient => dragCoefficient;
        public float WheelRadius => wheelRadius;
        public float SuspensionTravel => suspensionTravel;
        public float SuspensionStiffness => suspensionStiffness;
        public float TireFriction => tireFriction;
        public float MaxSteerAngle => maxSteerAngle;
        public float SteerSpeed => steerSpeed;
        public float BrakeTorque => brakeTorque;
    }
}

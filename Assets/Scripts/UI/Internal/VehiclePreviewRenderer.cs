using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;

namespace R8EOX.UI.Internal
{
    internal class VehiclePreviewRenderer : MonoBehaviour,
        IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private UnityEngine.UI.RawImage targetImage;
        [SerializeField] private float autoRotateSpeed = 15f;
        [SerializeField] private float dragRotateMultiplier = 0.5f;

        private GameObject stageRoot;
        private Transform turntable;
        private Camera previewCamera;
        private RenderTexture previewTexture;
        private GameObject currentPreviewInstance;
        private float currentRotationVelocity;
        private bool isDragging;

        internal void Initialize()
        {
            stageRoot = new GameObject("VehiclePreviewStage");
            stageRoot.transform.position = new Vector3(0f, -500f, 0f);

            var turntableObj = new GameObject("Turntable");
            turntableObj.transform.SetParent(stageRoot.transform, false);
            turntable = turntableObj.transform;

            CreateLightRig();
            CreatePreviewCamera();

            previewTexture = new RenderTexture(1024, 1024, 24, RenderTextureFormat.ARGB32);
            previewTexture.Create();
            previewCamera.targetTexture = previewTexture;

            if (targetImage != null)
                targetImage.texture = previewTexture;
        }

        internal void SetVehicle(GameObject prefab)
        {
            if (prefab == null) return;

            if (currentPreviewInstance != null)
                Destroy(currentPreviewInstance);

            currentPreviewInstance = Instantiate(prefab, turntable);
            currentPreviewInstance.transform.localPosition = Vector3.zero;
            currentPreviewInstance.transform.localRotation = Quaternion.identity;

            int previewLayer = LayerMask.NameToLayer("VehiclePreview");
            SetLayerRecursive(currentPreviewInstance, previewLayer);
            StripComponents(currentPreviewInstance);

            turntable.localRotation = Quaternion.Euler(0f, 200f, 0f);
            FrameCamera();
        }

        internal void Teardown()
        {
            if (targetImage != null)
                targetImage.texture = null;

            if (previewTexture != null)
            {
                previewTexture.Release();
                Destroy(previewTexture);
                previewTexture = null;
            }

            if (stageRoot != null)
            {
                Destroy(stageRoot);
                stageRoot = null;
            }

            currentPreviewInstance = null;
            previewCamera = null;
            turntable = null;
        }

        private void Update()
        {
            if (turntable == null) return;

            if (!isDragging)
            {
                float rotation = autoRotateSpeed + currentRotationVelocity;
                turntable.Rotate(0f, rotation * Time.unscaledDeltaTime, 0f, Space.World);
                currentRotationVelocity = Mathf.Lerp(currentRotationVelocity, 0f, Time.unscaledDeltaTime * 5f);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isDragging = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (turntable == null) return;
            turntable.Rotate(0f, eventData.delta.x * -dragRotateMultiplier, 0f, Space.World);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isDragging = false;
            currentRotationVelocity = eventData.delta.x * -dragRotateMultiplier * 10f;
        }

        private void CreateLightRig()
        {
            CreateDirectionalLight("KeyLight",
                new Vector3(-40f, 35f, 0f), 1.2f, new Color(1f, 0.95f, 0.88f), true);
            CreateDirectionalLight("FillLight",
                new Vector3(-20f, -150f, 0f), 0.5f, new Color(0.75f, 0.82f, 1f), false);
            CreateDirectionalLight("RimLight",
                new Vector3(-15f, 180f, 0f), 0.5f, new Color(0.6f, 0.7f, 1f), false);
        }

        private void CreateDirectionalLight(string lightName, Vector3 rotation,
            float intensity, Color color, bool shadows)
        {
            var lightObj = new GameObject(lightName);
            lightObj.transform.SetParent(stageRoot.transform, false);
            lightObj.transform.localRotation = Quaternion.Euler(rotation);

            var light = lightObj.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = intensity;
            light.color = color;
            light.shadows = shadows ? LightShadows.Soft : LightShadows.None;
        }

        private void CreatePreviewCamera()
        {
            var camObj = new GameObject("PreviewCamera");
            camObj.transform.SetParent(stageRoot.transform, false);

            previewCamera = camObj.AddComponent<Camera>();
            previewCamera.cullingMask = 1 << LayerMask.NameToLayer("VehiclePreview");
            previewCamera.fieldOfView = 35f;
            previewCamera.clearFlags = CameraClearFlags.SolidColor;
            previewCamera.backgroundColor = new Color(0.08f, 0.08f, 0.09f);
            previewCamera.nearClipPlane = 0.1f;
            previewCamera.farClipPlane = 100f;

            var camData = camObj.AddComponent<UniversalAdditionalCameraData>();
            camData.renderType = CameraRenderType.Base;
        }

        private void FrameCamera()
        {
            if (currentPreviewInstance == null || previewCamera == null) return;

            var renderers = currentPreviewInstance.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) return;

            var bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
                bounds.Encapsulate(renderers[i].bounds);

            float padding = 1.3f;
            float maxExtent = bounds.extents.magnitude * padding;
            float fovRad = previewCamera.fieldOfView * 0.5f * Mathf.Deg2Rad;
            float distance = maxExtent / Mathf.Tan(fovRad);

            float elevationRad = 25f * Mathf.Deg2Rad;
            float azimuthRad = 35f * Mathf.Deg2Rad;

            var offset = new Vector3(
                Mathf.Sin(azimuthRad) * Mathf.Cos(elevationRad),
                Mathf.Sin(elevationRad),
                Mathf.Cos(azimuthRad) * Mathf.Cos(elevationRad)
            ) * distance;

            previewCamera.transform.position = bounds.center + offset;
            previewCamera.transform.LookAt(bounds.center);
        }

        private static void StripComponents(GameObject root)
        {
            var components = root.GetComponentsInChildren<Component>(true);
            for (int i = components.Length - 1; i >= 0; i--)
            {
                var c = components[i];
                if (c is Transform) continue;
                if (c is MeshFilter) continue;
                if (c is MeshRenderer) continue;
                if (c is SkinnedMeshRenderer) continue;
                if (c is Rigidbody rb)
                {
                    Destroy(rb);
                    continue;
                }
                if (c is MonoBehaviour mb)
                    Destroy(mb);
            }
        }

        private static void SetLayerRecursive(GameObject obj, int layer)
        {
            obj.layer = layer;
            for (int i = 0; i < obj.transform.childCount; i++)
                SetLayerRecursive(obj.transform.GetChild(i).gameObject, layer);
        }
    }
}

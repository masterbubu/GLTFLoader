using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class App : MonoBehaviour
{
    // Start is called before the first frame update

    // ------------------------------------------------------------------------
    async void Start()
    {
        m_LoadedObject = new GameObject("glTF");
        GLTFast.GltfAsset Asset = m_LoadedObject.AddComponent<GLTFast.GltfAsset>();
        await Asset.Load(m_URL);
        AdjustBounds();
    }

    // ------------------------------------------------------------------------
    void Update()
    {
        if (m_LoadedObject == null)
        {
            return;
        }
        // Orbit
        m_Camera.transform.position = m_LoadedObject.transform.position;
        m_Camera.transform.rotation = Quaternion.Euler(m_RotationX, m_RotationY, 0);
        m_Camera.transform.Translate(0, 0, -Mathf.Lerp(m_MinSize, m_MaxSize, m_Distance));
    }
    // ------------------------------------------------------------------------
    // Get encapsulated bounds
    Bounds GetObjectBounds(GameObject obj)
    {
        // Calculate bounds including all child renderers
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        Bounds bounds = new Bounds();

        for (int i = 0; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        return bounds;
    }
    // UI controllers
    // ------------------------------------------------------------------------
    public void OnCamXSliderValueChanged(float value)
    {
        m_RotationX = value;
    }
    // ------------------------------------------------------------------------
    public void OnCamYSliderValueChanged(float value)
    {
        m_RotationY = value;
    }
    // ------------------------------------------------------------------------
    public void OnCamDistanceSliderValueChanged(float value)
    {
        m_Distance = value;
    }
    // ------------------------------------------------------------------------
    public void OnInputFieldChanged(string value)
    {
        if (value != m_URL)
        {
            m_URL = value;
            LoadGltf();
        }
    }
    // ------------------------------------------------------------------------
    void AdjustBounds()
    {
        // Get the bounds of the target object
        Bounds targetBounds = GetObjectBounds(m_LoadedObject);
        Vector3 Size = targetBounds.size;
        float MaxSize = Mathf.Max(Size.x, Mathf.Max(Size.y, Size.z));
        Vector3 BacgroundSize = Vector3.one * MaxSize * BackgourdCubeScaleFator;

        m_BackgroundCube.transform.position = targetBounds.center;
        m_BackgroundCube.transform.localScale = BacgroundSize;

        float ModelFloor = Size.y * 0.5f;
        float BackgroundHeight = BacgroundSize.y * 0.5f;
        float Diff = BackgroundHeight - ModelFloor;

        m_BackgroundCube.transform.position += Vector3.up * Diff;

        m_MaxSize = 0.8f * 0.5f * Mathf.Max(BacgroundSize.x, Mathf.Max(BacgroundSize.y, BacgroundSize.z));
        m_MinSize = 1.25f * MaxSize;
    }
    // ------------------------------------------------------------------------
    // Load GLTF, and adjust the background cube to bound the model.

    async void LoadGltf()
    {
        if (m_LoadedObject != null)
        {
            GameObject.Destroy(m_LoadedObject);
        }

        if (!string.IsNullOrEmpty(m_URL))
        {
            m_LoadedObject = new GameObject("glTF");
            GLTFast.GltfAsset Asset = m_LoadedObject.AddComponent<GLTFast.GltfAsset>();
            await Asset.Load(m_URL);
            AdjustBounds();
        }
    }
    // Factoring the size of the background bounding box.
    public float BackgourdCubeScaleFator = 15;
    // Background walls.
    public GameObject m_BackgroundCube;
    // Viewing camera.
    public Camera m_Camera;

    // Loaded object.
    private GameObject m_LoadedObject;

    // Model to load url path.
    private string m_URL = "https://raw.githubusercontent.com/KhronosGroup/glTF-Sample-Models/master/2.0/Duck/glTF/Duck.gltf";

    // Min/Max zoom distances
    float m_MaxSize;
    float m_MinSize;

    private float m_Distance = 0.25f; // Distance from the target, computed from loaded model.
    private float m_RotationX = 45.0f; // Rotation angle around the X-axis in degrees
    private float m_RotationY = 30.0f; // Rotation angle around the Y-axis in degrees
}

using UnityEngine;
using UnityEngine.Timeline;

[ExecuteInEditMode]
[AddComponentMenu("Kvant/Warp")]
public class NoiseBall2 : MonoBehaviour, ITimeControl
{
    #region Exposed attributes

    [SerializeField] int _triangleCount = 100;

    public int triangleCount {
        get { return _triangleCount; }
        set { _triangleCount = value; }
    }

    [SerializeField] float _triangleExtent = 0.1f;

    public float triangleExtent {
        get { return _triangleExtent; }
        set { _triangleExtent = value; }
    }

    [SerializeField] float _shuffleSpeed = 4;

    public float shuffleSpeed {
        get { return _shuffleSpeed; }
        set { _shuffleSpeed = value; }
    }

    [SerializeField] float _noiseAmplitude = 1;

    public float noiseAmplitude {
        get { return _noiseAmplitude; }
        set { _noiseAmplitude = value; }
    }

    [SerializeField] float _noiseFrequency = 1;

    public float noiseFrequency {
        get { return _noiseFrequency; }
        set { _noiseFrequency = value; }
    }

    [SerializeField] Vector3 _noiseMotion = Vector3.up;

    public Vector3 noiseMotion {
        get { return _noiseMotion; }
        set { _noiseMotion = value; }
    }

    [SerializeField] Material _material;

    public Material material {
        get { return _material; }
    }

    [SerializeField] int _randomSeed;

    public int randomSeed {
        get { return _randomSeed; }
        set { _randomSeed = value; }
    }

    #endregion

    #region Hidden attributes

    [SerializeField, HideInInspector] ComputeShader _compute;

    #endregion

    #region Private fields

    Mesh _mesh;
    MaterialPropertyBlock _drawProps;

    ComputeBuffer _drawArgsBuffer;
    ComputeBuffer _positionBuffer;
    ComputeBuffer _normalBuffer;

    float _time;
    bool _timeControlled;

    #endregion

    #region Compute configurations

    const int kThreadCount = 128;
    int ThreadGroupCount { get { return _triangleCount / kThreadCount; } }
    int TriangleCount { get { return kThreadCount * ThreadGroupCount; } }

    #endregion

    #region MonoBehaviour functions

    void OnValidate()
    {
        _triangleCount = Mathf.Max(kThreadCount, _triangleCount);
        _triangleExtent = Mathf.Max(0, _triangleExtent);
        _noiseFrequency = Mathf.Max(0, _noiseFrequency);
    }

    void OnDisable()
    {
        // To avoid warnings, we release the compute buffers in OnDisable
        // rather than in OnDestroy.

        if (_drawArgsBuffer != null)
        {
            _drawArgsBuffer.Release();
            _drawArgsBuffer = null;
        }

        if (_positionBuffer != null)
        {
            _positionBuffer.Release();
            _positionBuffer = null;
        }

        if (_normalBuffer != null)
        {
            _normalBuffer.Release();
            _normalBuffer = null;
        }
    }

    void OnDestroy()
    {
        if (_mesh != null)
        {
            if (Application.isPlaying)
                Destroy(_mesh);
            else
                DestroyImmediate(_mesh);
        }
    }

    void LateUpdate()
    {
        // Lazy initialization of the single-triangle mesh.
        if (_mesh == null)
        {
            _mesh = new Mesh();
            _mesh.vertices = new Vector3 [3];
            _mesh.SetIndices(new [] {0, 1, 2}, MeshTopology.Triangles, 0);
            _mesh.UploadMeshData(true);
        }

        // Lazy initialization of the draw args compute buffer.
        if (_drawArgsBuffer == null)
        {
            _drawArgsBuffer = new ComputeBuffer(
                1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments
            );
        }

        // Allocate/Reallocate the compute buffers when it hasn't been
        // initialized or the triangle count was changed from the last frame.
        if (_positionBuffer == null || _positionBuffer.count != TriangleCount * 3)
        {
            if (_positionBuffer != null) _positionBuffer.Release();
            if (_normalBuffer != null) _normalBuffer.Release();

            _positionBuffer = new ComputeBuffer(TriangleCount * 3, 16);
            _normalBuffer = new ComputeBuffer(TriangleCount * 3, 16);

            _drawArgsBuffer.SetData(new uint[5] {3, (uint)TriangleCount, 0, 0, 0});
        }

        // Invoke the update compute kernel.
        var kernel = _compute.FindKernel("Update");

        _compute.SetFloat("Time", _time * _shuffleSpeed);
        _compute.SetFloat("Extent", _triangleExtent);
        _compute.SetFloat("NoiseAmplitude", _noiseAmplitude);
        _compute.SetFloat("NoiseFrequency", _noiseFrequency);
        _compute.SetVector("NoiseOffset", _noiseMotion * _time);
        _compute.SetFloat("RandomSeed", _randomSeed * 0.172749f);

        _compute.SetBuffer(kernel, "PositionBuffer", _positionBuffer);
        _compute.SetBuffer(kernel, "NormalBuffer", _normalBuffer);

        _compute.Dispatch(kernel, ThreadGroupCount, 1, 1);

        // Optional material properties.
        if (_drawProps == null) _drawProps = new MaterialPropertyBlock();

        _drawProps.SetMatrix("_LocalToWorld", transform.localToWorldMatrix);
        _drawProps.SetMatrix("_WorldToLocal", transform.worldToLocalMatrix);
        _drawProps.SetBuffer("_PositionBuffer", _positionBuffer);
        _drawProps.SetBuffer("_NormalBuffer", _normalBuffer);

        // Draw
        var bounds = new Bounds(transform.position, transform.lossyScale * 5);

        Graphics.DrawMeshInstancedIndirect(
            _mesh, 0, _material, bounds, _drawArgsBuffer, 0, _drawProps
        );

        // Advance the time.
        if (!_timeControlled)
        {
            if (Application.isPlaying)
                _time += Time.deltaTime;
            else
                _time = 1;
        }
    }

    #endregion

    #region ITimeControl functions

    public void OnControlTimeStart()
    {
        _timeControlled = true;
    }

    public void OnControlTimeStop()
    {
        _timeControlled = false;
    }

    public void SetTime(double time)
    {
        _time = (float)time;
    }

    #endregion
}

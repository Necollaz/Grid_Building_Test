using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Grid))]
public class GridOverlayRenderer : MonoBehaviour
{
    private const string SHADER_INTERNAL_COLORED = "Hidden/Internal-Colored";
    private const string PROP_SRC_BLEND = "_SrcBlend";
    private const string PROP_DST_BLEND = "_DstBlend";
    private const string PROP_CULL = "_Cull";
    private const string PROP_Z_WRITE = "_ZWrite";
    private const string PROP_Z_TEST = "_ZTest";

    [SerializeField] private Color _gridColor = new Color(1f, 1f, 1f, 0.2f);
    [SerializeField] private int _extraMarginCells = 2;
    [SerializeField] private int _extentXInCells = 50;
    [SerializeField] private int _extentYInCells = 50;
    [SerializeField] private bool _fitToCameraView = true;

    private Material _lineMaterial;
    private Grid _grid;

    private void Awake()
    {
        _grid = GetComponent<Grid>();
    }

    private void OnEnable()
    {
        if (_lineMaterial == null)
        {
            _lineMaterial = new Material(Shader.Find(SHADER_INTERNAL_COLORED));
            _lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            _lineMaterial.SetInt(PROP_SRC_BLEND, (int)BlendMode.SrcAlpha);
            _lineMaterial.SetInt(PROP_DST_BLEND, (int)BlendMode.OneMinusSrcAlpha);
            _lineMaterial.SetInt(PROP_CULL, (int)CullMode.Off);
            _lineMaterial.SetInt(PROP_Z_WRITE, 0);
            _lineMaterial.SetInt(PROP_Z_TEST, (int)CompareFunction.Always);
        }
    }

    private void OnRenderObject()
    {
        if (_grid == null || _lineMaterial == null)
            return;

        Camera currentCamera = Camera.current;
        
        if (currentCamera == null)
            return;

        int minCellX, maxCellX, minCellY, maxCellY;

        if (_fitToCameraView)
        {
            float cameraToGridZDistance = Mathf.Abs(currentCamera.transform.position.z - _grid.transform.position.z);

            Vector3 bottomLeftWorld = currentCamera.ViewportToWorldPoint(new Vector3(0f, 0f, cameraToGridZDistance));
            Vector3 topLeftWorld = currentCamera.ViewportToWorldPoint(new Vector3(0f, 1f, cameraToGridZDistance));
            Vector3 bottomRightWorld = currentCamera.ViewportToWorldPoint(new Vector3(1f, 0f, cameraToGridZDistance));
            Vector3 topRightWorld = currentCamera.ViewportToWorldPoint(new Vector3(1f, 1f, cameraToGridZDistance));

            Matrix4x4 worldToLocal = _grid.transform.worldToLocalMatrix;
            Vector3 bottomLeftLocal = worldToLocal.MultiplyPoint3x4(bottomLeftWorld);
            Vector3 topLeftLocal = worldToLocal.MultiplyPoint3x4(topLeftWorld);
            Vector3 bottomRightLocal = worldToLocal.MultiplyPoint3x4(bottomRightWorld);
            Vector3 topRightLocal = worldToLocal.MultiplyPoint3x4(topRightWorld);

            float minXLocal = Mathf.Min(bottomLeftLocal.x, topLeftLocal.x, bottomRightLocal.x, topRightLocal.x);
            float maxXLocal = Mathf.Max(bottomLeftLocal.x, topLeftLocal.x, bottomRightLocal.x, topRightLocal.x);
            float minYLocal = Mathf.Min(bottomLeftLocal.y, topLeftLocal.y, bottomRightLocal.y, topRightLocal.y);
            float maxYLocal = Mathf.Max(bottomLeftLocal.y, topLeftLocal.y, bottomRightLocal.y, topRightLocal.y);

            float cellSizeX = _grid.cellSize.x;
            float cellSizeY = _grid.cellSize.y;

            minCellX = Mathf.FloorToInt(minXLocal / cellSizeX) - _extraMarginCells;
            maxCellX = Mathf.CeilToInt(maxXLocal / cellSizeX) + _extraMarginCells;
            minCellY = Mathf.FloorToInt(minYLocal / cellSizeY) - _extraMarginCells;
            maxCellY = Mathf.CeilToInt(maxYLocal / cellSizeY) + _extraMarginCells;
        }
        else
        {
            minCellX = -_extentXInCells;
            maxCellX = _extentXInCells;
            minCellY = -_extentYInCells;
            maxCellY = _extentYInCells;
        }

        _lineMaterial.SetPass(0);
        
        GL.PushMatrix();
        GL.MultMatrix(_grid.transform.localToWorldMatrix);
        GL.Begin(GL.LINES);
        GL.Color(_gridColor);

        float cellSizeWorldX = _grid.cellSize.x;
        float cellSizeWorldY = _grid.cellSize.y;

        for (int x = minCellX; x <= maxCellX; x++)
        {
            float xPosition = x * cellSizeWorldX;
            GL.Vertex3(xPosition, minCellY * cellSizeWorldY, 0f);
            GL.Vertex3(xPosition, maxCellY * cellSizeWorldY, 0f);
        }

        for (int y = minCellY; y <= maxCellY; y++)
        {
            float yPosition = y * cellSizeWorldY;
            GL.Vertex3(minCellX * cellSizeWorldX, yPosition, 0f);
            GL.Vertex3(maxCellX * cellSizeWorldX, yPosition, 0f);
        }

        GL.End();
        GL.PopMatrix();
    }
}
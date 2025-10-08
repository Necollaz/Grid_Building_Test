using UnityEngine;

namespace ProjectGame.Scripts.Presentation.Views
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlacementCursorView : MonoBehaviour
    {
        private const int SORTING_ORDER_SPRITE = 999;
        
        [SerializeField] private float _cellWorldSize = 1f;
        
        private readonly Color valid = new Color(0f, 1f, 0f, 0.35f);
        private readonly Color invalid = new Color(1f, 0f, 0f, 0.35f);
        
        private SpriteRenderer _spriteRenderer;
        private Vector2Int _footprintCells = Vector2Int.one;
        private Vector2 _spriteWorldSize = Vector2.one;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            
            _spriteRenderer.sortingOrder = SORTING_ORDER_SPRITE;
            _spriteRenderer.drawMode = SpriteDrawMode.Simple;
            transform.localScale = Vector3.one;
        }

        public void TrySetRotation(int degrees) => transform.rotation = Quaternion.Euler(0, 0, degrees);
        
        public void TrySetValidity(bool isValid) => _spriteRenderer.color = isValid ? valid : invalid;
        
        public void TrySetVisible(bool visible) => gameObject.SetActive(visible);
        
        public void TrySetSprite(Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
            _spriteWorldSize = sprite ? sprite.bounds.size : Vector2.one;
            
            if (_spriteWorldSize.x <= 0)
                _spriteWorldSize.x = 1f;
            
            if (_spriteWorldSize.y <= 0)
                _spriteWorldSize.y = 1f;
            
            ApplyScale();
        }

        public void TrySetFootprintCells(Vector2Int footprintCells)
        {
            _footprintCells = new Vector2Int(Mathf.Max(1, footprintCells.x), Mathf.Max(1, footprintCells.y));
            
            ApplyScale();
        }
        
        public void TrySetGridOrigin(Vector2Int originGrid, System.Func<Vector2Int, Vector3> gridToWorld)
        {
            Vector3 leftBottomCenter = gridToWorld(originGrid);
            Vector3 half = new Vector3((_footprintCells.x - 1) * 0.5f, (_footprintCells.y - 1) * 0.5f, 0f);

            Vector3 spritePivotOffset = Vector3.zero;
            
            if (_spriteRenderer != null && _spriteRenderer.sprite != null)
                spritePivotOffset = _spriteRenderer.sprite.bounds.center;

            transform.position = leftBottomCenter + half - spritePivotOffset;
        }
        
        public void TrySetCellWorldSize(float size)
        {
            _cellWorldSize = Mathf.Max(0.001f, size);
            ApplyScale();
        }
        
        private void ApplyScale()
        {
            Vector2 target = new Vector2(_footprintCells.x * _cellWorldSize, _footprintCells.y * _cellWorldSize);
            transform.localScale = new Vector3(target.x / _spriteWorldSize.x, target.y / _spriteWorldSize.y, 1f);
        }
    }
}
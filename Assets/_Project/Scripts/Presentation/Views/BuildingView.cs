using System;
using UnityEngine;

namespace ProjectGame.Scripts.Presentation.Views
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class BuildingView : MonoBehaviour
    {
        private const float MIN_CELL_SIZE = 0.001f;
        private const int DEFAULT_SORTING_ORDER = 10;
        
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private BoxCollider2D _boxCollider2D;
        [SerializeField] private float _cellWorldSize = 1f;
        
        private Vector2Int _footprintCells = Vector2Int.one;
        
        public Vector2Int GridPosition { get; private set; }
        public string BuildingId { get; private set; }
        public int RotationDegrees { get; private set; }

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _boxCollider2D = GetComponent<BoxCollider2D>();
            
            _spriteRenderer.drawMode = SpriteDrawMode.Simple;
            transform.localScale = Vector3.one;
        }

         public void Initialize(string buildingId, Sprite sprite, Vector2Int gridOrigin, int rotationDegrees,
             Vector2Int footprintCells)
        {
            BuildingId = buildingId;
            GridPosition = gridOrigin;
            RotationDegrees = rotationDegrees;
            
            _footprintCells = new Vector2Int(Mathf.Max(1, footprintCells.x), Mathf.Max(1, footprintCells.y));

            _spriteRenderer.sprite = sprite;
            _spriteRenderer.sortingOrder = DEFAULT_SORTING_ORDER;
            
            _boxCollider2D.size = new Vector2(_footprintCells.x, _footprintCells.y);
            _boxCollider2D.offset = Vector2.zero;
            
            ApplyScaleToFootprint();
        }

        public void TrySetCellWorldSize(float size)
        {
            _cellWorldSize = Mathf.Max(MIN_CELL_SIZE, size);
            
            ApplyScaleToFootprint();
        }

        public void TryApplyTransform(Func<Vector2Int, Vector3> gridToWorld)
        {
            Vector3 leftBottomCenter = gridToWorld(GridPosition);
            Vector3 half = new Vector3((_footprintCells.x - 1) * 0.5f, (_footprintCells.y - 1) * 0.5f, 0f);
            
            Vector3 spritePivotOffset = Vector3.zero;
            
            if (_spriteRenderer != null && _spriteRenderer.sprite != null)
                spritePivotOffset = _spriteRenderer.sprite.bounds.center;
            
            transform.position = leftBottomCenter + half - spritePivotOffset;
            transform.rotation = Quaternion.Euler(0f, 0f, RotationDegrees);
        }
        
        private void ApplyScaleToFootprint()
        {
            if (_spriteRenderer == null || _spriteRenderer.sprite == null)
                return;
            
            Vector2 targetWorld = new Vector2(_footprintCells.x * _cellWorldSize, _footprintCells.y * _cellWorldSize);
            Vector2 spriteWorldSize = _spriteRenderer.sprite.bounds.size;
            
            if (spriteWorldSize.x <= 0f)
                spriteWorldSize.x = 1f;
            
            if (spriteWorldSize.y <= 0f)
                spriteWorldSize.y = 1f;
            
            Vector3 parentLossy = transform.parent != null ? transform.parent.lossyScale : Vector3.one;
            float localX = targetWorld.x / (spriteWorldSize.x * Mathf.Abs(parentLossy.x));
            float localY = targetWorld.y / (spriteWorldSize.y * Mathf.Abs(parentLossy.y));
            transform.localScale = new Vector3(localX, localY, 1f);
        }
    }
}
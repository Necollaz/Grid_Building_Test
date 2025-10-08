using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectGame.Scripts.Domain.Grid
{
    public class GridOccupancyMap
    {
        private readonly Dictionary<Vector2Int, string> cellToInstanceId = new Dictionary<Vector2Int, string>();

        public bool WasTryGetAnyInstanceAtCell(Vector2Int cell, out string instanceId) => cellToInstanceId.TryGetValue(cell, out instanceId);
        
        public bool IsAreaFree(Vector2Int leftBottom, Vector2Int size) =>
            IterateArea(leftBottom, size, cell => !cellToInstanceId.ContainsKey(cell));

        public bool IsAreaFreeVerbose(Vector2Int leftBottom, Vector2Int size) =>
            IterateArea(leftBottom, size, cell => !cellToInstanceId.ContainsKey(cell));

        public void OccupyArea(Vector2Int leftBottom, Vector2Int size, string instanceId) =>
            ForEachCell(leftBottom, size, cell => cellToInstanceId[cell] = instanceId);

        public void FreeArea(Vector2Int leftBottom, Vector2Int size) =>
            ForEachCell(leftBottom, size, cell => cellToInstanceId.Remove(cell));
        
        private void ForEachCell(Vector2Int leftBottom, Vector2Int size, Action<Vector2Int> action)
        {
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    action(new Vector2Int(leftBottom.x + x, leftBottom.y + y));
                }
            }
        }

        private bool IterateArea(Vector2Int leftBottom, Vector2Int size, Func<Vector2Int, bool> predicate)
        {
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    Vector2Int cell = new Vector2Int(leftBottom.x + x, leftBottom.y + y);
                    
                    if (!predicate(cell))
                        return false;
                }
            }
            
            return true;
        }
    }
}
using UnityEngine;

public static class DespawnHelper
{
    public static bool ShouldDespawn(Vector2 position)
    {
        return !ArenaBoundaryScript.Instance.IsInside(position * 0.8f);
    }
}

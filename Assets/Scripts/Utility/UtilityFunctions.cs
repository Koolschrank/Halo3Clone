using UnityEngine;

public static class UtilityFunctions
{
    public static void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    public static void CopyLayerFromParent(GameObject obj)
    {
        obj.layer = obj.transform.parent.gameObject.layer;
    }
}

using UnityEngine;
using System.Collections;

public static class EntitiesUtils
{
    public static IEnumerator FlashInvert(SpriteRenderer sr, float duration)
    {
        sr.material.SetFloat("_InvertColors", 1);
        yield return new WaitForSeconds(duration);
        sr.material.SetFloat("_InvertColors", 0);
    }
}

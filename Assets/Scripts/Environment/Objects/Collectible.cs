using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : EnvironmentObjectBase
{
    public override void Release()
    {
        StartCoroutine(AnimateToRelease());
    }

    public IEnumerator AnimateToRelease()
    {
        yield return CollectibleUI.Instance.AnimateToImage(this);
        base.Release();
    }
}

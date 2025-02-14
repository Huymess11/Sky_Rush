using Sirenix.OdinInspector;
using UnityEngine;

public class OutlineShare : SingletonMultiScene<OutlineShare>
{
    [ReadOnly] public Material outlineMaskMaterial;

    [ReadOnly] public Material enableOutlineFillMaterial;

    protected override void Awake()
    {
        base.Awake();

        outlineMaskMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineMask"));

        enableOutlineFillMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineFill"));
    }
}

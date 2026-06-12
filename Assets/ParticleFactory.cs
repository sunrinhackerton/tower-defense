using UnityEngine;

/// <summary>
/// Runtime particle effect factory.
/// Creates fire-and-forget ParticleSystem GameObjects that
/// destroy themselves when playback finishes.
/// </summary>
public static class ParticleFactory
{
    // -------------------------------------------------------
    // Muzzle Flash
    // -------------------------------------------------------
    /// <summary>
    /// Spawns a brief cone-shaped muzzle flash at <paramref name="spawnTransform"/>.
    /// The object auto-destroys after 0.3 s.
    /// </summary>
    public static void CreateMuzzleFlash(Transform spawnTransform)
    {
        GameObject go = new GameObject("FX_MuzzleFlash");
        go.transform.position = spawnTransform.position;
        go.transform.rotation = spawnTransform.rotation;

        ParticleSystem ps = go.AddComponent<ParticleSystem>();

        // -- Main module --
        var main = ps.main;
        main.duration = 0.15f;
        main.loop = false;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.08f, 0.18f);
        main.startSpeed    = new ParticleSystem.MinMaxCurve(2f, 5f);
        main.startSize     = new ParticleSystem.MinMaxCurve(0.08f, 0.25f);
        main.startColor    = new Color(1f, 0.82f, 0.15f, 1f);
        main.maxParticles  = 20;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.stopAction    = ParticleSystemStopAction.Destroy;

        // -- Emission --
        var emit = ps.emission;
        emit.rateOverTime = 0;
        emit.SetBursts(new[] { new ParticleSystem.Burst(0f, 15) });

        // -- Shape: cone --
        var shape = ps.shape;
        shape.enabled     = true;
        shape.shapeType   = ParticleSystemShapeType.Cone;
        shape.angle       = 25f;
        shape.radius      = 0.05f;

        // -- Color over lifetime: fade out --
        var col = ps.colorOverLifetime;
        col.enabled = true;
        Gradient g = new Gradient();
        g.SetKeys(
            new[] { new GradientColorKey(new Color(1f, 0.9f, 0.3f), 0f),
                    new GradientColorKey(new Color(1f, 0.4f, 0.0f), 1f) },
            new[] { new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(0f, 1f) }
        );
        col.color = new ParticleSystem.MinMaxGradient(g);

        // -- Renderer: sorting layer FX --
        var rend = go.GetComponent<ParticleSystemRenderer>();
        rend.sortingLayerName = "Default";
        rend.sortingOrder     = 10;
        rend.renderMode       = ParticleSystemRenderMode.Billboard;

        ps.Play();
    }

    // -------------------------------------------------------
    // Hit Impact
    // -------------------------------------------------------
    /// <summary>
    /// Spawns an explosion burst at <paramref name="worldPos"/>.
    /// The object auto-destroys after 0.8 s.
    /// </summary>
    public static void CreateHitImpact(Vector3 worldPos)
    {
        GameObject go = new GameObject("FX_HitImpact");
        go.transform.position = worldPos;

        ParticleSystem ps = go.AddComponent<ParticleSystem>();

        // -- Main module --
        var main = ps.main;
        main.duration    = 0.3f;
        main.loop        = false;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.2f, 0.5f);
        main.startSpeed  = new ParticleSystem.MinMaxCurve(3f, 8f);
        main.startSize   = new ParticleSystem.MinMaxCurve(0.06f, 0.18f);
        main.startColor  = new Color(1f, 0.35f, 0.1f, 1f);
        main.gravityModifier = 0.5f;
        main.maxParticles  = 30;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.stopAction  = ParticleSystemStopAction.Destroy;

        // -- Emission --
        var emit = ps.emission;
        emit.rateOverTime = 0;
        emit.SetBursts(new[] { new ParticleSystem.Burst(0f, 25) });

        // -- Shape: sphere --
        var shape = ps.shape;
        shape.enabled   = true;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius    = 0.1f;

        // -- Color over lifetime: orange -> yellow -> transparent --
        var col = ps.colorOverLifetime;
        col.enabled = true;
        Gradient g = new Gradient();
        g.SetKeys(
            new[] { new GradientColorKey(new Color(1f, 0.3f, 0.1f), 0f),
                    new GradientColorKey(new Color(1f, 0.85f, 0.0f), 0.5f),
                    new GradientColorKey(new Color(0.8f, 0.2f, 0.0f), 1f) },
            new[] { new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(0.6f, 0.5f),
                    new GradientAlphaKey(0f, 1f) }
        );
        col.color = new ParticleSystem.MinMaxGradient(g);

        // -- Size over lifetime: shrink --
        var sz = ps.sizeOverLifetime;
        sz.enabled = true;
        AnimationCurve curve = new AnimationCurve(
            new Keyframe(0f, 1f),
            new Keyframe(1f, 0f)
        );
        sz.size = new ParticleSystem.MinMaxCurve(1f, curve);

        // -- Renderer --
        var rend = go.GetComponent<ParticleSystemRenderer>();
        rend.sortingLayerName = "Default";
        rend.sortingOrder     = 10;
        rend.renderMode       = ParticleSystemRenderMode.Billboard;

        ps.Play();
    }
}

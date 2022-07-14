using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Assertions;

public class ShipMover : MonoBehaviour
{
    [SerializeField]
    private float rollMultiplier = 1f;
    [SerializeField]
    private AnimationCurve rollMultiplierThroughoutTurn = AnimationCurve.Constant(0f, 1f, 1f);

    [SerializeField]
    private List<ParticleSystem> engineParticles = null;
    private class EngineParticleData
    {
        public ParticleSystem system;
        public float originalEmissionRate;
        public float originalEmissionSpeed;
    }
    private List<EngineParticleData> engineParticleData = null;
    [SerializeField]
    private AnimationCurve engineRevToParticleRate = AnimationCurve.Constant(0f, 1f, 1f);
    [SerializeField]
    private AnimationCurve engineRevToParticleSpeed = AnimationCurve.Constant(0f, 1f, 1f);
    [SerializeField]
    private AnimationCurve engineRevToParticleConeAngle = AnimationCurve.Constant(0f, 1f, 0f);
    [SerializeField]
    private float engineRevReductionRate = 10f;

    private Vector2 turnStartPosition;
    private Vector2 turnEndPosition;
    public Vector2 TurnEndPosition => turnEndPosition;
    private Vector2 midPoint;
    public Vector2 MidPoint => midPoint;
    private float turnStartTime;
    private float currentEngineRev;

    private void Awake()
    {
    }

    private void Start()
    {
        turnStartPosition = new Vector2(transform.position.x, transform.position.z);
        midPoint = turnStartPosition;
        engineParticleData = engineParticles.Select(particles => new EngineParticleData{
            system = particles,
            originalEmissionRate = particles.emission.rateOverTimeMultiplier,
            originalEmissionSpeed = particles.main.startSpeedMultiplier,
        }).ToList();
    }

    public void UpdateFromPlanner(float turnLength)
    {
        var elapsedTimeThisTurn = Time.time - turnStartTime;
        var elapsedTimeProportionThisTurn = elapsedTimeThisTurn / turnLength;
        var position = QuadBezier(turnStartPosition, midPoint, turnEndPosition, elapsedTimeProportionThisTurn);
        var delta = 2f * elapsedTimeProportionThisTurn * (turnStartPosition - 2f * midPoint + turnEndPosition)
                  +                                 1f * (-2f * turnStartPosition + 2f * midPoint);

        var angle = Mathf.Atan2(delta.x, delta.y);
        var rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.up);

        var acceleration = 2f * (turnStartPosition - 2f * midPoint + turnEndPosition);
        var lateralAcceleration = delta.normalized.x * acceleration.y - delta.normalized.y * acceleration.x;
        var currentRollMultiplier = rollMultiplier * rollMultiplierThroughoutTurn.Evaluate(elapsedTimeProportionThisTurn);
        var roll = Quaternion.LookRotation(Vector3.forward, Vector3.up + Vector3.left * lateralAcceleration * currentRollMultiplier);

        var transverseAcceleration = Vector2.Dot(delta.normalized, acceleration);
        currentEngineRev = Mathf.Max(currentEngineRev - Time.deltaTime * engineRevReductionRate, transverseAcceleration);
        var particleRateMultiplier = engineRevToParticleRate.Evaluate(currentEngineRev);
        var particleSpeedMultiplier = engineRevToParticleSpeed.Evaluate(currentEngineRev);
        var particleConeAngle = engineRevToParticleConeAngle.Evaluate(currentEngineRev);
        foreach (var engineParticleSystem in engineParticleData)
        {
            var em = engineParticleSystem.system.emission;
            em.rateOverTimeMultiplier = particleRateMultiplier * engineParticleSystem.originalEmissionRate;
            var main = engineParticleSystem.system.main;
            main.startSpeedMultiplier = particleSpeedMultiplier * engineParticleSystem.originalEmissionSpeed;
            var shape = engineParticleSystem.system.shape;
            shape.angle = particleConeAngle;
        }

        transform.position = position.ToWorldPosition();
        transform.rotation = rotation * roll;
    }

    public void PrepareForNextTurn(float turnStartTime, Vector2 nextEndPosition)
    {
        var previousTurnEndVelocity = turnEndPosition - midPoint;
        this.turnStartTime = turnStartTime;
        turnStartPosition = turnEndPosition;
        turnEndPosition = nextEndPosition;

        midPoint = turnStartPosition + previousTurnEndVelocity; 
    }

    private Vector2 QuadBezier(Vector2 start, Vector2 mid, Vector2 end, float time)
    {
        return Vector2.Lerp(
            Vector2.Lerp(start, mid, time),
            Vector2.Lerp(mid, end, time),
            time
        );
    }
}

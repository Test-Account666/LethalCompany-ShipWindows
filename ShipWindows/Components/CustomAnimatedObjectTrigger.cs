using UnityEngine;

namespace ShipWindows.Components;

public class CustomAnimatedObjectTrigger : AnimatedObjectTrigger {
    private static readonly int _OnAnimatorHash = Animator.StringToHash("on");

    public void TriggerAnimation() {
        if (triggerByChance) {
            InitializeRandomSeed();
            if (triggerRandom.Next(100) >= (double) chancePercent) return;
        }

        if (isBool) {
            Debug.Log($"Triggering animated object trigger bool: setting to {!boolValue}");
            boolValue = !boolValue;
            if (triggerAnimator != null) triggerAnimator.SetBool(animationString, boolValue);
            if (triggerAnimatorB != null) triggerAnimatorB.SetBool(_OnAnimatorHash, boolValue);
        } else if (triggerAnimator != null) {
            triggerAnimator.SetTrigger(animationString);
        }

        SetParticleBasedOnBoolean();
        PlayAudio(boolValue);
        localPlayerTriggered = true;
        if (isBool) {
            onTriggerBool.Invoke(boolValue);
            UpdateAnimServerRpc(boolValue, playerWhoTriggered: (int) StartOfRound.Instance.localPlayerController.playerClientId);
        } else {
            UpdateAnimTriggerServerRpc();
        }
    }
}
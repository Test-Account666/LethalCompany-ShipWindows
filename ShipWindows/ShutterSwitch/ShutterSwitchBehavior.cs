using GameNetcodeStuff;
using UnityEngine;

namespace ShipWindows.ShutterSwitch;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(InteractTrigger))]
public class ShutterSwitchBehavior : MonoBehaviour {
    public static readonly int EnabledAnimatorHash = Animator.StringToHash("Enabled");
    public static ShutterSwitchBehavior Instance { get; private set; } = null!;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public Animator animator;
    public InteractTrigger interactTrigger;

    public AudioSource shutterSound;
    public AudioClip enableSound;
    public AudioClip disableSound;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private void Awake() => Instance = this;


    public void ToggleSwitch() {
        ToggleSwitch(!animator.GetBool(EnabledAnimatorHash));

        ShipWindows.networkManager?.ToggleShutters(animator.GetBool(EnabledAnimatorHash));
    }

    public void ToggleSwitch(PlayerControllerB playerControllerB) => ToggleSwitch();

    public void ToggleSwitch(bool enable, bool locked = false) {
        animator.SetBool(EnabledAnimatorHash, enable);

        interactTrigger.interactable = !locked;

        shutterSound.PlayOneShot(enable? enableSound : disableSound);
    }
}
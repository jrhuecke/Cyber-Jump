//Credit to KamielDev on StackExchange for providing this solution
//https://stackoverflow.com/questions/69741488/how-to-know-which-collider-triggered-the-call-to-ontriggerenter-in-unity-on-a-g

using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Delegates the call to OnTrigger for this object to another object.
/// </summary>
public class OnTrigger3DDelegator : MonoBehaviour
{
    private Collider caller;

    private void Awake()
    {
        caller = GetComponent<Collider>();
    }

    [Tooltip("Which function should be called when trigger was entered.")]
    public UnityEvent<OnTriggerDelegation> Enter;

    [Tooltip("Which function should be called when trigger was exited.")]
    public UnityEvent<OnTriggerDelegation> Exit;

    [Tooltip("Which function should be called when trigger stays.")]
    public UnityEvent<OnTriggerDelegation> Stay;

    void OnTriggerEnter(Collider other) => Enter.Invoke(new OnTriggerDelegation(caller, other));
    void OnTriggerExit(Collider other) => Exit.Invoke(new OnTriggerDelegation(caller, other));
    void OnTriggerStay(Collider other) => Stay.Invoke(new OnTriggerDelegation(caller, other));
}

/// <summary>
/// Stores which collider triggered this call and which collider belongs to the other object.
/// </summary>
public struct OnTriggerDelegation {

    /// <summary>
    /// Creates an OnTriggerDelegation struct.
    /// Stores which collider triggered this call and which collider belongs to the other object.
    /// </summary>
    /// <param name="caller">The trigger collider which triggered the call.</param>
    /// <param name="other">The collider which belongs to the other object.</param>
    public OnTriggerDelegation(Collider caller, Collider other)
    {
        Caller = caller;
        Other = other;
    }

    /// <summary>
    /// The trigger collider which triggered the call.
    /// </summary>
    public Collider Caller { get; private set; }

    /// <summary>
    /// The other collider.
    /// </summary>
    public Collider Other { get; private set; }
}
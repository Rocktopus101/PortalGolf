using UnityEngine;
using System.Collections.Generic;

public class PortalController : MonoBehaviour
{
    public enum PortalColor { Red, Blue, Green, Yellow }
    public PortalColor portalColor;

    private static Dictionary<PortalColor, List<PortalController>> portalPairs = new();

    private void Awake()
    {
        if (!portalPairs.ContainsKey(portalColor))
            portalPairs[portalColor] = new List<PortalController>();

        portalPairs[portalColor].Add(this);
    }

    private void OnDestroy()
    {
        if (portalPairs.ContainsKey(portalColor))
            portalPairs[portalColor].Remove(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Ball")) return;

        var rb = other.attachedRigidbody;
        if (rb == null) return;

        var helper = other.GetComponent<BallTeleportHelper>();
        if (helper == null || !helper.CanTeleport) return;

        var target = GetOtherPortalInSet();
        if (target == null || target == this) return;
        rb.position = target.transform.position;
        rb.linearVelocity = target.transform.right.normalized * rb.linearVelocity.magnitude;
        helper.TriggerCooldown(target);
    }

    private PortalController GetOtherPortalInSet()
    {
        foreach (var portal in portalPairs[portalColor])
        {
            if (portal != this)
                return portal;
        }
        return null;
    }
}

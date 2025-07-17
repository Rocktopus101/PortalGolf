using UnityEngine;

public class BallTeleportHelper : MonoBehaviour
{
    public PortalController JustTeleportedFrom;
    private float teleportCooldown = 0.2f; 
    private float timer = 0f;

    public bool CanTeleport => timer <= 0f;

    void Update()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
                JustTeleportedFrom = null;
        }
    }

    public void TriggerCooldown(PortalController portal)
    {
        JustTeleportedFrom = portal;
        timer = teleportCooldown;
    }
}

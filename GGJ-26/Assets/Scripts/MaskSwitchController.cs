using System.Collections;
using UnityEngine;

public class MaskSwitchController : InteractionController
{
    ModelController factionController;

    [SerializeField] private float attackCoolDown = 3f;

    bool isSwitchAvailable = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        factionController = GetComponent<ModelController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TrySwitchMask()
    {
        if (!isSwitchAvailable) return;
        isSwitchAvailable = false;

        var otherCharacter = GetClosestCharacter();

        if (otherCharacter == null) return;

        var currentFaction = factionController.GetFaction();

        var otherFactionController = otherCharacter.GetComponent<ModelController>();

        var otherFaction = otherFactionController.GetFaction();

        if(currentFaction == otherFaction) return;
        
        factionController.SetFaction(otherFaction);
        otherFactionController.SetFaction(currentFaction);

        StartCoroutine(SwitchCoolDownCoroutine());
    }

    IEnumerator SwitchCoolDownCoroutine()
    {
        yield return new WaitForSeconds(attackCoolDown);
        isSwitchAvailable = true;
    }


}

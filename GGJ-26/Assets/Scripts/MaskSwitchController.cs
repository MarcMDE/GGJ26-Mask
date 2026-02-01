using System.Collections;
using UnityEngine;

public class MaskSwitchController : InteractionController
{
    ModelController factionController;
    Animator animator;

    [SerializeField] private float switchCoolDown = 3f;
    [SerializeField] private GameObject successParticles;
    [SerializeField] private GameObject failureParticles;

    bool isSwitchAvailable = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        factionController = GetComponent<ModelController>();
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TrySwitchMask()
    {
        /*Debug.Log("switch input " + gameObject.name);

        if (!isSwitchAvailable) return;

        Debug.Log("ready");
        isSwitchAvailable = false;

        var otherCharacter = GetClosestCharacter();

        if (otherCharacter == null) return;

        var currentFaction = factionController.GetFaction();

        var otherFactionController = otherCharacter.GetComponent<ModelController>();

        var otherFaction = otherFactionController.GetFaction();
        Debug.Log("factions: "+ currentFaction + ", "+otherFaction);
        if (currentFaction == otherFaction) return;
        
        factionController.SetFaction(otherFaction);
        otherFactionController.SetFaction(currentFaction);

        StartCoroutine(SwitchCoolDownCoroutine());*/
        Debug.Log("switch input " + gameObject.name);
        if (isSwitchAvailable) StartCoroutine(SwitchMaskCR());
        else if (!animator.GetCurrentAnimatorStateInfo(0).IsName("SwitchMask"))
        {
            failureParticles.SetActive(false);
            failureParticles.SetActive(true);
        }
    }

    IEnumerator SwitchMaskCR()
    {
        Debug.Log("switch input " + gameObject.name);

        Debug.Log("ready");
        isSwitchAvailable = false;

        animator.SetBool("SwitchQueued", true);

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("SwitchMask") )
        {
            if (!animator.GetBool("SwitchQueued"))
            {
                isSwitchAvailable = true;
                yield break;
            }
            yield return null;
        }

        var otherCharacter = GetClosestCharacter();

        if (otherCharacter == null)
        {
            isSwitchAvailable = true;
            yield break;
        }
        var currentFaction = factionController.GetFaction();

        var otherFactionController = otherCharacter.GetComponent<ModelController>();

        var otherFaction = otherFactionController.GetFaction();
        Debug.Log("factions: " + currentFaction + ", " + otherFaction);
        if (currentFaction == otherFaction)
        {
            isSwitchAvailable = true;
            yield break;
        }
        factionController.SetFaction(otherFaction);
        otherFactionController.SetFaction(currentFaction);
        successParticles.SetActive(false);
        successParticles.SetActive(true);
        yield return new WaitForSeconds(switchCoolDown);
        isSwitchAvailable = true;
    }
}

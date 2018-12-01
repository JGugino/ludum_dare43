using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownController : MonoBehaviour {

    public static TownController instance;

    public GameObject townPersonPrefab, spawnedPersonParent;

    public int townMaxSize = 500, townMinSize, townCurrentSize = 0, totalPersonsKilled = 0, maxPersonsKilled = 20;

    public int currentTownHappiness = 0, maxTownHappiness = 200, minTownHappiness;

    public int happinessIncrement = 5;

    public float currentHappyDelay, maxHappyDelay = 100;

    private bool populatingTown = false;

    public List<Transform> townPersonsTransforms;
    public List<Transform> townHomesTransforms;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        currentHappyDelay = maxHappyDelay;

        townMinSize = townMaxSize / 2;

        minTownHappiness = maxTownHappiness / 2;

        currentTownHappiness = maxTownHappiness;

        UIController.instance.updatePopulationText();
        UIController.instance.updateHappinessText();
        UIController.instance.updateKilledText();

        populateTown();
    }

    private void Update()
    {
        if (!GameController.instance.isPaused)
        {
            increaseTownHappiness();
            decreaseTownHappiness();

            if (populatingTown)
            {
                populateTown();
            }

            if (townCurrentSize >= townMaxSize)
            {
                populatingTown = false;
            }
        }
    }

    public void increaseTownHappiness()
    {
        if (currentTownHappiness < maxTownHappiness || GameController.instance.personsAverageHappiness > 62)
        {
            if (townCurrentSize <= townMaxSize && townCurrentSize > townMinSize)
            {
                currentHappyDelay--;

                if (currentHappyDelay <= 0)
                {
                    if (currentTownHappiness < maxTownHappiness)
                    {
                        currentTownHappiness += happinessIncrement;

                        resetHappyDelay();

                        UIController.instance.updateHappinessText();

                        return;
                    }
                }
            }
        }

        
    }

    public void decreaseTownHappiness()
    {
        if (townCurrentSize < townMinSize || GameController.instance.personsAverageHappiness < 62 || totalPersonsKilled > maxPersonsKilled/2)
        {
            if (currentTownHappiness > minTownHappiness)
            {
                currentHappyDelay--;
                if (currentHappyDelay < 0)
                {
                    currentTownHappiness -= happinessIncrement;
                    resetHappyDelay();

                    UIController.instance.updateHappinessText();

                    return;
                }
            }
            else if (currentTownHappiness <= minTownHappiness || totalPersonsKilled > maxPersonsKilled)
            {
                currentHappyDelay--;
                if (currentHappyDelay < 0)
                {
                    currentTownHappiness -= happinessIncrement * 2;

                    resetHappyDelay();

                    UIController.instance.updateHappinessText();

                    return;
                }
            }
        }
    }

    private void resetHappyDelay()
    {
        currentHappyDelay = maxHappyDelay;
    }

    public void populateTown()
    {
        if (townCurrentSize < townMaxSize)
        {
            populatingTown = true;

            PersonSpawner.instance.spawnPerson();
        }
    }

    public void killTownPerson()
    {
        townPersonsTransforms.Remove(CameraManager.instance.getFocusTarget());

        TownPersonController tpc = CameraManager.instance.getFocusTarget().GetComponent<TownPersonController>();

        string gender = tpc.getPersonGender();

        if (gender == "Male")
        {
            PersonSpawner.instance.currentMales--;
            UIController.instance.updateSliders();
        }else if (gender == "Female")
        {
            PersonSpawner.instance.currentFemales--;
            UIController.instance.updateSliders();
        }

        totalPersonsKilled++;
        UIController.instance.updateKilledText();

        tpc.killPerson();

        UIController.instance.updatePopulationText();
    }
}

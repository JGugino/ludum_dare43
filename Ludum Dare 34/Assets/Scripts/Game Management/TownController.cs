using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownController : MonoBehaviour {

    public static TownController instance;

    public GameObject townPersonPrefab, spawnedPersonParent;

    public int townMaxSize = 500, townMinSize, townCurrentSize = 0, totalPersonsKilled = 0, maxPersonsKilled = 20;

    public int currentTownHappiness = 0, maxTownHappiness = 200, minTownHappiness;

    public int happinessIncreaseIncrement = 10, happinessDecreaseIncrement = 4;

    public float currentHappyDelay, maxHappyDelay = 100;

    public float currentKillDelay, maxKillDelay = 1000;

    private bool populatingTown = false;

    private bool decreasingHappiness = false, increasingHappiness = false;

    public List<Transform> townPersonsTransforms;
    public List<Transform> townHomesTransforms;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        GameObject[] homes = GameObject.FindGameObjectsWithTag("House");

        foreach (GameObject h in homes)
        {
            townHomesTransforms.Add(h.transform);
        }

        currentHappyDelay = maxHappyDelay;

        currentKillDelay = maxKillDelay;

        townMinSize = townMaxSize / 2;

        minTownHappiness = maxTownHappiness / 2;

        currentTownHappiness = maxTownHappiness / 2;

        UIController.instance.updatePopulationText();
        UIController.instance.updateHappinessText();
        UIController.instance.updateKilledText();

        populateTown();
    }

    private void Update()
    {
        if (!GameController.instance.isPaused)
        {
            if (!increasingHappiness)
            {
                decreaseChecks();
            }

            if (!decreasingHappiness)
            {
                increaseChecks();
            }

            decreasePersonsKilled();
        }

        if (populatingTown)
        {
            populateTown();
        }

        if (townCurrentSize >= townMaxSize)
        {
            populatingTown = false;
        }
    }

    public void decreaseChecks()
    {
        if (townCurrentSize > townMaxSize)
        {
            decreaseTownHappiness();
        }

        if (townCurrentSize < townMinSize)
        {
            decreaseTownHappiness();
        }

        if (GameController.instance.personsAverageHappiness < 60)
        {
            decreaseTownHappiness();
        }

        if (totalPersonsKilled > maxPersonsKilled / 2)
        {
            decreaseTownHappiness();
        }

        if (totalPersonsKilled > 4)
        {
            decreaseTownHappiness();
        }

        if (totalPersonsKilled > maxPersonsKilled)
        {
            decreaseTownHappiness(true);
        }
    }

    public void increaseChecks()
    {
        if (GameController.instance.personsAverageHappiness > 60)
        {
            increaseTownHappiness();
        }

        if (townCurrentSize <= townMaxSize && townCurrentSize > townMinSize)
        {
            increaseTownHappiness();
        }
    }

    public void increaseTownHappiness()
    {
        increasingHappiness = true;

        currentHappyDelay--;

        if (currentHappyDelay <= 0)
        {
            currentTownHappiness += happinessIncreaseIncrement;

            resetHappyDelay();

            UIController.instance.updateHappinessText();

            increasingHappiness = false;
            return;
        }

    }

    public void decreaseTownHappiness(bool doubleHit = false)
    {
        decreasingHappiness = true;

        if (!doubleHit)
        {
            currentHappyDelay--;
            if (currentHappyDelay < 0)
            {
                currentTownHappiness -= happinessDecreaseIncrement;
                resetHappyDelay();

                UIController.instance.updateHappinessText();

                decreasingHappiness = false;
                return;
            }
        }
        else if (doubleHit)
        {
            currentHappyDelay--;
            if (currentHappyDelay < 0)
            {
                currentTownHappiness -= happinessDecreaseIncrement * 2;
                resetHappyDelay();

                UIController.instance.updateHappinessText();

                decreasingHappiness = false;
                return;
            }
        }
    }

    public void decreasePersonsKilled()
    {
        if (totalPersonsKilled > 0)
        {
            currentKillDelay--;

            if (currentKillDelay <= 0)
            {
                totalPersonsKilled--;
                currentKillDelay = maxKillDelay;
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

    public void addTownsPerson(Transform newPerson)
    {
        townPersonsTransforms.Add(newPerson);

        UIController.instance.updatePopulationText();
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

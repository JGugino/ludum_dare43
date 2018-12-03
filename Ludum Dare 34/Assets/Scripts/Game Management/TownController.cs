using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownController : MonoBehaviour {

    public static TownController instance;

    public GameObject townPersonPrefab, spawnedPersonParent;

    public int townMaxSize = 500, townMinSize, townCurrentSize = 0, totalPersonsKilled = 0, maxPersonsKilled = 20;

    public int currentTownHappiness = 0, maxTownHappiness = 200, minTownHappiness;

    public int happinessIncreaseIncrement = 10, happinessDecreaseIncrement = 4;

    private float currentHappyDelay, maxHappyDelay = 100;

    private float currentKillDelay, maxKillDelay = 1000;

    private bool populatingTown = false;

    public int currentMurderDelay, maxMurderDelay = 800;

    public bool murder = false;

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

        townMinSize = townMaxSize - 10;

        minTownHappiness = maxTownHappiness / 2;

        currentTownHappiness = maxTownHappiness / 2;

        currentMurderDelay = maxMurderDelay;

        UIController.instance.updatePopulationText();
        UIController.instance.updateHappinessText();
        UIController.instance.updateKilledText();

        //populateTown();
    }

    private void Update()
    {
        if (!GameController.instance.isPaused)
        {
            if (murder)
            {
                currentMurderDelay--;

                if (currentMurderDelay <= 0)
                {
                    murder = false;

                    currentMurderDelay = maxMurderDelay;
                }
            }

            if (!GameController.instance.isPaused)
            {
                if (GameController.instance.townHappy && !murder)
                {
                    increaseChecks();
                }
                else if (!GameController.instance.townHappy)
                {
                    decreaseChecks();
                }

                if (murder || MetorSpawner.instance.startMetorShower)
                {
                    decreaseTownHappiness();
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
    }

    public void decreaseChecks()
    {
        if (townCurrentSize > townMaxSize)
        {
            decreaseTownHappiness();
        }

        if (totalPersonsKilled > maxPersonsKilled / 2)
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
        if (townCurrentSize <= townMaxSize && townCurrentSize > townMinSize)
        {
            increaseTownHappiness();
        }
    }

    public void increaseTownHappiness()
    {
        currentHappyDelay--;

        if (currentHappyDelay <= 0)
        {
            currentTownHappiness += happinessIncreaseIncrement;

            resetHappyDelay();

            UIController.instance.updateHappinessText();
            return;
        }

    }

    public void decreaseTownHappiness(bool doubleHit = false)
    {
        if (!doubleHit)
        {
            currentHappyDelay--;
            if (currentHappyDelay < 0)
            {
                currentTownHappiness -= happinessDecreaseIncrement;
                resetHappyDelay();

                UIController.instance.updateHappinessText();

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

    public void killPerson(Transform _person)
    {
        if (GameController.instance.happyPeople.Contains(_person))
        {
            GameController.instance.happyPeople.Remove(_person);
        }
        else if (GameController.instance.unHappyPeople.Contains(_person))
        {
            GameController.instance.unHappyPeople.Remove(_person);
        }

        TownController.instance.townPersonsTransforms.Remove(_person);

        TownPersonController tpc = _person.GetComponent<TownPersonController>();

        string gender = tpc.getPersonGender();

        if (gender == "Male")
        {
            PersonSpawner.instance.currentMales--;
            UIController.instance.updateSliders();
        }
        else if (gender == "Female")
        {
            PersonSpawner.instance.currentFemales--;
            UIController.instance.updateSliders();
        }

        TownController.instance.totalPersonsKilled++;
        UIController.instance.updateKilledText();

        tpc.killPerson();

        UIController.instance.updatePopulationText();
    }

    public void killTownPerson()
    {
        if (currentMurderDelay > 0)
        {
            murder = true;
            currentMurderDelay = maxMurderDelay;
        }

        if (GameController.instance.happyPeople.Contains(CameraManager.instance.getFocusTarget()))
        {
            GameController.instance.happyPeople.Remove(CameraManager.instance.getFocusTarget());
        }
        else if (GameController.instance.unHappyPeople.Contains(CameraManager.instance.getFocusTarget()))
        {
            GameController.instance.unHappyPeople.Remove(CameraManager.instance.getFocusTarget());
        }

        townPersonsTransforms.Remove(CameraManager.instance.getFocusTarget());

        TownPersonController tpc = CameraManager.instance.getFocusTarget().GetComponent<TownPersonController>();

        string gender = tpc.getPersonGender();

        if (gender == "Male")
        {
            PersonSpawner.instance.currentMales--;
            UIController.instance.updateSliders();
        }
        else if (gender == "Female")
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

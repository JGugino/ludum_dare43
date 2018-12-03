using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodController : MonoBehaviour
{

    public static GodController instance;

    public int currentBeelzAnger, maxBeelzAnger = 500, minBeelzAnger;

    public int currentAngerDelay, maxAngerDelay = 100;

    public int beelzAngerIncrement = 10, beelzAngerDecreaseIncrement = 35;

    public int currentDecreaseDelay, maxDecreaseDelay = 150;

    public int currentSacrifice, maxSacrifice = 10;

    public List<Transform> sacrifices;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        currentAngerDelay = maxAngerDelay;

        currentDecreaseDelay = maxDecreaseDelay;

        minBeelzAnger = maxBeelzAnger / 2;
    }

    private void Update()
    {
        if (!GameController.instance.isPaused)
        {
            if (currentBeelzAnger > 0)
            {
                currentDecreaseDelay--;

                if (currentDecreaseDelay < 0)
                {
                    currentBeelzAnger -= beelzAngerDecreaseIncrement;

                    UIController.instance.updateBeelzSlider();

                    currentDecreaseDelay = maxDecreaseDelay;
                }
            }

            if (currentBeelzAnger > minBeelzAnger)
            {
                if (!MetorSpawner.instance.startMetorShower)
                {
                    MetorSpawner.instance.useDefault = true;
                    MetorSpawner.instance.startMetorShower = true;
                }
            }
            else if (currentBeelzAnger < minBeelzAnger)
            {
                if (MetorSpawner.instance.startMetorShower)
                {
                    MetorSpawner.instance.startMetorShower = false;
                }
            }

            if (currentBeelzAnger > maxBeelzAnger)
            {
                if (!MetorSpawner.instance.startMetorShower)
                {
                    MetorSpawner.instance.useDefault = false;
                    MetorSpawner.instance.startMetorShower = true;
                }
            }

            if (!TownController.instance.murder)
            {
                if (TownController.instance.currentTownHappiness > TownController.instance.maxTownHappiness)
                {
                    increaseBeelzAnger();
                }

                if (TownController.instance.townCurrentSize > TownController.instance.townMaxSize)
                {
                    increaseBeelzAnger();
                }

                if (TownController.instance.totalPersonsKilled > TownController.instance.maxPersonsKilled / 2)
                {
                    increaseBeelzAnger();
                }
            }
        }
    }

    public void increaseBeelzAnger()
    {
        currentAngerDelay--;

        if (currentAngerDelay < 0)
        {
            if (currentBeelzAnger < maxBeelzAnger)
            {
                currentBeelzAnger += beelzAngerIncrement;

                UIController.instance.updateBeelzSlider();

                currentAngerDelay = maxAngerDelay;

                currentDecreaseDelay = maxDecreaseDelay;
            }
        }
    }

    public void decreaseBeelzAnger()
    {
        currentAngerDelay--;

        if (currentAngerDelay < 0)
        {
            if (currentBeelzAnger > 0)
            {
                currentBeelzAnger -= beelzAngerDecreaseIncrement;

                UIController.instance.updateBeelzSlider();

                currentAngerDelay = maxAngerDelay;
            }
        }
    }


    public void massSacrifice()
    {
        foreach (Transform t in TownController.instance.townPersonsTransforms)
        {
            if (!sacrifices.Contains(t))
            {
                if (currentSacrifice < maxSacrifice)
                {
                    sacrifices.Add(t);
                    currentSacrifice++;

                    if (currentSacrifice >= maxSacrifice)
                    {
                        foreach (Transform _t in sacrifices)
                        {
                            TownPersonController tpc = _t.GetComponent<TownPersonController>();

                            tpc.movePersonPosition(GameController.instance.firePitPosition.position);
                        }
                    }
                }
            }
        }
    }
}

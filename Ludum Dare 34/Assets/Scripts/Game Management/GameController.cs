using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public static GameController instance;

    public GameObject enviroment;

    public Transform firePitPosition;

    public int loadWaitTime = 2;

    public bool isPaused = true;

    private int currentCheckDelay = 0, maxCheckDelay = 450;

    public List<Transform> happyPeople, unHappyPeople;

    public bool townHappy = true;

    private void Awake()
    {
        instance = this;

        currentCheckDelay = maxCheckDelay;
    }

    private void Update()
    {
        if (!isPaused)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                quitGame();
            }

            if (Input.GetKeyDown(KeyCode.F1))
            {
                if (UIController.instance.gameUIScreen.activeSelf)
                {
                    UIController.instance.toggleGameUIScreen(false);
                }
                else if (!UIController.instance.gameUIScreen.activeSelf)
                {
                    UIController.instance.toggleGameUIScreen(true);
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (!isPaused)
        {
            currentCheckDelay--;
            if (currentCheckDelay <= 0)
            {
                checkPersonsHappiness();

                currentCheckDelay = maxCheckDelay;
            }

            if (Input.GetMouseButtonDown(0) && CameraManager.instance.getFocusTarget() == null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.CompareTag("Firepit"))
                    {
                        GodController.instance.massSacrifice();
                    }
                }
            }
        }
    }

    public void checkPersonsHappiness()
    {
        foreach (Transform t in TownController.instance.townPersonsTransforms)
        {
            if (t != null)
            {
                TownPersonController tcp = t.GetComponent<TownPersonController>();

                if (tcp.getPersonHappiness() > tcp.getMaxHappiness() / 2)
                {
                    if (!happyPeople.Contains(t))
                    {
                        if (!unHappyPeople.Contains(t))
                        {
                            happyPeople.Add(t);
                        }
                        else
                        {
                            unHappyPeople.Remove(t);
                            happyPeople.Add(t);
                        }
                    }
                }

                if (tcp.getPersonHappiness() < tcp.getMaxHappiness() / 2)
                {
                    if (!unHappyPeople.Contains(t))
                    {
                        if (!happyPeople.Contains(t))
                        {
                            unHappyPeople.Add(t);
                        }
                        else
                        {
                            happyPeople.Remove(t);
                            unHappyPeople.Add(t);
                        }
                    }
                }
            }
        }

        if (happyPeople.ToArray().Length > unHappyPeople.ToArray().Length)
        {
            townHappy = true;
        }else if (happyPeople.ToArray().Length < unHappyPeople.ToArray().Length)
        {
            townHappy = false;
        }

        //Debug.Log("Happy People: " + happyPeople.ToArray().Length + ", Unhappy People: " + unHappyPeople.ToArray().Length);

        return;
    }

    public void startGame()
    {
        UIController.instance.toggleLoadingScreen(true);
        UIController.instance.toggleMainMenu(false);
        UIController.instance.toggleGameUIScreen(true);

        isPaused = false;

        enviroment.SetActive(true);

        TownController.instance.populateTown();

        UIController.instance.maleSlider.maxValue = PersonSpawner.instance.maxMales;
        UIController.instance.femaleSlider.maxValue = PersonSpawner.instance.maxFemales;
        UIController.instance.beelzAngerSlider.maxValue = GodController.instance.maxBeelzAnger;

        UIController.instance.updateSliders();

        StartCoroutine(StartLoading());
    }

    public void quitGame()
    {
        isPaused = true;
        UIController.instance.toggleLoadingScreen(true);

        for (int i = 0; i < TownController.instance.townPersonsTransforms.ToArray().Length; i++)
        {
            TownPersonController[] objects = TownController.instance.spawnedPersonParent.GetComponentsInChildren<TownPersonController>();

            foreach (TownPersonController t in objects)
            {
                Destroy(t.gameObject);
            }

            TownController.instance.townPersonsTransforms.RemoveAt(i);
        }

        for (int h = 0; h < happyPeople.ToArray().Length; h++)
        {
            for (int u = 0; u < unHappyPeople.ToArray().Length; u++)
            {
                happyPeople.RemoveAt(h);
                unHappyPeople.RemoveAt(u);
            }
        }

        PersonSpawner.instance.currentMales = 0;
        PersonSpawner.instance.currentFemales = 0;

        TownController.instance.currentTownHappiness = TownController.instance.minTownHappiness;

        TownController.instance.totalPersonsKilled = 0;

        TownController.instance.townCurrentSize = 0;

        TownController.instance.murder = false;

        GodController.instance.currentBeelzAnger = 0;

        enviroment.SetActive(false);
        UIController.instance.toggleGameUIScreen(false);
        UIController.instance.toggleMainMenu(true);

        StartCoroutine(StartLoading());
    }

    public void exitGame()
    {
        Application.Quit();
    }

    private IEnumerator StartLoading()
    {
        yield return new WaitForSeconds(loadWaitTime);

        UIController.instance.toggleLoadingScreen(false);
    }
}

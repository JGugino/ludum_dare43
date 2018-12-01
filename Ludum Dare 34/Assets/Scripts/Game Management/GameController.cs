using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public static GameController instance;

    public int loadWaitTime = 2;

    public bool isPaused = false;

    private int currentCheckDelay = 0, maxCheckDelay = 600;

    public int personsAverageHappiness = 0, totalHappiness = 0;

    private void Awake()
    {
        instance = this;

        currentCheckDelay = maxCheckDelay;
    }

    private void LateUpdate()
    {
        currentCheckDelay--;
        if (currentCheckDelay <= 0)
        {
            checkPersonsHappiness();

            currentCheckDelay = maxCheckDelay;
        }
    }

    public void checkPersonsHappiness()
    {
        totalHappiness = 0;

        foreach (Transform t in TownController.instance.townPersonsTransforms)
        {
            TownPersonController tcp = t.GetComponent<TownPersonController>();

            totalHappiness += (int)tcp.getPersonHappiness();
        }

        personsAverageHappiness = totalHappiness / TownController.instance.townPersonsTransforms.ToArray().Length;

        Debug.Log("Average Happiness: " + personsAverageHappiness + ", Total Happiness: " + totalHappiness);

        return;
    }

    public void startGame()
    {
        UIController.instance.toggleLoadingScreen(true);
        UIController.instance.toggleMainMenu(false);
        UIController.instance.toggleGameUIScreen(true);

        StartCoroutine(StartLoading());
    }

    public void exitGame()
    {

    }

    private IEnumerator StartLoading()
    {
        yield return new WaitForSeconds(loadWaitTime);

        UIController.instance.toggleLoadingScreen(false);
    }
}

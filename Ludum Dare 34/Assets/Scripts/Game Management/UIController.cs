﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIController : MonoBehaviour {

    public static UIController instance;

    public GameObject mainScreen, loadingScreen, settingsScreen, controlsScreen, gameUIScreen;

    public GameObject actionBarObject, infoBoxObject;

    public Slider maleSlider, femaleSlider, beelzAngerSlider;

    public TextMeshProUGUI infoNameText, infoGenderText, infoAgeText, infoHappinessText, infoSpeedText, infoPartnerText, infoTaskText;

    public TextMeshProUGUI populationText, killedPersonsText, happinessText;

    private void Awake()
    {
        instance = this;
    }

    public void updateSliders()
    {
        maleSlider.value = PersonSpawner.instance.currentMales;
        femaleSlider.value = PersonSpawner.instance.currentFemales;
    }

    public void updateBeelzSlider()
    {
        beelzAngerSlider.value = GodController.instance.currentBeelzAnger;
    }

    #region Text Updaters

    public void updatePopulationText()
    {
        TownController.instance.townCurrentSize = TownController.instance.townPersonsTransforms.ToArray().Length;

        populationText.text = "Town Population: " + TownController.instance.townCurrentSize + "/" + TownController.instance.townMaxSize;
    }

    public void updateHappinessText()
    {
        happinessText.text = "Town Happiness: " + TownController.instance.currentTownHappiness + "/" + TownController.instance.maxTownHappiness;
    }

    public void updateKilledText()
    {
        killedPersonsText.text = "Person(s) Killed: " + TownController.instance.totalPersonsKilled + "/" + TownController.instance.maxPersonsKilled;
    }

    public void updateInfoBoxText(TownPersonController _tcp)
    {
        infoNameText.text = "Name: " + _tcp.getPersonName();
        infoGenderText.text = "Gender: " + _tcp.getPersonGender();
        infoAgeText.text = "Age: " + _tcp.getPersonAge();
        infoHappinessText.text = "Happiness: " + _tcp.getPersonHappiness();
        infoSpeedText.text = "Speed: " + _tcp.getPersonSpeed();
        if (_tcp.getPersonsPartner() != null)
        {
            infoPartnerText.text = "Partner: " + _tcp.getPersonsPartner().GetComponent<TownPersonController>().getPersonName();
        }
        else
        {
            infoPartnerText.text = "Partner: Single";
        }

        if (_tcp.tpa.getCurrentTask() == "none")
        {
            infoTaskText.text = "Task: Doing nothing.";
        }else if (_tcp.tpa.getCurrentTask() != "none")
        {
            infoTaskText.text = "Task: " + _tcp.tpa.getCurrentTask();
        }
        
    }

    public void updateHappinessInfoText(float _happiness)
    {
        if (infoBoxObject.activeSelf)
        {
            infoHappinessText.text = "Happiness: " + _happiness;
        }
    }

    public void updateTaskInfoText(string _task)
    {
        if (infoBoxObject.activeSelf)
        {
            infoTaskText.text = "Task: " + _task;
        }
    }

    #endregion

    #region Screen Toggles

    public void toggleMainMenu(bool _open)
    {
        mainScreen.SetActive(_open);
    }

    public void toggleLoadingScreen(bool _open)
    {
        loadingScreen.SetActive(_open);
    }

    public void toggleSettingsScreen(bool _open)
    {
        settingsScreen.SetActive(_open);
    }

    public void toggleControlsScreen(bool _open)
    {
        controlsScreen.SetActive(_open);
    }

    public void toggleGameUIScreen(bool _open)
    {
        gameUIScreen.SetActive(_open);
    }

    public void toggleActionBar(bool _open)
    {
        actionBarObject.SetActive(_open);
    }

    public void toggleInfoBox()
    {
        if (infoBoxObject.activeSelf)
        {
            infoBoxObject.SetActive(false);
        }
        else if (!infoBoxObject.activeSelf)
        {
            infoBoxObject.SetActive(true);
            if (CameraManager.instance.getFocusTarget() != null)
            {
                updateInfoBoxText(CameraManager.instance.getFocusTarget().GetComponent<TownPersonController>());
            }
        }
    }

    #endregion
}

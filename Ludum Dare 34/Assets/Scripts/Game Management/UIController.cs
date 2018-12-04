using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIController : MonoBehaviour {

    public static UIController instance;

    public GameObject mainScreen, loadingScreen, settingsScreen, controlsScreen, creditsScreen, gameUIScreen;

    public GameObject actionBarObject, infoBoxObject;

    public Slider beelzAngerSlider;

    public Slider populationSlider, happinessSlider, killedPersonSlider, happySlider, unhappySlider, infoHappinessSlider;

    public TextMeshProUGUI infoNameText, infoGenderText, infoAgeText, infoSpeedText, infoPartnerText, infoTaskText;

    private void Awake()
    {
        instance = this;
    }

    public void updateSliders() {
        happySlider.maxValue = TownController.instance.townMaxSize;
        unhappySlider.maxValue = TownController.instance.townMaxSize;

        happySlider.value = GameController.instance.happyPeople.ToArray().Length;
        unhappySlider.value = GameController.instance.unHappyPeople.ToArray().Length;
    }

    public void updateBeelzSlider()
    {
        beelzAngerSlider.value = GodController.instance.currentBeelzAnger;
    }

    #region Text Updaters

    public void updatePopulationSlider()
    {
        populationSlider.maxValue = TownController.instance.townMaxSize;

        TownController.instance.townCurrentSize = TownController.instance.townPersonsTransforms.ToArray().Length;

        populationSlider.value = TownController.instance.townCurrentSize;
    }

    public void updateHappinessSlider()
    {
        happinessSlider.maxValue = TownController.instance.maxTownHappiness;

        happinessSlider.value = TownController.instance.currentTownHappiness;
    }

    public void updateKilledSlider()
    {
        killedPersonSlider.maxValue = TownController.instance.maxPersonsKilled;

        killedPersonSlider.value = TownController.instance.totalPersonsKilled;
    }

    public void updateInfoBoxText(TownPersonController _tcp)
    {
        infoNameText.text = _tcp.getPersonName();
        infoGenderText.text = "Gender: " + _tcp.getPersonGender();
        infoAgeText.text = "Age: " + _tcp.getPersonAge();
        infoHappinessSlider.value = _tcp.getPersonHappiness();
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

    public void updateHappinessInfoSlider(float _happiness)
    {
        if (infoBoxObject.activeSelf)
        {
            infoHappinessSlider.value = _happiness;
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

    public void toggleCreditsScreen(bool _open)
    {
        creditsScreen.SetActive(_open);
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

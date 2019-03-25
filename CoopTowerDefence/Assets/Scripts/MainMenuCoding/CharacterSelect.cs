using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    public Saving saving;
    public Transform characterLayoutPanel;
    public GameObject characterpanel, newCharacterPanel;
    public int maxSaves;
    public GameObject characterCreatePanel;
    public Text classNameInput, classDescriptionInput,classAbilityInput, classStatsInput,classDamageTypeInput,classCrowdControlInput;
    int CurrentIndex = -1;
    public GameObject mainMenu;
    public Image yourCharacterPanel;

    public void Start()
    {
        saving.LoadData();
        SetSelectableCharacters();
    }

    public void SetSelectableCharacters()
    {
        foreach (Transform child in characterLayoutPanel)
            Destroy(child.gameObject);
        for (int i = 0; i < saving.data.classSaves.Count; i++)
        {
            GameObject panel = Instantiate(characterpanel, characterLayoutPanel);
            panel.GetComponent<CharacterPanelInfo>().SetInfo(saving.data.classSaves[i].classLevel, this, saving.classes[saving.data.classSaves[i].classIndex], i);
        }
        if (saving.data.classSaves.Count < maxSaves)
        {
            GameObject panel = Instantiate(newCharacterPanel, characterLayoutPanel);
            panel.GetComponent<CharacterPanelInfo>().SetInfo(this);
        }
    }

    public void OnMouseEnter(int index)
    {
        SetInfo(index);
    }

    public void SetInfo(int index)
    {
        if(index == -1)
        {
            classNameInput.text = "";
            classDescriptionInput.text = "";
            classAbilityInput.text = "";
            classStatsInput.text =
                "Damage: " + "\n" +
                "Health: " + "\n" +
                "Mana: " + "\n" +
                "MovementSpeed: " + "\n" +
                "JumpHeight: ";
            classDamageTypeInput.text = "";
            classCrowdControlInput.text = "";
        }
        else
        {
            classNameInput.text = saving.classes[index].className;
            classDescriptionInput.text = saving.classes[index].classInformation;
            classAbilityInput.text = saving.classes[index].ability;
            classStatsInput.text =
                "Damage: " + saving.classes[index].baseStats.damage.ToString() + "\n" +
                "Health: " + saving.classes[index].baseStats.health.ToString() + "\n" +
                "Mana: " + saving.classes[index].baseStats.mana.ToString() + "\n" +
                "MovementSpeed: " + saving.classes[index].baseStats.movementSpeed.ToString() + "\n" +
                "JumpHeight: " + saving.classes[index].baseStats.movementSpeed.ToString();
            switch (saving.classes[index].damageStyle)
            {
                case ClassScriptableObjectBaseStats.DamageStyle.Melee:
                    classDamageTypeInput.text = "Melee";
                    break;
                case ClassScriptableObjectBaseStats.DamageStyle.Ranged:
                    classDamageTypeInput.text = "Ranged";
                    break;
                case ClassScriptableObjectBaseStats.DamageStyle.Combined:
                    classDamageTypeInput.text = "Combined";
                    break;
            }
            switch (saving.classes[index].crowdControl)
            {
                case ClassScriptableObjectBaseStats.CrowdControl.Bad:
                    classCrowdControlInput.text = "Bad";
                    break;
                case ClassScriptableObjectBaseStats.CrowdControl.Decent:
                    classCrowdControlInput.text = "Decent";
                    break;
                case ClassScriptableObjectBaseStats.CrowdControl.Good:
                    classCrowdControlInput.text = "Good";
                    break;
            }
        }
    }

    public void OnHoverExit()
    {
        SetInfo(CurrentIndex);
    }

    public void OnButtonPressed(int index)
    {
        CurrentIndex = index;
        SetInfo(index);
    }

    public void NewCharacter()
    {
        CurrentIndex = -1;
        characterCreatePanel.SetActive(true);
    }

    public void AddNewCharacter()
    {
        if(CurrentIndex != -1)
        {
            saving.data.classSaves.Add(new ClassSaveData() { classIndex = CurrentIndex, classLevel = 1 });
            characterCreatePanel.SetActive(false);
            SetSelectableCharacters();
        }
    }

    public void PickedCharacter(int index)
    {
        saving.currentlyPlayingWith = index;
        mainMenu.SetActive(true);
        yourCharacterPanel.sprite = saving.classes[saving.data.classSaves[index].classIndex].image;
    }
}

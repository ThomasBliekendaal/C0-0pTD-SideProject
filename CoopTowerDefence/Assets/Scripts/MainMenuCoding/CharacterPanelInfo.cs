using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPanelInfo : MonoBehaviour
{
    public CharacterSelect select;
    public int index;
    public Text nameInput, levelInput;
    public bool newCharacter;

    public void SetInfo(int level, CharacterSelect _select, ClassScriptableObjectBaseStats characterClass, int _index)
    {
        levelInput.text = "lvl: " + level.ToString();
        select = _select;
        nameInput.text = characterClass.className;
        index = _index;
        newCharacter = false;
    }

    public void SetInfo(CharacterSelect _select)
    {
        select = _select;
        newCharacter = true;
    }

    public void OnButtonPress()
    {
        if (newCharacter)
            select.NewCharacter();
        else
            select.PickedCharacter(index);
    }
}

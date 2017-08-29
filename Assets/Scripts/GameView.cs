using System;
using UnityEngine.UI;

public class GameView : Singleton<GameView> {


    public void SetText(Text textElement, string text) {
        textElement.text = text;
    }

    public void SetText(string elementName, string text) {
        Globals.Instance.UnityObjects[elementName].GetComponent<Text>().text = text;
    }

    public void DisableButton(Button button) {
        button.interactable = false;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameView : Singleton<GameView> {


    public void SetText(Text textElement, string text) {
        textElement.text = text;
    }

    public void SetText(string elementName, string text) {
        Globals.Instance.UnityObjects[elementName].GetComponent<Text>().text = text;
    }
}
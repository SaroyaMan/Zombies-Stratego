using UnityEngine.UI;

public static class GameView {


    public static void SetText(Text textElement, string text) {
        textElement.text = text;
    }

    public static void SetText(string elementName, string text) {
        Globals.Instance.UnityObjects[elementName].GetComponent<Text>().text = text;
    }

    public static void DisableButton(Button button) {
        button.interactable = false;
    }
}
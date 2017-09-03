using UnityEngine;
using UnityEngine.UI;

public static class GameView {

    private static Color defaultColor = new Color(0, 0, 0, 0);
    private static Color transparentColor = new Color(0, 0, 0, 0.5f);

    public static Color transitionColor = new Color(0.980f, 0.922f, 0.843f);

    public static void SetText(Text textElement, string text) {
        textElement.text = text;
    }

    public static void SetText(string elementName, string text) {
        Globals.Instance.UnityObjects[elementName].GetComponent<Text>().text = text;
    }

    public static void DisableButton(Button button) {
        button.interactable = false;
    }

    public static void DisableButton(string elementName) {
        Globals.Instance.UnityObjects[elementName].GetComponent<Button>().interactable = false;
    }

    public static void EnableButton(string elementName) {
        Globals.Instance.UnityObjects[elementName].GetComponent<Button>().interactable = true;
    }

    public static void SetImage(string elementName, Sprite sprite) {
        Globals.Instance.UnityObjects[elementName].GetComponent<Image>().sprite = sprite;
    }

    public static void MakeScreenDark() {
        Globals.Instance.UnityObjects["Canvas"].GetComponent<Image>().color = transparentColor;
    }

    public static void MakeScreenNormal() {
        Globals.Instance.UnityObjects["Canvas"].GetComponent<Image>().color = defaultColor;
    }
}
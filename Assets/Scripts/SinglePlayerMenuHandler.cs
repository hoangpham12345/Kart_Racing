using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SinglePlayerMenuHandler : MonoBehaviour
{
    public List<Sprite> mapThumbnails;
    public List<Sprite> kartThumbnails;
    public GameObject mapDisplay;
    public GameObject kartDisplay;
    public GameObject mapText;
    public GameObject kartText;
    public int mapIndex;
    public int kartIndex;
    public GameOptions gameOption;

    public void Start()
    {
        UpdateMap();
        UpdateKart();
    }

    public void NextMap()
    {
        if (mapIndex < mapThumbnails.Count - 1)
            mapIndex++;
        UpdateMap();
    }

    public void PreviousMap()
    {
        if (mapIndex > 0)
            mapIndex--;
        UpdateMap();
    }

    private void UpdateMap()
    {
        Image img = mapDisplay.GetComponent<Image>();
        img.sprite = mapThumbnails[mapIndex];
        mapText.GetComponent<Text>().text = "Map " + (mapIndex);
        gameOption.map = mapIndex;
    }


    public void NextKart()
    {
        if (kartIndex < kartThumbnails.Count - 1)
            kartIndex++;
        UpdateKart();
    }

    public void PreviousKart()
    {
        if (kartIndex > 0)
            kartIndex--;
        UpdateKart();
    }

    private void UpdateKart()
    {
        Image img = kartDisplay.GetComponent<Image>();
        img.sprite = kartThumbnails[kartIndex];
        kartText.GetComponent<Text>().text = "Kart " + (kartIndex + 1);
        gameOption.kart = kartIndex;
    }
}

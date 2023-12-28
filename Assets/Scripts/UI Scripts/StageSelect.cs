using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSelect : MonoBehaviour
{
    [SerializeField]
    private GameObject gridElementPrefab, cursor;
    [SerializeField]
    private Image mapNameImage, parallax1, parallax2, parallax3;
    [SerializeField]
    private SpriteRenderer mapSprite, foreground;

    private List<GameObject> mapIcons = new();

    private MapData[] maps;

    public Sprite randomIcon, randomName, randomMap, randomParallax;

    private int mapId;

    private InputManager inputs = new();

    private void Start()
    {
        if (GameManager.instance == null) Destroy(gameObject);

        maps = GameManager.instance.maps;
        mapId = GameManager.instance.mapID;
        for (int i = 0; i < maps.Length + 1; i++)
        {
            GameObject mapIcon = Instantiate(gridElementPrefab, transform);
            if (i == maps.Length)
                mapIcon.GetComponent<Image>().sprite = randomIcon;
            else
                mapIcon.GetComponent<Image>().sprite = maps[i].mapIcon;

            mapIcons.Add(mapIcon);
        }

        Render();
    }

    private void Update()
    {
        Render();
        
    }

    private void Render()
    {
        if (Time.timeSinceLevelLoad < 0.05f) return;
        inputs.GetMenuInputs(-1);

        if (inputs.left)
            mapId = (int)Mathf.Repeat(mapId - 1, maps.Length + 1);

        if (inputs.right)
            mapId = (int)Mathf.Repeat(mapId + 1, maps.Length + 1);

        GameManager.instance.mapID = mapId;
        cursor.transform.position = mapIcons[mapId].transform.position;

        if (Time.timeSinceLevelLoad > 0.1f)
        {
            if (inputs.attack || Input.GetKeyDown(KeyCode.Space))
                SceneManager.LoadScene("Character Select");
            else if (inputs.shield)
                SceneManager.LoadScene("Options");
        }

        if (mapId == maps.Length)
        {
            mapSprite.sprite = randomMap;
            mapNameImage.sprite = randomName;
            parallax1.sprite = randomParallax;
            parallax2.gameObject.SetActive(false);
            parallax3.gameObject.SetActive(false);
            foreground.gameObject.SetActive(false);
            mapSprite.gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
        }
        else
        {
            mapSprite.sprite = maps[mapId].sprite;
            mapNameImage.sprite = maps[mapId].mapNameSprite;

            parallax1.gameObject.SetActive(maps[mapId].parallax1.visible);
            parallax1.sprite = maps[mapId].parallax1.sprite;
            parallax1.rectTransform.localPosition = maps[mapId].parallax1.offsetOnStageSelect;

            parallax2.gameObject.SetActive(maps[mapId].parallax2.visible);
            parallax2.sprite = maps[mapId].parallax2.sprite;
            parallax2.rectTransform.localPosition = maps[mapId].parallax2.offsetOnStageSelect;

            parallax3.gameObject.SetActive(maps[mapId].parallax3.visible);
            parallax3.sprite = maps[mapId].parallax3.sprite;
            parallax3.rectTransform.localPosition = maps[mapId].parallax3.offsetOnStageSelect;

            mapSprite.gameObject.transform.localPosition = maps[mapId].stageSelectOffset;

            foreground.gameObject.SetActive(maps[mapId].foreground.visible);
            foreground.sprite = maps[mapId].foreground.sprite;
            foreground.gameObject.transform.localPosition = maps[mapId].foreground.offset;
        }
    }
}

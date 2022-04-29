using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSceneUiManager : MonoBehaviour
{

    public GameObject buttoPanelPrefab,scrollViewContent,loadingPanel;
    public Sprite[] buttonSprite;
    public Sprite startingSprite;

    private void Start()
    {
        LocalPlayer.LoadGame();
        Object[] maps = Resources.LoadAll("Levels");
        Debug.Log("leng " + maps.Length);
        Resources.UnloadUnusedAssets();

        int buttonTextIndex = maps.Length;

        //Instatiate button panel depent upon then number of txt files present in resource/levels folder
        for (int i = 0; i < maps.Length/buttoPanelPrefab.transform.childCount; i++)
        {
            var buttonpanel = Instantiate(buttoPanelPrefab);
            buttonpanel.transform.parent = scrollViewContent.transform;
            buttonpanel.name = "panel" + i;
            //to assign properties to the button in button panel
            foreach (Transform button in buttonpanel.transform)
            {

                if (buttonTextIndex <= LocalPlayer.MaxLevel)
                {
                    Debug.Log("index " + (int)(buttonTextIndex));

                    int spriteIndex = (int)Random.Range(1f, (float)(buttonSprite.Length - 1));
                    button.GetComponent<Button>().image.sprite = buttonSprite[spriteIndex];
                    button.GetComponent<Button>().onClick.AddListener(() => {
                        LocalPlayer.CurrentLevel =  int.Parse(button.GetComponentInChildren<Text>().text);
                        Debug.Log("current level from enu " + LocalPlayer.CurrentLevel);
                        LocalPlayer.SaveGame();
                        SceneManager.LoadScene(1);
                    });
                }
                else
                {
                    button.GetComponent<Button>().image.sprite = buttonSprite[0];
                }
                button.GetComponentInChildren<Text>().text = buttonTextIndex.ToString();
                buttonTextIndex--;
             }

            //if(i == (maps.Length / buttoPanelPrefab.transform.childCount - 1))
            //{
            //    buttoPanelPrefab.GetComponent<Image>().sprite = startingSprite;
            //}
        }

     
        if(!GameManager.isComingBackFromGameScene)
            StartCoroutine(loading());
        else
            loadingPanel.SetActive(false);
    //#if UNITY_ANDROID
    //    AdsScripta.instance.ShowBottomBannerAds();
    //    AdsScripta.instance.ShowTopBannerAds();
    //#endif
    }


    IEnumerator loading()
    {
        yield return new WaitForSeconds(3);
        loadingPanel.SetActive(false);
    }
}

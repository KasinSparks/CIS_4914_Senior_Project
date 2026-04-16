using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonSFX : MonoBehaviour
{
    public AudioSource button_sfx_source;

    public void PlayButtonSFXThenLoadScene(string scene_name)
    {
        StartCoroutine(this.PlaySoundThenLoadScene(scene_name));
    }

    private IEnumerator PlaySoundThenLoadScene(string scene_name)
    {
        this.button_sfx_source.Play();
        float clip_legnth = this.button_sfx_source.clip.length;

        yield return new WaitForSeconds(clip_legnth);

        // Switch scenes
        LoadGameScene.LoadScene(scene_name);
    }

    public void PlayButtonSFXThenLoadSceneDirect(string scene_name)
    {
        StartCoroutine(this.PlaySoundThenLoadSceneDirect(scene_name));
    }

    private IEnumerator PlaySoundThenLoadSceneDirect(string scene_name)
    {
        this.button_sfx_source.Play();
        float clip_legnth = this.button_sfx_source.clip.length;

        yield return new WaitForSeconds(clip_legnth);

        // Switch scenes
        SceneManager.LoadScene(scene_name);
    }

    private IEnumerator PlaySFXThenSetToActive(GameObject obj, bool active)
    {
        this.button_sfx_source.Play();
        yield return new WaitForSeconds(this.button_sfx_source.clip.length);

        obj.SetActive(active);
    }
    
    public void PlayButtonClickSFXThenSetToNotActive(GameObject obj)
    {
        StartCoroutine(this.PlaySFXThenSetToActive(obj, false));
    }

    public void PlayButtonClickSFXThenSetToActive(GameObject obj)
    {
        StartCoroutine(this.PlaySFXThenSetToActive(obj, true));
    }
}
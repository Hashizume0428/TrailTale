using UnityEngine;

using UnityEngine.SceneManagement;

using UnityEngine.UI;
using System.Collections; // �R���[�`�����g�p���邽�߂ɕK�v�ł�

public class GameManager : MonoBehaviour

{

    [SerializeField] GameObject configPanel;

    // �V�[���J�ڂ܂ł̃��O���ԁi�b�j
    [Header("Scene Transition Settings")]
    [SerializeField] private float sceneTransitionDelay = 0.8f;

    // --- �V���O���g���p�^�[�� (����GameManager��DontDestroyOnLoad�Ȃ�) ---
    // ��������GameManager��DontDestroyOnLoad�ŉi��������Ă���Ȃ�A
    // �ȉ��̃V���O���g��������Awake�ɒǉ����Ă��������B
    // private static GameManager instance = null;
    // public static GameManager Instance { get { return instance; } }
    // void Awake() {
    //     if (instance != null && instance != this) { Destroy(this.gameObject); return; }
    //     instance = this;
    //     DontDestroyOnLoad(this.gameObject);
    // }

    // --- �����̃��\�b�h�i�C���j ---

    public void StartButton() // ���\�b�h����StartBotton����StartButton�ɏC�����܂����i�����j
    {
        Debug.Log("Start Button clicked.");
        StartCoroutine(LoadSceneWithDelay("home")); // �R���[�`�����J�n
    }

    public void ShowConfigPanel() // ���\�b�h����ShowconfigPanel����ShowConfigPanel�ɏC�����܂����i�����j
    {
        if (configPanel != null)
        {
            configPanel.SetActive(true);
            Debug.Log("Config Panel shown.");
        }
        else
        {
            Debug.LogWarning("Config Panel is not assigned in GameManager.");
        }
    }

    public void HideConfigPanel() // ���\�b�h����HideConfigPanel����HideConfigPanel�ɏC�����܂����i�����j
    {
        if (configPanel != null)
        {
            configPanel.SetActive(false);
            Debug.Log("Config Panel hidden.");
        }
        else
        {
            Debug.LogWarning("Config Panel is not assigned in GameManager.");
        }
    }

    public void ItemPageButton() // ���\�b�h����ItemPageBotton����ItemPageButton�ɏC�����܂����i�����j
    {
        Debug.Log("ItemPageButton clicked.");
        StartCoroutine(LoadSceneWithDelay("Item")); // �R���[�`�����J�n
    }

    public void StatusPageButton() // ���\�b�h����StatusPageBotton����StatusPageButton�ɏC�����܂����i�����j
    {
        Debug.Log("StatusPageButton clicked.");
        StartCoroutine(LoadSceneWithDelay("Status")); // �R���[�`�����J�n
    }

    public void ScenarioPageButton() // ���\�b�h����ScenarioPageBotton����ScenarioPageButton�ɏC�����܂����i�����j
    {
        Debug.Log("ScenarioPageButton clicked.");
        StartCoroutine(LoadSceneWithDelay("ScenarioTest")); // �R���[�`�����J�n
    }

    public void SettingButton() // ���\�b�h����SettingBotton����SettingButton�ɏC�����܂����i�����j
    {
        Debug.Log("SettingButton clicked.");
        StartCoroutine(LoadSceneWithDelay("Setting")); // �R���[�`�����J�n
    }

    public void BacknumberButton() // ���\�b�h����BackbumerBotton����BacknumberButton�ɏC�����܂����i�����j
    {
        Debug.Log("BacknumberButton clicked.");
        StartCoroutine(LoadSceneWithDelay("Backnumber")); // �R���[�`�����J�n
    }

    public void TitleButton() // ���\�b�h����TitleBotton����TitleButton�ɏC�����܂����i�����j
    {
        Debug.Log("TitleButton clicked.");
        StartCoroutine(LoadSceneWithDelay("Title")); // �R���[�`�����J�n
    }

    public void HomeButton() // ���\�b�h����HomeBotton����HomeButton�ɏC�����܂����i�����j
    {
        Debug.Log("HomeButton clicked.");
        StartCoroutine(LoadSceneWithDelay("home")); // �R���[�`�����J�n
    }

    // --- �V�K�ǉ����\�b�h ---

    /// <summary>
    /// �w�肳�ꂽ���ԑҋ@������A�V�[�������[�h����R���[�`��
    /// </summary>
    /// <param name="sceneName">���[�h����V�[���̖��O</param>
    private IEnumerator LoadSceneWithDelay(string sceneName)
    {
        // �����ŁA�N���b�N���ꂽ�{�^���𖳌�������Ȃǂ�UI�t�B�[�h�o�b�N������Ɨǂ��ł��傤
        // ��: EventSystem.current.currentSelectedGameObject.GetComponent<Button>().interactable = false;

        Debug.Log($"Waiting for {sceneTransitionDelay} seconds before loading scene: {sceneName}");
        yield return new WaitForSeconds(sceneTransitionDelay); // �w�肳�ꂽ�b���ҋ@

        Debug.Log($"Loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName); // �V�[�������[�h
    }
}
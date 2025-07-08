using UnityEngine;
using TMPro; // TextMeshProUGUI���g���ꍇ�͕K�{�ł��B
using System.Collections; // �R���[�`�����g���ꍇ�͕K�{�ł��B
using System.Collections.Generic; // Queue���g���ꍇ�͕K�{�ł��B

// MonoBehaviour���p�����邱�ƂŃI�u�W�F�N�g�ɃR���|�[�l���g�Ƃ���
// �A�^�b�`���邱�Ƃ��ł���悤�ɂȂ�
public class TextManager : MonoBehaviour
{
    // SerializeField�Ə�����private�ȃp�����[�^�[�ł�
    // �C���X�y�N�^�[��Œl��ύX�ł���
    [SerializeField]
    private TextMeshProUGUI mainText; // ���C���̃e�L�X�g�\���p
    // private TextMeshProUGUI nameText; // ���O�̕\���p �� �N���b���Ă��邩�̋@�\�͍폜�ς݂̂��߁A�����ł͍폜���܂��񂪁A����nameText���g���Ă��Ȃ��ꍇ�͍폜���Ă��������B

    [SerializeField]
    private TextMeshProUGUI speedDisplayText; // ���݂̑��x�\���p

    [Header("Text Settings")] // �C���X�y�N�^�[�ł̕\���𕪂���₷������
    // captionSpeed��PlayerPrefs���烍�[�h���邽�߁A�����ł�[SerializeField]��t���܂���B
    private float captionSpeed;

    // �V�������x�i�K���`����l�B�C���X�y�N�^�[���璲���ł���悤��SerializeField���t���܂��B
    [SerializeField, Header("Caption Speeds (seconds per char)")]
    private float fastSpeed = 0.02f;     // �������x�i�b/�����j
    [SerializeField]
    private float captionSpeed = 0.05f; // 1�����\�����Ƃ̑ҋ@���� (�b)

    // �e�L�X�g�̃y�[�W��؂蕶��
    private const char SEPARATE_PAGE = '&';
    // PlayerPrefs�Ŏg�p����L�[���i�萔�j
    private const string CAPTION_SPEED_KEY = "CaptionSpeed"; // �������L�[��

    // �e�X�g�p�̃e�L�X�g�B���O�̃t�H�[�}�b�g���s�v�ɂȂ邽�ߏC�����܂����B
    [TextArea(3, 10)] // �C���X�y�N�^�[�ŕ����s���͂ł���悤�ɂ���
    [SerializeField]
    private string _fullStoryText =
        "Hello,World!&����̓e�L�X�g�\���̃T���v���ł�&����ɂ��́I&���̃y�[�W�͂���ŏI��肾��B"; // ���O�\�����폜�����ꍇ�̃T���v��

    // 1�������\�����邽�߂̃L���[
    private Queue<char> _charQueue;
    // �y�[�W�i�s�j���Ƃɕ\�����邽�߂̃L���[
    private Queue<string> _pageQueue;

    // ���ݎ��s���̕�������R���[�`����ێ�
    private Coroutine _displayCoroutine;

    // MonoBehaviour���p�����Ă���ꍇ�����
    // �ŏ��̍X�V�֐�(Update���\�b�h)���Ă΂�鎞�ɍŏ��ɌĂ΂��
    private void Start()
    {
        // �������������J�n
        Init();

        // Start()�ł�UpdateSpeedDisplayText()���Ăяo���Ă������ƂŁA
        // �V�[�����[�h���ɃI�u�W�F�N�g���A�N�e�B�u�ł���΂����ɕ\�������B
        // �������AOnEnable()�ł��Ăяo�����߁A�ݒ��ʂ���A�N�e�B�u����A�N�e�B�u�ɂȂ�ۂɂ��Ή��ł���B
        UpdateSpeedDisplayText();
    }

    // �Q�[���I�u�W�F�N�g���A�N�e�B�u�ɂȂ邽�тɌĂяo�����
    private void OnEnable()
    {
        // �I�u�W�F�N�g���A�N�e�B�u�ɂȂ����Ƃ��ɁA���݂̑��x�\�����X�V
        // ����ɂ��A�ݒ��ʂ���A�N�e�B�u����A�N�e�B�u�ɂȂ����ۂɂ��\�����X�V�����
        // �������AStart()���OnEnable()�̕����������s����邱�Ƃ����邽�߁A
        // captionSpeed���܂����[�h����Ă��Ȃ��\�����l������
        if (captionSpeed == 0 && PlayerPrefs.HasKey(CAPTION_SPEED_KEY)) // �܂����[�h����Ă��Ȃ��A���L�[�����݂���ꍇ
        {
            captionSpeed = PlayerPrefs.GetFloat(CAPTION_SPEED_KEY, normalSpeed);
        }
        else if (captionSpeed == 0) // �L�[�����݂��Ȃ��ꍇ
        {
            captionSpeed = normalSpeed;
        }

        UpdateSpeedDisplayText();
    }

    // MonoBehaviour���p�����Ă���ꍇ�����
    // ���t���[���Ă΂��
    private void Update()
    {
        // ��(=0)�N���b�N���ꂽ��OnClick���\�b�h���Ăяo��
        if (Input.GetMouseButtonDown(0))
        {
            OnClick();
        }
    }

    /// <summary>
    /// ��������w�肵����؂蕶�����Ƃɋ�؂�A�L���[�Ɋi�[�������̂�Ԃ��i�y�[�W��؂�p�j
    /// </summary>
    private Queue<string> SeparatePages(string str, char sep)
    {
        string[] strs = str.Split(sep);
        Queue<string> queue = new Queue<string>();
        foreach (string l in strs)
        {
            queue.Enqueue(l);
        }
        return queue;
    }

    /// <summary>
    /// ����1�������Ƃɋ�؂�A�L���[�Ɋi�[�������̂�Ԃ��i�Z���t�̕�������p�j
    /// </summary>
    private Queue<char> SeparateCharacters(string str)
    {
        char[] chars = str.ToCharArray();
        Queue<char> charQueue = new Queue<char>();
        foreach (char c in chars)
        {
            charQueue.Enqueue(c);
        }
        return charQueue;
    }

    /// <summary>
    /// �L���[����1���������o���ĕ\������
    /// �L���[����ɂȂ�����false��Ԃ�
    /// </summary>
    private bool OutputChar()
    {
        if (_charQueue == null || _charQueue.Count <= 0)
        {
            return false; // �L���[�ɉ����i�[����Ă��Ȃ����false��Ԃ�
        }
        mainText.text += _charQueue.Dequeue();
        return true;
    }

    /// <summary>
    /// �������肷��R���[�`��
    /// </summary>
    private IEnumerator ShowChars(float wait)
    {
        // OutputChar���\�b�h��false��Ԃ�(=�L���[����ɂȂ�)�܂Ń��[�v����
        while (OutputChar())
        {
            yield return new WaitForSeconds(wait); // wait�b�����ҋ@
        }
        _displayCoroutine = null; // �R���[�`�����I��������null�ɂ���
        yield break;
    }

    /// <summary>
    /// 1�s�̃e�L�X�g��ǂݍ��݁A�L���[�Ɋi�[���A����������J�n����
    /// </summary>
    private void ReadLine(string text)
    {
        // �����̕�������R���[�`��������Β�~
        if (_displayCoroutine != null)
        {
            StopCoroutine(_displayCoroutine);
            _displayCoroutine = null;
        }

        // ���O�\���@�\���s�v�ɂȂ������߁A���O�̕�����ݒ�Ɋւ��鏈�����폜���܂��B
        // �����ȑO��_fullStoryText�̌`�����ێ����Ă���ꍇ�A�����ł̕ύX���l�����Ă��������B
        // �i�Ⴆ�΁A"�i���[�^�[�u�Z���t�v"�̂悤�Ȍ`������u�i���[�^�[�v�Ɓu�v���폜����K�v������ꍇ�j
        // ���݂�_fullStoryText�̃t�H�[�}�b�g�i���O�Ȃ��j�ɍ��킹�Ē������܂����B

        mainText.text = ""; // ���C���e�L�X�g����x�N���A
        _charQueue = SeparateCharacters(text); // �e�L�X�g�S�̂𕶎��L���[�ɕϊ�

        // �V������������R���[�`�����J�n���A�Q�Ƃ�ێ�
        _displayCoroutine = StartCoroutine(ShowChars(captionSpeed));
    }

    /// <summary>
    /// �S�����u���ɕ\������
    /// </summary>
    private void OutputAllChar()
    {
        // ��������R���[�`�������s���ł���Β�~
        if (_displayCoroutine != null)
        {
            StopCoroutine(_displayCoroutine);
            _displayCoroutine = null;
        }

        // �L���[����ɂȂ�܂Ŏc��̕�����S�ĕ\��
        while (OutputChar()) ;
    }

    /// <summary>
    /// ����������i�ŏ��̃y�[�W��ǂݍ��ށj
    /// </summary>
    private void Init()
    {
        _pageQueue = SeparatePages(_fullStoryText, SEPARATE_PAGE);
        ShowNextPage();
    }

    /// <summary>
    /// ���̃y�[�W�i�s�j��\������
    /// </summary>
    private bool ShowNextPage()
    {
        if (_pageQueue.Count <= 0)
        {
            Debug.Log("���ׂẴe�L�X�g�y�[�W��\�����܂����B");
            return false;
        }
        ReadLine(_pageQueue.Dequeue());
        return true;
    }

    /// <summary>
    /// �N���b�N�����Ƃ��̏����i�S���\���܂��͎��̃y�[�W�֐i�ށj
    /// </summary>
    private void OnClick()
    {
        // �܂��������蒆�ł���ΑS���\��
        if (_charQueue != null && _charQueue.Count > 0)
        {
            OutputAllChar();
        }
        else
        {
            // �S���\�����I����Ă���Ύ��̃y�[�W��
            if (!ShowNextPage())
            {
                // �S�Ẵy�[�W�\�������������ꍇ�̏���
                // ��: �V�[���J�ځA����̃C�x���g�̔����Ȃ�
                Debug.Log("���ꂪ�I�����܂����I");
            }
        }
    }

    // --- ���x�ݒ胁�\�b�h ---
    /// <summary>
    /// �\�����x���u�����v�ɐݒ肵�APlayerPrefs�ɕۑ����܂��B
    /// </summary>
    public void SetSpeedFast()
    {
        captionSpeed = fastSpeed;
        PlayerPrefs.SetFloat(CAPTION_SPEED_KEY, captionSpeed); // PlayerPrefs�ɑ��x��ۑ�
        UpdateSpeedDisplayText(); // ���x�\���e�L�X�g���X�V
        Debug.Log("�������葬�x���u�����v�ɐݒ肵�܂���: " + captionSpeed);
    }

    /// <summary>
    /// �\�����x���u���ʁv�ɐݒ肵�APlayerPrefs�ɕۑ����܂��B
    /// </summary>
    public void SetSpeedNormal()
    {
        captionSpeed = normalSpeed;
        PlayerPrefs.SetFloat(CAPTION_SPEED_KEY, captionSpeed); // PlayerPrefs�ɑ��x��ۑ�
        UpdateSpeedDisplayText(); // ���x�\���e�L�X�g���X�V
        Debug.Log("�������葬�x���u���ʁv�ɐݒ肵�܂���: " + captionSpeed);
    }

    /// <summary>
    /// �\�����x���u�x���v�ɐݒ肵�APlayerPrefs�ɕۑ����܂��B
    /// </summary>
    public void SetSpeedSlow()
    {
        captionSpeed = slowSpeed;
        PlayerPrefs.SetFloat(CAPTION_SPEED_KEY, captionSpeed); // PlayerPrefs�ɑ��x��ۑ�
        UpdateSpeedDisplayText(); // ���x�\���e�L�X�g���X�V
        Debug.Log("�������葬�x���u�x���v�ɐݒ肵�܂���: " + captionSpeed);
    }

    /// <summary>
    /// ���݂̕\�����x�ɉ����āA���x�\���e�L�X�g���X�V���܂��B
    /// </summary>
    private void UpdateSpeedDisplayText()
    {
        if (speedDisplayText == null)
        {
            Debug.LogWarning("Speed Display Text (TextMeshProUGUI) is not assigned in the Inspector.");
            return;
        }

        string speedText = "";
        // ���������_���̔�r�ɂ� Mathf.Approximately ���g�p���܂�
        if (Mathf.Approximately(captionSpeed, fastSpeed))
        {
            speedText = "�͂₢";
        }
        else if (Mathf.Approximately(captionSpeed, normalSpeed))
        {
            speedText = "�ӂ�";
        }
        else if (Mathf.Approximately(captionSpeed, slowSpeed))
        {
            speedText = "�������";
        }
        else
        {
            speedText = "�s���ȑ��x"; // �\�����Ȃ��l�̏ꍇ
        }

        speedDisplayText.text = "���݂̕\�����x�F" + speedText;
    }
}
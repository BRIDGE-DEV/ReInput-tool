using UnityEngine;
using System.Runtime.InteropServices;

public class UserOutputStore : MonoBehaviour
{
    [DllImport("user32.dll")]
    static extern uint keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

    private const int KEYEVENTF_KEYDOWN = 0x0000;
    private const int KEYEVENTF_EXTENDEDKEY = 0x0001;
    private const int KEYEVENTF_KEYUP = 0x0002;
    
    private FileDataHandler _fileDataHandler;
    private KeyInfoList _keyInfoList;
    private ConvertKeycode _convertKeycode;

    private const string FileName = "KeyInfoList.json";
    private bool isMouseOverGame = false;

    void Start()
    {
        _convertKeycode = new ConvertKeycode();
        
        // 읽어오기
        _fileDataHandler = new FileDataHandler(Application.persistentDataPath, FileName);
        _keyInfoList = _fileDataHandler.Load();
    }

    void Update()
    {
        
        Vector3 mousePosition = Input.mousePosition;
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        bool isMouseOver = mousePosition.x > 0 && mousePosition.x < Screen.width && mousePosition.y > 0 && mousePosition.y < Screen.height;
        if (isMouseOver && !isMouseOverGame)
        {
            Time.timeScale = 1f;
            isMouseOverGame = true;
        }
        else if (!isMouseOver && isMouseOverGame)
        {
            Time.timeScale = 0f;
            isMouseOverGame = false;
        }


        if (isMouseOverGame)
        {
            for (int i = _keyInfoList.keyInfos.Count - 1; i >= 0; i--)
            {
                if (Time.time >= _keyInfoList.keyInfos[i].time)
                {
                    byte key = _convertKeycode.GetWindowKeyCoe(_keyInfoList.keyInfos[i].key);
                    if (_keyInfoList.keyInfos[i].keyStatus == true)
                    {
                        keybd_event(key, 0, KEYEVENTF_KEYDOWN, 0);
                    }
                    else
                    {
                        keybd_event(key, 0, KEYEVENTF_KEYUP, 0);
                    }
                
                    _keyInfoList.keyInfos.RemoveAt(i);
                }
            }
        }
    }
}

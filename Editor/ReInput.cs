using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEditor.UIElements;

public class ReInput : EditorWindow
{
    private const string KEYINFOLIST_FILE_NAME = "KeyInfoList.json";
    private const string PLAYER_PREFS_KEY_AUTO_RECORDING = "REINPUT_AUTO_RECORDING";

    private KeyInfoList keyInfoList;
    private FileDataHandler fileDataHandler;

    private static GameObject inputStore;
    private GameObject outputStore;

    public static bool IsAutoRecording
    {
        get => PlayerPrefs.GetInt(PLAYER_PREFS_KEY_AUTO_RECORDING, 0) != 0;
        private set => PlayerPrefs.SetInt(PLAYER_PREFS_KEY_AUTO_RECORDING, value ? 1 : 0);
    }

    [MenuItem("Window/ReInput")]
    public static void ShowWindow()
    {
        var reInput = GetWindow<ReInput>("Custom Package - ReInput");
        reInput.titleContent = new GUIContent("ReInput");
    }

    private void OnEnable()
    {
        fileDataHandler = new FileDataHandler(Application.persistentDataPath, KEYINFOLIST_FILE_NAME);
        keyInfoList = fileDataHandler.Load();

        rootVisualElement.Add(CreateKeyInfoList());
        
        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.bridgedev.reinput/Editor/ReInput.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        rootVisualElement.Add(labelFromUXML);
        
        var saveButton = rootVisualElement.Q<Button>("button-saveJson");
        saveButton.clicked += OnSaveButtonClicked;
        
        var startRecordingButton = rootVisualElement.Q<Button>("button-startRecording");
        startRecordingButton.clicked += OnStartRecordingButtonClicked;
        
        var stopRecordingButton = rootVisualElement.Q<Button>("button-stopRecording");
        stopRecordingButton.clicked += OnStopRecordingButtonClicked;

        var autoRecordingToggle = rootVisualElement.Q<Toggle>("toggle-autoRecoding");
        autoRecordingToggle.value = IsAutoRecording;

        autoRecordingToggle.RegisterValueChangedCallback(evt =>
        {
            IsAutoRecording = evt.newValue;
        });
        
        var startReInputButton = rootVisualElement.Q<Button>("button-startReInput");
        startReInputButton.clicked += OnStartReInputButtonClicked;
        
        var stopReInputButton = rootVisualElement.Q<Button>("button-stopReInput");
        stopReInputButton.clicked += OnStopReInputButtonClicked;
    }

    private VisualElement CreateKeyInfoList()
    {
        var keyInfoListElement = new VisualElement
        {
            name = "keyInfo-list",
            style =
            {
                width = StyleKeyword.Auto,
                height = StyleKeyword.Auto
            }
        };

        foreach (var keyInfo in keyInfoList.keyInfos)
        {
            keyInfoListElement.contentContainer.Add(CreateKeyInfoFoldout(keyInfo));
        }
        
        // var foldOut = new Foldout
        // {
        //     text = "Input Key List",
        //     value = false
        // };
        //
        // foldOut.contentContainer.Add(keyInfoListElement);
        
        var scrollView = new ScrollView
        {
            style =
            {
                width = StyleKeyword.Auto,
                height = 1000
            }
        };

        scrollView.contentContainer.Add(keyInfoListElement);
        
        return scrollView;
    }

    private VisualElement CreateKeyInfoFoldout(KeyInfo keyInfo)
    {
        var keyInfoElement = new VisualElement
        {
            name = "keyInfo-element"
        };

        var keyCodeField = new TextField("Key")
        {
            value = keyInfo.key.ToString()
        };

        keyCodeField.RegisterValueChangedCallback(evt => {
            var targetKeyInfo = keyInfoList.keyInfos.Find(info => info.time.Equals(keyInfo.time));
            targetKeyInfo.key = (KeyCode)Enum.Parse(typeof(KeyCode), evt.newValue);
        });

        keyInfoElement.Add(keyCodeField);

        var timeField = new FloatField("Time")
        {
            value = keyInfo.time
        };

        timeField.RegisterValueChangedCallback(evt => {
            var targetKeyInfo = keyInfoList.keyInfos.Find(info => info.time.Equals(keyInfo.time));
            targetKeyInfo.time = evt.newValue;
        });

        keyInfoElement.Add(timeField);

        var keyStatusField = new Toggle("KeyStatus")
        {
            value = keyInfo.keyStatus
        };

        keyStatusField.RegisterValueChangedCallback(evt => {
            var targetKeyInfo = keyInfoList.keyInfos.Find(info => info.time.Equals(keyInfo.time));
            targetKeyInfo.keyStatus = evt.newValue;
        });

        keyInfoElement.Add(keyStatusField);

        var foldout = new Foldout
        {
            text = keyInfo.key.ToString(),
            value = true
        };

        foldout.contentContainer.Add(keyInfoElement);

        return foldout;
    }

    private void OnSaveButtonClicked()
    {
        try
        {
            keyInfoList.keyInfos.Sort((info1, info2) => info1.time.CompareTo(info2.time));

            fileDataHandler.Save(keyInfoList);
            
            Debug.Log("[ReInput] Save Success!");
            
            EditorUtility.RequestScriptReload();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
    
    public static void OnStartRecordingButtonClicked()
    {
        Debug.Log($"[ReInput] Start Recording!");

        if (inputStore != null)
        {
            Debug.LogError($"[ReInput] It's already playing input now!");
            return;
        }

        var inputStorePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.bridgedev.reinput/Runtime/InputStore.prefab");
        inputStore = Instantiate(inputStorePrefab);
    }
    
    private static void OnStopRecordingButtonClicked()
    {
        Debug.Log($"[ReInput] Stop Recording!");

        if (inputStore != null)
        {
            Destroy(inputStore);
            inputStore = null;
        }
        else
        {
            Debug.LogError($"[ReInput] There's no input system now.");
        }
    }
    
    private void OnStartReInputButtonClicked()
    {
        Debug.Log($"[ReInput] Start ReInput!");

        if (outputStore != null)
        {
            Debug.LogError($"[ReInput] It's already playing output now!");
            return;
        }
        
        EditorApplication.EnterPlaymode();

        var outputStorePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.bridgedev.reinput/Runtime/OutputStore.prefab");
        outputStore = Instantiate(outputStorePrefab);
    }
    
    private void OnStopReInputButtonClicked()
    {
        Debug.Log($"[ReInput] Stop ReInput!");

        if (outputStore != null)
        {
            Destroy(outputStore);
            outputStore = null;
        }
        else
        {
            Debug.LogError($"[ReInput] There's no output system now.");
        }
    }
}

[InitializeOnLoad]
public static class PlayModeStateListener
{
    static PlayModeStateListener()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        switch (state)
        {
            case PlayModeStateChange.EnteredPlayMode:
                if (ReInput.IsAutoRecording)
                {
                    ReInput.OnStartRecordingButtonClicked();
                }

                break; 
            case PlayModeStateChange.ExitingPlayMode:
                EditorUtility.RequestScriptReload();

                break;
        }
    }
}
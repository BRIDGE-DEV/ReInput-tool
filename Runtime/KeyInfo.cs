using UnityEngine;

[System.Serializable]
// struct가 pass by value여서 Editor에서 저장에 이슈가 있었음.
// struct를 class로 수정하여 해결. - Hyeonwoo, 2023.02.16.
public class KeyInfo    
{
    public KeyCode key;
    // Editor에서 key, time, 상태를 수정할 때 time으로 해당 정보를 찾는다.
    // 시간은 무조건 고유할 것이므로 ID를 겸한다. - Hyeonwoo, 2023.02.16.
    public float time;
    public bool keyStatus;

    public KeyInfo(KeyCode key, float time, bool status)
    {
        this.key = key;
        this.time = time;
        this.keyStatus = status;
    }
}
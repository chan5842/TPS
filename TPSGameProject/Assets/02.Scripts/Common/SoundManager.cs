using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public float Volume = 1f;                // 사운드 음량
    public bool isSoundMute = false;    // 음소거 판단
    public static SoundManager soundManager;

    private void Awake()
    {
        if (soundManager == null)
            soundManager = this;
        else
            Destroy(gameObject);

        // 다음 씬으로 넘어가도 오브젝트가 파괴되지 않음
        DontDestroyOnLoad(gameObject);
    }

    // 사운드 공용 함수(포폴 필수)
    public void PlaySoundFunc(Vector3 pos, AudioClip audioClip)
    {
        // 음소거 옵션 설정 시 바로 나옴
        if (isSoundMute) return;

        GameObject soundObj = new GameObject("Sfx");
        // 사운드 발생 위치 지정
        soundObj.transform.position = pos;

        // 생성한 게임 오브젝트의 오디오소스 컴포넌트 추가
        AudioSource source = soundObj.AddComponent<AudioSource>();
        source.clip = audioClip;
        source.minDistance = 10f;
        source.minDistance = 30f;
        source.volume = Volume;
        source.Play();
        Destroy(soundObj, audioClip.length);
    }
}


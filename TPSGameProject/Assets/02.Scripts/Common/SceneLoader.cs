using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public CanvasGroup fadeCG;
    [Range(0.5f, 2f)]
    public float fadeDuration = 1f; // 페이드 지속 시간

    // 키, 값 으로 이루어진 딕셔너리(짝을 이룸)
    public Dictionary<string, LoadSceneMode> loadScenes 
        = new Dictionary<string, LoadSceneMode>();

    void InitSceneInfo() // 호출할 씬 정보 초기화
    {
        loadScenes.Add("Level_1", LoadSceneMode.Additive);
        loadScenes.Add("BattleFieldScene", LoadSceneMode.Additive);
    }
    
    // Start 함수와 마찬가지로 콜백된다
    IEnumerator Start()
    {
        InitSceneInfo();

        fadeCG.alpha = 1.0f; // 초기 투명도 값 설정(불투명)

        foreach(var _loadScene in loadScenes)
        {
            // 여러개의 씬을 코루틴으로 호출
            yield return StartCoroutine(LoadScene(_loadScene.Key, _loadScene.Value));
        }
        StartCoroutine(Fade(0.0f));
    }

    IEnumerator LoadScene(string sceneName, LoadSceneMode mode)
    {
        // 비동기 방식으로 씬 로드
        yield return SceneManager.LoadSceneAsync(sceneName, mode);
        
        // 씬 호출(현재 씬 SceneLoader는 제거)
        Scene loadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        // 호출된 씬을 활성화
        SceneManager.SetActiveScene(loadedScene);
    }

    IEnumerator Fade(float finalAlpha)
    {
        // 라이트 매핑이 깨지는 것을 방지하기 위해 스테이지 씬을 활성화
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Level_1"));
        fadeCG.blocksRaycasts = true;
        // 절대값 함수의 백분율을 계산
        float fadeSpeed = Mathf.Abs(fadeCG.alpha - finalAlpha) / fadeDuration;
        // 알파값 조정
        while(!Mathf.Approximately(fadeCG.alpha, finalAlpha))
        {
            // 보간 함수(Lerp와 비슷함)
            fadeCG.alpha = Mathf.MoveTowards(fadeCG.alpha, finalAlpha, fadeSpeed * Time.deltaTime);
            yield return null;  // 한프레임 돈다
        }
        fadeCG.blocksRaycasts = false;

        // 페이드 인이 완료된 후 SceneLoader씬은 삭제
        SceneManager.UnloadSceneAsync("SceneLoader");
    }
}

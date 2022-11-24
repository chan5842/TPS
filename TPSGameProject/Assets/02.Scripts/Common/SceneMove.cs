using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMove : MonoBehaviour
{
    public void OnClickStatrBtn()
    {
        //SceneManager.LoadScene("Level_1");
        //SceneManager.LoadScene("BattleFieldScene", LoadSceneMode.Additive); // 씬 병합

        SceneManager.LoadScene("SceneLoader");

        // 로드 되는 동안에만 기존에 있는 씬을 삭제하고 새로운 씬을 로드
        //SceneManager.LoadScene("BattleFieldScene", LoadSceneMode.Single); 

        // 새로운 빈 씬을 생성
        //SceneManager.CreateScene();

        // 씬을 비동기 방식으로 로드(동기 방식 : 씬을 이동 할때 아무것도 못함)
        //SceneManager.LoadSceneAsync();

        // 소스 씬을 다른씬으로 통합(소스 씬은 모든 게임오브젝트가 통합된 후 삭제 된다)
        //SceneManager.MergeScenes();

        // 현재 씬에 있는 특정 오브젝트를 다른 씬으로 이동시킨다.
        //SceneManager.MoveGameObjectToScene();

        // 현재 씬에 있는 모든 게임 오브젝트를 삭제 한다
        //SceneManager.UnloadScene(GameObject go, Scene scene);
    }
}

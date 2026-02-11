using System.Collections;
using UnityEngine;

public class ReturnSelfToPool : MonoBehaviour
{
    public float destroyTime = 2f;
    private Coroutine _returnToPoolTimerCoroutine;
    

    private void OnEnable()
    {
        _returnToPoolTimerCoroutine = StartCoroutine(ReturnToPoolAfterTime());
    }
    
    private IEnumerator ReturnToPoolAfterTime()
    {
        float elapsedTime = 0f;
        while(elapsedTime < destroyTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        this.gameObject.SetActive(false);
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }

}

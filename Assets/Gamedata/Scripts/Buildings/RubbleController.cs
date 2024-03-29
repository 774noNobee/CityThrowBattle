using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubbleController : MonoBehaviour
{
    [SerializeField]
    float remainTime;
    IEnumerator coroutine;
    [SerializeField]
    Rigidbody rb;
    [SerializeField]
    Vector2 forceRange;
    [SerializeField]
    Vector2 torqueRange;
    void Start()
    {
        coroutine = null;
        rb= GetComponent<Rigidbody>();
        int ramdomForce = Random.Range((int)forceRange.x, (int)forceRange.y);
        int randomTorque=Random.Range((int)torqueRange.x, (int)torqueRange.y);
        rb.AddForce(Vector3.up * ramdomForce, ForceMode.Impulse);
        rb.AddTorque(Vector3.left * randomTorque, ForceMode.Impulse);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground")&&coroutine==null)
        {

            coroutine = FadeOut();
            StartCoroutine(coroutine);
        }
    }

    /// <summary>
    /// ä¢‚IÇ™èôÅXÇ…ìßñæÇ…Ç»ÇÈ
    /// </summary>
    /// <returns></returns>
    IEnumerator FadeOut()
    {
        Vector3 newScale = Vector3.zero;
        yield return new WaitForSeconds(remainTime);
        while (transform.localScale.x>0)
        {
            newScale.x=transform.localScale.x-Time.deltaTime;
            newScale.y=transform.localScale.y-Time.deltaTime;
            newScale.z=transform.localScale.z-Time.deltaTime;
            if(newScale.x<0)
            {
                newScale.x = 0;
                newScale.y = 0;
                newScale.z = 0;
            }
            transform.localScale=newScale;
            yield return null;
        }
        Destroy(gameObject);
        yield break;
    }
    
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupBehavior : MonoBehaviour
{
    [SerializeField] TMP_Text notifText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform);
    }

    public void NewPopup(string text, Color textColor)
    {
        transform.LookAt(Camera.main.transform);
        transform.localScale = Vector3.one;
        notifText.text = text;
        notifText.color = textColor;
        StartCoroutine(PopupLerp());
    }

    IEnumerator PopupLerp()
    {
        Vector3 startingPos = transform.position;
        Vector3 endPos = startingPos + Vector3.up * 0.5f;

        Vector3 startingScale = transform.localScale;
        Vector3 endScale = startingScale * 1.25f;

        float timer = 0f;
        while (timer < 1f)
        {
            transform.position = Vector3.Lerp(startingPos, endPos, timer);
            transform.localScale = Vector3.Lerp(startingScale, endScale, timer);

            if (timer > 0.5f)
            {
                notifText.alpha = 1 - 2 * (timer - 0.5f);
            }

            yield return new WaitForFixedUpdate();
            timer += Time.fixedDeltaTime;
        }
        Destroy(gameObject);
    }
}

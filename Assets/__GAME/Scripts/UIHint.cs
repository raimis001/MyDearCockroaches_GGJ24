using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIHint : MonoBehaviour
{
    [SerializeField]
    TMP_Text caption;

    public string Caption { set => SetCaption(value);  }

    void SetCaption(string value)
    {
        StopAllCoroutines();

        caption.text = value;

        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        Vector2 pos = Vector2.zero;

        ((RectTransform)caption.transform).anchoredPosition = pos;

        Color c = caption.color;
        c.a = 1;
        caption.color = c;

        float max = 3f;
        float time = max;

        while (time > 0)
        {
            yield return null;
            time -= Time.deltaTime;
            c.a = time / max;
            caption.color = c;
            pos.y += Mathf.Lerp(0, 0.001f, 1 - c.a);
            ((RectTransform)caption.transform).anchoredPosition = pos;
        }

        caption.text = string.Empty;
        ((RectTransform)caption.transform).anchoredPosition = Vector2.zero;

    }
}

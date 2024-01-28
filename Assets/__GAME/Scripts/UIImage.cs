using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIImage : MonoBehaviour
{
    [SerializeField]
    NeedsKind kind;

    float need => Face.Needs[kind].value;
    Image sprite;

    private void Start()
    {
        sprite = GetComponent<Image>();
    }

    void Update()
    {
        Color c = sprite.color;
        c.a = Face.gameEnded ? 0 : need;
        sprite.color = c;
    }
}

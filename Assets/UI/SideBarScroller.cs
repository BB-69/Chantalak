using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SideBarScroller : MonoBehaviour {
    [SerializeField]
    private Renderer imageRenderer;
    public float speed;
 
    void Update()
    {
        imageRenderer.material.mainTextureOffset += new Vector2(0,speed*Time.deltaTime);
    }
}

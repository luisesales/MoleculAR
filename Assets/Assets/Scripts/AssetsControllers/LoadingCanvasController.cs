using System;
using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LoadingCanvasController : MonoBehaviour
{
    [SerializeField] Image loadingWheel;
    [SerializeField] TMP_Text loadingText;
    [SerializeField] GameObject loadingFx;
    [Range(0,1)]
    public float  progress = 0f;

    // Update is called once per frame
    void Update()
    {
        loadingWheel.fillAmount = progress;
        loadingText.text = MathF.Floor(progress * 100).ToString();
        loadingFx.transform.rotation = Quaternion.Euler(0, 0, -progress * 360);
    }

    public void ReactivateParticles()
    {
        ParticleSystem ps = loadingFx.transform.GetChild(0).GetComponent<ParticleSystem>();
        ps.Stop();
        ps.Play();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zawarudo : MonoBehaviour
{
    [SerializeField]
    private float maxScale = 200;
    [SerializeField]
    private float expandSpeed = 800;
    [SerializeField]
    private float shrinkSpeed = 600;
    [SerializeField]
    private float delayTime = 1;
    [SerializeField]
    private float holdTime = 2;
    [SerializeField]
    private List<GameObject> waveList;
    [SerializeField]
    private List<float> waveExpandDelayList;
    [SerializeField]
    private List<float> waveShrinkDelayList;
    private List<bool> waveExpandingList = new List<bool>();
    private List<bool> waveShrinkingList = new List<bool>();
    [SerializeField]
    private float waveExpandSpeed = 800;
    [SerializeField]
    private float waveShrinkSpeed = 400;
    [SerializeField]
    private float waveMaxScale = 0.2f;
    [SerializeField]
    private float scale;
    [SerializeField]
    private bool isExpanding = false;
    [SerializeField]
    private bool isShrinking = false;
    [SerializeField]
    private float maxScaleX;
    [SerializeField]
    private AudioClip dio;
    private AudioSource audioSource;
    private void Start()
    {
        Initialize();
    }
    private void Update()
    {
        UpdateState();
    }
    private void Initialize()
    {
        transform.localScale = new Vector3(scale, scale, scale);
        audioSource = GetComponent<AudioSource>();
        for (int i = 0; i < waveList.Count; ++i)
        {
            waveExpandingList.Add(false);
            waveShrinkingList.Add(false);
        }
        audioSource.PlayOneShot(dio);
        maxScaleX = Vector3.Distance(Camera.main.transform.position, transform.position) / 2 * 3.14f;
        Invoke(nameof(Expand), delayTime);
        for (int i = 0; i < waveList.Count; i++)
        {
            StartCoroutine(ExpandWave(i, waveExpandDelayList[i]));
            StartCoroutine(ShrinkWave(i, waveShrinkDelayList[i]));
        }
    }
    private void FacingCamera()
    {
        Vector3 facingCameraDirection = (Camera.main.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.FromToRotation(Vector3.right, facingCameraDirection);
    }
    private void UpdateState()
    {
        FacingCamera();
        /*  if (Input.GetKeyDown(KeyCode.P))
              Time.timeScale = 1;*/
        /*if (Input.GetKeyDown(KeyCode.O) && !isExpanding && !isShrinking && Time.timeScale > 0)
        {
            audioSource.PlayOneShot(dio);
            maxScaleX = Vector3.Distance(Camera.main.transform.position, transform.position) / 2 * 3.14f;
            Invoke(nameof(Expand), delayTime);
            for (int i = 0; i < waveList.Count; i++)
            {
                StartCoroutine(ExpandWave(i, waveExpandDelayList[i]));
                StartCoroutine(ShrinkWave(i, waveShrinkDelayList[i]));
            }
        }*/
        if (isExpanding)
        {
            scale += expandSpeed * Time.deltaTime;
            transform.localScale = new Vector3(Mathf.Min(scale, maxScaleX), scale, scale);
            if (scale > maxScale)
            {
                isExpanding = false;
                Invoke(nameof(Shrink), holdTime);
            }
        }
        if (isShrinking)
        {
            scale -= shrinkSpeed * Time.deltaTime;
            if (scale <= 0)
            {
                isExpanding = false;
                isShrinking = false;
                scale = 0;
                Time.timeScale = 0;
            }
            transform.localScale = new Vector3(Mathf.Min(scale, maxScaleX), scale, scale);
        }
        for (int i = 0; i < waveList.Count; i++)
        {
            if (waveExpandingList[i])
            {
                float s = waveList[i].transform.localScale.x;
                s += waveExpandSpeed * Time.deltaTime;
                if (s >= waveMaxScale)
                    waveExpandingList[i] = false;
                waveList[i].transform.localScale = new Vector3(Mathf.Min(s, waveMaxScale), 1, Mathf.Min(s, waveMaxScale));
            }
            if (waveShrinkingList[i])
            {
                float s = waveList[i].transform.localScale.x;
                s -= waveShrinkSpeed * Time.deltaTime;
                if (s <= 0)
                    waveShrinkingList[i] = false;
                waveList[i].transform.localScale = new Vector3(Mathf.Max(s, 0), 1, Mathf.Max(s, 0));
            }
        }

    }
    private void Expand()
    {
        isExpanding = true;
        scale = 0;
    }
    private void Shrink()
    {
        isShrinking = true;
    }
    private IEnumerator ExpandWave(int index, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        waveExpandingList[index] = true;
        waveShrinkingList[index] = false;
        waveList[index].transform.localScale = new Vector3(0, 1, 0);
    }
    private IEnumerator ShrinkWave(int index, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        waveExpandingList[index] = false;
        waveShrinkingList[index] = true;
    }
}

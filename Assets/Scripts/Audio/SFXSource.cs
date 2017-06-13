using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXSource : MonoBehaviour
{
    #region Editor

    [Serializable]
    public struct SFX
    {
        public string Name;
        public AudioClip Clip;
        public bool Loop;
        public float LoopDelay;
    }
    public SFX[] SoundEffects;

    #endregion


    #region Properties

    private AudioSource mSource;
    private Dictionary<string, SFX> mSFXMap;

    #endregion


    void Start()
    {
        mSource = gameObject.AddComponent<AudioSource>();
        mSFXMap = new Dictionary<string, SFX>();
        for (int i = 0; i < SoundEffects.Length; i++)
        {
            mSFXMap.Add(SoundEffects[i].Name, SoundEffects[i]);
        }
    }


    #region Public Methods

    public void PlaySFX(string Name)
    {
        if(mSFXMap.ContainsKey(Name) && !mSource.isPlaying)
        {
            SFX S = mSFXMap[Name];
            mSource.clip = S.Clip;
            mSource.Play();
            if (S.Loop)
            {
                StartCoroutine(LoopSFX(S));
            }
        }
    }

    public void PauseSFX()
    {
        if (mSource.isPlaying)
        {
            mSource.Pause();
        }
        else if(mSource.clip != null)
        {
            mSource.Play();
        }
    }

    public void StopSFX()
    {
        mSource.Stop();
        mSource.clip = null;
        StopAllCoroutines();
    }

    #endregion


    #region Private Methods

    private IEnumerator LoopSFX(SFX S)
    {
        if(S.LoopDelay < 0)
        {
            yield return null;
        }

        yield return new WaitForSeconds(S.Clip.length + S.LoopDelay);

        if (mSource.clip == S.Clip)
        {
            mSource.Play();
            StartCoroutine(LoopSFX(S));
        }
    }

    #endregion
}

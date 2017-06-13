using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public AudioSource MusicSource;

    public AudioClip[] MusicFiles;

    private Dictionary<GameObject, SFXSource> mSources;

    new void Awake()
    {
        base.Awake();
        mSources = new Dictionary<GameObject, SFXSource>();
        //mClipMap = new Dictionary<string, AudioClip>();
        //for (int i = 0; i < Music.Length; i++)
        //{
        //    mClipMap.Add(Music[i].name, Music[i]);
        //}
        //PlayMusic("GameMusic");
    }

    public void PlayMusic(string Name)
    {
        for(int i = 0; i < MusicFiles.Length; i++)
        {
            if(MusicFiles[i].name == Name)
            {
                MusicSource.clip = MusicFiles[i];
                break;
            }
        }
        
        MusicSource.Play();
    }

    public void PlaySoundEffect(GameObject SourceObject, string FileName)
    {
        if(mSources.ContainsKey(SourceObject))
        {
            mSources[SourceObject].PlaySFX(FileName);
        }
        else
        {
            SFXSource Source = SourceObject.GetComponent<SFXSource>();
            if (Source == null)
            {
                Debug.LogWarning(SourceObject.name + " does not possess component SFXSource.");
                return;
            }
            mSources.Add(SourceObject, Source);
            Source.PlaySFX(FileName);
        }
    }

    public void StopSFX(GameObject SourceObject)
    {
        if (mSources.ContainsKey(SourceObject))
        {
            mSources[SourceObject].StopSFX();
        }
        else
        {
            Debug.LogWarning(SourceObject + " has not yet played any sound effects.");
        }
    }
}

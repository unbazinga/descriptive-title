using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Animator playerAnimator;
    public AnimationClip[] AnimationClips;
    protected AnimationClip _curAnimationClip;
    private static AnimationController _Instance;
    public static AnimationController Instance
    {
        get { return _Instance; }
    }

    void Awake()
    {
        if (_Instance != null && _Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _Instance = this;
        //DontDestroyOnLoad(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        foreach (var clip in AnimationClips)
        {
            Debug.Log(clip.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public AnimationClip GetCurrentAnimation() => _curAnimationClip;
}
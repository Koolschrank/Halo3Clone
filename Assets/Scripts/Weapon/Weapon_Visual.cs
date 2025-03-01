using MoreMountains.Feedbacks;
using MoreMountains.FeedbacksForThirdParty;
using MoreMountains.Tools;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Weapon_Visual : Weapon_Model
{
    [SerializeField] Animator animator;

    public UnityEvent OnShoot = new UnityEvent();

    public override void SetUp(Weapon_Arms weapon)
    {


        base.SetUp(weapon);
        weapon.OnReloadStart += Reload;
        weapon.OnSwitchInStart += SwitchIn;
        weapon.OnSwitchOutStart += SwitchOut;
        weapon.OnShot += Shoot;
        weapon.OnMeleeStart += MeleeAttackStart;

    }

    public override void OnDestroy()
    {

        if (weapon == null) return;
        base.OnDestroy();
        weapon.OnReloadStart -= Reload;
        weapon.OnSwitchInStart -= SwitchIn;
        weapon.OnSwitchOutStart -= SwitchOut;
        weapon.OnShot -= Shoot;
        weapon.OnMeleeStart -= MeleeAttackStart;

    }



    public void Shoot()
    {
        // if current animation is shoot reset it
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Shoot"))
        {
            animator.Play("Shoot", 0, 0);
        }
        else
        {
            animator.Play("Shoot");
        }

        OnShoot.Invoke();


    }

    public void Reload(float time)
    {
        // set animation time to time
        var reloadClip = GetAnimationClipByName("Reload");
        var animationLenght = GetAnimationLenght(reloadClip);
        SetAnimationSpeed(reloadClip, animationLenght, time);


        animator.SetTrigger("Reload");
    }

    public void SwitchIn(float time)
    {
        
        // get child
        var child = transform.GetChild(0);
        
        var switchInClip = GetAnimationClipByName("SwitchIn");
        var animationLenght = GetAnimationLenght(switchInClip);
        SetAnimationSpeed(switchInClip, animationLenght, time);

        animator.SetTrigger("SwitchIn");
        
    }

    public void SwitchOut(float time)
    {
        var switchOutClip = GetAnimationClipByName("SwitchOut");
        var animationLenght = GetAnimationLenght(switchOutClip);
        SetAnimationSpeed(switchOutClip, animationLenght, time);

        animator.SetTrigger("SwitchOut");
        
    }

    public void ThrowGranadeStart(GranadeStats granade)
    {
        var time = granade.ThrowTime;

        var throwGranadeClip = GetAnimationClipByName("Throw");
        var animationLenght = GetAnimationLenght(throwGranadeClip);

        SetAnimationSpeed(throwGranadeClip, animationLenght, time);

        animator.SetTrigger("Throw");
    }

    public void MeleeAttackStart(float time)
    {
        var meleeAttackClip = GetAnimationClipByName("Melee");
        var animationLenght = GetAnimationLenght(meleeAttackClip);
        SetAnimationSpeed(meleeAttackClip, animationLenght, time);
        animator.SetTrigger("Melee");
    }



    AnimationClip GetAnimationClipByName(string animationName)
    {
        animationName = GetTextAfterLastUnderscore(animationName);


        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            var clipName = GetTextAfterLastUnderscore(clip.name);
            if (clipName == animationName)
            {
                return clip; // Return the matching AnimationClip
            }
        }
        return null; // Return null if not found
    }

    public float GetAnimationLenght(AnimationClip animationClip)
    {
        return animationClip.length;
    }

    public void SetAnimationSpeed(AnimationClip clip, float animationLenght, float animationTime)
    {
        //AnimationSpeed
        var speed = animationLenght / animationTime;

        animator.SetFloat("AnimationSpeed", speed); // Adjust speed of animation using a float parameter

    }

    string GetTextAfterLastUnderscore(string input)
    {
        int index = input.LastIndexOf('_');

        return index >= 0 ? input.Substring(index + 1) : input; // Return original if no "_"
    }

    



}


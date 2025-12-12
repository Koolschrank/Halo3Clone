using System;
using UnityEngine;

public class GranadeAnker : MonoBehaviour
{
	public Action OnGranadeThrowStart;
	public Action OnGranadeThrowEnd;
    public Animator animator;
	public Transform anker;
    Arm arm;



    public void SetUp(Arm arm)
    {
        this.arm = arm;
        arm.OnGranadeThrowStarted += ( stats, time) => PlayThrowAnimation(stats, time);

	}


    public void PlayThrowAnimation(GranadeStats granade, float throwTime)
    {
		// forec play IdleAnimtion
		animator.Play("Idle", 0, 0f);

		float delayPercent = granade.ThrowDelay / granade.ThrowTime;
		float animationDuration =  throwTime;

		var throughInClip = GetAnimationClipByName("Throw");
		var animationLenght = throughInClip.length;
		SetAnimationSpeed(throughInClip, animationLenght, animationDuration);


		
		animator.SetTrigger("Throw");
		OnGranadeThrowStart?.Invoke();

	}

	public void GranadeThrowEnd()
	{
		OnGranadeThrowEnd?.Invoke();
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

}

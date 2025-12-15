using UnityEngine;

public class FPS_Arms : MonoBehaviour
{
    public Transform rightTarget;
    public Transform leftTarget;
    public float reloadTransitionTime = 0.1f;
    bool inReload;
    float reloadTransitionProgress = 0f;
    public float granadeTransitionTime = 0.1f;
    bool inGranade;
    float granadeTransitionProgress = 0f;

    public float leftHandTransitionTime = 0.1f;
	bool hasLeftWeapon;
    float leftHandTransitionProgress = 0f; 


	Transform rightAnker;
    Transform leftAnker;
    Transform reloadAnker;
    Transform leftWeaponAnker;
    
    public GranadeAnker granadeAnker; 

    public SkinnedMeshRenderer armsMeshRenderer;

    public void SetUp(PlayerAnimation animation)
    {
        armsMeshRenderer.enabled = true;
		animation.AddMesh(armsMeshRenderer);
    }

    public void SetLeftWeaponAnker(Transform leftAnker)
    {
        this.leftWeaponAnker = leftAnker;
        hasLeftWeapon = true;
    }

    public void RemoveLeftWeaponAnker()
    {
        this.leftWeaponAnker = null;
        hasLeftWeapon = false;
    }


	public void SetRightAnker(Transform rightAnker)
        {
        this.rightAnker = rightAnker;
	}

    public void SetLeftAnker(Transform leftAnker)
    {
        this.leftAnker = leftAnker;
	}

    public void SetReloadAnker(Transform reloadAnker)
    {
        this.reloadAnker = reloadAnker;
        inReload = false;
    }

    public void StartReload()
    {
        inReload = true;
    }

    public void StopReload()
    {
        inReload = false;
	}

	private void Start()
	{
		granadeAnker.OnGranadeThrowStart += () => 
        {
            Debug.Log("Granade throw started");
			inGranade = true;
        };
        granadeAnker.OnGranadeThrowEnd += () => 
        {
            inGranade = false;
        };
	}


	private void LateUpdate()
	{
        if (rightAnker == null || leftAnker == null || reloadAnker == null || rightTarget == null || leftTarget == null)
            return;

		float reloadProgress = 0f;
        if (inReload)
        {
            reloadTransitionProgress += Time.deltaTime;
            reloadTransitionProgress = Mathf.Min(reloadTransitionProgress, reloadTransitionTime);
            reloadProgress = reloadTransitionProgress / reloadTransitionTime;

		}
        else
        {
            reloadTransitionProgress -= Time.deltaTime;
            reloadTransitionProgress = Mathf.Max(reloadTransitionProgress, 0f);
            reloadProgress = reloadTransitionProgress / reloadTransitionTime;
		}
        float granadeProgress = 0f;
        if (inGranade)
        {
            Debug.Log("Granade in transition");
			granadeTransitionProgress += Time.deltaTime;
			granadeTransitionProgress = Mathf.Min (granadeTransitionProgress, granadeTransitionTime);
            granadeProgress = granadeTransitionProgress / granadeTransitionTime;
        }
        else
        {
			granadeTransitionProgress -= Time.deltaTime /2;
            granadeTransitionProgress = Mathf.Max(granadeTransitionProgress, 0f);
			granadeProgress = granadeTransitionProgress / granadeTransitionTime;


		}

        float leftHandProgress = 0f;
        if (hasLeftWeapon)
        {
            leftHandTransitionProgress += Time.deltaTime;
            leftHandTransitionProgress = Mathf.Min(leftHandTransitionProgress, leftHandTransitionTime);
            leftHandProgress = leftHandTransitionProgress / leftHandTransitionTime;
        }
        else
        {
            leftHandTransitionProgress -= Time.deltaTime;
            leftHandTransitionProgress = Mathf.Max(leftHandTransitionProgress, 0f);
            leftHandProgress = leftHandTransitionProgress / leftHandTransitionTime;
		}

		Vector3 currentLeftPos = Vector3.Lerp(leftAnker.position, reloadAnker.position, reloadProgress);
        Quaternion currentLeftRot = Quaternion.Slerp(leftAnker.rotation, reloadAnker.rotation, reloadProgress);

        currentLeftPos = Vector3.Lerp(currentLeftPos, granadeAnker.anker.position, granadeProgress);
        currentLeftRot = Quaternion.Slerp(currentLeftRot, granadeAnker.anker.rotation, granadeProgress);

        if (leftWeaponAnker != null)
        {
            currentLeftPos = Vector3.Lerp(currentLeftPos, leftWeaponAnker.position, leftHandProgress);
            currentLeftRot = Quaternion.Slerp(currentLeftRot, leftWeaponAnker.rotation, leftHandProgress);
		}


		// copy position and rotation from ankerts to targets
		if (rightAnker != null && rightTarget != null)
        {
            rightTarget.position = rightAnker.position;
            rightTarget.rotation = rightAnker.rotation;
		}
        if (leftAnker != null && leftTarget != null)
        {
            leftTarget.position = currentLeftPos;
            leftTarget.rotation = currentLeftRot;
		}

	}
}

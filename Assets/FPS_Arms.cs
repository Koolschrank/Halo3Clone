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


	Transform rightAnker;
    Transform leftAnker;
    Transform reloadAnker;
    public GranadeAnker granadeAnker; 

    public SkinnedMeshRenderer armsMeshRenderer;

    public void SetUp(PlayerAnimation animation)
    {
        armsMeshRenderer.enabled = true;
		animation.AddMesh(armsMeshRenderer);
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

            Vector3 currentLeftPos = Vector3.Lerp(leftAnker.position, reloadAnker.position, reloadProgress);
        Quaternion currentLeftRot = Quaternion.Slerp(leftAnker.rotation, reloadAnker.rotation, reloadProgress);

        currentLeftPos = Vector3.Lerp(currentLeftPos, granadeAnker.anker.position, granadeProgress);
        currentLeftRot = Quaternion.Slerp(currentLeftRot, granadeAnker.anker.rotation, granadeProgress);


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

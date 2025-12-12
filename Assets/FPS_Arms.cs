using UnityEngine;

public class FPS_Arms : MonoBehaviour
{
    public Transform rightTarget;
    public Transform leftTarget;

    Transform rightAnker;
    Transform leftAnker;

    public void SetRightAnker(Transform rightAnker)
        {
        this.rightAnker = rightAnker;
	}

    public void SetLeftAnker(Transform leftAnker)
    {
        this.leftAnker = leftAnker;
	}


	private void LateUpdate()
	{
		// copy position and rotation from ankerts to targets
        if (rightAnker != null && rightTarget != null)
        {
            rightTarget.position = rightAnker.position;
            rightTarget.rotation = rightAnker.rotation;
		}
        if (leftAnker != null && leftTarget != null)
        {
            leftTarget.position = leftAnker.position;
            leftTarget.rotation = leftAnker.rotation;
		}

	}
}

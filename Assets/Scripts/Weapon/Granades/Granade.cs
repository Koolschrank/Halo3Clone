using System.Collections.Generic;
using UnityEngine;

public class Granade : MonoBehaviour
{


    List<Transform> granadeCopys = new List<Transform>();
    [SerializeField] float  copyTransitionSpeed = 10f;

    Vector3 lastPosition;
    GameObject owner;

    // set bullet copy
    public void AddGranadeCopy(Transform granadeCopy)
    {
        granadeCopys.Add(granadeCopy);
    }

    // start
    void Start()
    {
        lastPosition = transform.position;
    }


    void Update()
    {
        var currentPos = transform.position;
        var direction = transform.position - lastPosition;

        if (granadeCopys != null)
        {
            foreach (var granade in granadeCopys)
            {
                granade.position += direction;
                granade.position = Vector3.MoveTowards(granade.position, currentPos, copyTransitionSpeed * Time.deltaTime);
                granade.transform.rotation = transform.rotation;
            }
        }

        lastPosition = currentPos;
    }

    private void OnDestroy()
    {
        foreach (var granade in granadeCopys)
        {
            if (granade != null)
                Destroy(granade.gameObject);
        }
        granadeCopys.Clear();
    }

    // set owner
    public void SetOwner(GameObject owner)
    {
        this.owner = owner;
    }

    public GameObject GetOwner()
    {
        return owner;
    }
}

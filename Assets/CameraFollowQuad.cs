using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowQuad : MonoBehaviour {
    public Transform follow_target;
    public Transform camera_transform;
    private Vector3 offset;

	// Use this for initialization
	void Start () {
        offset = camera_transform.position - follow_target.position;
	}
	
	// Update is called once per frame
	void Update () {
        camera_transform.transform.position = follow_target.position + offset;	
	}
}

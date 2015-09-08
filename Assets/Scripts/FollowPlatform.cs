using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FollowPlatform : MonoBehaviour {

    public enum PlatformStatus
    {
        MoveTowards,
        Lerp
    }

    public RunningPlatform _platform;

    public int speed = 1;
    public float forward = .2f;
    public PlatformStatus platformstatus = PlatformStatus.MoveTowards;

    IEnumerator<Transform> _currentPoint;

	void Start () 
    {

        if (_platform == null)
        {
            Debug.LogError("Platform is null", gameObject);
            return;
        }

        _currentPoint = _platform.GetPlatformEnumerator();
        _currentPoint.MoveNext();

        if (_currentPoint.Current == null)
            return;

        transform.position = _currentPoint.Current.position;	
	}
	
	
	void Update () 
    {
        if (platformstatus == PlatformStatus.MoveTowards)
            transform.position = Vector3.MoveTowards(transform.position, _currentPoint.Current.position, Time.deltaTime * speed);
        if (platformstatus == PlatformStatus.Lerp)
            transform.position = Vector3.Lerp(transform.position, _currentPoint.Current.position, Time.deltaTime * speed);

        var distanceSquard = (transform.position - _currentPoint.Current.position).sqrMagnitude;
        if (distanceSquard < forward * forward)
            _currentPoint.MoveNext();
	}
}

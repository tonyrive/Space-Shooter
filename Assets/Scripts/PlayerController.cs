using UnityEngine;
using System.Collections;

[System.Serializable]
public class Boundary
{
	public float xMin, xMax, zMin, zMax;
}

public class PlayerController : MonoBehaviour 
{
	public float speed;
	public Boundary boundary;
	public float tilt;

	public GameObject shot;
	public Transform shotSpawn;

	public float fireRate;
	private float nextFire;

	public float dampingRadius;
	public float velocityLag;
	private GameController gameController;
	
	void Start () 
	{
		GameObject gameControllerObject = GameObject.FindWithTag("GameController");
		if(gameControllerObject != null)
			gameController = gameControllerObject.GetComponent<GameController>();
		
		if(gameController == null)
			Debug.Log ("Cannot find 'GameController' script");
	}

	void Update()
	{
		if(Input.GetButton("Fire1") && Time.time > nextFire && !gameController.isPause)
		{
			nextFire = Time.time + fireRate;
			//GameObject clone = (GameObject)Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
			Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
			GetComponent<AudioSource>().Play();
		}
	}

	void FixedUpdate()
	{
		Vector3? touchPos = null;
		Vector3 target;

		if(Input.mousePresent && Input.GetMouseButton(0))
		{
			touchPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
		}
		else if(Input.touchCount > 0)
		{
			touchPos = new Vector3(Input.touches[0].position.x, Input.touches[0].position.y, 0.0f);
		}

		target = new Vector3();

		if(touchPos != null)
		{
			target = Camera.main.ScreenToWorldPoint(touchPos.Value);
			target.y = GetComponent<Rigidbody>().position.y;
		}

		Vector3 offset = target - GetComponent<Rigidbody>().position;

		float magnitude = offset.magnitude;
		if(magnitude > dampingRadius)
			magnitude = dampingRadius;

		float dampening = magnitude / dampingRadius;

		Vector3 desiredVelocity = offset.normalized * speed * dampening;
		GetComponent<Rigidbody>().velocity += (desiredVelocity - GetComponent<Rigidbody>().velocity) * velocityLag;

		GetComponent<Rigidbody>().position = new Vector3
		(
			Mathf.Clamp(GetComponent<Rigidbody>().position.x, boundary.xMin, boundary.xMax),
			0.0f, 
			Mathf.Clamp(GetComponent<Rigidbody>().position.z, boundary.zMin, boundary.zMax)
		);
		
		GetComponent<Rigidbody>().rotation = Quaternion.Euler(0.0f, 0.0f, GetComponent<Rigidbody>().velocity.x * -tilt);
	}

	/*
	void FixedUpdate()
	{
		float moveHoz = Input.GetAxis("Horizontal");
		float moveVer = Input.GetAxis("Vertical");

		Vector3 movement = new Vector3(moveHoz, 0.0f, moveVer);
		rigidbody.velocity = movement * speed;

		rigidbody.position = new Vector3
		(
			Mathf.Clamp(rigidbody.position.x, boundary.xMin, boundary.xMax),
			0.0f, 
			Mathf.Clamp(rigidbody.position.z, boundary.zMin, boundary.zMax)
		);

		rigidbody.rotation = Quaternion.Euler(0.0f, 0.0f, rigidbody.velocity.x * -tilt);
	}
	*/
}


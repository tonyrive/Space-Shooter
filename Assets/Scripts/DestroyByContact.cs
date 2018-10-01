using UnityEngine;
using System.Collections;

public class DestroyByContact : MonoBehaviour 
{

	public GameObject explosion;
	public GameObject playerExplosion;
	public int scoreValue;
	private GameController gameController;

	void OnTriggerEnter(Collider other) 
	{
		//Debug.Log(other.name);
		if(other.tag == "Boundary") return;

		Instantiate(explosion, transform.position, transform.rotation);

		if(other.tag == "Player")
		{
			Instantiate(playerExplosion, transform.position, transform.rotation);
			gameController.GameOver();
		}
		else
        {
			gameController.AddScore(scoreValue);
		}

		Destroy(other.gameObject);
		Destroy(gameObject);

	}

	void Start () 
	{
		GameObject gameControllerObject = GameObject.FindWithTag("GameController");
		if(gameControllerObject != null)
			gameController = gameControllerObject.GetComponent<GameController>();

		if(gameController == null)
			Debug.Log ("Cannot find 'GameController' script");
	}
}

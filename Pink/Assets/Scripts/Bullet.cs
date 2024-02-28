using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Bullet : MonoBehaviour
{
    // Bullet Movement
    //private CharacterController controller;
    private Vector3 direction;
    [SerializeField] private float speed = 20f;
    [SerializeField] private float range = 100f;
    private float distance = 0f;
    private GameObject owner = null;

    // Explosion
    [SerializeField] private GameObject explosion;
    [SerializeField] private float outerRadius = 10f;

    //[SerializeField] private LayerMask groundLayers;
    //private Rigidbody rigidbody;

    private void Awake()
    {
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public void Shoot(Vector3 _direction, GameObject _owner)
    {
        owner = _owner;
        direction = _direction;
    }

    // Update is called once per frame
    void Update()
    {
        // Move
        this.transform.Translate(direction * speed * Time.deltaTime);
        distance += speed * Time.deltaTime;

        if(distance > range)
        {
            DestroyImmediate(this.gameObject);
        }

    }

    /// <summary>
    /// On Collision, explode
    /// </summary>
    /// <param name="collision">Colision data</param>
    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Bullet has collided with " + collision.gameObject);
        GameObject explosionObj = Instantiate(explosion, this.transform.position, Quaternion.identity);
        explosionObj.transform.localScale = new Vector3(outerRadius, outerRadius, outerRadius);
        explosionObj.GetComponent<Explosion>().outerRadius = outerRadius;
        explosionObj.GetComponent<Explosion>().owner = this.owner;
        Destroy(this.gameObject);
    }

}

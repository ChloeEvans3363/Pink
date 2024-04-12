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
    public bool isLobShot = false;
    [SerializeField] private Vector3 gravity = new Vector3(0.0f, -9.8f * 2.0f * 2, 0.0f);
    private Vector3 downwardVelocity = new Vector3(0.0f, 0.0f, 0.0f);
    [SerializeField] private float speed = 35f;
    [SerializeField] private float range = 100f;
    private float distance = 0f;
    private GameObject owner = null;
    private float lifeTime = 0.0f;
    [SerializeField] private float maxLifeTime = 10.0f;
    [SerializeField] private GameObject bulletBody;


    // Explosion
    [SerializeField] private GameObject explosion;
    private float outerRadius = 10f;

    //[SerializeField] private LayerMask groundLayers;
    //private Rigidbody rigidbody;

    public float OuterRadius
    {
        get { return outerRadius; }
        set {  outerRadius = value; }
    }

    public GameObject Explosion
    {
        get { return explosion; }
        set { explosion = value; }
    }

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
        lifeTime += Time.deltaTime;
        this.transform.Translate(direction * speed * Time.deltaTime);
        if(isLobShot)
        {
            downwardVelocity += gravity * Time.deltaTime;
            this.transform.Translate(downwardVelocity * Time.deltaTime);
            //distance += Mathf.Abs(downwardVelocity.y * Time.deltaTime);
        }
        distance += speed * Time.deltaTime;

        // Rotate the bullet body to face the direction it's moving in
        if (direction != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(-direction);
            bulletBody.transform.rotation = rotation;
        }

        if (distance > range || lifeTime > maxLifeTime)
        {
            DestroyImmediate(gameObject);
        }

    }

    /// <summary>
    /// On Collision, explode
    /// </summary>
    /// <param name="collision">Colision data</param>
    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.tag == "AudioListener") { return; }
        //Debug.Log("Bullet has collided with " + collision.gameObject);
        if ((collision.gameObject.tag == "Bullet" && collision.gameObject.GetComponent<Bullet>().owner == owner) || (
            collision.gameObject.transform.parent != null && collision.gameObject.transform.parent.gameObject == owner))
            return;

        GameObject explosionObj = Instantiate(explosion, this.transform.position, Quaternion.identity);
        explosionObj.transform.localScale = new Vector3(outerRadius, outerRadius, outerRadius);
        explosionObj.GetComponent<Explosion>().outerRadius = outerRadius;
        explosionObj.GetComponent<Explosion>().owner = this.owner;
        Destroy(this.gameObject);
    }

}

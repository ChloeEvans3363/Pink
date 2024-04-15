using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float lifeTime = 0.5f;
    public GameObject owner = null;

    // Explosion
    [SerializeField] private GameObject explosion;
    public float outerRadius = 10f;

    // Rocket Jump
    public float explosionForce = 40f;

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
        transform.localScale = new Vector3(outerRadius, outerRadius, outerRadius);
        MoveAudioListener.Instance.MoveListener(this.transform);
    }

    // Update is called once per frame
    void Update()
    {
        lifeTime -= Time.deltaTime;
        if(lifeTime <= 0f ) { Destroy(this.gameObject); }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") { return; }
        float dist = Vector3.Distance(other.transform.position, this.transform.position);
        //Debug.Log("Explosion has triggered with " + other.gameObject + " " + dist + " ft away");
        if(dist <= outerRadius/2)
        {
            other.gameObject.GetComponent<Player>().TakeDamage(2, this.gameObject);
            //other.gameObject.GetComponent<Player>().AddExplosionForce(transform.position, outerRadius, explosionForce);
        }
        else
        {
            other.gameObject.GetComponent<Player>().TakeDamage(1, this.gameObject);
        }
    }

}

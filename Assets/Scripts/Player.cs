using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
    }

    void CalculateMovement()
    {
        #region User Input

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInout = Input.GetAxis("Vertical");

        //transform.Translate(Vector3.right * horizontalInput *  _speed * Time.deltaTime);
        //transform.Translate(Vector3.up * verticalInout *  _speed * Time.deltaTime);

        //More optimal -> less "new" keyword usage
        Vector3 direction = new Vector3(horizontalInput, verticalInout, 0);
        transform.Translate(direction * _speed * Time.deltaTime);

        #endregion

        #region Player Bounds

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);

        if (transform.position.x > 11)
        {
            transform.position = new Vector3(-11, transform.position.y, 0);
        }
        else if (transform.position.x < -11)
        {
            transform.position = new Vector3(11, transform.position.y, 0);
        }

        #endregion
    }
}

using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField]
    private float parallaxSpeedX, parallaxSpeedY;

    private Transform cameraTrans;
    private float startPositionX, startPositionY;
    private float spriteSizeX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraTrans = Camera.main.transform;
        startPositionX = transform.position.x;
        startPositionY = transform.position.y;
        spriteSizeX = GetComponent<SpriteRenderer>().bounds.size.x;
        //Debug.Log(startPositionX);
        //Debug.Log(startPositionY);
    }

    void LateUpdate()
    {
        float relativeDistX = cameraTrans.position.x * parallaxSpeedX;
        float relativeDistY = (cameraTrans.position.y - startPositionY) * parallaxSpeedY ;
       // Debug.Log(relativeDistY);
        transform.position = new Vector3(startPositionX + relativeDistX, startPositionY + relativeDistY, transform.position.z);

        //loop
        float relativeCameraDist = cameraTrans.position.x * (1 - parallaxSpeedX);
        if(relativeCameraDist > startPositionX + spriteSizeX)
        {
            startPositionX += spriteSizeX;
        }
        else if(relativeCameraDist < startPositionX - spriteSizeX)
        {
            startPositionX -= spriteSizeX;
        }
    }
}

using UnityEngine;

public class AnimatedSprite : MonoBehaviour
{
    //can be done with unity built-in features but its easy to implement

    public Sprite[] sprites;
    public float framerate = 1f/6f; //default can be changed later

    private SpriteRenderer spriteRenderer;
    
    private int frame;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        InvokeRepeating(nameof(Animate), framerate, framerate);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }



    public void OnTheEnd(){
        InvokeRepeating(nameof(Animate), framerate, framerate);
    }

    private void Animate()
    {
        frame++;

        if(frame >= sprites.Length){
            frame = 0;
        }
        if(frame >= 0 && frame < sprites.Length){
        spriteRenderer.sprite = sprites[frame];
        }
    }
    //unity animation is not bad but too much commplex for this project
}

using UnityEngine;

public class EntityMovement : MonoBehaviour
{
    public float speed = 1f;
    public Vector2 direction = Vector2.left; //these two are public because it can be edited in editor for each entity(goomba,mushrooms etc.)
  private new Rigidbody2D rigidbody;
  private Vector2 velocity;


  private void Awake()
  {

    rigidbody = GetComponent<Rigidbody2D>();
    enabled = false;

  }

  private void OnBecameVisible()
  {
    enabled = true;
  }
  private void OnBecameInvisible()
  {
    enabled = false;
  }

  private void OnEnable()
  {
    rigidbody.WakeUp();
  }
  private void OnDisable()
  {
    rigidbody.linearVelocity = Vector2.zero; 
    rigidbody.Sleep();
  }

  private void FixedUpdate()
  {
    velocity.x = direction.x * speed;
    velocity.y += Physics2D.gravity.y * Time.fixedDeltaTime;
    // unity's gravity we can declare a public gravity var for change gravity for each entity if we want

    rigidbody.MovePosition(rigidbody.position + velocity * Time.fixedDeltaTime);

    if(rigidbody.Raycast(direction)){
        direction = -direction;
    }

    if(rigidbody.Raycast(Vector2.down)){
        velocity.y = Mathf.Max(velocity.y, 0f);
    }


  }



}



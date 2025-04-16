using UnityEngine;

public class DeathBarrier : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player")){
            other.gameObject.SetActive(false);
            GameManager.Instance.Resetlevel(3f);
            //death animation??
        }else{
            Destroy(other.gameObject);
        }
    }
}

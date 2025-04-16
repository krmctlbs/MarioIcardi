using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class FlagPole : MonoBehaviour
{

    public Transform flag;
    public Transform poleBottom;
    public Transform castle;
    public float speed = 6f; //not time based for smoother animation to castle (wherever it is)

    public int nextWorld = 1;
    public int nexStage = 1;

    


    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player")){
            StartCoroutine(MoveTo(flag, poleBottom.position));
            StartCoroutine(LevelCompleteSequence(other.transform));

        }
    }

    private IEnumerator LevelCompleteSequence(Transform player)
    {
        player.GetComponent<PlayerMovement>().enabled = false;
        
        //when triggered if mario(small/big)'s animated sprite script is disabled(there is no input)
        //briefly animation keep running is there is an input s - if player jumps before the trigger and 
        //atm of the triggering theres no input so mario will keep sliding without animation 
     

        yield return(MoveTo(player, poleBottom.position));
        yield return(MoveTo(player, player.position + Vector3.right));
        yield return(MoveTo(player, player.position + Vector3.right + Vector3.down));
        yield return(MoveTo(player, castle.position));

        player.gameObject.SetActive(false);


        yield return new WaitForSeconds(2f);

        GameManager.Instance.LoadLevel(nextWorld,nexStage); // it can be edited in the editor


    }
    
    private IEnumerator MoveTo(Transform subject, Vector3 destination)
    {
        while(Vector3.Distance(subject.position, destination) > 0.125f){
            subject.position = Vector3.MoveTowards(subject.position, destination, speed * Time.deltaTime);

            yield return null;
        }

        subject.position = destination;
        
    }
}

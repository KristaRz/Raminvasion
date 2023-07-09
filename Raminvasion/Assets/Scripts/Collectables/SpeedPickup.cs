
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SpeedPickup : MonoBehaviour
{
    //[SerializeField] private PlayerTag _ForThisPlayer;
    [SerializeField] private float _SpeedAmount = 3;

    [SerializeField] public bool triggerd=false;
    private float lerpSpeed=15;
    [SerializeField] private CollectableType _CollectableType;

    public GameObject player;

    private Vector3 initalPosition;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CollectablesHandler.Instance.ChangeSpeed(_SpeedAmount, _CollectableType);
            

            player=other.gameObject;

            triggerd=true;
            
            Animator foodAnim=gameObject.GetComponent<Animator>();
            foodAnim.Play("Collected");
        }
    }


    public void DestroyObj(){
        Destroy(gameObject);
    }

    private void Update() {
        if(triggerd && gameObject!=null){
            //not very performant to do this every update, but idk how else :(
            initalPosition=gameObject.transform.position;
            float playerHeight=player.GetComponent<CharacterController>().height;
            Vector3 playerHeadPos=player.transform.position+new Vector3(0,playerHeight,0);
            

            transform.position = Vector3.Lerp(initalPosition, playerHeadPos , Time.deltaTime * lerpSpeed);
        }
    }

}

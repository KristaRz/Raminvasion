
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SpeedPickup : MonoBehaviour
{
    //[SerializeField] private PlayerTag _ForThisPlayer;
    [SerializeField] private float _SpeedAmount = 3;

    [SerializeField] public bool triggerd=false;
    [SerializeField] private float lerpSpeed=30;
    [SerializeField] private CollectableType _CollectableType;

    private GameObject player;

    private Vector3 initalPosition;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CollectablesHandler.Instance.ChangeSpeed(_SpeedAmount, _CollectableType);
            // Destroy(gameObject);
            player=other.gameObject;

            initalPosition=gameObject.transform.position;

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
            transform.position = Vector3.Lerp(initalPosition, player.transform.position, Time.deltaTime * lerpSpeed);
        }
    }

}

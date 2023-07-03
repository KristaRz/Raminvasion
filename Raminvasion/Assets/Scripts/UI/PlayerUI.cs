// Created 02.07.2023 by Krista Plagemann //
// Contains some references and scales images when you collect collectables.//

using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PlayerUI : MonoBehaviour
{
    public Image EnemyStateImage;
    public Image OtherPlayerStateImage;
    public TextMeshProUGUI DistanceText;
    public TextMeshProUGUI DistanceOtherPlayer;

    [SerializeField] private GameObject _CollectablePlus;
    [SerializeField] private GameObject _CollectableImage;

    [SerializeField] private Sprite _OnionCollectable;

    private bool scaling = false;

    private void Start()
    {
        CollectablesHandler.Instance.OnCollected += CollectedType;
    }

    public void CollectedType(CollectableType type)
    {
        if(type == CollectableType.Onion)
        {
            GameObject toScalePlus = _CollectablePlus;
            GameObject toScaleImage = _CollectableImage;

            if (scaling) // making copies if we are already scaling one 
            {
                toScalePlus = Instantiate(_CollectablePlus, transform.position, Quaternion.identity);
                toScalePlus.name = "PlusCopy";
                toScaleImage = Instantiate(_CollectableImage, transform.position, Quaternion.identity);
                toScaleImage.name = "ImageCopy";

                ScaleThis(toScalePlus, true);
                ScaleThis(toScaleImage, true);
                scaling = true;
            }
            else
            {
                ScaleThis(toScalePlus, false);
                ScaleThis(toScaleImage, false);
            }

            toScaleImage.GetComponent<Image>().overrideSprite = _OnionCollectable;
        }
    }

    public void ScaleThis(GameObject toScaleObject, bool destroyAfter)
    {

        LeanTween.scale(toScaleObject, Vector3.one, 1f).setOnComplete(() =>
        {
            LeanTween.scale(toScaleObject, Vector3.zero, 0.5f).setOnComplete(() =>
            {
                if (destroyAfter)    // getting rid of the copy when done
                    Destroy(toScaleObject);
                scaling = false;
            });
        });
    }
}

using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{

    public int spriteIndex;
    public Sprite cardFrontGraphics;
    public Sprite cardBackGraphics;
    public CardData cardDatas;
    public bool isHidden;
    private Image image;

    bool isFlipped;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }
    public void CardFlip()
    {
        //Debug.Log("cardFlip Triggered");
        this.transform.DORotate(new Vector3(0, 90, 0), 0.3f, RotateMode.Fast).OnComplete(() =>
        {

            image.sprite = cardFrontGraphics;
            this.transform.DORotate(new Vector3(0, 180, 0), 0.3f, RotateMode.Fast).OnComplete(() =>
            {
                GamePlayManager.instance.OnCardSelect(this);
            });

        });




    }
    public void CardFlipBack()
    {
        //Debug.Log("CardFlipBack Triggered");
        this.transform.DORotate(new Vector3(0, 90, 0), 0.3f, RotateMode.Fast).OnComplete(() =>
        {

            image.sprite = cardBackGraphics;
            this.transform.DORotate(new Vector3(0, 0, 0), 0.3f, RotateMode.Fast);


        });


    }

    public void OnSelect()
    {
        AudioPlayer.Instance.PlayAudio(1);
        CardFlip();


    }
}

using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class GameEngine : MonoBehaviour
{
    private int[] playersHidenCardNumber = new int[4], playersShownCardNumber = new int[4];

    [SerializeField] GameObject card;
    private GameObject spawnedCard;

    [SerializeField] MainMenuUI mainMenuUI;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FireScreenRay();
        }

        if (Input.GetMouseButtonDown(1))
        {
            SpawnStack(0, 50);
        }
    }

    void FireScreenRay()
    {
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(cameraRay, out RaycastHit hitInfo))
        {
            if (hitInfo.collider.gameObject.tag == "Card")
            {
                hitInfo.collider.gameObject.tag = "UsedCard";
                CardMovement card = hitInfo.collider.GetComponent<CardMovement>();
                card.MoveCard();
            }
        }
    }

    public void SetplayersCardNumber(int[] HidenNumber, int[] ShownNumber)
    {
        playersHidenCardNumber = HidenNumber;
        playersShownCardNumber = ShownNumber;
    }

    public void SpawnCard(int id)
    {
        spawnedCard = Instantiate(card);
        Vector3 cardPosition = GameMeneger.instance.playersCardPositions[id];
        spawnedCard.transform.position = new Vector3 (cardPosition.x, cardPosition.y + GameMeneger.instance.playersHiddenCards[id] * 0.01f,
            cardPosition.z);
        GameMeneger.instance.playersHiddenCards[id]++;
    }

    public void SpawnStack(int id, int number)
    {
        for (int i = 0; i < number; i++)
        {
            SpawnCard(id);
        }
    }
}

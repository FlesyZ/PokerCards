using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// all poker cards
    /// </summary>
    public List<GameObject> cards = new List<GameObject>();

    private string[] shapes = { "Spades", "Heart", "Club", "Diamond" };

    private void Start()
    {
        GetCards();
    }

    /// <summary>
    /// 取得所有撲克牌
    /// </summary>
    private void GetCards()
    {
        if (cards.Count >= 52) return;

        for (int x = 0; x < shapes.Length; x++)
        {
            for (int y = 1; y <= 13; y++)
            {
                string num = y.ToString();
                switch (y)
                {
                    case 1:
                        num = "A";
                        break;
                    case 11:
                        num = "J";
                        break;
                    case 12:
                        num = "Q";
                        break;
                    case 13:
                        num = "K";
                        break;
                }

                GameObject card = Resources.Load<GameObject>("PlayingCards_" + num + shapes[x]);
                cards.Add(card);
            }
        }
    }

    /// <summary>
    /// 取得指定花色的卡牌
    /// </summary>
    /// <param name="shape"></param>
    public void PickupByShape(string shape)
    {
        // pick up cards with the assigned shape
        var pickedup = cards.Where((x) => x.name.Contains(shape));

        StartCoroutine(CallCards(pickedup));
    }

    public void PickupByRandomShape()
    {
        PickupByShape(shapes[Random.Range(0, 4)]);
    }

    public void Rearrange()
    {
        var sorted = from card in cards
                     where card.name.Contains(shapes[3]) || card.name.Contains(shapes[2]) || card.name.Contains(shapes[1]) || card.name.Contains(shapes[0])
                     select card;

        StartCoroutine(CallCards(sorted));
    }

    private void Shuffle(List<GameObject> chosen)
    {
        List<GameObject> shuffle = chosen.ToArray().ToList();
        List<GameObject> pile = new List<GameObject>();

        for (int i = 0; i < cards.Count; i++)
        {
            int r = Random.Range(0,shuffle.Count);

            GameObject temp = shuffle[r];
            pile.Add(temp);
            shuffle.RemoveAt(r);
        }

        StartCoroutine(CallCards(pile));
    }

    public void ShuffleField()
    {
    /*  List<GameObject> field = new List<GameObject>();

        if (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
                field.Add(transform.GetChild(i).gameObject);
            Shuffle(field);
        }
        else
    */      Shuffle(cards);
    }

    private IEnumerator CallCards(IEnumerable<GameObject> picked)
    {
        yield return StartCoroutine(TakeoutFieldCards());
        
        // create picked-up cards
        foreach (var item in picked) Instantiate(item, transform);

        StartCoroutine(SetChildPosition());
    }

    private IEnumerator TakeoutFieldCards()
    {
        while (transform.childCount > 0)
        {
            Destroy(transform.GetChild(0).gameObject);
            yield return new WaitForSeconds(0.03f);
        }
    }

    private IEnumerator SetChildPosition()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            yield return new WaitForSeconds(0.07f);

            Transform child = transform.GetChild(i);
            child.eulerAngles = new Vector3(180, 0, 0);
            child.localScale = Vector3.one * 20;

            // posX = (cardPos - 6) * space
            // posX resets on every 13 cards.
            float posX = i % 13;
            // posY = 4 - y * space
            // posY adds on every 13 cards.
            float posY = i / 13;

            child.position = new Vector3((posX - 6) * 1.33f, 4 - posY * 1.86f, 0);

        }
    }
}

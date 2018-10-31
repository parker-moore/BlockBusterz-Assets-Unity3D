using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Class Description: public class Block
 * Controls functions of block gameObject such as: collision detection, spawning coins (aka gems), color of block, Hitpoints of block,
 */

public class Block : MonoBehaviour
{
    public int boxNum;
    public bool timesTwo = false;
    public GameObject gemPrefab;

    private int boxHue;
    private TextMesh tm;

    void Start()
    {
        //all blocks start active
        if (PlayerPrefManager.GetBool(gameObject.name) == false)
        {
            gameObject.SetActive(false);
        }
    }

    // Use this for initialization
    void OnEnable()
    {
        GameManager.OnNewRound += SaveNum;

        tm = GetComponent<TextMesh>();
        GetComponent<BoxCollider2D>().enabled = true;//disable box collider and sprite render so animation can play out
        transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true; 
        GetComponent<MeshRenderer>().enabled = true;

        if (PlayerPrefManager.GetBool(gameObject.name) == true && PlayerPrefManager.GetBool("gameRunning") == true)
        {
            boxNum = PlayerPrefManager.GetBlockNum(gameObject.name);
            boxHue = PlayerPrefManager.GetBlockHue(gameObject.name);

            GetComponent<MeshRenderer>().sortingLayerName = "front";
            tm.text = boxNum.ToString();
            GetComponentInChildren<SpriteRenderer>().color = Color.HSVToRGB((boxHue * 5f) / 359f, 200f / 255f, 235 / 255f);
            transform.position = new Vector3(PlayerPrefManager.GetXPosition(gameObject.name), PlayerPrefManager.GetYPosition(gameObject.name), PlayerPrefManager.GetZPosition(gameObject.name));
            transform.localScale = new Vector3(PlayerPrefManager.GetBlockScaleX(gameObject.name), PlayerPrefManager.GetBlockScaleY(gameObject.name), 1);
        }
        else
        {
            PlayerPrefManager.SetBool(gameObject.name, false);

            if (timesTwo == true)
            {
                int num = GameManager.gm.blockColor * 2;
                if (num < 71)//hue only goes up to 359, must compensate since boxHue is multiplied by 5 for considerable color change with each hit
                    boxHue = num;
                else
                    boxHue = num - 70;

                boxNum = GameManager.gm.gameRound * 2;  // block hit point starts at twice the gameRound number
            }
            else
            {
                boxNum = GameManager.gm.gameRound;
                boxHue = GameManager.gm.blockColor;
            }

            GetComponent<MeshRenderer>().sortingLayerName = "front";
            tm.text = boxNum.ToString();
            GetComponentInChildren<SpriteRenderer>().color = Color.HSVToRGB((5f * boxHue) / 359f, 200f / 255f, 235 / 255f);
        }
    }

    void OnDisable()
    {
        timesTwo = false;
        GameManager.OnNewRound -= SaveNum;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.tag == "Ball")
        {
            if (boxNum > 1)
            {
                boxNum--;
                if (boxHue == 0)//hue only goes up to 359, must compensate
                    boxHue += 70;
                else
                    boxHue--;
                tm.text = boxNum.ToString();
                transform.GetChild(0).GetComponent<Animator>().SetBool("enable", true);
                GetComponentInChildren<SpriteRenderer>().color = Color.HSVToRGB((5f * boxHue) / 359f, 200f / 255f, 235f / 255f);
            }
            else
            {
                GetComponent<BoxCollider2D>().enabled = false;//disable box collider and sprite render so animation can play out
                transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;

                GetComponent<MeshRenderer>().enabled = false;
                if (RandomBool() == true)
                {
                    SpawnGem();
                }
                GetComponent<Animator>().SetBool("Death", true);//activates death animation
            }
        }

        if (other.collider.tag == "Bottom Barrier")//if hit bottom barrier destroy this gameObject
            gameObject.SetActive(false);
    }

    public void DestroyGameObject()
    {
        PlayerPrefManager.SetBool(gameObject.name, false);
        Debug.Log(PlayerPrefManager.GetBool(gameObject.name));
        gameObject.SetActive(false);
    }

    bool RandomBool()// 50% chance of some event to occur
    {
        float num = Random.Range(0, 2);
        if (num == 0)
            return false;
        else
            return true;
    }

    void SpawnGem()
    {
        GameObject gem = Instantiate(gemPrefab, transform);
        if (gem != null)
        {
            gem.transform.position = transform.position;
            gem.transform.parent = null;
        }
        return;
    }

    void SaveNum()
    {
        PlayerPrefManager.SetBlockNum(boxNum, gameObject.name);
    }

    void OnApplicationQuit()
    {
        GameManager.OnNewRound -= SaveNum;

        if (gameObject.activeSelf == true)
        {

            PlayerPrefManager.SetBool(gameObject.name, true);//if name is true OnEnable then must set inital position

            if (GetComponent<MoveDown>().isMoving == true)
                PlayerPrefManager.SetPosition(GetComponent<MoveDown>().newPos.x, GetComponent<MoveDown>().newPos.y, transform.position.z, gameObject.name);
            else
                PlayerPrefManager.SetPosition(transform.position.x,transform.position.y, transform.position.z, gameObject.name);

            PlayerPrefManager.SetBlockNum(boxNum, gameObject.name);
            PlayerPrefManager.SetBlockHue(boxHue, gameObject.name);
            PlayerPrefManager.SetBlockScaleX(transform.localScale.x, gameObject.name);
            PlayerPrefManager.SetBlockScaleY(transform.localScale.y, gameObject.name);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BallsType
{
    yellow,
    blue,
    red,
    green
}
public enum PowerUpType
{
    none,
    timeadd,
    scoreadd,
    deleteTime,
}
public class BallsController : MonoBehaviour
{
    public BallsWithColor myColor;

    public GameObject linePrefab;
    public GameObject ripplePrefab;

    public SpriteRenderer myrenderer;
    public SpriteRenderer PowerUpRenderer;

    public string prevObjName;


    public bool isCalled;
    public bool beingDestroyed;

    public List<LineController> connectedLines = new();
    [SerializeField] private ConnectedObject nextObj;
    public PowerUpType myType = PowerUpType.none;

    void Start()
    {
        BallsWithColor returnedColor = GameManager.instance.GetRandomColor();
        myColor = new BallsWithColor
        {
            ballSprite =returnedColor.ballSprite,
            ballsType = returnedColor.ballsType,
            ballColor = new Color
            {
                r=returnedColor.ballColor.r,
                g=returnedColor.ballColor.g,
                b=returnedColor.ballColor.b,
                a=1,
            }
        };
        myrenderer.sprite = myColor.ballSprite;
        if (Random.Range(0, 100) < GameManager.instance.powerUpSpawnChance)
        {
            PowerUpRenderer.gameObject.SetActive(true);
            PowerUpWithType temp = GameManager.instance.GetRandomPowerUp();
            if (temp.type == PowerUpType.deleteTime)
            {
                temp = GameManager.instance.GetRandomPowerUp();
            }
            if (temp.type == PowerUpType.deleteTime)
            {
                PowerUpRenderer.transform.localScale = Vector3.one;
            }
            PowerUpRenderer.sprite = temp.sprite;

            myType = temp.type;
        }
        else
        {
            PowerUpRenderer.gameObject.SetActive(false);
            myType = PowerUpType.none;
        }
    }

    void Update()
    {
        
    }

    private bool connectedObjectHasMe(BallsController controller)
    {
        if (controller.nextObj == null)
            return false;
        if (controller.nextObj.connectedBall == null)
            return false;
        if (controller.nextObj.connectedBall == this)
            return true;
        BallsController nextcontoller = controller;
        while(nextcontoller != null)
        {
            if (nextcontoller == this)
                return true;
            if (nextcontoller.nextObj == null)
                return false;
            nextcontoller = nextcontoller.nextObj.connectedBall;
        }

        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball"))
        {
            BallsController other = collision.GetComponent<BallsController>();

            if (myType == PowerUpType.deleteTime || other.myType == PowerUpType.deleteTime)
                return;

            if (other == null)
                return;
            if (myColor.ballsType != other.myColor.ballsType)
                return;

            //if (!isObjectInList(collision.GetComponent<BallsController>()) )
            if (nextObj == null && !connectedObjectHasMe(other))
            {
                Vector2 pos = new((transform.position.x + collision.transform.position.x) / 2,
                (transform.position.y + collision.transform.position.y) / 2);
                Vector3 targ = pos;
                targ.z = 0;
                Vector3 objectPos = transform.position;
                targ.x -= objectPos.x;
                targ.y -= objectPos.y;

                float angle = Mathf.Atan2(targ.y, targ.x) * Mathf.Rad2Deg;
                GameObject spawnedLine = Instantiate(linePrefab, pos, Quaternion.Euler(new Vector3(0, 0, angle)));
                spawnedLine.GetComponent<SpriteRenderer>().color = myColor.ballColor;
                LineController spawnLineControl = spawnedLine.GetComponent<LineController>();

                nextObj = new ConnectedObject
                {
                    connectedBall = collision.GetComponent<BallsController>(),
                    spawnedLine = spawnedLine
                };
                if (!connectedLines.Contains(spawnLineControl))
                {
                    if (!spawnLineControl.connectedBalls.Contains(this))
                        spawnLineControl.connectedBalls.Add(this);
                    if (!spawnLineControl.connectedBalls.Contains(other))
                        spawnLineControl.connectedBalls.Add(other);
                    connectedLines.Add(spawnLineControl);

                    if (!other.connectedLines.Contains(spawnLineControl))
                    {
                        other.connectedLines.Add(spawnLineControl);
                    }
                }
                
            }
            else
            {
                LineController temp = other.GetCommonLine(this);
                if (temp != null)
                {

                    if (!connectedLines.Contains(temp))
                    {
                        connectedLines.Add(temp);
                    }
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball"))
        {
            BallsController other = collision.GetComponent<BallsController>();
            if (myType == PowerUpType.deleteTime || other.myType == PowerUpType.deleteTime)
                return;

            if (myColor.ballsType != other.myColor.ballsType)
                return;

            //if (isObjectInList(collision.GetComponent<BallsController>()))
            if (nextObj == null &&!connectedObjectHasMe(other))
            {
                Vector2 pos = new((transform.position.x + collision.transform.position.x) / 2,
                (transform.position.y + collision.transform.position.y) / 2);
                Vector3 targ = pos;
                targ.z = 0;
                Vector3 objectPos = transform.position;
                targ.x -= objectPos.x;
                targ.y -= objectPos.y;

                //Off by 90 degrees for some reasons
                float angle = Mathf.Atan2(targ.y, targ.x) * Mathf.Rad2Deg;
                GameObject spawnedLine = Instantiate(linePrefab, pos, Quaternion.Euler(new Vector3(0, 0, angle)));
                spawnedLine.GetComponent<SpriteRenderer>().color = myColor.ballColor;
                LineController spawnLineControl = spawnedLine.GetComponent<LineController>();
                nextObj = new ConnectedObject
                {
                    connectedBall = collision.GetComponent<BallsController>(),
                    spawnedLine = spawnedLine
                };
                if (connectedLines == null || !connectedLines.Contains(spawnLineControl))
                {
                    if (!spawnLineControl.connectedBalls.Contains(this))
                        spawnLineControl.connectedBalls.Add(this);
                    if (!spawnLineControl.connectedBalls.Contains(other))
                        spawnLineControl.connectedBalls.Add(other);

                    connectedLines.Add(spawnLineControl);
                    if (!other.connectedLines.Contains(spawnLineControl))
                    {
                        other.connectedLines.Add(spawnLineControl);
                    }
                }
            }
            else if (nextObj != null)
            {
                //SetLinePos(collision.GetComponent<BallsController>());
                Vector2 pos = new((transform.position.x + nextObj.connectedBall.transform.position.x) / 2,
                (transform.position.y + nextObj.connectedBall.transform.position.y) / 2);
                Vector3 targ = pos;
                targ.z = 0;
                Vector3 objectPos = transform.position;
                targ.x -= objectPos.x;
                targ.y -= objectPos.y;
                float angle = Mathf.Atan2(targ.y, targ.x) * Mathf.Rad2Deg;
                nextObj.spawnedLine.transform.SetPositionAndRotation(pos, Quaternion.Euler(new Vector3(0, 0, angle)));

            }
            LineController temp = other.GetCommonLine(this);
            if (temp != null)
            {
                if (connectedLines==null || !connectedLines.Contains(temp.GetComponent<LineController>()))
                {
                    connectedLines.Add(temp.GetComponent<LineController>());
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball"))
        {
            if (beingDestroyed)
                return;
            BallsController other = collision.GetComponent<BallsController>();
            if (myType == PowerUpType.deleteTime || other.myType == PowerUpType.deleteTime)
                return;

            if (myColor.ballsType != collision.GetComponent<BallsController>().myColor.ballsType)
                return;
            //if (isObjectInList(collision.GetComponent<BallsController>()))
            LineController temp = GetCommonLine(other);
            if (temp != null)
            {
                connectedLines.Remove(temp);
            }
            LineController othertemp = other.GetCommonLine(this);
            if (othertemp != null)
            {

                if (other.connectedLines.Contains(othertemp))
                {
                    other.connectedLines.Remove(othertemp);
                }
            }
            if (nextObj != null)
            {
                LineController spawnLineControl = nextObj.spawnedLine.GetComponent<LineController>();
                //DestroyObjectInList(collision.GetComponent<BallsController>());
                
                Destroy(nextObj.spawnedLine);
                nextObj=null;
            }
            
        }
    }

    LineController GetCommonLine(BallsController other)
    {
        foreach (LineController item in connectedLines)
        {
            if (item.connectedBalls.Contains(other))
                return item;
        }
        return null;
    }

    public bool hasConnectedLine()
    {
        if (connectedLines == null)
            return false;
        foreach (LineController item in connectedLines)
        {
            if (item)
                return true;
        }
        return false;
    }
    public void DestroyOnlyMe()
    {
        
        beingDestroyed = true;
        foreach (LineController item in connectedLines)
        {
            if (item)
            {
                if (!item.isCalled)
                {
                    item.isCalled = true;
                    item.DestroyBalls(this);
                }
                Destroy(item.gameObject);

            }
        }
        Instantiate(ripplePrefab, transform.position, Quaternion.identity);
        GameManager.instance.AddScore(1);
        switch (myType)
        {
            case PowerUpType.timeadd:
                GameManager.instance.AddTime(5);
                UIManager.instance.ShowCombo(transform.position, "Time \n+5");
                SoundManager.instance.PlaySound(SoundType.powerUps);
                break;
            case PowerUpType.scoreadd:
                GameManager.instance.AddScore(100);
                UIManager.instance.ShowCombo(transform.position, "Score \n+100");
                SoundManager.instance.PlaySound(SoundType.powerUps);
                break;
            case PowerUpType.deleteTime:
                GameManager.instance.ReduceTime(3);
                UIManager.instance.ShowCombo(transform.position, "Time \n-3");
                SoundManager.instance.PlaySound(SoundType.powerUps);
                break;
            case PowerUpType.none:
                break;
            default:
                break;
        }
        Destroy(gameObject);

    }

}

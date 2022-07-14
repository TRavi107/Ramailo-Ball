using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    public List<BallsController> connectedBalls = new(2);
    public bool isCalled;

    public void DestroyBalls(BallsController sender)
    {
        foreach (BallsController item in connectedBalls)
        {
            if (item)
            {

                if (item != sender)
                {
                    if (!item.isCalled)
                    {
                        item.DestroyOnlyMe();
                    }
                }
                item.isCalled = true;
            }
        }
    }
}

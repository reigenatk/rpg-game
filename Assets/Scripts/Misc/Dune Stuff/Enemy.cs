using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DuneStuff
{
    public class Enemy : MonoBehaviour
    {

        Player player;
        Vector2 targetPos;
        LayerMask obstacleMask, walkableMask;
        List<Vector2> availableMovement = new List<Vector2>();
        bool isMoving;
        public float enemySpeed, aggroRange, chaseSpeed, attackCooldown;
        public Vector2 enemyMoveTime;
        List<Node> nodesList = new List<Node>();
        Vector2 hitSize = Vector2.one * 0.8f;
        DungeonManager d;

        void Start()
        {
            obstacleMask = LayerMask.GetMask("Wall", "Enemy", "Player");
            walkableMask = LayerMask.GetMask("Wall", "Enemy");
            targetPos = transform.position;
            player = FindObjectOfType<Player>();
            d = FindObjectOfType<DungeonManager>();

            // create a custom "update()" function
            StartCoroutine(Movement());
        }

        IEnumerator Movement()
        {
            while (true)
            {
                // executes every 0.1 sec
                float randomNum = Random.Range(enemyMoveTime.x, enemyMoveTime.y);
                yield return new WaitForSeconds(randomNum);
                //if (!isMoving)
                //{
                //    float dist = Vector2.Distance(transform.position, player.transform.position);
                //    if (dist <= aggroRange)
                //    {
                //        attack range
                //        if (dist <= 1.1f)
                //        {
                //            Attack();
                //            yield return new WaitForSeconds(attackCooldown);
                //        }
                //        else
                //        {
                //            move to attack
                //           Vector2 newPos = FindNextStep(transform.position, player.transform.position);
                //            Debug.Log(newPos);
                //            Debug.Log(targetPos);
                //            if (newPos != targetPos)
                //            {
                //                targetPos = newPos;
                //                StartCoroutine(EnemyMove(chaseSpeed));
                //            }
                //            else
                //            {
                //                Patrol();
                //            }
                //        }
                //    }
                //    else
                //    {
                //        move around randomly
                //        Patrol();
                //    }

                // }
                Patrol();
            }
        }

        // Update is called once per frame
        void Update()
        {
        }

        bool isInBounds(Vector2 v)
        {
            if (v.x < -d.inBounds || v.x > d.inBounds || v.y < -d.inBounds || v.y > d.inBounds)
            {
                return false;
            }
            return true;
        }

        void Patrol()
        {
            availableMovement.Clear();
            Collider2D hitUp = Physics2D.OverlapBox(targetPos + Vector2.up, hitSize, 0, obstacleMask);
            if (!hitUp && isInBounds(targetPos + Vector2.up))
            {
                availableMovement.Add(targetPos + Vector2.up);
            }
            Collider2D hitDown = Physics2D.OverlapBox(targetPos + Vector2.down, hitSize, 0, obstacleMask);
            if (!hitDown && isInBounds(targetPos + Vector2.down))
            {
                availableMovement.Add(targetPos + Vector2.down);
            }
            Collider2D hitLeft = Physics2D.OverlapBox(targetPos + Vector2.left, hitSize, 0, obstacleMask);
            if (!hitLeft && isInBounds(targetPos + Vector2.left))
            {
                availableMovement.Add(targetPos + Vector2.left);
            }
            Collider2D hitRight = Physics2D.OverlapBox(targetPos + Vector2.right, hitSize, 0, obstacleMask);
            if (!hitRight && isInBounds(targetPos + Vector2.right))
            {
                availableMovement.Add(targetPos + Vector2.right);
            }

            if (availableMovement.Count > 0)
            {
                int randomNum = Random.Range(0, availableMovement.Count);
                Vector2 newTargetPos = availableMovement[randomNum];

                if (newTargetPos == targetPos + Vector2.left)
                {
                    transform.localScale = new Vector2(-1, transform.localScale.y);
                }
                else if (newTargetPos == targetPos + Vector2.right)
                {
                    transform.localScale = new Vector2(1, transform.localScale.y);
                }
                targetPos = availableMovement[randomNum];
            }

            StartCoroutine(EnemyMove(Random.Range(enemyMoveTime.x, enemyMoveTime.y)));
        }

        IEnumerator EnemyMove(float waitSpeed)
        {
            isMoving = true;


            while (Vector2.Distance(transform.position, targetPos) > 0.01f)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPos, enemySpeed * Time.deltaTime);


                yield return null;
            }

            transform.position = targetPos;

            yield return new WaitForSeconds(waitSpeed);
            isMoving = false;
        }

        void Attack()
        {

        }

        void checkNode(Vector2 chkPoint, Vector2 parent)
        {
            Collider2D hit = Physics2D.OverlapBox(chkPoint, hitSize, 0, walkableMask);
            if (!hit)
            {
                nodesList.Add(new Node(chkPoint, parent));
            }
        }

        Vector2 FindNextStep(Vector2 startPos, Vector2 endPos)
        {
            int listIndex = 0;
            Vector2 myPos = startPos;
            nodesList.Clear();
            nodesList.Add(new Node(startPos, startPos));

            while (myPos != endPos && nodesList.Count > 0 && listIndex < 1000)
            {
                checkNode(myPos + Vector2.up, myPos);
                checkNode(myPos + Vector2.down, myPos);
                checkNode(myPos + Vector2.left, myPos);
                checkNode(myPos + Vector2.right, myPos);

                listIndex++;
                if (listIndex < nodesList.Count)
                {
                    myPos = nodesList[listIndex].position;
                }
            }
            if (myPos == endPos)
            {
                nodesList.Reverse();
                for (int i = 0; i < nodesList.Count; i++)
                {
                    if (myPos == nodesList[i].position)
                    {
                        if (nodesList[i].parent == startPos)
                        {
                            return myPos;
                        }
                        else
                        {
                            myPos = nodesList[i].parent;
                        }
                    }
                }
            }

            return startPos;
        }


    }
}
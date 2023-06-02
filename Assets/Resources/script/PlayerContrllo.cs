using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerContrllo : MonoBehaviour
{
    public Camera maincamera;
    private bool isMoving = false;
    private bool isRoll = false;
    private bool isMelee = false;
    private float rollcooltime = 0;
    private float meleeAttackcooltime;
    private int meleeAttackindex = 0;
    private int rollindex = 0;
    private Vector2 input;
    private float stateIndex_X;
    private float stateIndex_Y;
    private float tempfloatspeed = 2;
    private Animator animator;
    private Vector3 test;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        meleeAttackAnimationContrllo();
        StateSet(stateIndex_X, stateIndex_Y);
        checkState();
        playmove();
        checkRollcool();
        PlayerRoll();
        chekMeleeAttackcool();
        PlayerMeleeAttack();
        test = transform.position;
        test.z = -7;
        maincamera.transform.position = Vector3.Lerp(maincamera.transform.position, test, tempfloatspeed * 6);
        Debug.Log(Input.mousePosition+"======="+Camera.main.WorldToScreenPoint(transform.position));

    }
    private void meleeAttackAnimationContrllo()
    {
        Vector3 playerOnScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 mousePosition = Input.mousePosition;
        if (mousePosition.x > playerOnScreenPosition.x && mousePosition.y >
            playerOnScreenPosition.y && mousePosition.y*Screen.width > mousePosition.x*Screen.height)
        {
            stateIndex_X = 1;
            stateIndex_Y = 1;
        }
        else if (mousePosition.x < playerOnScreenPosition.x && mousePosition.y >
            playerOnScreenPosition.y && MathF.Abs(mousePosition.y-playerOnScreenPosition.y) > MathF.Abs(mousePosition.x-playerOnScreenPosition.x))
        {
            stateIndex_X = -1;
            stateIndex_Y = 1;
        }
        else if (mousePosition.x > playerOnScreenPosition.x && mousePosition.y >
            playerOnScreenPosition.y && mousePosition.y * Screen.width < mousePosition.x * Screen.height ||
            mousePosition.x > playerOnScreenPosition.x && mousePosition.y <
            playerOnScreenPosition.y && MathF.Abs(mousePosition.y - playerOnScreenPosition.y) < MathF.Abs(mousePosition.x - playerOnScreenPosition.x))
        {
            stateIndex_X = 1;
            stateIndex_Y = 0;
        }
        else if (mousePosition.x < playerOnScreenPosition.x && mousePosition.y >
            playerOnScreenPosition.y && MathF.Abs(mousePosition.y - playerOnScreenPosition.y) * Screen.width < MathF.Abs(mousePosition.x - playerOnScreenPosition.x) * Screen.height ||
            mousePosition.x < playerOnScreenPosition.x && mousePosition.y <
            playerOnScreenPosition.y && mousePosition.y * Screen.width > mousePosition.x * Screen.height)
        {
            stateIndex_X = -1;
            stateIndex_Y = 0;
        }
        else if (mousePosition.x > playerOnScreenPosition.x && mousePosition.y <
            playerOnScreenPosition.y && MathF.Abs(mousePosition.y - playerOnScreenPosition.y) > MathF.Abs(mousePosition.x - playerOnScreenPosition.x))
        {
            stateIndex_X = 1;
            stateIndex_Y = -1;
        }
        else if (mousePosition.x < playerOnScreenPosition.x && mousePosition.y <
            playerOnScreenPosition.y && mousePosition.y * Screen.width < mousePosition.x * Screen.height)
        {
            stateIndex_X = -1;
            stateIndex_Y = -1;
        }
    }
    private void chekMeleeAttackcool()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (meleeAttackindex == 0)
            {
                isMelee = true;
                PlayerManager.instance.meleeAttack();
                meleeAttackcooltime = 0;
                meleeAttackindex++;
            }
            else
            {
                if (meleeAttackcooltime > 0.4f)
                {
                    isMelee = true;
                    PlayerManager.instance.meleeAttack();
                    meleeAttackcooltime = 0;
                }

            }

        }
    }
    private void checkRollcool()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {

            if (rollindex == 0)
            {
                isRoll = true;
                rollcooltime = 0;
                rollindex++;
            }
            else
            {
                if (rollcooltime > 3f)
                {
                    isRoll = true;
                    rollcooltime = 0;
                    
                }
            }
        }
        
    }
    public void roll()
    {
        Vector3 targetposition = transform.position;
        targetposition.x += input.x * 4;
        targetposition.y += input.y * 4;
        if ((transform.position - targetposition).magnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.Lerp(transform.position, targetposition, tempfloatspeed * Time.deltaTime*1f);
            animator.SetBool("isroll", isRoll);
        }

    }
    public void PlayerRoll()
    {
        if (isRoll)
        {
            roll();
            rollcooltime += Time.deltaTime;
        }

        if(rollcooltime > 0.15f&&isRoll)
        {
            isRoll = false;
            animator.SetBool("isroll", isRoll);

        }
        if (!isRoll&&rollcooltime < 3.1f)
        {
            rollcooltime += Time.deltaTime;
        }
    }
    public void PlayerMeleeAttack()
    {
        if (isMelee)
        {
            animator.SetBool("isAttack", isMelee);
            meleeAttackcooltime += Time.deltaTime;
        }
        if (meleeAttackcooltime > 0.15f&&isMelee)
        {
            isMelee= false;
            animator.SetBool("isAttack", isMelee);
        }
        if (meleeAttackcooltime<0.4f&&!isMelee)
        {
            meleeAttackcooltime += Time.deltaTime;
        }
    }
    public void playerHiting(PlayerManager playerManager)
    {

    }
    public void playmove()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");
            //if (input.x != 0)
            //{
            //    input.y = 0;
            //}
            animator.SetBool("ismove", isMoving);
            if (input != Vector2.zero&&!isRoll)
            {
                Vector3 targetposition = transform.position;
                targetposition.x += input.x;
                targetposition.y += input.y;
                if (!isRoll)
                {
                    StartCoroutine(moving(targetposition));
                }

            }
        }
    }
    public void StateSet(float x,float y)
    {
        animator.SetFloat("idlex", x);
        animator.SetFloat("idley", y);
        animator.SetFloat("walkx", x);
        animator.SetFloat("walky", y);
        animator.SetFloat("rollx", x);
        animator.SetFloat("rolly", y);
    }
    public void checkState()
    {
        animator.SetBool("isroll", isRoll);
    }
    IEnumerator moving(Vector3 targetposition)
    {
        if ((transform.position - targetposition).magnitude > Mathf.Epsilon)
        {
            isMoving = true;
            animator.SetBool("ismove", isMoving);
            transform.position = Vector3.MoveTowards(transform.position, targetposition, tempfloatspeed * Time.deltaTime);
            isMoving = false;
            yield return null;
        }
    }
}
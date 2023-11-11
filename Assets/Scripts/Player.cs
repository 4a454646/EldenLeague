using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using Pathfinding;

public class Player : MonoBehaviour, IWalkAnimation {
    public GameObject clickEffectPrefab; 
    private Vector3 targetPosition;
    private WaitForSeconds frameTime = new WaitForSeconds(1f / 60f);
    
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private CustomPathFinder cpf;
    private Animator anim;

    [Header("Roll Settings")]
    [SerializeField] public bool canRoll = true;
    [SerializeField] public bool isRolling = false;
    [SerializeField] public float rollCooldown = 1f;
    [SerializeField] private float s = 1f;
    [SerializeField] private float q = 2f;
    [SerializeField] private Sprite[] rollSprites;
    public int[] spriteIndices;
    public float[] slideSpeeds;

    private void Start() {
        sr = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        anim = transform.GetChild(0).gameObject.GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cpf = GetComponent<CustomPathFinder>();
        cpf.toControl = gameObject;
        spriteIndices = new int[] 
            {0,0,0,1,1,1,2,2,2,3,3,3,4,4,4,4,4,4,5,5,5,5,5,6,6,6,6,6,7,7,7,7,8};
        slideSpeeds = new float[] 
            {s,s,s,s,s,s,s,s,s,q,q,q,q,q,q,q,q,q,q,q,q,q,q,s,s,s,s,s,s,s,s,s,s};
    }

    private void Update() {

        if (!cpf.reachedEndOfPath && !isRolling) {
            SetFacingDirection(cpf.steeringTarget);
        }

        // if (Input.GetMouseButtonDown(0)) {
        //     startMoving = false;
        //     animator.SetTrigger("doPreHold");
        // }

        // if (Input.GetMouseButton(0)) {
        //     startMoving = false;
        //     animator.SetBool("isHolding", true);    
        // }

        // if (Input.GetMouseButtonUp(0)) {
        //     animator.SetBool("isHolding", false);
        //     animator.SetBool("isWalking", false);
        // }

        if (Input.GetKeyDown(KeyCode.Space) && canRoll) {
            StartCoroutine(Roll());
        }
        else if (Input.GetMouseButtonDown(1)) {
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0;
            cpf.destination = targetPosition;
            anim.SetBool("isWalking", true);
            StartCoroutine(ShrinkClickEffect(0.5f, 0.15f, 0.1f));
            StartCoroutine(ShrinkClickEffect(0.4f, 0.1f, 0f));
        }
    }

    public void OnTargetReached() {
        anim.SetBool("isWalking", false);
    }

    private IEnumerator ShrinkClickEffect(float initSize, float lifeTime, float initWait) {
        GameObject inst = Instantiate(clickEffectPrefab, targetPosition, Quaternion.identity);
        inst.transform.localScale = new Vector3(initSize, initSize, initSize);
        yield return new WaitForSeconds(initWait);
        for (int i = 0; i < 60; i++) {
            inst.transform.localScale = new Vector3(
                inst.transform.localScale.x - initSize / 60f,
                inst.transform.localScale.y - initSize / 60f,
                inst.transform.localScale.z - initSize / 60f
            );
            inst.transform.position = new Vector3(
                inst.transform.position.x,
                inst.transform.position.y - 1/300f,
                inst.transform.position.z
            );
            yield return new WaitForSeconds(lifeTime / 60f);
        }
        Destroy(inst);
    }

    private IEnumerator Roll() {
        isRolling = true;
        cpf.enabled = false;
        anim.enabled = false;
        canRoll = false;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        SetFacingDirection(mousePosition);
        Vector3 rollDirection = SetFacingDirection(mousePosition);

        for (int i = 0; i < spriteIndices.Length; i++) {
            sr.sprite = rollSprites[spriteIndices[i]];
            yield return frameTime;
            rb.MovePosition(transform.position + rollDirection * slideSpeeds[i]);
        }

        isRolling = false;
        cpf.enabled = true;
        yield return new WaitForSeconds(rollCooldown);
        anim.enabled = true;
        canRoll = true;
    }

    private Vector3 SetFacingDirection(Vector3 target) {
        Vector3 dir = target - transform.position;
        dir.z = 0;
        dir = dir.normalized;
        sr.flipX = dir.x < 0;
        return dir;
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;

public class Player : MonoBehaviour {
    public float walkSpeed = 4f;
    public GameObject clickEffectPrefab; 
    private Vector3 targetPosition;
    private bool startMoving = false;
    private WaitForSeconds frameTime = new WaitForSeconds(1f / 60f);
    private SpriteRenderer sr;

    [SerializeField] public Animator animator;

    [Header("Roll Settings")]
    [SerializeField] public bool canRoll = true;
    [SerializeField] public bool rollIFrames = false;
    [SerializeField] public float rollCooldown = 1f;
    [SerializeField] private float s = 1f;
    [SerializeField] private float q = 2f;
    [SerializeField] private Sprite[] rollSprites;
    [SerializeField] public int[] spriteIndices;
    [SerializeField] public float[] slideSpeeds;

    private void Start() {
        sr = GetComponent<SpriteRenderer>();
        spriteIndices = new int[] 
            { 0, 0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 6, 6, 6, 7, 7, 7, 8 };
        slideSpeeds = new float[] 
            { s, s, s, s, s, s, s, s, s, q, q, q, q, q, q, q, q, q, q, q, q, q, q, q, s, s, s, s, s, s, s };
    }

    private void Update() {
        if (Input.GetMouseButtonDown(1)) {
            startMoving = true;
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0;
            StartCoroutine(ShrinkClickEffect(0.5f, 0.15f, 0.1f));
            StartCoroutine(ShrinkClickEffect(0.4f, 0.1f, 0f));
        }

        if (Input.GetKeyDown(KeyCode.Space) && canRoll) {
            StartCoroutine(Roll());
        }

        if (!rollIFrames && startMoving) {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, walkSpeed * Time.deltaTime);
            animator.SetBool("isWalking", true);
        }

        if(transform.position == targetPosition) {
            animator.SetBool("isWalking", false);
            startMoving = false;
        }
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
        canRoll = false;
        rollIFrames = true;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 rollDirection = mousePosition - transform.position;
        rollDirection.z = 0;
        rollDirection = rollDirection.normalized;
        sr.flipX = rollDirection.x < 0;

        for (int i = 0; i < spriteIndices.Length; i++) {
            sr.sprite = rollSprites[spriteIndices[i]];
            transform.position += rollDirection * slideSpeeds[i];
            yield return frameTime;
        }

        rollIFrames = false;
        yield return new WaitForSeconds(rollCooldown);
        canRoll = true;
    }
}
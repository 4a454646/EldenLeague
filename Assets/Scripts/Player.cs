using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;

public class Player : MonoBehaviour {
    public float walkSpeed = 4f;
    public GameObject clickEffectPrefab; 
    private Vector3 targetPosition;
    private bool startMoving = false;
    private WaitForSeconds frameTime = new WaitForSeconds(1f / 60f);
    [SerializeField] public bool canRoll = true;
    [SerializeField] public bool rollIFrames = false;
    [SerializeField] public float rollCooldown = 1f;
    [SerializeField] private float slowSlide = 1f;
    [SerializeField] private float quickSlide = 2f;

    private void Update() {
        if (Input.GetMouseButtonDown(1)) {
            startMoving = true;
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0;
            StartCoroutine(ShrinkClickEffect(1, 0.15f, 0.1f));
            StartCoroutine(ShrinkClickEffect(0.8f, 0.1f, 0f));
        }
        if (Input.GetKeyDown(KeyCode.Space) && canRoll) {
            StartCoroutine(Roll());
        }

        if (!rollIFrames && startMoving) {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, walkSpeed * Time.deltaTime);
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
                inst.transform.position.y - 1/180f,
                inst.transform.position.z
            );
            yield return new WaitForSeconds(lifeTime / 60f);
        }
        Destroy(inst);
    }

    private IEnumerator Roll() {
        canRoll = false;
        rollIFrames = true;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        // Get the direction to the mouse position at the time of cast
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 rollDirection = (mousePosition - transform.position);
        rollDirection.z = 0;
        rollDirection = rollDirection.normalized;

        // 3 frames of on the ground (prep jump), sliding slowly
        spriteRenderer.color = Color.red;
        for (int i = 0; i < 6; i++) {
            transform.position += rollDirection * slowSlide;
            yield return frameTime;
        }

        // 3 frames of on the ground (mid jump), sliding quickly
        spriteRenderer.color = Color.green;
        for (int i = 0; i < 6; i++) {
            transform.position += rollDirection * quickSlide;
            yield return frameTime;
        }

        // 4 frames of in the air (flying), sliding quickly
        spriteRenderer.color = Color.blue;
        for (int i = 0; i < 8; i++) {
            transform.position += rollDirection * quickSlide;
            yield return frameTime;
        }

        // 4 frames of on the ground (start roll), sliding slowly
        spriteRenderer.color = Color.yellow;
        for (int i = 0; i < 8; i++) {
            transform.position += rollDirection * slowSlide;
            yield return frameTime;
        }

        // 4 frames of on the ground (rolling), sliding slowly
        spriteRenderer.color = Color.magenta;
        for (int i = 0; i < 4; i++) {
            transform.position += rollDirection * slowSlide;
            yield return frameTime;
        }

        // 2 frames of recovery (not moving)
        spriteRenderer.color = Color.white;

        rollIFrames = false;
        yield return new WaitForSeconds(rollCooldown);
        canRoll = true;
    }
}
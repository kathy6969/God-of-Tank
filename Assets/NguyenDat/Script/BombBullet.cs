using System.Collections;
using UnityEngine;

public class BombBullet : MonoBehaviour
{
    public enum BombType
    {
        ExplodeArea,    // nổ 1 vùng cố định (spawn explosionPrefab)
        Split4,         // tách 4 viên theo 4 hướng (up,right,down,left)
        LargeSplit6     // đạn lớn ở vị trí, sau delay tách ra 6 viên
    }

    [Header("Bomb settings")]
    public BombType bombType = BombType.ExplodeArea;
    public float delay = 2f;               // thời gian chờ trước khi nổ/tách
    public GameObject childPrefab;         // prefab đạn con (dùng cho split)
    public GameObject explosionPrefab;     // prefab hiệu ứng nổ (dùng cho ExplodeArea)
    public float childSpeed = 6f;          // tốc độ đạn con khi tách
    public float childLifeTime = 2f;       // lifetime cho đạn con (nếu <=0 thì bỏ)
    public float spawnRadius = 0.1f;       // offset nhỏ khi spawn con (tránh đè chồng)

    void Start()
    {
        StartCoroutine(DelayedAction());
    }

    IEnumerator DelayedAction()
    {
        yield return new WaitForSeconds(delay);

        switch (bombType)
        {
            case BombType.ExplodeArea:
                DoExplodeArea();
                break;

            case BombType.Split4:
                DoSplit(4);
                break;

            case BombType.LargeSplit6:
                DoSplit(6);
                break;
        }

        // destroy the bomb itself (explosion prefab may handle its own lifetime)
        Destroy(gameObject);
    }

    void DoExplodeArea()
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        // TODO: thêm logic gây damage nếu bạn muốn (trigger collider, v.v.)
    }

    void DoSplit(int pieces)
    {
        if (childPrefab == null)
        {
            Debug.LogWarning("childPrefab not assigned for split.");
            return;
        }

        // spawn pieces evenly distributed around 360 degrees
        float angleStep = 360f / pieces;
        float startAngle = 0f; // có thể chỉnh offset nếu muốn

        for (int i = 0; i < pieces; i++)
        {
            float angle = startAngle + i * angleStep;
            Quaternion rot = Quaternion.Euler(0f, 0f, angle) * transform.rotation;
            Vector3 dir = rot * Vector3.up;

            Vector3 spawnPos = transform.position + (Vector3)(dir.normalized * spawnRadius);
            GameObject child = Instantiate(childPrefab, spawnPos, rot);

            if (child.TryGetComponent<Rigidbody2D>(out var rb))
            {
                rb.linearVelocity = dir.normalized * childSpeed;
            }

            if (childLifeTime > 0f)
                Destroy(child, childLifeTime);
        }
    }
}

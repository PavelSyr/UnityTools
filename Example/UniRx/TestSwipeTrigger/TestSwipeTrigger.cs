using System.Collections;
using UniRx;
using UniRx.Triggers; // Triggers Namepsace
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class TestSwipeTrigger : MonoBehaviour
{
    public int frames = 5;
    public float speed = 1.0f;

    private Vector3 startPos;
    private Vector3 dir = Vector2.zero;
    private Coroutine translateCoroutine;

    void Start()
    {
        gameObject.AddComponent<ObservableSwipeTrigger>()
            .SwipeAsSingel()
            .SwipeWithTime(1.0f)
            .SwipWithLength(0.5f)
            .OnSwipeAsObservable()
            .Where(s => s.IsHorOrVert)
            .Subscribe(s =>
            {
                startPos = s.start;
                dir = s.FourDirection;
                StartTranslateCoroutine(dir);
            });
    }

    private void StartTranslateCoroutine(Vector2 dir)
    {
        if (translateCoroutine != null)
            StopCoroutine(translateCoroutine);

        translateCoroutine = StartCoroutine(Translate(dir));
    }

    private IEnumerator Translate(Vector2 dir)
    {
        for (int i = 0; i < frames; i++)
        {
            yield return null;
            transform.Translate(dir * Time.deltaTime * speed);
        }
    }

    private void OnDrawGizmos()
    {
        var sp = Camera.main.ScreenToWorldPoint(startPos);
        sp.z = -3;

        Gizmos.DrawLine(sp, sp + dir);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(sp, sp + Vector3.right * Vector3.Dot(dir, Vector3.right));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(sp, sp + Vector3.left * Vector3.Dot(dir, Vector3.left));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(sp, sp + Vector3.up * Vector3.Dot(dir, Vector3.up));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(sp, sp + Vector3.down * Vector3.Dot(dir, Vector3.down));
    }
}

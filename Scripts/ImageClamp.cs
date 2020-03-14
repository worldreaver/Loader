using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class ImageClamp : MonoBehaviour
{
    public EDirection eDirection = EDirection.Right;
    [Range(0.001f, 1)] public float speed;

    private RawImage _image;

    private void Awake()
    {
        _image = GetComponent<RawImage>();
    }

    private void Update()
    {
        var r = _image.uvRect;
        switch (eDirection)
        {
            case EDirection.Right:
                r.x -= Time.deltaTime * speed;
                break;
            case EDirection.Left:
                r.x += Time.deltaTime * speed;
                break;
            case EDirection.Up:
                r.y -= Time.deltaTime * speed;
                break;
            case EDirection.Down:
                break;
            default:
                r.y += Time.deltaTime * speed;
                break;
        }
        _image.uvRect = r;
    }

    [System.Serializable]
    public enum EDirection
    {
        Left,
        Right,
        Up,
        Down,
    }
}
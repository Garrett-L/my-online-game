using UnityEngine;

public class BuildUIAnim : MonoBehaviour
{

    private float targetY;
    private float hiddenY = -90f;
    public float currentValue;
    private bool animating;
    public bool isEntering;
    [SerializeField] private AnimationCurve enterAnim;
    [SerializeField] private AnimationCurve exitAnim;


    void Start() {
        targetY = transform.position.y;
        transform.position = new Vector3(transform.position.x, hiddenY, transform.position.z);
    }

    void Update() {
        if(!animating) return;
        currentValue += Time.deltaTime;
        if (isEntering) 
        {
            transform.position = new Vector3(transform.position.x, NewLerp(hiddenY, targetY, enterAnim.Evaluate(currentValue)));
            if (currentValue >= enterAnim.keys[enterAnim.length-1].time) animating = false;
        }
        else 
        {
            transform.position = new Vector3(transform.position.x, NewLerp(hiddenY, targetY, exitAnim.Evaluate(currentValue)));
            if (currentValue >= exitAnim.keys[exitAnim.length-1].time) animating = false;
        }
    }

    public bool Animate() {
        if (animating) return false;
        currentValue = 0;
        isEntering = !isEntering;
        animating = true;
        return true;
    }

    private float NewLerp(float a, float b, float t) {
        return t * (b-a) + a;
    }
}

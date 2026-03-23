using UnityEngine;

public class AnimationTest : MonoBehaviour
{
    [Range(2,-2)]
    public float horizontal;
    [Range(2,-2)]
    public float vertical;

    private Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("Horizontal", horizontal);
        animator.SetFloat("Vertical", vertical);
    }
}

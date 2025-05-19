using UnityEngine;

public class NyxScript : MonoBehaviour
{
    public Animator animator;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
        animator.SetFloat("zSpeed", 0f);
        animator.SetFloat("xSpeed", 0f);
    }
}

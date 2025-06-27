using UnityEngine;

public class Paddle : MonoBehaviour
{
    private InputReader inputReader;
    private Vector2 moveInput;

    [SerializeField] private float moveSpeed;

    private void Awake()
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputReader = InputReader.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = inputReader.MoveInput;
        
        transform.Translate(moveSpeed * Time.deltaTime * moveInput);

        inputReader.ResetInputs();
    }
}

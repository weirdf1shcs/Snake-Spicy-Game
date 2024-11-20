using UnityEngine;

public class Object : MonoBehaviour
{
    public enum Type
    {
        Pepper,
        Banana
    }

    public SnakeController sc;
    public Type type;
    public void NextStep()
    {
        Debug.Log(type.ToString());
        switch (type)
        {
            case Type.Pepper:
                break;
            case Type.Banana:
                sc.Grow();
                break;
        }
        Destroy(gameObject);
    }
}

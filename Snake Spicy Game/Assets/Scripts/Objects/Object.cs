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
        switch (type)
        {
            case Type.Pepper:
                AudioManager.instance.PlayPepperSFX();
                GameManager.instance.GoBackwards();
                break;
            case Type.Banana:
                AudioManager.instance.PlayBananaSFX();
                sc.Grow();
                break;
        }

        GameManager.instance.objects.Remove(this.gameObject);
        GameManager.instance.CheckForCompletion();
        Destroy(gameObject);
    }
}

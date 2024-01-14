using UnityEngine;

public interface ColorSetterInterface
{
    void Refresh();

    void SetColor(float time);
}

[ExecuteInEditMode]
public class LightColorController : MonoBehaviour
{
    public static LightColorController Lc;
    [SerializeField] [Range(0,1)]public float time;

    private ColorSetterInterface[] setters;
    private float currentTime = 0;

    public float timeValue => currentTime;

    public void Awake()
    {
        Lc = this;
    }

    public void GetSetters()
    {
        setters = GetComponentsInChildren<ColorSetterInterface>();
        foreach (var setter in setters)
            setter.Refresh();
    }

    private void OnEnable()
    {
        time = 0.01f;
        GetSetters();
        UpdateSetters();
    }

    private void OnDisable()
    {
        time = 0.01f;
        UpdateSetters();
    }

    private void Update()
    {
        if (currentTime != time)
            UpdateSetters();
    }

    public void UpdateSetters()
    {
        currentTime = time;

        foreach (var setter in setters)
            setter.SetColor(time);
    }
}

using UnityEngine;

public class TestScript : MonoBehaviour
{
    public int someValue;

    // Start is called before the first frame update
    private void Start()
    {
        someValue = 6;
        //mylogs Probably remove this later
        if (Debug.isDebugBuild) Debug.Log($"<color=purple>SomeText{someValue}</color>");
    }

    // Update is called once per frame
    public void Update()
    {
        //mylogs Probably remove this later
        if (Debug.isDebugBuild) Debug.Log("<color=purple>SomeText</color>");
    }
}
using UnityEngine;
using System.Collections;


public class EnableInEditorOrNot : MonoBehaviour{
    public bool enable_in_editor;
    public bool enable_in_device;

    void Start()
    {
#if UNITY_EDITOR
        this.gameObject.SetActive(enable_in_editor);
#else
        this.gameObject.SetActive(enable_in_device);
#endif
    }

}
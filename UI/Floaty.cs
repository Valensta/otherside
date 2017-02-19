using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Floaty : MonoBehaviour
{
    public float lifespan = 0.5f;
    protected Vector3 velocity;
    protected float TIME;
    protected float delta_y = 0.9945f;
    protected Vector3 my_position;
    private Plane plane = new Plane(Vector3.up, Vector3.zero);
    private Vector3 v3OrgPos;
    public RectTransform my_parent;
    public float shift_scale;
    public Vector3 shift_offset;
    protected Vector3 initial_pos;
    Transform events;
    void Update()
    {
        TIME += Time.deltaTime;
        velocity.y = delta_y * velocity.y;
        my_position.y += Time.deltaTime * velocity.y;
        my_parent.anchoredPosition = my_position;

        if (TIME > lifespan) { _returnMe(); }

    }
    public void Init(Vector3 pos)
    {
        velocity = new Vector3(0f, 2f, 0f);
        if (events == null) events = EagleEyes.Instance.events.transform;
        my_parent.transform.SetParent(events);
        my_parent.localPosition = Vector3.one;
        my_parent.localScale = Vector3.one*shift_scale;
        my_parent.localRotation = Quaternion.identity;
        TIME = 0f;
        initial_pos = pos;

        Show.UIToWorld(EagleEyes.Instance.events.GetComponent<RectTransform>(), initial_pos + shift_offset, my_parent);

        my_position = my_parent.anchoredPosition;
    }

    protected void _returnMe()
    {
        Peripheral.Instance.zoo.returnObject(this.gameObject, false); return;
    }
}


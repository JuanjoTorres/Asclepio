using UnityEngine;

public class CollisionPainter : MonoBehaviour
{
    public Color paintColor;

    public float radius = 1;
    public float strength = 1;
    public float hardness = 1;

    private void OnCollisionStay(Collision other)
    {
        Paintable p = other.collider.GetComponent<Paintable>();
        if (p != null)
        {
            Debug.Log("Soy pintable!");
            Debug.Log("radius: " + radius);
            Debug.Log("hardness: " + hardness);
            Debug.Log("strength: " + strength);
            Debug.Log("paintColor: " + paintColor);

            Vector3 pos = other.contacts[0].point;
            PaintManager.instance.paint(p, pos, radius, hardness, strength, paintColor);
        }
    }
}

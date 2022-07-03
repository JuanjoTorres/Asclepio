using EzySlice;
using UnityEngine;
using UnityEngine.UI;

public class BladeModeScript : MonoBehaviour
{
    private const float MIN_ANGLE = 0.0f;
    private const float MAX_ANGLE = 180.0f;

    public bool bladeMode;
    public Transform cutPlane;
    public Text degreesInput;

    [Range(MIN_ANGLE, MAX_ANGLE)] public float PlaneOrientation = MIN_ANGLE;
    public Material crossMaterial;
    public LayerMask layerMask;

    void Start()
    {
        cutPlane.gameObject.SetActive(true);
    }

    void Update()
    {
        if (bladeMode)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation,Camera.main.transform.rotation,.2f);
            degreesInput.text = PlaneOrientation.ToString();
            RotatePlane();
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 13)
        {
            Slice();
            print("He tocado el slicer");
        }

        print("He entrado en colision");
    }

    public void Slice()
    {
        Collider[] hits = Physics.OverlapBox(cutPlane.position, new Vector3(5, 0.1f, 5), cutPlane.rotation, layerMask);

        if (hits.Length <= 0)
            return;

        for (int i = 0; i < hits.Length; i++)
        {
            SlicedHull hull = SliceObject(hits[i].gameObject, crossMaterial);
            if (hull != null)
            {
                GameObject bottom = hull.CreateLowerHull(hits[i].gameObject, crossMaterial);
                GameObject top = hull.CreateUpperHull(hits[i].gameObject, crossMaterial);
                AddHullComponents(bottom);
                AddHullComponents(top);
                Destroy(hits[i].gameObject);
            }
        }
    }

    public void AddHullComponents(GameObject go)
    {
        go.layer = 12; // Layer Sliceable
        Rigidbody rb = go.AddComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        MeshCollider collider = go.AddComponent<MeshCollider>();
        collider.convex = true;

        rb.AddExplosionForce(100, go.transform.position, 20);
    }

    public SlicedHull SliceObject(GameObject obj, Material crossSectionMaterial = null)
    {
        // slice the provided object using the transforms of this object
        if (obj.GetComponent<MeshFilter>() == null)
            return null;

        return obj.Slice(cutPlane.position, cutPlane.up, crossSectionMaterial);
    }

    public void RotatePlane()
    {
        cutPlane.transform.rotation = new Quaternion(0.0f, 0.0f, PlaneOrientation / MAX_ANGLE, 1.0f);
    }
}

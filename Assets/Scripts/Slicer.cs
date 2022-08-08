using EzySlice;
using UnityEngine;
using UnityEngine.UI;

public class Slicer : MonoBehaviour
{
    public LayerMask sliceMask;
    public bool isTouched;
    public bool isPress;
    public Transform cutPlane;
    public Text degreesOfCut;

    public Material materialAfterSlice;

    private void Update()
    {
        if (isTouched && isPress)
        {
            isTouched = false;

            Collider[] hits = Physics.OverlapBox(transform.position, new Vector3(1, 0.1f, 0.1f), cutPlane.rotation, sliceMask);

            if (hits.Length <= 0)
                return;

            for (int i = 0; i < hits.Length; i++)
            {
                SlicedHull hull = SliceObject(hits[i].gameObject);
                if (hull != null)
                {
                    GameObject top = hull.CreateUpperHull(hits[i].gameObject, materialAfterSlice);
                    GameObject bottom = hull.CreateLowerHull(hits[i].gameObject, materialAfterSlice);

                    AddHullComponents(top);
                    AddHullComponents(bottom);

                    Destroy(hits[i].gameObject);
                }
            }
        }
    }

    public void AddHullComponents(GameObject go)
    {
        go.layer = LayerMask.NameToLayer("Grab");
        go.tag ="Model";
        Rigidbody rb = go.AddComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        BoxCollider collider = go.AddComponent<BoxCollider>();
        go.AddComponent<XROffsetGrabInteractable>();

        rb.AddExplosionForce(100, go.transform.position, 20);
    }

    private SlicedHull SliceObject(GameObject obj, Material crossSectionMaterial = null)
    {
        return obj.Slice(transform.position, transform.up, crossSectionMaterial);
    }


}
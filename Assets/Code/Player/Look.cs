using UnityEngine;

public class Look : MonoBehaviour
{
    [SerializeField] private Transform _eyesTransform;

    public Painting GetLookingPainting()
    {
        Ray ray = new Ray(_eyesTransform.position, _eyesTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f))
        {
            Painting painting = hitInfo.collider.GetComponent<Painting>();

            if (painting != null)
            {
                return painting;
            }
        }

        return null;
    }
}

using UnityEngine;
using System.Collections;

public class Artist : MonoBehaviour
{
    [Header("General Data")]
    [SerializeField] private DescriptionConversionTableSO _allDescriptionConversionTable;

    [Header("Components")]
    [SerializeField] private RuntimeAnimatorController _animatorController;
    private ArtistSO _artistSO;
    private Animator _animator;
    private AnimationClip _animationClip;
    private GameObject _lastMesh;

    public void InitializeArtist(ArtistSO artistSO, Transform spawnTransform)
    {
        _artistSO = artistSO;

        InitializeMesh();
        InitializeAnimation();

        // Change position and rotation
        transform.position = spawnTransform.position;
        transform.rotation = spawnTransform.rotation;
    }

    private void InitializeMesh()
    {
        if (_lastMesh != null)
        {
            Destroy(_lastMesh);
        }

        foreach (DescriptionMeshPair meshPair in _allDescriptionConversionTable.DescriptionToMesh)
        {
            if (meshPair.Description.Contains(_artistSO.ArtistModel.MeshDescription))
            {
                _lastMesh = Instantiate(meshPair.MeshPrefab, transform);
                _animator = _lastMesh.GetComponent<Animator>();
                break;
            }
        }
    }

    private void InitializeAnimation()
    {
        foreach (DescriptionAnimationPair animationPair in _allDescriptionConversionTable.DescriptionToAnimation)
        {
            if (animationPair.Description.Contains(_artistSO.ArtistModel.AnimationDescription))
            {
                _animationClip = animationPair.AnimationClip;
                break;
            }
        }

        _animator.runtimeAnimatorController = _animatorController;
        _animator.Play(_animationClip.name);
    }
}

using System.Collections;
using UnityEngine;

public class CharacterShadow : MonoBehaviour
{
    public Transform shadowTransform;
    public float maxDist;
    public Vector3 checkHeightOffset;
    public Vector3 hitOffset;
    public Vector3 maxScale, minScale;

    private void OnEnable()
    {
        if (shadowTransform == null)
        {
            Debug.LogError("Shadow Transform is not assigned.");
            return;
        }

        // Start the shadow calculation routine
        StartCoroutine(CalculateShadowRoutine());
    }
    
    private void OnDisable()
    {
        // Stop the shadow calculation routine
        StopCoroutine(CalculateShadowRoutine());
    }

    private IEnumerator CalculateShadowRoutine()
    {
        float yPos = 0;
        while(true)
        {
            RaycastHit hit;

            bool isHit = Physics.Raycast(transform.position + checkHeightOffset, Vector3.down, out hit, maxDist, LayerMask.GetMask("Default"));
            if (isHit)
            {
                shadowTransform.gameObject.SetActive(true);
                shadowTransform.localScale = Vector3.Lerp(minScale, maxScale, 1.0f - (hit.distance / maxDist));
                shadowTransform.position = hit.point + hitOffset;
            }
            else
            {
                shadowTransform.gameObject.SetActive(false);
            }

            yield return null;    
        }
    }
}

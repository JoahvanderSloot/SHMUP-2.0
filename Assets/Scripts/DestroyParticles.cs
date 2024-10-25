using UnityEngine;

public class DestroyParticles : MonoBehaviour
{
    [SerializeField] bool m_genericExplotion;

    void Start()
    {
        if (m_genericExplotion)
        {
            FindObjectOfType<AudioManager>().Play("Explotion");
        }

        Destroy(gameObject, 1);
    }
}

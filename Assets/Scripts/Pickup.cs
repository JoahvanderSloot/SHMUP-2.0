using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] string m_name;
    int m_randomName;
    [SerializeField] List<Sprite> m_sprites;
    SpriteRenderer m_playerSpriteRenderer;

    private void Start()
    {
        Destroy(gameObject, 4);
        m_playerSpriteRenderer = GetComponent<SpriteRenderer>();
        m_randomName = Random.Range(0, 4);
    }

    private void Update()
    {
        switch (m_randomName)
        {
            case 0:
                m_name = "HP";
                transform.localScale = new Vector3(5f, 5f, 5f);
                break;
            case 1:
                 m_name = "Upgrade";
                transform.localScale = new Vector3(2f, 2f, 2f);
                break;
            case 2:
                m_name = "Instakill";
                transform.localScale = new Vector3(5f, 5f, 5f);
                break;
            case 3:
                m_name = "Shield";
                transform.localScale = new Vector3(5.5f, 5.5f, 5.5f);
                break;
        }

        m_playerSpriteRenderer.sprite = m_sprites[m_randomName];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement _playerMovement = collision.GetComponent<PlayerMovement>();
        if(_playerMovement != null)
        {
            _playerMovement.PowerUp(m_name);
            Destroy(gameObject);
        }
    }
}
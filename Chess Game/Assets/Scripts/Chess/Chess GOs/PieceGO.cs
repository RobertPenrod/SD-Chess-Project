using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PieceGO : MonoBehaviour
{
    Piece piece;
    float moveSpeed = 5f;

    SpriteRenderer spriteRend;

    public void BindPiece(Piece p)
    {
        piece = p;
        spriteRend = GetComponent<SpriteRenderer>();
        spriteRend.sprite = piece.icon;

        gameObject.name = p.name.Substring(0, p.name.Length - 7); // -7 to remove "(clone)" form name
    }

    private void Update()
    {
        LerpPos();
    }

    void LerpPos()
    {
        if (piece == null) return;

        Vector2 targetPos = piece.currentPos;
        Vector2 lerpPos = Vector2.Lerp(transform.position, targetPos, moveSpeed * Time.deltaTime);
        Vector3 newPos = (Vector3)lerpPos + Vector3.forward * transform.position.z;
        transform.position = newPos;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PieceGO : MonoBehaviour
{
    public Piece piece { get; private set; }
    float moveSpeed = 5f;
    SpriteRenderer spriteRend;


    public void BindPiece(Piece p)
    {
        piece = p;
        spriteRend = GetComponent<SpriteRenderer>();
        spriteRend.sprite = piece.icon;

        gameObject.name = p.name.Substring(0, p.name.Length - 7); // -7 to remove "(clone)" form name

        float darkC = 0.5f;
        spriteRend.color = p.teamNumber == 1 ? Color.white : new Color(darkC, darkC, darkC);

        p.OnPromotion_Event += () =>
        {
            spriteRend.sprite = p.icon;
        };
    }

    private void Update()
    {
        LerpToBoardPos();
    }

    void LerpToBoardPos()
    {
        if (piece == null) return;

        Vector2 currentPos = transform.localPosition;
        Vector2 targetPos = piece.currentPos;
        float distance = Vector2.Distance(currentPos, targetPos);
        float lerpT = moveSpeed * Time.deltaTime * ExtensionMethods.Remap(distance, 0f, 5f, 2f, 1f);
        Vector2 lerpPos = Vector2.Lerp(currentPos, targetPos, lerpT);

        float depth = ExtensionMethods.Remap(distance, 0f, 0.2f, 0f, -1f);
        Vector3 newPos = (Vector3)lerpPos + Vector3.forward * depth;
        transform.localPosition = newPos;

        //float s = ExtensionMethods.Remap(distance, 0f, 0.5f, 1f, 1.1f);
        //transform.localScale = new Vector3(s, s, s);
    }
}

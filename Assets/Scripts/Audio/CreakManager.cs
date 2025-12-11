using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreakManager : MonoBehaviour
{
    public List<CreakEmitter> shelfEmitters;

    [Header("Timing")]
    public float minDelay = 6f;
    public float maxDelay = 18f;

    [Header("Chance of 2nd creak shortly after")]
    [Range(0f, 1f)]
    public float doubleCreakChance = 0.25f;

    public float secondCreakMinOffset = 0.3f;
    public float secondCreakMaxOffset = 1.2f;

    private void Start()
    {
        StartCoroutine(CreakRoutine());
    }

    IEnumerator CreakRoutine()
    {
        while (true)
        {
            // wait before the next creak
            float wait = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(wait);

            if (shelfEmitters.Count == 0) continue;

            // pick random shelf
            CreakEmitter shelf = shelfEmitters[Random.Range(0, shelfEmitters.Count)];
            shelf.PlayCreak();

            // second creak nearby or on another source
            if (Random.value < doubleCreakChance)
            {
                float offset = Random.Range(secondCreakMinOffset, secondCreakMaxOffset);
                yield return new WaitForSeconds(offset);

                CreakEmitter shelf2 = shelfEmitters[Random.Range(0, shelfEmitters.Count)];
                shelf2.PlayCreak();
            }
        }
    }
}
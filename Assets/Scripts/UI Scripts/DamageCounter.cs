using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DamageCounter : MonoBehaviour
{
    public int damage;
    public bool showDamage;

    [SerializeField]
    private Image[] digits;

    [SerializeField]
    private Sprite[] numbers;

    [SerializeField]
    private Gradient damageGradient;

    private Dictionary<char, Sprite> numberDict;

    void Start()
    {
        showDamage = false;

        numberDict = new()
        {
            {'0', numbers[0] },
            {'1', numbers[1] },
            {'2', numbers[2] },
            {'3', numbers[3] },
            {'4', numbers[4] },
            {'5', numbers[5] },
            {'6', numbers[6] },
            {'7', numbers[7] },
            {'8', numbers[8] },
            {'9', numbers[9] },
        };
    }

    // Update is called once per frame
    void LateUpdate()
    {
        string damageString = damage.ToString(); 

        for (int i = 0; i < digits.Length; i++)
        {
            if (damageString.Length > i && !showDamage)
            {
                digits[i].gameObject.SetActive(true);
                digits[i].sprite = numberDict[damageString[i]];
                digits[i].color = damageGradient.Evaluate(damage / 1000f);
            }
            else
                digits[i].gameObject.SetActive(false);
        }
    }
}

/**
 * 
 * Author: Sujay Chand
 * 
 *  Button text infomation
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject CorrespondingText;

    // On enable deactive text
    private void OnEnable()
    {
        CorrespondingText.SetActive(false);
    }

    /*
     * Render text
     */
    public void OnPointerEnter(PointerEventData eventData)
    {
        CorrespondingText.SetActive(true);
    }

    /*
     *  Deactive text
     */
    public void OnPointerExit(PointerEventData eventData)
    {
        CorrespondingText.SetActive(false);
    }
}

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class BonusPanel : MonoBehaviour
{
    
    [Title("Set in Inspector")]
    [SerializeField] private CanvasGroup m_previousButton;
    [SerializeField] private CanvasGroup m_nextButton;
    [SerializeField] private List<GameObject> m_bonusMiniatures;

    private int m_currentPage;

    private void Start()
    {
        m_currentPage = 0;
    }

    public void NextPage()
    {
        // Hide last page and show new page
        int i = 0;
        while (i < 8 && i + m_currentPage * 4 < m_bonusMiniatures.Count)
        {
            Debug.Log(i + m_currentPage * 4);
            if (i < 4)
            {
                m_bonusMiniatures[i + m_currentPage * 4].SetActive(false);
            }
            else
            {
                m_bonusMiniatures[i + m_currentPage * 4].SetActive(true);
            }

            i++;
        }
        
        m_currentPage++;
        
        // Show/hide arrow buttons
        PanelManager.ShowCanvasGroup(true, m_previousButton);
        if (m_currentPage == Math.Ceiling(m_bonusMiniatures.Count / 4f) - 1)
        {
            PanelManager.ShowCanvasGroup(false, m_nextButton);
        }
    }

    public void PreviousPage()
    {
        
        Debug.Log("RESTE = " + m_bonusMiniatures.Count % 4);
        
        // Hide last page and show new page
        int i = 7;
        if (m_currentPage == Math.Ceiling(m_bonusMiniatures.Count / 4f) - 1)
        {
            i = m_bonusMiniatures.Count % 4 == 0 ? 7 : (m_bonusMiniatures.Count % 4) + 3 ;
        }
        
        Debug.Log("I = "+ i);
        
        m_currentPage--;
        
        while (i >= 0)
        {
            Debug.Log(i + m_currentPage * 4);
            if (i > 3)
            {
                m_bonusMiniatures[i + m_currentPage * 4].SetActive(false);
            }
            else
            {
                m_bonusMiniatures[i + m_currentPage * 4].SetActive(true);
            }

            i--;
        }

        // Show/hide arrow buttons
        PanelManager.ShowCanvasGroup(true, m_nextButton);
        if (m_currentPage == 0)
        {
            PanelManager.ShowCanvasGroup(false, m_previousButton);
        }

    }
}

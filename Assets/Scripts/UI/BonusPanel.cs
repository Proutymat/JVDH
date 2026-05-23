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
        ResetUI();
    }
    

    public void ResetUI()
    {
        m_currentPage = 0;
        PanelManager.ShowCanvasGroup(false, m_previousButton);
        PanelManager.ShowCanvasGroup(true, m_nextButton);
        
        // Disable every page and set lock state
        for (int i = 0; i < m_bonusMiniatures.Count; i++)
        {
            m_bonusMiniatures[i].SetActive(false);
            m_bonusMiniatures[i].transform.GetChild(0).gameObject.SetActive(!SaveManager.Instance.Data.progression.success[i]);
        }
        
        // Enable first page
        for (int i = 0; i < 4; i++)
        {
            m_bonusMiniatures[i].SetActive(true);
        }
    }

    public void NextPage()
    {
        // Hide last page and show new page
        int i = 0;
        while (i < 8 && i + m_currentPage * 4 < m_bonusMiniatures.Count)
        {
            m_bonusMiniatures[i + m_currentPage * 4].SetActive(i > 3);
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
        // Hide last page and show new page
        int i = 7;
        if (m_currentPage == Math.Ceiling(m_bonusMiniatures.Count / 4f) - 1)
        {
            i = m_bonusMiniatures.Count % 4 == 0 ? 7 : (m_bonusMiniatures.Count % 4) + 3 ;
        }
        
        m_currentPage--;
        
        while (i >= 0)
        {
            m_bonusMiniatures[i + m_currentPage * 4].SetActive(i < 4);
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

using Sirenix.OdinInspector;
using UnityEngine;

public class BonusButton : MonoBehaviour
{
    [Title("Set in inspector")]
    [SerializeField] private BonusPanel m_bonusPanel;
    
    public void ClickButton()
    {
        SoundManager.Instance.PlayClicSound();
        PanelManager.Instance.SetPanel(PanelState.Bonus, FadeStyle.FadeInAndOut, m_bonusPanel.ResetUI);
    }
}

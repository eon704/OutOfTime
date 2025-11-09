using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
  public class MainMenuUI : MonoBehaviour
  {
    [SerializeField] private Image foregroundTint;
    [SerializeField] private Button playButton;
    [SerializeField] private CinemachineCamera mainCinemachineCamera;
    
    private void Awake()
    {
      playButton.onClick.AddListener(OnPlay);
    }

    private void Start()
    {
      foregroundTint
        .DOFade(0f, 0.5f)
        .OnComplete(() => foregroundTint.gameObject.SetActive(false));
    }

    private void OnPlay()
    {
      
    }
  }
}
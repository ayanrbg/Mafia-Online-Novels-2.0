using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class TutorialScenario : MonoBehaviour
{
    private bool waitForNext = false;
    public TypewriterUtility typewriterUtility;
    public CanvasGroup background;
    public PhaseHandler phaseHandlerScript;
    [Header("Welcome Panel")]
    public GameObject welcomePanel;
    public CanvasGroup welcomeText;
    public GameObject sunAnimWelcome;

    [Header("Mafia and Citizen")]
    public GameObject mafiaAndCitizenPanel;
    public TextMeshProUGUI title2sideText;
    public CanvasGroup mafiaText;
    public CanvasGroup citizenText;
    public TextMeshProUGUI rolesText;
    [Header("Lets Learn role!")]
    public GameObject rolePanel;
    public TextMeshProUGUI roleText;
    [Header("Start game")]
    public GameObject startGamePanel;
    public StateAnimation dayAnimation;
    public CanvasGroup sherifText;
    [Header("Excellent!")]
    public GameObject excellentPanel;
    public TextMeshProUGUI excellentText;
    public TextMeshProUGUI youSherifText;
    public TextMeshProUGUI atNightText;
    [Header("Chat Day to Night")]
    public GameObject chatPanel;
    public GameObject[] dayToNightMessages;
    public StateAnimation nightAnimation;
    [Header("Chat After Night")]
    public GameObject[] afterNightMessages;
    public BottomSheetDrag nightVotePanel;
    public GameObject[] sherifVoteResult;
    public GameObject[] playerMessages; 
    
    public int selectedPlayerId = 1;
    public CanvasGroup sendMessageButton;
    public StateAnimation voteAnimation;
    public BottomSheetDrag dayVotePanel;
    public PlayerSlot[] playersDayVote;
    public StateAnimation winAnimation;
    [Header("Chat After Night")]
    public GameObject endTutorialPanel;
    public CanvasGroup endTutorialText;
    
    void Start()
    {
        StartCoroutine(RunScenario());
        PlayerPrefs.SetString("isFirstEntry",  "false");
    }

    IEnumerator RunScenario()
    {
        // Шаг 1
        background.gameObject.SetActive(true);
        welcomePanel.SetActive(true);
        FadeIn(welcomeText, 1.5f);
        RotateForever(sunAnimWelcome.transform, 10f);
        yield return WaitForNextClick();
        
        
        welcomePanel.SetActive(false);
        mafiaAndCitizenPanel.SetActive(true);
        mafiaText.alpha = 0f;
        citizenText.alpha = 0f;
        typewriterUtility.StartTyping(title2sideText, "В этой игре 2 стороны", 1.5f);
        yield return new WaitForSeconds(2f);
        
        
        FadeIn(mafiaText, .7f);
        yield return new WaitForSeconds(.5f);
        
        
        FadeIn(citizenText, .7f);
        yield return new WaitForSeconds(1f);
        
        
        typewriterUtility.StartTyping(rolesText, "У каждой стороны свои роли",  1.5f);
        yield return WaitForNextClick();
        
        
        mafiaAndCitizenPanel.SetActive(false);
        rolePanel.SetActive(true); 
        typewriterUtility.StartTyping(roleText, "Давай узнаем твою роль!",  1.5f);
        yield return WaitForNextClick();
        
        
        rolePanel.SetActive(false);
        startGamePanel.SetActive(true);
        dayAnimation.Play();
        sherifText.alpha = 0f;
        FadeIn(sherifText, 1f);
        yield return new WaitForSeconds(1f);
        
        FadeOut(sherifText, 0.7f);
        yield return new WaitForSeconds(1f);

        startGamePanel.SetActive(false); 
        excellentPanel.SetActive(true);
        typewriterUtility.StartTyping(excellentText,"Отлично!", 0.8f);
        yield return new WaitForSeconds(1f);
        
        typewriterUtility.StartTyping(youSherifText,"Ты - Шериф", .7f);
        yield return new WaitForSeconds(1.4f);

        typewriterUtility.StartTyping(atNightText,"Ночью ты можешь узнать роль любого игрока!", 2f);
        yield return WaitForNextClick();
        
        phaseHandlerScript.SetDayTimer(12);
        excellentPanel.SetActive(false);
        chatPanel.SetActive(true);
        FadeOut(background, 1f);
        dayToNightMessages[0].SetActive(true);
        yield return new WaitForSeconds(1.5f);
        
        dayToNightMessages[1].SetActive(true);
        yield return new WaitForSeconds(2f);
        
        dayToNightMessages[2].SetActive(true);
        yield return new WaitForSeconds(2.5f);
        
        dayToNightMessages[3].SetActive(true);
        yield return new WaitForSeconds(1.5f);
        
        dayToNightMessages[4].SetActive(true);
        yield return new WaitForSeconds(2.5f);
        
        dayToNightMessages[5].SetActive(true);
        yield return new WaitForSeconds(1.5f);
        
        dayToNightMessages[6].SetActive(true); // ночь наступила
        dayToNightMessages[7].SetActive(true); // Мафия выбирает жертву
        phaseHandlerScript.SetNightTimer(8);
        nightAnimation.Play();
        nightVotePanel.Open(); // игрок выбирает посмотреть роль
        FadeIn(background, 1f);
        yield return WaitForNextClick();
        
        FadeOut(background, 1f);
        nightVotePanel.Close();
        dayAnimation.Play();
        phaseHandlerScript.SetDayTimer(12);
        afterNightMessages[0].SetActive(true); //убит игрок
        afterNightMessages[1].SetActive(true);
        sherifVoteResult[selectedPlayerId].SetActive(true);
        yield return new WaitForSeconds(2f);

        afterNightMessages[2].SetActive(true); // неожиданно
        yield return new WaitForSeconds(2f);
        
        afterNightMessages[3].SetActive(true); // шериф жив?
        yield return new WaitForSeconds(2f);
        
        afterNightMessages[4].SetActive(true); // надеюсь..
        yield return new WaitForSeconds(2f);

        FadeIn(sendMessageButton, 2f);
        yield return WaitForNextClick(); // отправить сообщение кнопка
        
        sendMessageButton.gameObject.SetActive(false);
        playerMessages[selectedPlayerId].gameObject.SetActive(true); // сообщение Я шериф это игрок ..
        sendMessageButton.gameObject.SetActive(false);
        yield return new WaitForSeconds(2f);

        
        voteAnimation.Play();
        phaseHandlerScript.SetVoteTimer(7);
        int voteCount = 0;
        dayVotePanel.Open();
        yield return new WaitForSeconds(1.5f);
        
        AddVoteForPlayer(voteCount++);
        yield return new WaitForSeconds(2f);
        
        AddVoteForPlayer(voteCount++);
        yield return new WaitForSeconds(2.5f);
        
        AddVoteForPlayer(voteCount++);
        yield return new WaitForSeconds(1f);
        
        AddVoteForPlayer(voteCount++);
        yield return new WaitForSeconds(1f);
        FadeIn(background, 1f);
        dayVotePanel.Close();
        yield return new WaitForSeconds(0.5f);
        winAnimation.Play();
        yield return new WaitForSeconds(1.5f);
        
        endTutorialPanel.SetActive(true);
        FadeIn(endTutorialText, 1.2f);
        yield return new WaitForSeconds(2f);
        LoadingManager.Instance.LoadMainScene();
        //EndGameVictory();
    }

    public void SendVote(int id)
    {
        selectedPlayerId = id;
        waitForNext = false;
    }

    public void AddVoteForPlayer(int _voteCount)
    {
        playersDayVote[selectedPlayerId].ShowVoteForTutorial(selectedPlayerId, _voteCount, false);
    }
    IEnumerator WaitForNextClick()
    {
        waitForNext = true;
        while (waitForNext)
            yield return null;
    }

    public void NextStepButton() 
    {
        waitForNext = false;
    }

    // ---- ТВОИ МЕТОДЫ ----
    
    #region  AnimationMethods
    public Tween FadeIn(CanvasGroup canvasGroup, float duration) // плавное появление
    {
        canvasGroup.alpha = 0f;
        canvasGroup.gameObject.SetActive(true);

        return canvasGroup
            .DOFade(1f, duration)
            .SetEase(Ease.OutQuad);
    }
    public Tween FadeOut(CanvasGroup canvasGroup, float duration) // плавное скрытие
    {
        return canvasGroup
            .DOFade(0f, duration)
            .SetEase(Ease.InQuad)
            .OnComplete(() => canvasGroup.gameObject.SetActive(false));
    }
    public Tween RotateForever(Transform target, float speed)
    {
        return target
            .DORotate(new Vector3(0, 0, 360f), speed, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }
    public void StopRotation(Transform target)
    {
        target.DOKill();
    }


    #endregion
    

}
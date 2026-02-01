using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI troopersText;
    [SerializeField] private TextMeshProUGUI masksText;
    [SerializeField] private TextMeshProUGUI grenadesText;
    [SerializeField] private TextMeshProUGUI gasText;
    [SerializeField] private TextMeshProUGUI barragesText;
    [SerializeField] private TextMeshProUGUI reinforcementsText;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTrooperText();
        UpdateMasksText();
        UpdateGrenadesText();
        UpdateGasText();
        UpdateBarragesText();
        UpdateReinforcementsText();
    }



    private void UpdateTrooperText()
    {
        int trooperCount = TeamManager.instance.GetTrooperList(TeamManager.Team.FRIENDLY).Count;
        troopersText.text = "" + trooperCount;
    }

    private void UpdateMasksText()
    {
        //int trooperCount = TeamManager.instance.GetTrooperList(TeamManager.Team.FRIENDLY).Count;
        //troopersText.text = "Troopers: " + trooperCount;

        masksText.text = "" + ItemManager.instance.currentMasks;
    }

    private void UpdateGrenadesText()
    {
        //int trooperCount = TeamManager.instance.GetTrooperList(TeamManager.Team.FRIENDLY).Count;
        //troopersText.text = "Troopers: " + trooperCount;

        grenadesText.text = "x  " + ItemManager.instance.currentFrags + " / " + ItemManager.instance.maxFrags;
    }

    private void UpdateGasText()
    {
        //int trooperCount = TeamManager.instance.GetTrooperList(TeamManager.Team.FRIENDLY).Count;
        //troopersText.text = "Troopers: " + trooperCount;

        gasText.text = "x  " + ItemManager.instance.currentGasBombs + " / " + ItemManager.instance.maxGasBombs;
    }

    private void UpdateBarragesText()
    {
        //int trooperCount = TeamManager.instance.GetTrooperList(TeamManager.Team.FRIENDLY).Count;
        //troopersText.text = "Troopers: " + trooperCount;

        barragesText.text = "Barrages [D]: " + ItemManager.instance.currentBarrages + "/" + ItemManager.instance.maxBarrages;
    }

    private void UpdateReinforcementsText()
    {
        reinforcementsText.text = "x  " + ItemManager.instance.currentReinforcements + " / " + ItemManager.instance.maxReinforcements;
    }
}

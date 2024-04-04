using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDefinition
{
    public const string eventMainLine = "MAIN_LINE";
    public const string eventNextMainLine = "NEXT_MAIN_LINE";
    public const string eventAutoSave = "EVENT_AUTO_SAVE";
    //背包
    public const string eventAddItemToBag = "EVENT_ADD_ITEM_TO_BAG";
    public const string eventRemoveItemToBag = "EVENT_REMOVE_ITEM_TO_BAG";
    public const string eventReviseMoneyToBag = "EVENT_REVISE_MONEY_TO_BAG";
    public const string eventOnClickedToBag = "EVENT_ONCLICKED_TO_BAG";
    //武器
    public const string eventOnClickedToWeapon = "EVENT_ONCLICKED_TO_WEAPON";
    //任務
    public const string eventOnClickedToQuest = "EVENT_ONCLICKED_TO_QUEST";
    public const string eventQuestActivate = "EVENT_QUEST_ACTIVATE";
    public const string eventQuestCompleted = "EVENT_QUEST_COMPLETED";
    public const string eventQuestNPCMove = "EVENT_QUEST_NPC_MOVE";
    //戰鬥
    public const string eventGameOver = "EVENT_GAME_OVER";
    public const string eventIsHited = "EVENT_IS_HITED";
    public const string eventPlayerInvincible = "EVENT_PLAYER_INVINCIBLE";
    public const string eventExecution = "EVENT_EXECUTION";
    public const string eventSceneLoading = "EVENT_SCENE_LOADING";
    public const string eventPlayerCantMove = "EVENT_PLAYER_CANT_MOVE";
    public const string eventPlayerBlock = "EVENT_PLAYER_BLOCK";
    //對話
    public const string eventDialogEvent = "EVENT_DIALOG_EVENT";

}

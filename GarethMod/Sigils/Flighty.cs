using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DiskCardGame;
using UnityEngine;
using InscryptionAPI.Card;

namespace GarethMod
{
    public partial class Plugin
    {
        private void AddFlighty()
        {
            AbilityInfo flighty = AbilityManager.New(
                PluginGuid + ".flighty",
                "Flighty",
                "MISSING DESC",
                typeof(Garethmod_Flighty),
                "garethmod_flighty.png"
            );
            flighty.powerLevel = 2;
            flighty.metaCategories = new List<AbilityMetaCategory> { AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part1Modular };
            List<DialogueEvent.Line> lines = new List<DialogueEvent.Line>();
            DialogueEvent.Line line = new DialogueEvent.Line
            {
                text = "Curious, I am unsure what this one does..."
            };
            lines.Add(line);
            flighty.abilityLearnedDialogue = new DialogueEvent.LineSet(lines);

            Garethmod_Flighty.ability = flighty.ability;
        }
    }
    public class Garethmod_Flighty : AbilityBehaviour
    {
        public override Ability Ability => ability;

        public static Ability ability;

        public override bool RespondsToAttackEnded()
        {
            return true;
        }

        public override IEnumerator OnAttackEnded()
        {
            List<CardSlot> validTargets = Singleton<BoardManager>.Instance.PlayerSlotsCopy;

            validTargets.RemoveAll((CardSlot x) => x.Card != null);

            if (validTargets.Count == 1) //Moves so the player doesnt have to think about it
            {
                CardSlot selectedSlot = validTargets.First();
                yield return base.StartCoroutine(this.MoveToSlot(selectedSlot));
                if (selectedSlot != null)
                {
                    yield return base.PreSuccessfulTriggerSequence();
                    yield return base.LearnAbility(0f);
                }
            } else if (validTargets.Count > 0) //otherwise you choose where to go
            {
                Singleton<ViewManager>.Instance.Controller.SwitchToControlMode(Singleton<BoardManager>.Instance.ChoosingSlotViewMode, false);
                Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
                CardSlot selectedSlot = null;
                List<CardSlot> allSlotsCopy = Singleton<BoardManager>.Instance.AllSlotsCopy;
                allSlotsCopy.Remove(base.Card.Slot);
                yield return Singleton<BoardManager>.Instance.ChooseTarget(allSlotsCopy, validTargets, delegate (CardSlot s)
                {
                    selectedSlot = s;
                }, new Action<CardSlot>(this.OnInvalidTargetSelected), delegate (CardSlot s)
                {
                    if (s.Card != null)
                    {
                        //Debug.Log("found card");
                    }
                }, null, CursorType.Place);
                {
                    yield return base.StartCoroutine(this.MoveToSlot(selectedSlot));
                    if (selectedSlot != null)
                    {
                        yield return base.PreSuccessfulTriggerSequence();
                        yield return base.LearnAbility(0f);
                    }
                }
            }
            else //play animation for cant move if its stuck
            {
                base.Card.Anim.StrongNegationEffect();
            }
            yield break;
        }

        protected IEnumerator MoveToSlot(CardSlot destination)
        {
            if (destination != null)
            {
                CardSlot oldSlot = base.Card.Slot;
                yield return Singleton<BoardManager>.Instance.AssignCardToSlot(base.Card, destination, 0.1f, null, true);
                yield return this.PostSuccessfulMoveSequence(oldSlot);
                yield return new WaitForSeconds(0.25f);
                oldSlot = null;
            }
            else
            {
                base.Card.Anim.StrongNegationEffect();
                yield return new WaitForSeconds(0.15f);
            }
            yield break;
        }

        protected virtual IEnumerator PostSuccessfulMoveSequence(CardSlot oldSlot)
        {
            yield break;
        }


        protected virtual void OnInvalidTargetSelected(CardSlot targetSlot)
        {
        }

    }
    /*
    public class Garethmod_Flighty : AbilityBehaviour
    {
        //Get passive attack buff add to a roster check for changes and adjust accordingly
        public override Ability Ability => ability;
        private void Start()
        {
            this.gamblingMod = new CardModificationInfo();
            this.gamblingMod.nonCopyable = true;
            this.gamblingMod.singletonId = "gambling";
            base.Card.AddTemporaryMod(this.gamblingMod);

            this.saviorMod = new CardModificationInfo();
            this.saviorMod.nonCopyable = true;
            this.saviorMod.singletonId = "savior";
            base.Card.AddTemporaryMod(this.saviorMod);
        }

        public static Ability ability;
        public override bool RespondsToResolveOnBoard()
        {
            return true;
        }

        public override bool RespondsToUpkeep(bool playerUpkeep)
        {
            return base.Card.OpponentCard != playerUpkeep;
        }

        public override IEnumerator OnResolveOnBoard()
        {
            randomizeStats();
            yield break;
        }
        public override IEnumerator OnUpkeep(bool playerUpkeep)
        {       
            randomizeStats();
            yield break;
        }

        public void randomizeStats()
        {
            /*
            int oneTimeChange = 0;
            Debug.Log("registry: " + registry);
            if(registry != base.Card.GetPassiveAttackBuffs())
            {
                if (base.Card.GetPassiveAttackBuffs() == 0)
                {
                    oneTimeChange = -registry;
                }
                else
                {
                    oneTimeChange = -base.Card.GetPassiveAttackBuffs();
                }
                registry = base.Card.GetPassiveAttackBuffs();
                Debug.Log("updated registry: " + registry);
            }
            */
}
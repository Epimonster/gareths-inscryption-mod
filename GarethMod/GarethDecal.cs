using DiskCardGame;
using InscryptionAPI.Card;
using InscryptionAPI.Helpers;
using UnityEngine;

namespace GarethMod
{
    public partial class Plugin
    {
        public class GarethmodDecalBehavior : CardAppearanceBehaviour
        {
            public override void ApplyAppearance()
            {
                Texture2D GarethmodDecal = TextureHelper.GetImageAsTexture("garethmodwatermark.png");
                base.Card.Info.TempDecals.Clear();
                base.Card.Info.TempDecals.Add(GarethmodDecal);
            }
        }

        // Pass the appearance behaviour to the API by its Id.
        public readonly static CardAppearanceBehaviour.Appearance GarethmodDecal = CardAppearanceBehaviourManager.Add(PluginGuid, "GarethmodDecal", typeof(GarethmodDecalBehavior)).Id;
    }
}

﻿using FModel.Creator.Bundles;
using FModel.Creator.Texts;
using System.Collections.Generic;
using FModel.PakReader;
using FModel.PakReader.Parsers.Class;
using FModel.PakReader.Parsers.PropertyTagData;

namespace FModel.Creator.Bases
{
    public class BaseBundle
    {
        public Header DisplayStyle;
        public string DisplayName;
        public string FolderName;
        public string Watermark;
        public int Width = 1024;
        public int HeaderHeight = 261; // height is the header basically
        public int AdditionalSize = 50; // must be increased depending on the number of quests to draw
        public bool IsDisplayNameShifted;
        public List<Quest> Quests;
        public List<CompletionReward> CompletionRewards;

        public BaseBundle()
        {
            DisplayStyle = new Header();
            DisplayName = "";
            FolderName = "";
            Watermark = Properties.Settings.Default.ChallengeBannerWatermark;
            Quests = new List<Quest>();
            CompletionRewards = new List<CompletionReward>();
        }

        /// <summary>
        /// used for the settings
        /// </summary>
        public BaseBundle(string watermark) : this()
        {
            DisplayName = "{DisplayName}";
            FolderName = "{FolderName}";
            Watermark = watermark;
            Quests.Add(new Quest { Description = "", Count = 999, Reward = null });
            AdditionalSize += 89;
        }

        public BaseBundle(IUExport export, string assetFolder) : this()
        {
            if (export.GetExport<StructProperty>("DisplayStyle") is StructProperty displayStyle)
                DisplayStyle = new Header(displayStyle, assetFolder);
            if (export.GetExport<TextProperty>("DisplayName") is TextProperty displayName)
                DisplayName = Text.GetTextPropertyBase(displayName);

            if (export.GetExport<ArrayProperty>("CareerQuestBitShifts") is ArrayProperty careerQuestBitShifts)
            {
                foreach (SoftObjectProperty questPath in careerQuestBitShifts.Value)
                {
                    Package p = Utils.GetPropertyPakPackage(questPath.Value.AssetPathName.String);
                    if (p.HasExport() && !p.Equals(default))
                    {
                        var obj = p.GetExport<UObject>();
                        if (obj != null)
                            Quests.Add(new Quest(obj));
                    }
                }
            }

            if (export.GetExport<ArrayProperty>("BundleCompletionRewards") is ArrayProperty bundleCompletionRewards)
            {
                foreach (StructProperty completionReward in bundleCompletionRewards.Value)
                {
                    if (completionReward.Value is UObject reward &&
                        reward.TryGetValue("CompletionCount", out var c) && c is IntProperty completionCount &&
                        reward.TryGetValue("Rewards", out var r) && r is ArrayProperty rewards)
                    {
                        foreach (StructProperty rew in rewards.Value)
                        {
                            if (rew.Value is UObject re &&
                                re.TryGetValue("Quantity", out var q) && q is IntProperty quantity &&
                                re.TryGetValue("TemplateId", out var t) && t is StrProperty templateId &&
                                re.TryGetValue("ItemDefinition", out var d) && d is SoftObjectProperty itemDefinition)
                            {
                                if (!itemDefinition.Value.AssetPathName.IsNone &&
                                    !itemDefinition.Value.AssetPathName.String.Contains("/Items/Tokens/") &&
                                    !itemDefinition.Value.AssetPathName.String.Contains("/Items/Quests"))
                                {
                                    CompletionRewards.Add(new CompletionReward(completionCount, quantity, itemDefinition));
                                }
                                else if (!string.IsNullOrEmpty(templateId.Value))
                                {
                                    CompletionRewards.Add(new CompletionReward(completionCount, quantity, templateId.Value));
                                }
                            }
                        }
                    }
                }
            }

            FolderName = assetFolder;
            AdditionalSize += 95 * Quests.Count;
            if (CompletionRewards.Count > 0) AdditionalSize += 50 + (95 * CompletionRewards.Count);
        }
    }
}

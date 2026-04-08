using System;
using System.IO;
using System.Reflection;
using SML;
using static System.Net.Mime.MediaTypeNames;
using UnityEngine;
using SalemModLoaderUI;
using Server.Shared.Extensions;
using System.Collections.Generic;

namespace TOSFactionColor
{
    [Mod.SalemMod]
    public class Main
    {
        public void Start() 
        {
            Console.WriteLine("Modding time!");
            try
            {
                DictionaryExtensions.SetValue(Settings.SettingsCache, "Apply to Last Will", ModSettings.GetBool("Apply to Last Will", "pokegustavo.RoleFactionChanger"));
                DictionaryExtensions.SetValue(Settings.SettingsCache, "Apply to Death Note", ModSettings.GetBool("Apply to Death Note", "pokegustavo.RoleFactionChanger"));
            }
            catch (Exception ex)
            {
                Console.WriteLine("The rainbow faction crashed the mod. Contact pokegustavo. Error: " + ex.Message);
            }
        }
    }

    [DynamicSettings]
    public class Settings
    {
        public static Dictionary<string, object> SettingsCache = new Dictionary<string, object>
        {
            {
                "Apply to Last Will",
                false
            },
            {
                "Apply to Death Note",
                false
            }
        };

        public ModSettings.CheckboxSetting LastWill
        {
            get
            {
                ModSettings.CheckboxSetting checkboxSetting = new ModSettings.CheckboxSetting
                {
                    Name = "Apply to Last Will",
                    Description = "Enables the mod for last wills, this will simplify all role mentions, player mentions, etc, to a code format.",
                    DefaultValue = false,
                    AvailableInGame = false,
                    Available = true,
                    OnChanged = delegate (bool b)
                    {
                        DictionaryExtensions.SetValue(SettingsCache, "Apply to Last Will", b);
                    }
                };
                return checkboxSetting;

            }
        }

        public ModSettings.CheckboxSetting DeathNote
        {
            get
            {
                ModSettings.CheckboxSetting checkboxSetting = new ModSettings.CheckboxSetting
                {
                    Name = "Apply to Death Note",
                    Description = "Enables the mod for death notes, this will simplify all role mentions, player mentions, etc, to a code format.",
                    DefaultValue = false,
                    AvailableInGame = false,
                    Available = true,
                    OnChanged = delegate (bool b)
                    {
                        DictionaryExtensions.SetValue(SettingsCache, "Apply to Death Note", b);
                    }
                };
                return checkboxSetting;

            }
        }
    }
}

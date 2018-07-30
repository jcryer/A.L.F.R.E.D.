using System;
using System.Collections.Generic;
using System.Text;

namespace RPBot
{
    public static class WikiTests
    {
        public static string CreateInfoBox(InfoBoxObject o)
        {
            string response = $@"<aside class=""portable-infobox pi-background pi-europa pi-theme-wikia pi-layout-default""><h2 class=""pi-item pi-item-spacing pi-title"">{o.Alias}</h2><div class=""pi-image-collection""><ul class=""pi-image-collection-tabs"">";
            for (int i = 0; i < o.ImageNames.Length; i++)
            {
                response += $@"<li class=""pi-tab-link pi-item-spacing"" data-pi-tab=""pi-tab-{i}"">{o.ImageNames[i]}</li>";
            }
            response += "</ul>";
            for (int i = 0; i < o.ImagePaths.Length; i++)
            {
                response += $@"<div class=""pi-image-collection-tab-content"" id=""pi-tab-{i}""><figure class=""pi-item pi-image""><img src=""{o.ImagePaths[i]}"" class=""pi-image-thumbnail""></figure></div>";
            }

            response += $@"</div><div class=""pi-item pi-data pi-item-spacing pi-border-color""><h3 class=""pi-data-label pi-secondary-font"">Civilian Name</h3><div class=""pi-data-value pi-font"">{o.CivilianName}</div></div>";

            response += $@"<div class=""pi-item pi-data pi-item-spacing pi-border-color""><h3 class=""pi-data-label pi-secondary-font"">Relatives</h3><div class=""pi-data-value pi-font"">{o.Relatives}</div></div>";

            response += $@"<div class=""pi-item pi-data pi-item-spacing pi-border-color""><h3 class=""pi-data-label pi-secondary-font"">Alignment</h3><div class=""pi-data-value pi-font"">{o.Alignment}</div></div>";

            response += $@"<section class=""pi-item pi-group pi-border-color""><h2 class=""pi-item pi-header pi-secondary-font pi-item-spacing pi-secondary-background"">Biographical Information</h2>";

            response += $@"<div class=""pi-item pi-data pi-item-spacing pi-border-color""><h3 class=""pi-data-label pi-secondary-font"">Marital Status</h3><div class=""pi-data-value pi-font"">{o.Relationship}</div></div>";

            response += $@"<div class=""pi-item pi-data pi-item-spacing pi-border-color""><h3 class=""pi-data-label pi-secondary-font"">Age</h3><div class=""pi-data-value pi-font"">{o.Age}</div></div>";

            response += $@"<div class=""pi-item pi-data pi-item-spacing pi-border-color""><h3 class=""pi-data-label pi-secondary-font"">Date of Birth</h3><div class=""pi-data-value pi-font"">{o.DOB}</div></div>";

            response += $@"<div class=""pi-item pi-data pi-item-spacing pi-border-color""><h3 class=""pi-data-label pi-secondary-font"">Place of Birth</h3><div class=""pi-data-value pi-font"">{o.POB}</div></div>";

            response += $@"<div class=""pi-item pi-data pi-item-spacing pi-border-color""><h3 class=""pi-data-label pi-secondary-font"">Living Quarters</h3><div class=""pi-data-value pi-font"">{o.LivingQuarters}</div></div>";

            response += $@"</section><section class=""pi-item pi-group pi-border-color""><h2 class=""pi-item pi-header pi-secondary-font pi-item-spacing pi-secondary-background"">Physical Description</h2>";

            response += $@"<div class=""pi-item pi-data pi-item-spacing pi-border-color""><h3 class=""pi-data-label pi-secondary-font"">Species</h3><div class=""pi-data-value pi-font"">{o.Species}</div></div>";

            response += $@"<div class=""pi-item pi-data pi-item-spacing pi-border-color""><h3 class=""pi-data-label pi-secondary-font"">Gender</h3><div class=""pi-data-value pi-font"">{o.Gender}</div></div>";

            response += $@"<div class=""pi-item pi-data pi-item-spacing pi-border-color""><h3 class=""pi-data-label pi-secondary-font"">Height</h3><div class=""pi-data-value pi-font"">{o.Height}</div></div>";

            response += $@"<div class=""pi-item pi-data pi-item-spacing pi-border-color""><h3 class=""pi-data-label pi-secondary-font"">Weight</h3><div class=""pi-data-value pi-font"">{o.Weight}</div></div>";

            response += $@"<div class=""pi-item pi-data pi-item-spacing pi-border-color""><h3 class=""pi-data-label pi-secondary-font"">Hair Colour</h3><div class=""pi-data-value pi-font"">{o.HairColour}</div></div>";

            response += $@"<div class=""pi-item pi-data pi-item-spacing pi-border-color""><h3 class=""pi-data-label pi-secondary-font"">Eye Colour</h3><div class=""pi-data-value pi-font"">{o.EyeColour}</div></div>";

            response += "</section></aside>";
            return response;
        }

        public static string BuildBody (BodyObject o)
        {
            string response = $@"<h2><span class=""mw-headline"" id=""Personality""> Personality </span></h2>{o.Personality}";

            response += $@"<h2><span class=""mw-headline"" id=""Backstory""> Backstory </span></h2>{o.Backstory}";

            response += $@"<h2><span class=""mw-headline"" id=""Resources""> Resources </span></h2>{o.Resources}";

            response += $@"a";
            return response;
        }
    }

    public class InfoBoxObject
    {
        public string CivilianName;
        public string Alias;
        public string Relatives;
        public string Alignment;
        public string Relationship;
        public int Age;
        public string DOB;
        public string LivingQuarters;
        public string POB;
        public string Species;
        public string Gender;
        public string Height;
        public string Weight;
        public string HairColour;
        public string EyeColour;
        public string[] ImageNames;
        public string[] ImagePaths;

        public InfoBoxObject(string civilianName, string alias, string relatives, string alignment, string relationship, int age, string dOB, string livingQuarters, string pOB, string species, string gender, string height, string weight, string hairColour, string eyeColour, string[] imageNames, string[] imagePaths)
        {
            CivilianName = civilianName;
            Alias = alias;
            Relatives = relatives;
            Alignment = alignment;
            Relationship = relationship;
            Age = age;
            DOB = dOB;
            LivingQuarters = livingQuarters;
            POB = pOB;
            Species = species;
            Gender = gender;
            Height = height;
            Weight = weight;
            HairColour = hairColour;
            EyeColour = eyeColour;
            ImageNames = imageNames;
            ImagePaths = imagePaths;
        }
    }

    public class BodyObject
    {
        public string Personality;
        public string Backstory;
        public string Resources;
        public string Equipment;
        public string Specialisations;
        public int SourceOfPower;
        public string Power;

        public BodyObject(string personality, string backstory, string resources, string equipment, string specialisations, int sourceOfPower, string power)
        {
            Personality = personality;
            Backstory = backstory;
            Resources = resources;
            Equipment = equipment;
            Specialisations = specialisations;
            SourceOfPower = sourceOfPower;
            Power = power;
        }
    }
}

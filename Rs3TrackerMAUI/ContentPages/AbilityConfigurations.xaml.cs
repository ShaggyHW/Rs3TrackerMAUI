using HtmlAgilityPack;
using Microsoft.Maui.ApplicationModel;
using Newtonsoft.Json;
using Rs3TrackerMAUI.Classes;
using System.Runtime;
using static Rs3TrackerMAUI.Classes.DisplayClasses;

namespace Rs3TrackerMAUI.ContentPages;

public partial class AbilityConfigurations : ContentPage {
    private List<Ability> abilities = new List<Ability>();
    List<Ability> abils = new List<Ability>();
    string mainDir = "";
    public AbilityConfigurations() {
        InitializeComponent();
#if WINDOWS
            mainDir = AppDomain.CurrentDomain.BaseDirectory;
#endif
#if MACCATALYST
        mainDir = AppDomain.CurrentDomain.BaseDirectory.Replace("Rs3TrackerMAUI.app/Contents/MonoBundle", "");
#endif
        if (!Directory.Exists(mainDir + "Images"))
            Directory.CreateDirectory(mainDir + "Images");
        if (!Directory.Exists(mainDir + "PersonalImages"))
            Directory.CreateDirectory(mainDir + "PersonalImages");
        Loaded += AbilityConfigurations_Loaded;
        
    }

    private void AbilityConfigurations_Loaded(object sender, EventArgs e) {
        if (File.Exists(mainDir + "mongoAbilities.json")) {
            abilities = JsonConvert.DeserializeObject<List<Ability>>(File.ReadAllText(mainDir + "mongoAbilities.json"));
            if (abilities != null) {
                //var keybinds = abilities.OrderBy(i => i.name).ToList();
                //foreach (var key in keybinds) {
                //    //dgSettings.Items.Add(key);
                //}
                abilities = abilities.OrderBy(i => i.name).ToList();
                dgSettings.ItemsSource = abilities;
            }
        }

        LoadCombo();
    }

    public class Abilities {
        public string Content { get; set; }
        public string Tag { get; set; }
    }

    public List<Abilities> abilitiesList = new List<Abilities>();
    private void LoadCombo() {
        Images.ItemsSource = null;
        var Abils = Directory.GetFiles(mainDir + "Images", "*.*").Where(s => s.ToLower().EndsWith(".png") || s.ToLower().EndsWith(".jpg")).ToList();

        foreach (var name in Abils) {
            var index = name.LastIndexOf('\\');
            PickOptions ComboBoxItem = new PickOptions();

            string finalName = name.Substring(index + 1, name.Length - index - 1);
            abilitiesList.Add(new Abilities() {
                Content = finalName.Split('.')[0],
                Tag = "Images"
            });
            //Images.Items.Add(finalName.Split('.')[0]);
        }

        Abils = Directory.GetFiles(mainDir + "PersonalImages", "*.*").Where(s => s.ToLower().EndsWith(".png") || s.ToLower().EndsWith(".jpg")).ToList();
        foreach (var name in Abils) {
            var index = name.LastIndexOf('\\');
            PickOptions ComboBoxItem = new PickOptions();

            string finalName = name.Substring(index + 1, name.Length - index - 1);
            abilitiesList.Add(new Abilities() {
                Content = finalName.Split('.')[0],
                Tag = "Images"
            });
        }

        Images.ItemsSource = abilitiesList;
    }
    private void Images_SelectedIndexChanged(object sender, EventArgs e) {
        if (Images.SelectedIndex != -1) {

            var selection = Images.Items[Images.SelectedIndex];
            var ability = abilitiesList.Where(a => a.Content.Equals(selection)).FirstOrDefault();

            ImageSource imageSource;
            var img = Path.Combine(mainDir, ability.Tag, ability.Content + ".png");
            imageSource = ImageSource.FromFile(img);
            //imageSource = ImageSourceFromBitmap(bitmap);
            imgAbil.Source = imageSource;
            txtAbilName.Text = ability.Content.Replace("_", " ");
        } else {
            imgAbil.Source = null;
        }
    }
    //TODO
    //UPDATE PATHING
    public void GetAbils() {
        WikiParser wikiParser = new WikiParser();
        string Code = wikiParser.getHTMLCode("Abilities");
        var doc = new HtmlDocument();
        doc.LoadHtml(Code);
        var tables = doc.DocumentNode.SelectNodes("//table[@class='wikitable sortable']");
        abils = new List<Ability>();
        List<Task> tasks = new List<Task>();
        for (int k = 0; k < tables.Count(); k++) {
            var table = tables[k];
            string type = "";
            switch (k) {
                case 0:
                    type = "Melee_";
                    break;
                case 1:
                    type = "Melee_";
                    break;
                case 2:
                    type = "Range_";
                    break;
                case 3:
                    type = "Mage_";
                    break;
                case 4:
                    type = "Necromancy_";

                    break;
                case 5:
                    type = "Defense_";

                    break;
                case 6:
                    type = "Constitution_";
                    break;
            }
            for (int i = 1; i < table.ChildNodes.Count(); i++) {
                for (int j = 2; j < table.ChildNodes[i].ChildNodes.Count(); j += 2) {
                    string name = table.ChildNodes[i].ChildNodes[j].ChildNodes[1].InnerText.Replace("\n", "").Trim();

                    string imgURL = "";
                    try {
                        string imgHTML = table.ChildNodes[i].ChildNodes[j].ChildNodes[3].InnerHtml.Replace("\n", "").Trim();
                        var index = imgHTML.IndexOf("srcset");

                        string htmlrest = imgHTML.Substring(index, imgHTML.Length - 1 - index).Replace("srcset=\"", "");
                        index = htmlrest.IndexOf("?");
                        imgURL = htmlrest.Substring(0, index);


                    } catch (Exception ex) {
                        string imgHTML = table.ChildNodes[i].ChildNodes[j].ChildNodes[3].InnerHtml.Replace("\n", "").Trim();
                        var index = imgHTML.IndexOf("src");

                        string htmlrest = imgHTML.Substring(index, imgHTML.Length - 1 - index).Replace("src=\"", "");
                        index = htmlrest.IndexOf("?");
                        imgURL = htmlrest.Substring(0, index);
                    }
                    if (!imgURL.Contains("images") || !imgURL.Contains(".png")) {
                        imgURL = "";
                    }
                    if (type.Equals("Necromancy_") && j.Equals(2)) {
                        name = name + "_Auto";
                    }
                    string coolDown = "";
                    if (type.Equals("Necromancy_")) {
                        coolDown = table.ChildNodes[i].ChildNodes[j].ChildNodes[17].InnerText.Replace("\n", "").Trim();
                    } else {
                        coolDown = table.ChildNodes[i].ChildNodes[j].ChildNodes[15].InnerText.Replace("\n", "").Trim();
                    }
                    double CD = 0;
                    try {
                        CD = Convert.ToDouble(coolDown);
                    } catch { }
                    //tasks.Add(Task.Factory.StartNew(() => SetAbility(wikiParser, name, type, CD, imgURL)));
                    SetAbility(wikiParser, name, type, CD, imgURL);
                }
            }
        }

        Code = wikiParser.getHTMLCode("Ancient_Curses");
        doc = new HtmlDocument();
        doc.LoadHtml(Code);
        tables = doc.DocumentNode.SelectNodes("//table[@class='wikitable sticky-header align-left-2 align-left-4']");
        foreach (var table in tables) {
            for (int i = 1; i < table.ChildNodes.Count(); i++) {
                for (int j = 4; j < table.ChildNodes[i].ChildNodes.Count(); j += 2) {
                    string name = table.ChildNodes[i].ChildNodes[j].ChildNodes[3].InnerText.Replace("\n", "").Trim();
                    string imgURL = "";
                    try {
                        string imgHTML = table.ChildNodes[i].ChildNodes[j].ChildNodes[1].InnerHtml.Replace("\n", "").Trim();
                        var index = imgHTML.IndexOf("srcset");

                        string htmlrest = imgHTML.Substring(index, imgHTML.Length - 1 - index).Replace("srcset=\"", "");
                        index = htmlrest.IndexOf("?");
                        imgURL = htmlrest.Substring(0, index);


                    } catch (Exception ex) {
                        string imgHTML = table.ChildNodes[i].ChildNodes[j].ChildNodes[1].InnerHtml.Replace("\n", "").Trim();
                        var index = imgHTML.IndexOf("src");

                        string htmlrest = imgHTML.Substring(index, imgHTML.Length - 1 - index).Replace("src=\"", "");
                        index = htmlrest.IndexOf("?");
                        imgURL = htmlrest.Substring(0, index);
                    }
                    if (!imgURL.Contains("images") || !imgURL.Contains(".png")) {
                        imgURL = "";
                    }
                    //tasks.Add(Task.Factory.StartNew(() => SetAbility(wikiParser, name, "Curses_", 0, imgURL)));
                    SetAbility(wikiParser, name, "Curses_", 0, imgURL);
                }
            }
        }

        Code = wikiParser.getHTMLCode("Prayer");
        doc = new HtmlDocument();
        doc.LoadHtml(Code);
        tables = doc.DocumentNode.SelectNodes("//table[@class='wikitable sticky-header sortable align-left-7']");
        foreach (var table in tables) {
            for (int i = 1; i < table.ChildNodes.Count(); i++) {
                for (int j = 2; j < table.ChildNodes[i].ChildNodes.Count(); j += 2) {
                    //Ability ability = new Ability();
                    string name = table.ChildNodes[i].ChildNodes[j].ChildNodes[1].InnerText.Replace("\n", "").Trim();

                    string imgURL = "";
                    try {
                        string imgHTML = table.ChildNodes[i].ChildNodes[j].ChildNodes[3].InnerHtml.Replace("\n", "").Trim();
                        var index = imgHTML.IndexOf("srcset");

                        string htmlrest = imgHTML.Substring(index, imgHTML.Length - 1 - index).Replace("srcset=\"", "");
                        index = htmlrest.IndexOf("?");
                        imgURL = htmlrest.Substring(0, index);


                    } catch (Exception ex) {
                        string imgHTML = table.ChildNodes[i].ChildNodes[j].ChildNodes[3].InnerHtml.Replace("\n", "").Trim();
                        var index = imgHTML.IndexOf("src");

                        string htmlrest = imgHTML.Substring(index, imgHTML.Length - 1 - index).Replace("src=\"", "");
                        index = htmlrest.IndexOf("?");
                        imgURL = htmlrest.Substring(0, index);
                    }
                    if (!imgURL.Contains("images") || !imgURL.Contains(".png")) {
                        imgURL = "";
                    }
                    //tasks.Add(Task.Factory.StartNew(() => SetAbility(wikiParser, name, "Prayer_", 0, imgURL)));
                    SetAbility(wikiParser, name, "Prayer_", 0, imgURL);
                }
            }
        }

        Code = wikiParser.getHTMLCode("Standard_spells");
        doc = new HtmlDocument();
        doc.LoadHtml(Code);
        tables = doc.DocumentNode.SelectNodes("//table[@class='wikitable sortable align-center-2 align-left-7']");
        foreach (var table in tables) {
            for (int i = 1; i < table.ChildNodes.Count(); i++) {
                for (int j = 2; j < table.ChildNodes[i].ChildNodes.Count(); j += 2) {
                    //Ability ability = new Ability();
                    string name = table.ChildNodes[i].ChildNodes[j].ChildNodes[1].InnerText.Replace("\n", "").Trim();
                    string imgURL = "";
                    try {
                        string imgHTML = table.ChildNodes[i].ChildNodes[j].ChildNodes[3].InnerHtml.Replace("\n", "").Trim();
                        var index = imgHTML.IndexOf("srcset");

                        string htmlrest = imgHTML.Substring(index, imgHTML.Length - 1 - index).Replace("srcset=\"", "");
                        index = htmlrest.IndexOf("?");
                        imgURL = htmlrest.Substring(0, index);


                    } catch (Exception ex) {
                        string imgHTML = table.ChildNodes[i].ChildNodes[j].ChildNodes[3].InnerHtml.Replace("\n", "").Trim();
                        var index = imgHTML.IndexOf("src");

                        string htmlrest = imgHTML.Substring(index, imgHTML.Length - 1 - index).Replace("src=\"", "");
                        index = htmlrest.IndexOf("?");
                        imgURL = htmlrest.Substring(0, index);
                    }
                    if (!imgURL.Contains("images") || !imgURL.Contains(".png")) {
                        imgURL = "";
                    }
                    //tasks.Add(Task.Factory.StartNew(() => SetAbility(wikiParser, name, "Spells_", 0, imgURL)));
                    SetAbility(wikiParser, name, "Spells_", 0, imgURL);
                }
            }
        }


        Code = wikiParser.getHTMLCode("Ancient_Magicks");
        doc = new HtmlDocument();
        doc.LoadHtml(Code);
        tables = doc.DocumentNode.SelectNodes("//table[@class='wikitable sortable align-left-7']");
        foreach (var table in tables) {
            for (int i = 1; i < table.ChildNodes.Count(); i++) {
                for (int j = 2; j < table.ChildNodes[i].ChildNodes.Count(); j += 2) {
                    //Ability ability = new Ability();
                    string name = table.ChildNodes[i].ChildNodes[j].ChildNodes[1].InnerText.Replace("\n", "").Trim();
                    string imgURL = "";
                    try {
                        string imgHTML = table.ChildNodes[i].ChildNodes[j].ChildNodes[3].InnerHtml.Replace("\n", "").Trim();
                        var index = imgHTML.IndexOf("srcset");

                        string htmlrest = imgHTML.Substring(index, imgHTML.Length - 1 - index).Replace("srcset=\"", "");
                        index = htmlrest.IndexOf("?");
                        imgURL = htmlrest.Substring(0, index);


                    } catch (Exception ex) {
                        string imgHTML = table.ChildNodes[i].ChildNodes[j].ChildNodes[3].InnerHtml.Replace("\n", "").Trim();
                        var index = imgHTML.IndexOf("src");

                        string htmlrest = imgHTML.Substring(index, imgHTML.Length - 1 - index).Replace("src=\"", "");
                        index = htmlrest.IndexOf("?");
                        imgURL = htmlrest.Substring(0, index);
                    }
                    if (!imgURL.Contains("images") || !imgURL.Contains(".png")) {
                        imgURL = "";
                    }
                    //   tasks.Add(Task.Factory.StartNew(() => SetAbility(wikiParser, name, "Spells_", 0, imgURL)));
                    SetAbility(wikiParser, name, "Spells_", 0, imgURL);
                }
            }
        }

        Code = wikiParser.getHTMLCode("Lunar_spells");
        doc = new HtmlDocument();
        doc.LoadHtml(Code);
        tables = doc.DocumentNode.SelectNodes("//table[@class='wikitable sortable align-center-2 align-center-4 align-center-6']");
        foreach (var table in tables) {
            for (int i = 1; i < table.ChildNodes.Count(); i++) {
                for (int j = 2; j < table.ChildNodes[i].ChildNodes.Count(); j += 2) {
                    //Ability ability = new Ability();
                    string name = table.ChildNodes[i].ChildNodes[j].ChildNodes[1].InnerText.Replace("\n", "").Trim();

                    string imgURL = "";
                    try {
                        string imgHTML = table.ChildNodes[i].ChildNodes[j].ChildNodes[3].InnerHtml.Replace("\n", "").Trim();
                        var index = imgHTML.IndexOf("srcset");

                        string htmlrest = imgHTML.Substring(index, imgHTML.Length - 1 - index).Replace("srcset=\"", "");
                        index = htmlrest.IndexOf("?");
                        imgURL = htmlrest.Substring(0, index);


                    } catch (Exception ex) {
                        string imgHTML = table.ChildNodes[i].ChildNodes[j].ChildNodes[3].InnerHtml.Replace("\n", "").Trim();
                        var index = imgHTML.IndexOf("src");

                        string htmlrest = imgHTML.Substring(index, imgHTML.Length - 1 - index).Replace("src=\"", "");
                        index = htmlrest.IndexOf("?");
                        imgURL = htmlrest.Substring(0, index);
                    }
                    if (!imgURL.Contains("images") || !imgURL.Contains(".png")) {
                        imgURL = "";
                    }
                    //   tasks.Add(Task.Factory.StartNew(() => SetAbility(wikiParser, name, "Spells_", 0, imgURL)));
                    SetAbility(wikiParser, name, "Spells_", 0, imgURL);
                }
            }
        }

        Code = wikiParser.getHTMLCode("Incantations");
        doc = new HtmlDocument();
        doc.LoadHtml(Code);
        tables = doc.DocumentNode.SelectNodes("//table[@class='wikitable sortable align-left-7']");
        foreach (var table in tables) {
            for (int i = 1; i < table.ChildNodes.Count(); i++) {
                for (int j = 2; j < table.ChildNodes[i].ChildNodes.Count(); j += 2) {
                    //Ability ability = new Ability();
                    string name = table.ChildNodes[i].ChildNodes[j].ChildNodes[1].InnerText.Replace("\n", "").Replace("&#160;", "").Trim();
                    string coolDown = "";

                    coolDown = table.ChildNodes[i].ChildNodes[j].ChildNodes[9].InnerText.Replace("\n", "").Replace("seconds", "").Trim();
                    bool itsMinutes = false;
                    if (coolDown.Contains("minute")) {
                        itsMinutes = true;
                        coolDown = coolDown.Replace("minute", "");
                    }
                    double CD = 0;
                    try {
                        CD = Convert.ToDouble(coolDown);
                        if (itsMinutes)
                            CD = CD * 60;
                    } catch { }
                    string imgURL = "";
                    try {
                        string imgHTML = table.ChildNodes[i].ChildNodes[j].ChildNodes[1].InnerHtml.Replace("\n", "").Trim();
                        var index = imgHTML.IndexOf("srcset");

                        string htmlrest = imgHTML.Substring(index, imgHTML.Length - 1 - index).Replace("srcset=\"", "");
                        index = htmlrest.IndexOf("?");
                        imgURL = htmlrest.Substring(0, index);


                    } catch (Exception ex) {
                        string imgHTML = table.ChildNodes[i].ChildNodes[j].ChildNodes[3].InnerHtml.Replace("\n", "").Trim();
                        var index = imgHTML.IndexOf("src");

                        string htmlrest = imgHTML.Substring(index, imgHTML.Length - 1 - index).Replace("src=\"", "");
                        index = htmlrest.IndexOf("?");
                        imgURL = htmlrest.Substring(0, index);
                    }
                    if (!imgURL.Contains("images") || !imgURL.Contains(".png")) {
                        imgURL = "";
                    }

                    SetAbility(wikiParser, name, "Spells_", CD, imgURL);
                    //tasks.Add(Task.Factory.StartNew(() => SetAbility(wikiParser, name, "Spells_", CD, imgURL)));
                }
            }
        }

        //Task.WaitAll(tasks.ToArray());
        var preImport = JsonConvert.DeserializeObject<List<Ability>>(File.ReadAllText(mainDir+"mongoAbilities.json"));
        if (preImport != null) {
            for (int i = 0; i < preImport.Count(); i++) {
                if (preImport[i].name.Contains("_Import")) {
                    preImport.RemoveAt(i);
                    i--;
                }
            }

            preImport.AddRange(abils);
        } else {
            preImport = abils;
        }
        var stream = File.Create(mainDir+"mongoAbilities.json");
        stream.Close();
        File.WriteAllText(mainDir+"mongoAbilities.json", JsonConvert.SerializeObject(preImport, Formatting.Indented));

        LoadCombo();
        var abilsOrder = abils.OrderBy(i => i.name).ToList();
        //TODO
        //dgSettings.Items.Clear();
        //foreach (var ab in abilsOrder) {
        //    dgSettings.Items.Add(ab);
        //}
        dgSettings.ItemsSource = abilsOrder;

        DisplayAlert("INFO","ABILITIES IMPORTED","OK");
    }
    private void SetAbility(WikiParser wikiParser, string name, string table = "", double cooldown = 0, string imgURL = "") {
        try {
            Ability ability = new Ability();
            string fileName = "";
            if (string.IsNullOrEmpty(imgURL)) {
                if (table.Equals("Spells_"))
                    fileName = wikiParser.SaveImage(name + "_icon");
                else
                    fileName = wikiParser.SaveImage(name);
            } else {
                fileName = wikiParser.SaveImageFROMURL(name, imgURL);
            }
            if (string.IsNullOrEmpty(fileName))
                return;
            //string img = table.ChildNodes[i].ChildNodes[2].ChildNodes[3].InnerText.Replace("\n", "");                            
            ability.name = table + name + "_Import";
            ability.cooldown = cooldown;
            ability.img = Path.Combine(mainDir+"Images", fileName + ".png");
            abils.Add(ability);
        } catch (Exception ex) {

        }
    }

    private async void Import_Clicked(object sender, EventArgs e) {
        var answer = await DisplayAlert("Warning", "This is going to replace all the abilities! are you sure you want to continue?", "Continue", "Cancel");
        if (answer) {
            ActivityIndicator activityIndicator = new ActivityIndicator { IsRunning = true };
            GetAbils();
            activityIndicator.IsRunning = false;
        }
      
    }

    private void btnDelete_Clicked(object sender, EventArgs e) {

    }

    private void btnSave_Clicked(object sender, EventArgs e) {

    }

    private void reloadCombo_Clicked(object sender, EventArgs e) {

    }

    private void btnAdd_Clicked(object sender, EventArgs e) {

    }


}
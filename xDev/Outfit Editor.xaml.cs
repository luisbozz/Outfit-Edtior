using Microsoft.Win32;
using mry;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;



namespace xDev
{
    /// <summary>
    /// Interaction logic for Outfit Editor.xaml
    /// </summary>
    public partial class OutfitEditor : Window
    {
        public static BackgroundWorker mWorker;
        public static DispatcherTimer timercheckgta = new DispatcherTimer();
        mem m = new mem();
        public OutfitEditor()
        {
            InitializeComponent();
        }

        string offset_url = "https://amethxst.dev/offset_xdev_1.71.txt";
        string masks = "https://raw.githubusercontent.com/root-cause/v-clothingnames/master/masks.json";
        string female_accessories = "https://raw.githubusercontent.com/root-cause/v-clothingnames/master/female_accessories.json";
        string female_hair = "https://raw.githubusercontent.com/root-cause/v-clothingnames/master/female_hair.json";
        string female_legs = "https://raw.githubusercontent.com/root-cause/v-clothingnames/master/female_legs.json";
        string female_shoes = "https://raw.githubusercontent.com/root-cause/v-clothingnames/master/female_shoes.json";
        string female_tops = "https://raw.githubusercontent.com/root-cause/v-clothingnames/master/female_tops.json";
        string female_torsos = "https://raw.githubusercontent.com/root-cause/v-clothingnames/master/female_torsos.json";
        string female_undershirts = "https://raw.githubusercontent.com/root-cause/v-clothingnames/master/female_undershirts.json";
        string male_accessories = "https://raw.githubusercontent.com/root-cause/v-clothingnames/master/male_accessories.json";
        string male_hair = "https://raw.githubusercontent.com/root-cause/v-clothingnames/master/male_hair.json";
        string male_legs = "https://raw.githubusercontent.com/root-cause/v-clothingnames/master/male_legs.json";
        string male_shoes = "https://raw.githubusercontent.com/root-cause/v-clothingnames/master/male_shoes.json";
        string male_tops = "https://raw.githubusercontent.com/root-cause/v-clothingnames/master/male_tops.json";
        string male_torsos = "https://raw.githubusercontent.com/root-cause/v-clothingnames/master/male_torsos.json";
        string male_undershirts = "https://raw.githubusercontent.com/root-cause/v-clothingnames/master/male_undershirts.json";
        string props_female_bracelets = "https://raw.githubusercontent.com/root-cause/v-clothingnames/master/props_female_bracelets.json";
        string props_female_ears = "https://raw.githubusercontent.com/root-cause/v-clothingnames/master/props_female_ears.json";
        string props_female_glasses = "https://raw.githubusercontent.com/root-cause/v-clothingnames/master/props_female_glasses.json";
        string props_female_hats = "https://raw.githubusercontent.com/root-cause/v-clothingnames/master/props_female_hats.json";
        string props_female_watches = "https://raw.githubusercontent.com/root-cause/v-clothingnames/master/props_female_watches.json";
        string props_male_bracelets = "https://raw.githubusercontent.com/root-cause/v-clothingnames/master/props_male_bracelets.json";
        string props_male_ears = "https://raw.githubusercontent.com/root-cause/v-clothingnames/master/props_male_ears.json";
        string props_male_glasses = "https://raw.githubusercontent.com/root-cause/v-clothingnames/master/props_male_glasses.json";
        string props_male_hats = "https://raw.githubusercontent.com/root-cause/v-clothingnames/master/props_male_hats.json";
        string props_male_watches = "https://raw.githubusercontent.com/root-cause/v-clothingnames/master/props_male_watches.json";

        long OUTFIT_version = 0x0;
        long WORLD_version = 0x0;

        string AOB_WORLD = "";
        string AOB_OUTFIT = "";

        int OFFSET_imp = 0x14760;
        int OFFSET_name = 0x14C30;
        int OFFSET_mask = 0x12920;
        int OFFSET_hair = 0x12928;
        int OFFSET_gloves = 0x12930;
        int OFFSET_legs = 0x12938;
        int OFFSET_para = 0x12940;
        int OFFSET_feet = 0x12948;
        int OFFSET_misc = 0x12950;
        int OFFSET_top2 = 0x12958;
        int OFFSET_armor = 0x12960;
        int OFFSET_decals = 0x12968;
        int OFFSET_top = 0x12970;
        int OFFSET_hat = 0x13A38;
        int OFFSET_glasses = 0x13A40;
        int OFFSET_earrings = 0x13A48;
        int OFFSET_watches = 0x13A68;
        int OFFSET_bracelets = 0x13A68;

        int OFFSET_texture = 0x890;
        int OFFSET_texture_head = 0x698;

        int[] OFFSET_spoof;

        private bool _suspendOutfitUi = false;

        //LINK Xenvious & yAmethxst
        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hyperlink = (Hyperlink)sender;
            try
            {
                System.Diagnostics.Process.Start(hyperlink.NavigateUri.AbsoluteUri);
            }
            catch (Exception)
            {
                //
            }
        }

        //DISCORD
        private void BtnDiscord_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://discord.com/invite/UwAsC6GSJk/");
        }


        //EXPORT V1
        private void ExportButton_Click1(object sender, RoutedEventArgs e)
        {
            string outfitName = tbname.Text;

            StringBuilder fileContent = new StringBuilder($"name = {outfitName}\n");

            Dictionary<string, string> categoryMap = new Dictionary<string, string>
    {
        { "top", "Top" },
        { "top2", "Undershirt" },
        { "legs", "Legs" },
        { "feet", "Feet" },
        { "misc", "Accessories" },
        { "para", "Bags" },
        { "mask", "Mask" },
        { "gloves", "Gloves" },
        { "decals", "Decals" },
        { "armor", "Armor" },
        { "hat", "Hats" },
        { "glasses", "Glasses" },
        { "ears", "Ears" },
        { "watches", "Watches" },
        { "bracelets", "Bracelets" }

    };

            foreach (string internalName in categoryMap.Keys)
            {
                TextBox idTextBox = this.FindName($"tb{internalName}") as TextBox;
                TextBox textureTextBox = this.FindName($"tb{internalName}texture") as TextBox;
                string id = idTextBox?.Text;
                string texture = textureTextBox?.Text;

                if (id != null && texture != null)
                {
                    string displayName = categoryMap[internalName];
                    fileContent.AppendLine($"{displayName} = {id}/{texture}");
                }
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text File (*.txt)|*.txt";
            saveFileDialog.FileName = outfitName;

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;

                File.WriteAllText(filePath, fileContent.ToString());
            }
        }

        //EXPORT V2
        private void ExportButton_Click2(object sender, RoutedEventArgs e)
        {
            string outfitName = tbname.Text;

            StringBuilder fileContent = new StringBuilder($"name = {outfitName}\n");

            Dictionary<string, string> categoryMap = new Dictionary<string, string>
    {
        { "top", "Top" },
        { "top2", "Undershirt" },
        { "legs", "Legs" },
        { "feet", "Feet" },
        { "misc", "Accessories" },
        { "para", "Bags" },
        { "mask", "Mask" },
        { "gloves", "Gloves" },
        { "decals", "Decals" },
        { "armor", "Armor" },
        { "hat", "Hats" },
        { "glasses", "Glasses" },
        { "ears", "Ears" },
        { "watches", "Watches" },
        { "bracelets", "Bracelets" }


    };

            foreach (string internalName in categoryMap.Keys)
            {
                TextBox idTextBox = this.FindName($"tb{internalName}") as TextBox;
                TextBox textureTextBox = this.FindName($"tb{internalName}texture") as TextBox;
                string id = idTextBox?.Text;
                string texture = textureTextBox?.Text;

                if (id != null && texture != null)
                {
                    string displayName = categoryMap[internalName];
                    fileContent.AppendLine($"{displayName} = {id}");
                    fileContent.AppendLine($"{displayName} variation = {texture}");
                }
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text File (*.txt)|*.txt";
            saveFileDialog.FileName = outfitName;

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;

                File.WriteAllText(filePath, fileContent.ToString());
            }
        }


        //IMPORT
        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text File (*.txt)|*.txt";

            if (openFileDialog.ShowDialog() == true)
            {
                string[] fileLines = File.ReadAllLines(openFileDialog.FileName);
                List<string> errorMessages = new List<string>();

                tbname.Text = "yAmethxs on X";
                ResetTextBoxes();

                HashSet<string> processedCategories = new HashSet<string>();

                foreach (string line in fileLines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    if (line.ToLower().StartsWith("name"))
                    {
                        string[] nameParts = line.Split('=',':');
                        tbname.Text = nameParts.Length > 1 && !string.IsNullOrWhiteSpace(nameParts[1]) ? nameParts[1].Trim() : "yAmethxs on X";
                        continue;
                    }

                    string[] parts = line.Split(new char[] { '=', '/', ':'}, StringSplitOptions.RemoveEmptyEntries);

                    if (line.Contains("/"))
                    {
                        if (parts.Length != 3 || !int.TryParse(parts[1].Trim(), out _) || !int.TryParse(parts[2].Trim(), out _))
                        {
                            errorMessages.Add($"Format error in line: {line}");
                            continue;
                        }
                    }
                    else
                    {
                        if (parts.Length != 2 || !int.TryParse(parts[1].Trim(), out _))
                        {
                            errorMessages.Add($"Value missing or format error in line: {line}");
                            continue;
                        }
                    }

                    string categoryLower = parts[0].Trim().ToLower();
                    processedCategories.Add(categoryLower);
                    ApplyValue(categoryLower, parts[1].Trim(), true, errorMessages);

                    if (parts.Length == 3)
                    {
                        ApplyValue(categoryLower + " variation", parts[2].Trim(), true, errorMessages);
                    }
                }

                List<string> missingCategories = CheckForMissingCategories(processedCategories);

                string message = "File imported successfully.";

                if (missingCategories.Count > 0 || errorMessages.Count > 0)
                {
                    var missingCategoriesMsg = missingCategories.Count > 0 ? "Missing categories: " + string.Join(", ", missingCategories) : "";
                    var errorMessagesMsg = errorMessages.Count > 0 ? "Errors found:" : "";

                    message += "\nAttention: " + missingCategoriesMsg;

                    if (errorMessages.Count > 0)
                    {
                        message += "\n" + errorMessagesMsg;
                        foreach (var errorMessage in errorMessages)
                        {
                            message += "\n" + errorMessage;
                        }
                    }
                }

                MessageBox.Show(message);
                ;
            }
        }



        private List<string> CheckForMissingCategories(HashSet<string> processedCategories)
        {
            List<string> missingCategories = new List<string>();
            List<string> allCategories = new List<string>
    {
        "top", "undershirt", "legs", "feet", "accessories", "bags", "mask", "gloves", "decals", "armor", "hats", "glasses", "ears", "watches", "bracelets"
    };
            foreach (string category in allCategories)
            {
                string categoryCheck = category;
                if (category == "bracelets" && processedCategories.Contains("bracelet"))
                {
                    continue;
                }

                if (!processedCategories.Contains(categoryCheck))
                {
                    missingCategories.Add(category);
                }
            }
            return missingCategories;
        }



        private void ResetTextBoxes()
        {
            tbtop.Text = "X";
            tbtoptexture.Text = "X";
            tbtop2.Text = "X";
            tbtop2texture.Text = "X";
            tblegs.Text = "X";
            tblegstexture.Text = "X";
            tbfeet.Text = "X";
            tbfeettexture.Text = "X";
            tbmisc.Text = "X";
            tbmisctexture.Text = "X";
            tbpara.Text = "X";
            tbparatexture.Text = "X";
            tbmask.Text = "X";
            tbmasktexture.Text = "X";
            tbgloves.Text = "X";
            tbglovestexture.Text = "X";
            tbdecals.Text = "X";
            tbdecalstexture.Text = "X";
            tbarmor.Text = "X";
            tbarmortexture.Text = "X";
            tbhat.Text = "X";
            tbhattexture.Text = "X";
            tbglasses.Text = "X";
            tbglassestexture.Text = "X";
            tbears.Text = "X";
            tbearstexture.Text = "X";
            tbwatches.Text = "X";
            tbwatchestexture.Text = "X";
            tbbracelets.Text = "X";
            tbbraceletstexture.Text = "X";
        }


        private void ApplyValue(string category, string value, bool isFormat1, List<string> errorMessages)
        {
            // Gestione del singolare e plurale per "bracelets"
            if (category.Equals("bracelet", StringComparison.OrdinalIgnoreCase))
            {
                category = "bracelets";
            }
            else if (category.Equals("bracelet variation", StringComparison.OrdinalIgnoreCase))
            {
                category = "bracelets variation";
            }

            if (category.Equals("name", StringComparison.OrdinalIgnoreCase))
            {
                tbname.Text = string.IsNullOrWhiteSpace(value) ? "yAmethxs on X" : value;
                return;
            }

            TextBox idTextBox = FindTextBoxForCategory(category);
            if (idTextBox != null)
            {
                // Se il valore è vuoto, assegna "X" alla TextBox
                if (string.IsNullOrWhiteSpace(value))
                {
                    idTextBox.Text = "X";
                }
                else // Altrimenti, assegna il valore alla TextBox
                {
                    idTextBox.Text = value;
                }
            }
        }




        private TextBox FindTextBoxForCategory(string category)
        {
            switch (category.ToLower())
            {
                case "top":
                    return tbtop;
                case "top variation":
                    return tbtoptexture;
                case "undershirt":
                    return tbtop2;
                case "undershirt variation":
                    return tbtop2texture;
                case "legs":
                    return tblegs;
                case "legs variation":
                    return tblegstexture;
                case "feet":
                    return tbfeet;
                case "feet variation":
                    return tbfeettexture;
                case "accessories":
                    return tbmisc;
                case "accessories variation":
                    return tbmisctexture;
                case "bags":
                    return tbpara;
                case "bags variation":
                    return tbparatexture;
                case "mask":
                    return tbmask;
                case "mask variation":
                    return tbmasktexture;
                case "gloves":
                    return tbgloves;
                case "gloves variation":
                    return tbglovestexture;
                case "decals":
                    return tbdecals;
                case "decals variation":
                    return tbdecalstexture;
                case "armor":
                    return tbarmor;
                case "armor variation":
                    return tbarmortexture;
                case "hats":
                    return tbhat;
                case "hats variation":
                    return tbhattexture;
                case "glasses":
                    return tbglasses;
                case "glasses variation":
                    return tbglassestexture;
                case "ears":
                    return tbears;
                case "ears variation":
                    return tbearstexture;
                case "watches":
                    return tbwatches;
                case "watches variation":
                    return tbwatchestexture;
                case "bracelets":
                    return tbbracelets;
                case "bracelets variation":
                    return tbbraceletstexture;
                default:
                    return null;
            }
        }










        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timercheckgta.Interval = new TimeSpan(0, 0, 1);
            timercheckgta.Tick += Timercheckgta_Tick;
            timercheckgta.Start();

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            RenderOptions.ProcessRenderMode = RenderMode.Default;

            mWorker = new BackgroundWorker();
            mWorker.DoWork += new DoWorkEventHandler(worker_DoWork);

            Style s = new Style();
            s.Setters.Add(new Setter(UIElement.VisibilityProperty, Visibility.Collapsed));
            MainPages.ItemContainerStyle = s;

            Offsets();
            m.OpenProcess("gta5"); //try open process

            List<string> modules = getModules();

            if (m.IsProcOpen)
            {
                //if (modules.Contains("EOSSDK-Win64-Shipping.dll"))
                //{
                //    version = epic;
                //    tbspoof.Text = m.memory(OFFSET_name_epic).GetString(16);
                //}
                //else if (modules.Contains("steam_api64.dll"))
                //{
                //    version = steam;
                //    tbspoof.Text = m.memory(OFFSET_name_steam).GetString(16);
                //}
                //else
                //{
                //    version = rstar;
                //    tbspoof.Text = m.memory(OFFSET_name_rstar).GetString(16);
                //}

                OUTFIT_version = getOutfitPTR();
                WORLD_version = getWorldPTR();

                cbadv.IsChecked = true;

                Lblattach.Foreground = new SolidColorBrush(Colors.Green);
                Lblattach.Text = "Attached";
                Lblpid.Text = Process.GetProcessesByName("GTA5")[0].Id.ToString();
            }

            using (WebClient client = new WebClient())
            {
                //masks
                try
                {
                    JObject maskjson = JObject.Parse(client.DownloadString(new Uri(masks)).Replace(Environment.NewLine, ""));

                    var masksjson_desirialized = maskjson.ToObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>();

                    ObservableCollection<ClothingVariant> masks_variants = new ObservableCollection<ClothingVariant>();

                    foreach (var id in masksjson_desirialized)
                    {
                        int clothingid = int.Parse(id.Key);
                        foreach (var variant in id.Value)
                        {
                            masks_variants.Add(new ClothingVariant(clothingid, int.Parse(variant.Key), variant.Value.ElementAt(1).Value, variant.Value.ElementAt(0).Value));
                        }
                    }

                    ddmask.SetBinding(ComboBox.ItemsSourceProperty, new Binding { Source = masks_variants });
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }


                //accessories
                try
                {
                    JObject maskjson = JObject.Parse(client.DownloadString(new Uri(male_accessories)).Replace(Environment.NewLine, ""));

                    var masksjson_desirialized = maskjson.ToObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>();

                    ObservableCollection<ClothingVariant> masks_variants = new ObservableCollection<ClothingVariant>();

                    foreach (var id in masksjson_desirialized)
                    {
                        int clothingid = int.Parse(id.Key);
                        foreach (var variant in id.Value)
                        {
                            masks_variants.Add(new ClothingVariant(clothingid, int.Parse(variant.Key), variant.Value.ElementAt(1).Value, variant.Value.ElementAt(0).Value));
                        }
                    }

                    ddaccessories.SetBinding(ComboBox.ItemsSourceProperty, new Binding { Source = masks_variants });
                }
                catch (Exception) { }

                //tops
                try
                {
                    JObject maskjson = JObject.Parse(client.DownloadString(new Uri(male_tops)).Replace(Environment.NewLine, ""));

                    var masksjson_desirialized = maskjson.ToObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>();

                    ObservableCollection<ClothingVariant> masks_variants = new ObservableCollection<ClothingVariant>();

                    foreach (var id in masksjson_desirialized)
                    {
                        int clothingid = int.Parse(id.Key);
                        foreach (var variant in id.Value)
                        {
                            masks_variants.Add(new ClothingVariant(clothingid, int.Parse(variant.Key), variant.Value.ElementAt(1).Value, variant.Value.ElementAt(0).Value));
                        }
                    }

                    ddtops.SetBinding(ComboBox.ItemsSourceProperty, new Binding { Source = masks_variants });
                }
                catch (Exception) { }

                //undershirts
                try
                {
                    JObject maskjson = JObject.Parse(client.DownloadString(new Uri(male_undershirts)).Replace(Environment.NewLine, ""));

                    var masksjson_desirialized = maskjson.ToObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>();

                    ObservableCollection<ClothingVariant> masks_variants = new ObservableCollection<ClothingVariant>();

                    foreach (var id in masksjson_desirialized)
                    {
                        int clothingid = int.Parse(id.Key);
                        foreach (var variant in id.Value)
                        {
                            masks_variants.Add(new ClothingVariant(clothingid, int.Parse(variant.Key), variant.Value.ElementAt(1).Value, variant.Value.ElementAt(0).Value));
                        }
                    }

                    ddundershirts.SetBinding(ComboBox.ItemsSourceProperty, new Binding { Source = masks_variants });
                }
                catch (Exception) { }

                //legs
                try
                {
                    JObject maskjson = JObject.Parse(client.DownloadString(new Uri(male_legs)).Replace(Environment.NewLine, ""));

                    var masksjson_desirialized = maskjson.ToObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>();

                    ObservableCollection<ClothingVariant> masks_variants = new ObservableCollection<ClothingVariant>();

                    foreach (var id in masksjson_desirialized)
                    {
                        int clothingid = int.Parse(id.Key);
                        foreach (var variant in id.Value)
                        {
                            masks_variants.Add(new ClothingVariant(clothingid, int.Parse(variant.Key), variant.Value.ElementAt(1).Value, variant.Value.ElementAt(0).Value));
                        }
                    }

                    ddlegs.SetBinding(ComboBox.ItemsSourceProperty, new Binding { Source = masks_variants });
                }
                catch (Exception) { }

                //shoes
                try
                {
                    JObject maskjson = JObject.Parse(client.DownloadString(new Uri(male_shoes)).Replace(Environment.NewLine, ""));

                    var masksjson_desirialized = maskjson.ToObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>();

                    ObservableCollection<ClothingVariant> masks_variants = new ObservableCollection<ClothingVariant>();

                    foreach (var id in masksjson_desirialized)
                    {
                        int clothingid = int.Parse(id.Key);
                        foreach (var variant in id.Value)
                        {
                            masks_variants.Add(new ClothingVariant(clothingid, int.Parse(variant.Key), variant.Value.ElementAt(1).Value, variant.Value.ElementAt(0).Value));
                        }
                    }

                    ddshoes.SetBinding(ComboBox.ItemsSourceProperty, new Binding { Source = masks_variants });
                }
                catch (Exception) { }

                //hair
                try
                {
                    JObject maskjson = JObject.Parse(client.DownloadString(new Uri(male_hair)).Replace(Environment.NewLine, ""));

                    var masksjson_desirialized = maskjson.ToObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>();

                    ObservableCollection<ClothingVariant> masks_variants = new ObservableCollection<ClothingVariant>();

                    foreach (var id in masksjson_desirialized)
                    {
                        int clothingid = int.Parse(id.Key);
                        foreach (var variant in id.Value)
                        {
                            masks_variants.Add(new ClothingVariant(clothingid, int.Parse(variant.Key), variant.Value.ElementAt(1).Value, variant.Value.ElementAt(0).Value));
                        }
                    }

                    ddhair.SetBinding(ComboBox.ItemsSourceProperty, new Binding { Source = masks_variants });
                }
                catch (Exception) { }

                //torsos
                try
                {
                    JObject maskjson = JObject.Parse(client.DownloadString(new Uri(male_torsos)).Replace(Environment.NewLine, ""));

                    var masksjson_desirialized = maskjson.ToObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>();

                    ObservableCollection<ClothingVariant> masks_variants = new ObservableCollection<ClothingVariant>();

                    foreach (var id in masksjson_desirialized)
                    {
                        int clothingid = int.Parse(id.Key);
                        foreach (var variant in id.Value)
                        {
                            masks_variants.Add(new ClothingVariant(clothingid, int.Parse(variant.Key), variant.Value.ElementAt(1).Value, variant.Value.ElementAt(0).Value));
                        }
                    }

                    ddtorsos.SetBinding(ComboBox.ItemsSourceProperty, new Binding { Source = masks_variants });
                }
                catch (Exception) { }

                //ear
                try
                {
                    JObject maskjson = JObject.Parse(client.DownloadString(new Uri(props_male_ears)).Replace(Environment.NewLine, ""));

                    var masksjson_desirialized = maskjson.ToObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>();

                    ObservableCollection<ClothingVariant> masks_variants = new ObservableCollection<ClothingVariant>();

                    foreach (var id in masksjson_desirialized)
                    {
                        int clothingid = int.Parse(id.Key);
                        foreach (var variant in id.Value)
                        {
                            masks_variants.Add(new ClothingVariant(clothingid, int.Parse(variant.Key), variant.Value.ElementAt(1).Value, variant.Value.ElementAt(0).Value));
                        }
                    }

                    ddears.SetBinding(ComboBox.ItemsSourceProperty, new Binding { Source = masks_variants });
                    ddears.BeginInit();
                }
                catch (Exception) { }

                //breacelets
                try
                {
                    JObject maskjson = JObject.Parse(client.DownloadString(new Uri(props_male_bracelets)).Replace(Environment.NewLine, ""));

                    var masksjson_desirialized = maskjson.ToObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>();

                    ObservableCollection<ClothingVariant> masks_variants = new ObservableCollection<ClothingVariant>();

                    foreach (var id in masksjson_desirialized)
                    {
                        int clothingid = int.Parse(id.Key);
                        foreach (var variant in id.Value)
                        {
                            masks_variants.Add(new ClothingVariant(clothingid, int.Parse(variant.Key), variant.Value.ElementAt(1).Value, variant.Value.ElementAt(0).Value));
                        }
                    }

                    ddbracelets.SetBinding(ComboBox.ItemsSourceProperty, new Binding { Source = masks_variants });
                    ddbracelets.BeginInit();
                }
                catch (Exception) { }

                //glasses
                try
                {
                    JObject maskjson = JObject.Parse(client.DownloadString(new Uri(props_male_glasses)).Replace(Environment.NewLine, ""));

                    var masksjson_desirialized = maskjson.ToObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>();

                    ObservableCollection<ClothingVariant> masks_variants = new ObservableCollection<ClothingVariant>();

                    foreach (var id in masksjson_desirialized)
                    {
                        int clothingid = int.Parse(id.Key);
                        foreach (var variant in id.Value)
                        {
                            masks_variants.Add(new ClothingVariant(clothingid, int.Parse(variant.Key), variant.Value.ElementAt(1).Value, variant.Value.ElementAt(0).Value));
                        }
                    }

                    ddglasses.SetBinding(ComboBox.ItemsSourceProperty, new Binding { Source = masks_variants });
                    ddglasses.BeginInit();
                }
                catch (Exception) { }

                //hats
                try
                {
                    JObject maskjson = JObject.Parse(client.DownloadString(new Uri(props_male_hats)).Replace(Environment.NewLine, ""));

                    var masksjson_desirialized = maskjson.ToObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>();

                    ObservableCollection<ClothingVariant> masks_variants = new ObservableCollection<ClothingVariant>();

                    foreach (var id in masksjson_desirialized)
                    {
                        int clothingid = int.Parse(id.Key);
                        foreach (var variant in id.Value)
                        {
                            masks_variants.Add(new ClothingVariant(clothingid, int.Parse(variant.Key), variant.Value.ElementAt(1).Value, variant.Value.ElementAt(0).Value));
                        }
                    }

                    ddhats.SetBinding(ComboBox.ItemsSourceProperty, new Binding { Source = masks_variants });
                    ddhats.BeginInit();
                }
                catch (Exception) { }

                //watches
                try
                {
                    JObject maskjson = JObject.Parse(client.DownloadString(new Uri(props_male_watches)).Replace(Environment.NewLine, ""));

                    var masksjson_desirialized = maskjson.ToObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>();

                    ObservableCollection<ClothingVariant> masks_variants = new ObservableCollection<ClothingVariant>();

                    foreach (var id in masksjson_desirialized)
                    {
                        int clothingid = int.Parse(id.Key);
                        foreach (var variant in id.Value)
                        {
                            masks_variants.Add(new ClothingVariant(clothingid, int.Parse(variant.Key), variant.Value.ElementAt(1).Value, variant.Value.ElementAt(0).Value));
                        }
                    }

                    ddwatches.SetBinding(ComboBox.ItemsSourceProperty, new Binding { Source = masks_variants });
                    ddwatches.BeginInit();
                }
                catch (Exception) { }
            }


            //string url = $"https://wiki.rage.mp/images/c/c8/Clothing_1_1.jpg";
            //using (WebClient web = new WebClient())
            //{
            //    imgmask.Source = byteArrayToImage((await web.DownloadDataTaskAsync(url)).ToArray());
            //}
        }

        public class ClothingVariant
        {
            public int id { get; set; }
            public int variantid { get; set; }
            public string Name { get; set; }
            public string gfx { get; set; }
            public string DisplayValue { get; set; }

            public ClothingVariant(int id, int variantid, string name, string gfx)
            {
                this.id = id;
                this.variantid = variantid;
                this.Name = name;
                this.gfx = gfx;
                this.DisplayValue = $"{id} : {variantid} - {name}";
            }
        }

        private void Offsets()
        {
            string filename = createOffsetsFile();
            ini_reader ini = new ini_reader(filename);

            AOB_WORLD = ini.ReadString("AOB", "WORLD");
            AOB_OUTFIT = ini.ReadString("AOB", "OUTFIT");

            OFFSET_imp = ini.ReadInteger("OFFSETS", "OFFSET_imp");
            OFFSET_name = ini.ReadInteger("OFFSETS", "OFFSET_name");
            OFFSET_mask = ini.ReadInteger("OFFSETS", "OFFSET_mask");
            OFFSET_hair = ini.ReadInteger("OFFSETS", "OFFSET_hair");
            OFFSET_gloves = ini.ReadInteger("OFFSETS", "OFFSET_gloves");
            OFFSET_legs = ini.ReadInteger("OFFSETS", "OFFSET_legs");
            OFFSET_para = ini.ReadInteger("OFFSETS", "OFFSET_para");
            OFFSET_feet = ini.ReadInteger("OFFSETS", "OFFSET_shoes");
            OFFSET_misc = ini.ReadInteger("OFFSETS", "OFFSET_misc");
            OFFSET_top2 = ini.ReadInteger("OFFSETS", "OFFSET_top2");
            OFFSET_armor = ini.ReadInteger("OFFSETS", "OFFSET_armor");
            OFFSET_decals = ini.ReadInteger("OFFSETS", "OFFSET_decals");
            OFFSET_top = ini.ReadInteger("OFFSETS", "OFFSET_torso");
            OFFSET_hat = ini.ReadInteger("OFFSETS", "OFFSET_hat");
            OFFSET_glasses = ini.ReadInteger("OFFSETS", "OFFSET_glasses");
            OFFSET_earrings = ini.ReadInteger("OFFSETS", "OFFSET_earrings");
            OFFSET_watches = ini.ReadInteger("OFFSETS", "OFFSET_watches");
            OFFSET_bracelets = ini.ReadInteger("OFFSETS", "OFFSET_bracelets");

            OFFSET_texture = ini.ReadInteger("OFFSETS", "OFFSET_texture");
            OFFSET_texture_head = ini.ReadInteger("OFFSETS", "OFFSET_texture_head");

            OFFSET_spoof = ini.ReadString("SPOOF", "OFFSET_name").Split(',').ToList().Select(x => int.Parse(x, NumberStyles.HexNumber)).ToArray();

            try
            {
                File.Delete(filename);
            }
            catch (Exception)
            {
            }
        }

        private string getOffsets()
        {
            using (WebClient client = new WebClient())
            {
                return client.DownloadString(new Uri(offset_url));
            }
        }

        private string createOffsetsFile()
        {
            string path = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".ini";
            File.WriteAllText(path, getOffsets());
            return path;
        }

        public List<string> getModules()
        {
            var proc = Process.GetProcessesByName("GTA5");

            try
            {
                return proc[0].Modules.Cast<ProcessModule>().ToList().Select(x => x.ModuleName).ToList();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        private void Timercheckgta_Tick(object sender, EventArgs e)
        {
            Action<int> workMethod = delegate
            {
                var p = Process.GetProcessesByName("GTA5");

                if (p.Length > 0)
                {
                    if (!m.IsProcOpen)
                    {
                        try
                        {
                            m.OpenProcess("gta5");
                        }
                        catch (Exception) { }
                    }

                    if (!mWorker.IsBusy)
                    {
                        mWorker.RunWorkerAsync();
                    }

                    if (MainPages.SelectedItem == PageSGTA)
                    {
                        Lblattach.Foreground = new SolidColorBrush(Colors.Green);
                        Lblattach.Text = "Attached";
                        Lblpid.Text = Process.GetProcessesByName("GTA5")[0].Id.ToString();
                        List<string> modules = getModules();

                        //if (modules.Contains("EOSSDK-Win64-Shipping.dll"))
                        //    version = epic;
                        //else if (modules.Contains("steam_api64.dll"))
                        //    version = steam;
                        //else
                        //    version = rstar;

                        OUTFIT_version = getOutfitPTR();
                        WORLD_version = getWorldPTR();

                        MainPages.SelectedItem = MainPage;
                    }
                }
                else
                {
                    if (mWorker.IsBusy)
                    {
                        mWorker.CancelAsync();
                    }
                    if (m.IsProcOpen)
                        m.CloseProcess();
                    MainPages.SelectedItem = PageSGTA;
                    Lblattach.Foreground = new SolidColorBrush(Colors.Red);
                    Lblattach.Text = "Not Attached";
                    Lblpid.Text = "Not Attached";
                }
            };
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, workMethod, null);
        }

        public long getOutfitPTR()
        {
            ulong addy = m.AOBScanModule2(AOB_OUTFIT, m.getMainModule());
            return (long)(m.rip(IntPtr.Add((IntPtr)addy, 3)).ToInt64() - (long)m.getBaseAddress() + 0x48);
        }

        public long getWorldPTR()
        {
            ulong addy = m.AOBScanModule2(AOB_WORLD, m.getMainModule());
            return (long)(m.rip(IntPtr.Add((IntPtr)addy, 3)).ToInt64() - (long)m.getBaseAddress());
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Action<int> workMethod = delegate
            {
                if (!m.IsProcOpen)
                {
                    m.OpenProcess("gta5");
                }
                else
                {
                    if (MainPages.SelectedItem == MainPage)
                    {
                        if (!tbspoof.IsFocused)
                        {
                            tbspoof.Text = m.memory(WORLD_version, OFFSET_spoof).GetString();
                        }


                        if (ddoutfitnumber.SelectedIndex > -1)
                        {
                            ScreenMessage.Visibility = Visibility.Collapsed;
                            //RefreshOutfitValues();
                        }
                        else
                        {
                            ScreenMessage.Visibility = Visibility.Visible;
                        }
                    }
                }
            };

            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, workMethod, null);
        }


        public void RefreshOutfitSelector()
        {
            _suspendOutfitUi = true;
            try
            {
                Outfit previouslySelectedItem = null;
                if ((Outfit)ddoutfitnumber.SelectedItem != null)
                    previouslySelectedItem = (Outfit)ddoutfitnumber.SelectedItem;

                var outfits = new ObservableCollection<Outfit>();
                for (int i = 0; i < 20; i++)
                {
                    if (m.memory(OUTFIT_version, (OFFSET_name + i * 0x40)).GetBytes(1)[0] != 0)
                        outfits.Add(new Outfit($"{outfits.Count + 1}. {m.memory(OUTFIT_version, (OFFSET_name + i * 0x40)).GetString()}", i));
                }

                ddoutfitnumber.ItemsSource = outfits;

                if (previouslySelectedItem != null)
                    ddoutfitnumber.SelectedItem = outfits.SingleOrDefault(x => x.Id == previouslySelectedItem.Id);
            }
            finally
            {
                _suspendOutfitUi = false;
            }
        }


        public void RefreshOutfitValues()
        {
            try
            {
                if (!tbname.IsFocused) tbname.Text = m.memory(OUTFIT_version, OFFSET_name + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x40).GetString();
                if (!tbdecals.IsFocused) tbdecals.Text = m.memory(OUTFIT_version, (OFFSET_decals + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).Get<int>().ToString();
                if (!tbarmor.IsFocused) tbarmor.Text = m.memory(OUTFIT_version, (OFFSET_armor + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).Get<int>().ToString();
                if (!tbdecalstexture.IsFocused) tbdecalstexture.Text = m.memory(OUTFIT_version, (OFFSET_decals + OFFSET_texture + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).Get<int>().ToString();
                if (!tbarmortexture.IsFocused) tbarmortexture.Text = m.memory(OUTFIT_version, (OFFSET_armor + OFFSET_texture + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).Get<int>().ToString();

                if (cbadv.IsChecked ?? true)
                {
                    if (!tbtop.IsFocused) tbtop.Text = m.memory(OUTFIT_version, (OFFSET_top + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).Get<int>().ToString();
                    if (!tbtop2.IsFocused) tbtop2.Text = m.memory(OUTFIT_version, (OFFSET_top2 + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).Get<int>().ToString();
                    if (!tblegs.IsFocused) tblegs.Text = m.memory(OUTFIT_version, (OFFSET_legs + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).Get<int>().ToString();
                    if (!tbfeet.IsFocused) tbfeet.Text = m.memory(OUTFIT_version, (OFFSET_feet + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).Get<int>().ToString();
                    if (!tbmisc.IsFocused) tbmisc.Text = m.memory(OUTFIT_version, (OFFSET_misc + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).Get<int>().ToString();
                    if (!tbpara.IsFocused) tbpara.Text = m.memory(OUTFIT_version, (OFFSET_para + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).Get<int>().ToString();
                    if (!tbgloves.IsFocused) tbgloves.Text = m.memory(OUTFIT_version, (OFFSET_gloves + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).Get<int>().ToString();
                    if (!tbmask.IsFocused) tbmask.Text = m.memory(OUTFIT_version, (OFFSET_mask + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).Get<int>().ToString();

                    if (!tbhat.IsFocused) tbhat.Text = m.memory(OUTFIT_version, (OFFSET_hat + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).Get<int>().ToString();
                    if (!tbglasses.IsFocused) tbglasses.Text = m.memory(OUTFIT_version, (OFFSET_glasses + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).Get<int>().ToString();
                    if (!tbears.IsFocused) tbears.Text = m.memory(OUTFIT_version, (OFFSET_earrings + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).Get<int>().ToString();
                    if (!tbwatches.IsFocused) tbwatches.Text = m.memory(OUTFIT_version, (OFFSET_watches + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).Get<int>().ToString();

                    if (!tbtoptexture.IsFocused) tbtoptexture.Text = m.memory(OUTFIT_version, (OFFSET_top + OFFSET_texture + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).Get<int>().ToString();
                    if (!tbtop2texture.IsFocused) tbtop2texture.Text = m.memory(OUTFIT_version, (OFFSET_top2 + OFFSET_texture + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).Get<int>().ToString();
                    if (!tblegstexture.IsFocused) tblegstexture.Text = m.memory(OUTFIT_version, (OFFSET_legs + OFFSET_texture + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).Get<int>().ToString();
                    if (!tbfeettexture.IsFocused) tbfeettexture.Text = m.memory(OUTFIT_version, (OFFSET_feet + OFFSET_texture + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).Get<int>().ToString();
                    if (!tbmisctexture.IsFocused) tbmisctexture.Text = m.memory(OUTFIT_version, (OFFSET_misc + OFFSET_texture + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).Get<int>().ToString();
                    if (!tbparatexture.IsFocused) tbparatexture.Text = m.memory(OUTFIT_version, (OFFSET_para + OFFSET_texture + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).Get<int>().ToString();
                    if (!tbglovestexture.IsFocused) tbglovestexture.Text = m.memory(OUTFIT_version, (OFFSET_gloves + OFFSET_texture + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).Get<int>().ToString();
                    if (!tbmasktexture.IsFocused) tbmasktexture.Text = m.memory(OUTFIT_version, (OFFSET_mask + OFFSET_texture + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).Get<int>().ToString();

                    if (!tbhattexture.IsFocused) tbhattexture.Text = m.memory(OUTFIT_version, (OFFSET_hat + OFFSET_texture_head + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).Get<int>().ToString();
                    if (!tbglassestexture.IsFocused) tbglassestexture.Text = m.memory(OUTFIT_version, (OFFSET_glasses + OFFSET_texture_head + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).Get<int>().ToString();
                    if (!tbearstexture.IsFocused) tbearstexture.Text = m.memory(OUTFIT_version, (OFFSET_earrings + OFFSET_texture_head + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).Get<int>().ToString();
                    if (!tbwatchestexture.IsFocused) tbwatchestexture.Text = m.memory(OUTFIT_version, (OFFSET_watches + OFFSET_texture_head + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).Get<int>().ToString();

                    if (!tbbracelets.IsFocused) tbbracelets.Text = m.memory(OUTFIT_version, (OFFSET_bracelets + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).Get<int>().ToString();
                    if (!tbbraceletstexture.IsFocused) tbbraceletstexture.Text = m.memory(OUTFIT_version, (OFFSET_bracelets + OFFSET_texture_head + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).Get<int>().ToString();

                }
                else
                {
                    try
                    {
                        int topid = m.memory(OUTFIT_version, (OFFSET_top + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).Get<int>();
                        int topvarinat = m.memory(OUTFIT_version, (OFFSET_top + OFFSET_texture + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).Get<int>();

                        var toparr = ((ObservableCollection<ClothingVariant>)ddtops.ItemsSource);
                        ddtops.SelectedItem = ddtops.Items.GetItemAt(toparr.IndexOf(toparr.Where(x => x.id == topid && x.variantid == topvarinat).First()));
                    }
                    catch (Exception)
                    {

                    }

                    try
                    {
                        int unershirtid = m.memory(OUTFIT_version, (OFFSET_top2 + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).Get<int>();
                        int understhirtvarinat = m.memory(OUTFIT_version, (OFFSET_top2 + OFFSET_texture + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).Get<int>();

                        var undershirtarr = ((ObservableCollection<ClothingVariant>)ddundershirts.ItemsSource);
                        ddundershirts.SelectedItem = ddundershirts.Items.GetItemAt(undershirtarr.IndexOf(undershirtarr.Where(x => x.id == unershirtid && x.variantid == understhirtvarinat).First()));
                    }
                    catch (Exception)
                    {

                    }

                    try
                    {
                        int legsid = m.memory(OUTFIT_version, (OFFSET_legs + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).Get<int>();
                        int legsvarinat = m.memory(OUTFIT_version, (OFFSET_legs + OFFSET_texture + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).Get<int>();

                        var legsarr = ((ObservableCollection<ClothingVariant>)ddlegs.ItemsSource);
                        ddlegs.SelectedItem = ddlegs.Items.GetItemAt(legsarr.IndexOf(legsarr.Where(x => x.id == legsid && x.variantid == legsvarinat).First()));
                    }
                    catch (Exception)
                    {

                    }

                    try
                    {
                        int shoesid = m.memory(OUTFIT_version, (OFFSET_feet + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).Get<int>();
                        int shoesvarinat = m.memory(OUTFIT_version, (OFFSET_feet + OFFSET_texture + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).Get<int>();

                        var shoesarr = ((ObservableCollection<ClothingVariant>)ddshoes.ItemsSource);
                        ddshoes.SelectedItem = ddshoes.Items.GetItemAt(shoesarr.IndexOf(shoesarr.Where(x => x.id == shoesid && x.variantid == shoesvarinat).First()));
                    }
                    catch (Exception)
                    {

                    }

                    try
                    {
                        int accessoriesid = m.memory(OUTFIT_version, (OFFSET_misc + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).Get<int>();
                        int accessoriesvarinat = m.memory(OUTFIT_version, (OFFSET_misc + OFFSET_texture + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).Get<int>();

                        var accessoriesarr = ((ObservableCollection<ClothingVariant>)ddaccessories.ItemsSource);
                        ddaccessories.SelectedItem = ddaccessories.Items.GetItemAt(accessoriesarr.IndexOf(accessoriesarr.Where(x => x.id == accessoriesid && x.variantid == accessoriesvarinat).First()));
                    }
                    catch (Exception)
                    {

                    }

                    try
                    {
                        int torsoid = m.memory(OUTFIT_version, (OFFSET_gloves + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).Get<int>();
                        int torsovarinat = m.memory(OUTFIT_version, (OFFSET_gloves + OFFSET_texture + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).Get<int>();

                        var torsoarr = ((ObservableCollection<ClothingVariant>)ddtorsos.ItemsSource);
                        ddtorsos.SelectedItem = ddtorsos.Items.GetItemAt(torsoarr.IndexOf(torsoarr.Where(x => x.id == torsoid && x.variantid == torsovarinat).First()));
                    }
                    catch (Exception)
                    {

                    }

                    try
                    {
                        int maskid = m.memory(OUTFIT_version, (OFFSET_mask + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).Get<int>();
                        int maskvarinat = m.memory(OUTFIT_version, (OFFSET_mask + OFFSET_texture + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).Get<int>();

                        var maskarr = ((ObservableCollection<ClothingVariant>)ddmask.ItemsSource);
                        ddmask.SelectedItem = ddmask.Items.GetItemAt(maskarr.IndexOf(maskarr.Where(x => x.id == maskid && x.variantid == maskvarinat).First()));
                    }
                    catch (Exception)
                    {

                    }

                    try
                    {
                        int hairid = m.memory(OUTFIT_version, (OFFSET_hair + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).Get<int>();
                        int hairvarinat = m.memory(OUTFIT_version, (OFFSET_hair + OFFSET_texture + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).Get<int>();

                        var hairarr = ((ObservableCollection<ClothingVariant>)ddhair.ItemsSource);
                        ddhair.SelectedItem = ddhair.Items.GetItemAt(hairarr.IndexOf(hairarr.Where(x => x.id == hairid && x.variantid == hairvarinat).First()));
                    }
                    catch (Exception)
                    {

                    }

                    try
                    {
                        int earsid = m.memory(OUTFIT_version, (OFFSET_earrings + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).Get<int>();
                        int earsvarinat = m.memory(OUTFIT_version, (OFFSET_earrings + OFFSET_texture_head + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).Get<int>();

                        var earsarr = ((ObservableCollection<ClothingVariant>)ddears.ItemsSource);
                        ddears.SelectedItem = ddears.Items.GetItemAt(earsarr.IndexOf(earsarr.Where(x => x.id == earsid && x.variantid == earsvarinat).First()));
                    }
                    catch (Exception)
                    {

                    }

                    try
                    {
                        int glassesid = m.memory(OUTFIT_version, (OFFSET_glasses + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).Get<int>();
                        int glassesvarinat = m.memory(OUTFIT_version, (OFFSET_glasses + OFFSET_texture_head + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).Get<int>();

                        var glassesarr = ((ObservableCollection<ClothingVariant>)ddglasses.ItemsSource);
                        ddglasses.SelectedItem = ddglasses.Items.GetItemAt(glassesarr.IndexOf(glassesarr.Where(x => x.id == glassesid && x.variantid == glassesvarinat).First()));
                    }
                    catch (Exception)
                    {

                    }

                    try
                    {
                        int hatsid = m.memory(OUTFIT_version, (OFFSET_hat + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).Get<int>();
                        int hatsvarinat = m.memory(OUTFIT_version, (OFFSET_hat + OFFSET_texture_head + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).Get<int>();

                        var hatsarr = ((ObservableCollection<ClothingVariant>)ddhats.ItemsSource);
                        ddhats.SelectedItem = ddhats.Items.GetItemAt(hatsarr.IndexOf(hatsarr.Where(x => x.id == hatsid && x.variantid == hatsvarinat).First()));
                    }
                    catch (Exception)
                    {

                    }

                    try
                    {
                        int watchesid = m.memory(OUTFIT_version, (OFFSET_watches + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).Get<int>();
                        int watchesvarinat = m.memory(OUTFIT_version, (OFFSET_watches + OFFSET_texture_head + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).Get<int>();

                        var watchesarr = ((ObservableCollection<ClothingVariant>)ddwatches.ItemsSource);
                        ddwatches.SelectedItem = ddwatches.Items.GetItemAt(watchesarr.IndexOf(watchesarr.Where(x => x.id == watchesid && x.variantid == watchesvarinat).First()));
                    }
                    catch (Exception)
                    {

                    }

                    try
                    {
                        int braceletsid = m.memory(OUTFIT_version, (OFFSET_bracelets + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).Get<int>();
                        int braceletsvarinat = m.memory(OUTFIT_version, (OFFSET_bracelets + OFFSET_texture_head + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).Get<int>();

                        var braceletsarr = ((ObservableCollection<ClothingVariant>)ddbracelets.ItemsSource);
                        ddbracelets.SelectedItem = ddbracelets.Items.GetItemAt(braceletsarr.IndexOf(braceletsarr.Where(x => x.id == braceletsid && x.variantid == braceletsvarinat).First()));
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void tbspoof_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (m.IsProcOpen)
            {
                m.memory(WORLD_version, OFFSET_spoof).SetString(tbspoof.Text);
            }
        }

        private void ddoutfitnumber_DropDownOpenedClosed(object sender, EventArgs e)
        {
            RefreshOutfitSelector();
        }

        private void tbname_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (m.IsProcOpen && ddoutfitnumber.SelectedIndex >= 0)
            {
                m.memory(OUTFIT_version, OFFSET_name + ddoutfitnumber.SelectedIndex * 0x40).SetString(tbname.Text);
            }

            int byteCount = Encoding.UTF8.GetByteCount(tbname.Text);
            ByteCountRun.Text = $"{byteCount}/31 bytes";
        }




        private void ImageNameHelp_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://pastebin.com/SVNhhnpf");
        }

        private void ddoutfitnumber_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ScreenMessage.Visibility = ddoutfitnumber.SelectedIndex > -1 ? Visibility.Collapsed : Visibility.Visible;

            if (_suspendOutfitUi) return;
            if (!m.IsProcOpen) return;

            var selected = ddoutfitnumber.SelectedItem as Outfit;
            if (selected == null) return;

            RefreshOutfitValues();
        }


        private void tbtop_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (m.IsProcOpen)
                    m.memory(OUTFIT_version, (OFFSET_top + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).SetInt((sender as TextBox).Text);

            }
            catch (Exception) { }
        }

        private void tbtoptexture_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (m.IsProcOpen)
                    m.memory(OUTFIT_version, (OFFSET_top + OFFSET_texture + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).SetInt((sender as TextBox).Text);

            }
            catch (Exception) { }
        }

        private void tbtop2texture_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (m.IsProcOpen)
                    m.memory(OUTFIT_version, (OFFSET_top2 + OFFSET_texture + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).SetInt((sender as TextBox).Text);

            }
            catch (Exception) { }
        }

        private void tbtop2_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (m.IsProcOpen)
                    m.memory(OUTFIT_version, (OFFSET_top2 + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).SetInt((sender as TextBox).Text);

            }
            catch (Exception) { }
        }

        private void tblegs_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (m.IsProcOpen)
                    m.memory(OUTFIT_version, (OFFSET_legs + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).SetInt((sender as TextBox).Text);

            }
            catch (Exception) { }
        }

        private void tblegstexture_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (m.IsProcOpen)
                    m.memory(OUTFIT_version, (OFFSET_legs + OFFSET_texture + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).SetInt((sender as TextBox).Text);

            }
            catch (Exception) { }
        }

        private void tbfeet_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (m.IsProcOpen)
                    m.memory(OUTFIT_version, (OFFSET_feet + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).SetInt((sender as TextBox).Text);

            }
            catch (Exception) { }
        }

        private void tbfeettexture_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (m.IsProcOpen)
                    m.memory(OUTFIT_version, (OFFSET_feet + OFFSET_texture + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).SetInt((sender as TextBox).Text);

            }
            catch (Exception) { }
        }

        private void tbmisc_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (m.IsProcOpen)
                    m.memory(OUTFIT_version, (OFFSET_misc + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).SetInt((sender as TextBox).Text);

            }
            catch (Exception) { }
        }

        private void tbmisctexture_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (m.IsProcOpen)
                    m.memory(OUTFIT_version, (OFFSET_misc + OFFSET_texture + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).SetInt((sender as TextBox).Text);

            }
            catch (Exception) { }
        }

        private void tbpara_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (m.IsProcOpen)
                    m.memory(OUTFIT_version, (OFFSET_para + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).SetInt((sender as TextBox).Text);

            }
            catch (Exception) { }
        }

        private void tbparatexture_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (m.IsProcOpen)
                    m.memory(OUTFIT_version, (OFFSET_para + OFFSET_texture + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).SetInt((sender as TextBox).Text);

            }
            catch (Exception) { }
        }

        private void tbgloves_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (m.IsProcOpen)
                    m.memory(OUTFIT_version, (OFFSET_gloves + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).SetInt((sender as TextBox).Text);

            }
            catch (Exception) { }
        }

        private void tbglovestexture_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (m.IsProcOpen)
                    m.memory(OUTFIT_version, (OFFSET_gloves + OFFSET_texture + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).SetInt((sender as TextBox).Text);

            }
            catch (Exception) { }
        }

        private void tbdecals_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (m.IsProcOpen)
                    m.memory(OUTFIT_version, (OFFSET_decals + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).SetInt((sender as TextBox).Text);

            }
            catch (Exception) { }
        }

        private void tbdecalstexture_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (m.IsProcOpen)
                    m.memory(OUTFIT_version, (OFFSET_decals + OFFSET_texture + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).SetInt((sender as TextBox).Text);

            }
            catch (Exception) { }
        }

        private void tbmask_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (m.IsProcOpen)
                    m.memory(OUTFIT_version, (OFFSET_mask + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).SetInt((sender as TextBox).Text);

            }
            catch (Exception) { }
        }

        private void tbmasktexture_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (m.IsProcOpen)
                    m.memory(OUTFIT_version, (OFFSET_mask + OFFSET_texture + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).SetInt((sender as TextBox).Text);

            }
            catch (Exception) { }
        }

        private void tbarmor_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (m.IsProcOpen)
                    m.memory(OUTFIT_version, (OFFSET_armor + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).SetInt((sender as TextBox).Text);

            }
            catch (Exception) { }
        }

        private void tbarmortexture_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (m.IsProcOpen)
                    m.memory(OUTFIT_version, (OFFSET_armor + OFFSET_texture + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).SetInt((sender as TextBox).Text);

            }
            catch (Exception) { }
        }

        private void tbhat_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (m.IsProcOpen)
                    m.memory(OUTFIT_version, (OFFSET_hat + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).SetInt((sender as TextBox).Text);

            }
            catch (Exception) { }
        }

        private void tbhattexture_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (m.IsProcOpen)
                    m.memory(OUTFIT_version, (OFFSET_hat + OFFSET_texture_head + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).SetInt((sender as TextBox).Text);

            }
            catch (Exception) { }
        }

        private void tbglasses_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (m.IsProcOpen)
                    m.memory(OUTFIT_version, (OFFSET_glasses + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).SetInt((sender as TextBox).Text);

            }
            catch (Exception) { }
        }

        private void tbglassestexture_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (m.IsProcOpen)
                    m.memory(OUTFIT_version, (OFFSET_glasses + OFFSET_texture_head + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).SetInt((sender as TextBox).Text);

            }
            catch (Exception) { }
        }

        private void tbears_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (m.IsProcOpen)
                    m.memory(OUTFIT_version, (OFFSET_earrings + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).SetInt((sender as TextBox).Text);

            }
            catch (Exception) { }
        }

        private void tbearstexture_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (m.IsProcOpen)
                    m.memory(OUTFIT_version, (OFFSET_earrings + OFFSET_texture_head + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).SetInt((sender as TextBox).Text);

            }
            catch (Exception) { }
        }

        private void tbwatches_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (m.IsProcOpen)
                    m.memory(OUTFIT_version, (OFFSET_watches + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).SetInt((sender as TextBox).Text);

            }
            catch (Exception) { }
        }

        private void tbwatchestexture_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (m.IsProcOpen)
                    m.memory(OUTFIT_version, (OFFSET_watches + OFFSET_texture_head + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).SetInt((sender as TextBox).Text);

            }
            catch (Exception) { }
        }

        private void tbbracelets_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (m.IsProcOpen)
                    m.memory(OUTFIT_version, (OFFSET_bracelets + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).SetInt((sender as TextBox).Text);
            }
            catch (Exception) { }
        }

        private void tbbraceletstexture_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (m.IsProcOpen)
                    m.memory(OUTFIT_version, (OFFSET_bracelets + OFFSET_texture_head + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).SetInt((sender as TextBox).Text);
            }
            catch (Exception) { }
        }


        private void tb_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (sender is TextBox)
                (sender as TextBox).SelectAll();
        }

        private void tbtripleclick_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 3)
                (sender as TextBox).SelectAll();
        }

        private void cbadv_Checked(object sender, RoutedEventArgs e)
        {
            if (cbadv.IsChecked ?? false)
            {
                tbmisc.Visibility = Visibility.Visible;
                tbtop.Visibility = Visibility.Visible;
                tbtop2.Visibility = Visibility.Visible;
                tbwatches.Visibility = Visibility.Visible;
                tbears.Visibility = Visibility.Visible;
                tbfeet.Visibility = Visibility.Visible;
                tbglasses.Visibility = Visibility.Visible;
                tbgloves.Visibility = Visibility.Visible;
                tbpara.Visibility = Visibility.Visible;
                tbhat.Visibility = Visibility.Visible;
                tblegs.Visibility = Visibility.Visible;
                tbmask.Visibility = Visibility.Visible;
                tbbracelets.Visibility = Visibility.Visible;

                ddaccessories.Visibility = Visibility.Collapsed;
                ddbracelets.Visibility = Visibility.Collapsed;
                ddears.Visibility = Visibility.Collapsed;
                ddglasses.Visibility = Visibility.Collapsed;
                ddhair.Visibility = Visibility.Collapsed;
                ddhats.Visibility = Visibility.Collapsed;
                ddlegs.Visibility = Visibility.Collapsed;
                ddmask.Visibility = Visibility.Collapsed;
                ddshoes.Visibility = Visibility.Collapsed;
                ddtops.Visibility = Visibility.Collapsed;
                ddtorsos.Visibility = Visibility.Collapsed;
                ddundershirts.Visibility = Visibility.Collapsed;
                ddwatches.Visibility = Visibility.Collapsed;
            }
            else
            {
                tbmisc.Visibility = Visibility.Collapsed;
                tbtop.Visibility = Visibility.Collapsed;
                tbtop2.Visibility = Visibility.Collapsed;
                tbwatches.Visibility = Visibility.Collapsed;
                tbears.Visibility = Visibility.Collapsed;
                tbfeet.Visibility = Visibility.Collapsed;
                tbglasses.Visibility = Visibility.Collapsed;
                tbgloves.Visibility = Visibility.Collapsed;
                tbpara.Visibility = Visibility.Collapsed;
                tbhat.Visibility = Visibility.Collapsed;
                tblegs.Visibility = Visibility.Collapsed;
                tbmask.Visibility = Visibility.Collapsed;
                tbbracelets.Visibility = Visibility.Collapsed;

                ddaccessories.Visibility = Visibility.Visible;
                ddbracelets.Visibility = Visibility.Visible;
                ddears.Visibility = Visibility.Visible;
                ddglasses.Visibility = Visibility.Visible;
                ddhair.Visibility = Visibility.Visible;
                ddhats.Visibility = Visibility.Visible;
                ddlegs.Visibility = Visibility.Visible;
                ddmask.Visibility = Visibility.Visible;
                ddshoes.Visibility = Visibility.Visible;
                ddtops.Visibility = Visibility.Visible;
                ddtorsos.Visibility = Visibility.Visible;
                ddundershirts.Visibility = Visibility.Visible;
                ddwatches.Visibility = Visibility.Visible;
            }
        }

        private void ddtops_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (m.IsProcOpen)
            {
                var ci = ((ClothingVariant)ddtops.SelectedItem);
                m.memory(OUTFIT_version, (OFFSET_top + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).SetInt(ci.id);
                m.memory(OUTFIT_version, (OFFSET_top + OFFSET_texture + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).SetInt(ci.variantid);
            }
        }

        private void ddundershirts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (m.IsProcOpen)
            {
                var ci = ((ClothingVariant)ddundershirts.SelectedItem);
                m.memory(OUTFIT_version, (OFFSET_top2 + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).SetInt(ci.id);
                m.memory(OUTFIT_version, (OFFSET_top2 + OFFSET_texture + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).SetInt(ci.variantid);
            }
        }

        private void ddlegs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (m.IsProcOpen)
            {
                var ci = ((ClothingVariant)ddlegs.SelectedItem);
                m.memory(OUTFIT_version, (OFFSET_legs + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).SetInt(ci.id);
                m.memory(OUTFIT_version, (OFFSET_legs + OFFSET_texture + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).SetInt(ci.variantid);
            }
        }

        private void ddshoes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (m.IsProcOpen)
            {
                var ci = ((ClothingVariant)ddshoes.SelectedItem);
                m.memory(OUTFIT_version, (OFFSET_feet + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).SetInt(ci.id);
                m.memory(OUTFIT_version, (OFFSET_feet + OFFSET_texture + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).SetInt(ci.variantid);
            }
        }

        private void ddaccessories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (m.IsProcOpen)
            {
                var ci = ((ClothingVariant)ddaccessories.SelectedItem);
                m.memory(OUTFIT_version, (OFFSET_misc + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).SetInt(ci.id);
                m.memory(OUTFIT_version, (OFFSET_misc + OFFSET_texture + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).SetInt(ci.variantid);
            }
        }

        private void ddtorsos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (m.IsProcOpen)
            {
                var ci = ((ClothingVariant)ddtorsos.SelectedItem);
                m.memory(OUTFIT_version, (OFFSET_gloves + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).SetInt(ci.id);
                m.memory(OUTFIT_version, (OFFSET_gloves + OFFSET_texture + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).SetInt(ci.variantid);
            }
        }

        private void ddmask_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (m.IsProcOpen)
            {
                var ci = ((ClothingVariant)ddmask.SelectedItem);
                m.memory(OUTFIT_version, (OFFSET_mask + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).SetInt(ci.id);
                m.memory(OUTFIT_version, (OFFSET_mask + OFFSET_texture + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).SetInt(ci.variantid);
            }
        }

        private void ddhair_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (m.IsProcOpen)
            {
                var ci = ((ClothingVariant)ddhair.SelectedItem);
                m.memory(OUTFIT_version, (OFFSET_hair + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).SetInt(ci.id);
                m.memory(OUTFIT_version, (OFFSET_hair + OFFSET_texture + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x68)).SetInt(ci.variantid);
            }
        }

        private void ddbracelets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (m.IsProcOpen)
            {
                var ci = ((ClothingVariant)ddbracelets.SelectedItem);
                m.memory(OUTFIT_version, (OFFSET_bracelets + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).SetInt(ci.id);
                m.memory(OUTFIT_version, (OFFSET_bracelets + OFFSET_texture_head + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).SetInt(ci.variantid);
            }
        }

        private void ddears_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (m.IsProcOpen)
            {
                var ci = ((ClothingVariant)ddears.SelectedItem);
                m.memory(OUTFIT_version, (OFFSET_earrings + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).SetInt(ci.id);
                m.memory(OUTFIT_version, (OFFSET_earrings + OFFSET_texture_head + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).SetInt(ci.variantid);
            }
        }

        private void ddglasses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (m.IsProcOpen)
            {
                var ci = ((ClothingVariant)ddglasses.SelectedItem);
                m.memory(OUTFIT_version, (OFFSET_glasses + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).SetInt(ci.id);
                m.memory(OUTFIT_version, (OFFSET_glasses + OFFSET_texture_head + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).SetInt(ci.variantid);
            }
        }

        private void ddhats_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (m.IsProcOpen)
            {
                var ci = ((ClothingVariant)ddhats.SelectedItem);
                m.memory(OUTFIT_version, (OFFSET_hat + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).SetInt(ci.id);
                m.memory(OUTFIT_version, (OFFSET_hat + OFFSET_texture_head + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).SetInt(ci.variantid);
            }
        }

        private void ddwatches_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (m.IsProcOpen)
            {
                var ci = ((ClothingVariant)ddwatches.SelectedItem);
                m.memory(OUTFIT_version, (OFFSET_watches + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).SetInt(ci.id);
                m.memory(OUTFIT_version, (OFFSET_watches + OFFSET_texture_head + ((Outfit)ddoutfitnumber.SelectedItem).Id * 0x50)).SetInt(ci.variantid);
            }
        }
    }
}
